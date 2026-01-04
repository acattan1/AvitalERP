using AvitalERP.Data;
using AvitalERP.Models;
using AvitalERP.Models.Hubspot;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AvitalERP.Services.Hubspot
{
    public class HubspotPollingService
    {
        private readonly HubspotClient _hubspot;
        private readonly AppDbContext _db;

        public HubspotPollingService(HubspotClient hubspot, AppDbContext db)
        {
            _hubspot = hubspot;
            _db = db;
        }

        public async Task<(int processed, int skipped)> RunOnceAsync(CancellationToken ct = default)
        {
            // 1) Resolver ClosedWonStageId automáticamente (por label en español)
            var (_, closedWonStageId) = await _hubspot.GetPipelineAndClosedWonStageIdsAsync(ct);

            // 2) Estado de sync
            var state = await _db.HubspotSyncStates.OrderBy(x => x.Id).FirstOrDefaultAsync(ct);
            if (state == null)
            {
                state = new HubspotSyncState
                {
                    LastClosedDateProcessedUtc = DateTime.UtcNow.AddDays(-30)
                };
                _db.HubspotSyncStates.Add(state);
                await _db.SaveChangesAsync(ct);
            }

            // 3) Buscar deals ClosedWon cerrados después del marcador
            var deals = await _hubspot.SearchClosedWonDealsAsync(closedWonStageId, state.LastClosedDateProcessedUtc, ct);

            var ordered = deals
                .Select(d => new { Deal = d, ClosedUtc = TryGetClosedDateUtc(d) })
                .Where(x => x.ClosedUtc.HasValue)
                .OrderBy(x => x.ClosedUtc!.Value)
                .ToList();

            int processed = 0, skipped = 0;
            DateTime? maxClosed = state.LastClosedDateProcessedUtc;

            foreach (var item in ordered)
            {
                ct.ThrowIfCancellationRequested();

                var deal = item.Deal;
                var dealId = deal.Id;

                // Idempotencia
                var exists = await _db.Proyectos.AnyAsync(p => p.HubspotDealId == dealId, ct);
                if (exists)
                {
                    skipped++;
                    maxClosed = Max(maxClosed, item.ClosedUtc);
                    continue;
                }

                // Company asociada (siempre hay, pero lo manejamos defensivo)
                string? hubspotCompanyId = null;
                string? hubspotCompanyName = null;

                var companyId = await _hubspot.GetCompanyIdForDealAsync(dealId, ct);
                if (companyId.HasValue)
                {
                    hubspotCompanyId = companyId.Value.ToString();

                    var company = await _hubspot.GetCompanyAsync(companyId.Value, ct);
                    hubspotCompanyName = company?.Properties.TryGetValue("name", out var nm) == true ? (nm ?? "") : "";
                    hubspotCompanyName = string.IsNullOrWhiteSpace(hubspotCompanyName) ? null : hubspotCompanyName.Trim();
                }

                // Intentar ligar con Cliente existente por HubspotCompanyId (SIN CREAR)
                int? clienteId = null;
                if (!string.IsNullOrWhiteSpace(hubspotCompanyId))
                {
                    clienteId = await _db.Clientes
                        .Where(c => c.HubspotCompanyId == hubspotCompanyId)
                        .Select(c => (int?)c.Id)
                        .FirstOrDefaultAsync(ct);
                }

                await CreateProyectoAsync(clienteId, hubspotCompanyId, hubspotCompanyName, deal, ct);

                processed++;
                maxClosed = Max(maxClosed, item.ClosedUtc);
            }

            // 4) Avanzar marcador
            if (maxClosed.HasValue && maxClosed.Value > state.LastClosedDateProcessedUtc)
            {
                state.LastClosedDateProcessedUtc = maxClosed.Value;
                await _db.SaveChangesAsync(ct);
            }

            return (processed, skipped);
        }

        private static DateTime? TryGetClosedDateUtc(HubspotDeal deal)
        {
            if (!deal.Properties.TryGetValue("closedate", out var v) || string.IsNullOrWhiteSpace(v))
                return null;

            if (long.TryParse(v, out var ms))
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;

            if (DateTimeOffset.TryParse(v, out var dto))
                return dto.UtcDateTime;

            return null;
        }

        private async Task CreateProyectoAsync(
            int? clienteId,
            string? hubspotCompanyId,
            string? hubspotCompanyName,
            HubspotDeal deal,
            CancellationToken ct)
        {
            var year = DateTime.Now.Year;
            var prefix = $"AVT-{year}-";

            // Folio: transacción simple
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var lastFolio = await _db.Proyectos
                .Where(p => p.Folio.StartsWith(prefix))
                .OrderByDescending(p => p.Folio)
                .Select(p => p.Folio)
                .FirstOrDefaultAsync(ct);

            var next = 1;
            if (!string.IsNullOrWhiteSpace(lastFolio) && lastFolio.Length >= prefix.Length + 4)
            {
                var tail = lastFolio.Substring(prefix.Length, 4);
                if (int.TryParse(tail, out var lastNum))
                    next = lastNum + 1;
            }

            var folio = $"{prefix}{next:0000}";

            var dealName = deal.Properties.TryGetValue("dealname", out var nm) ? (nm ?? "") : "";
            if (string.IsNullOrWhiteSpace(dealName)) dealName = "(Sin nombre)";

            // HubSpot "amount" lo tomamos como Subtotal inicial (luego lo editas en ERP)
            var subtotal = 0m;
            if (deal.Properties.TryGetValue("amount", out var am) && !string.IsNullOrWhiteSpace(am))
                decimal.TryParse(am, out subtotal);

            var aplicaIva = true;
            var tasaIva = 0.16m;

            var ivaImporte = aplicaIva ? Math.Round(subtotal * tasaIva, 2) : 0m;

            var aplicaRetIsr = false;
            var tasaRetIsr = 0m;
            var retIsrImporte = aplicaRetIsr ? Math.Round(subtotal * tasaRetIsr, 2) : 0m;

            var total = Math.Round(subtotal + ivaImporte - retIsrImporte, 2);

            var payload = JsonSerializer.Serialize(deal);

            var proyecto = new Proyecto
            {
                Folio = folio,
                HubspotDealId = deal.Id,

                HubspotCompanyId = hubspotCompanyId,
                HubspotCompanyName = hubspotCompanyName,

                ClienteId = clienteId, // puede ser null

                NombreProyecto = dealName.Trim(),

                Subtotal = subtotal,
                AplicaIVA = aplicaIva,
                TasaIVA = tasaIva,
                IvaImporte = ivaImporte,

                AplicaRetencionISR = aplicaRetIsr,
                TasaRetencionISR = tasaRetIsr,
                RetencionISRImporte = retIsrImporte,

                Total = total,

                Moneda = "MXN",
                Estado = ProyectoEstado.Nuevo,
                FechaCreacion = DateTime.Now,
                JsonPayloadOriginal = payload
            };

            _db.Proyectos.Add(proyecto);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        private static DateTime? Max(DateTime? a, DateTime? b)
        {
            if (!a.HasValue) return b;
            if (!b.HasValue) return a;
            return a.Value >= b.Value ? a : b;
        }
    }
}

using AvitalERP.Data;
using AvitalERP.Models;
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

            // 2) Tomamos last_check desde SQL (si no existe, usamos últimos 30 días para bootstrap)
            var state = await _db.Set<HubspotSyncState>().FirstOrDefaultAsync(s => s.Id == 1, ct);
            if (state == null)
            {
                state = new HubspotSyncState { Id = 1, LastClosedDateProcessedUtc = DateTime.UtcNow.AddDays(-30) };
                _db.Add(state);
                await _db.SaveChangesAsync(ct);
            }

            var deals = await _hubspot.SearchClosedWonDealsAsync(closedWonStageId, state.LastClosedDateProcessedUtc, ct);

            // Orden por closedate asc para avanzar marcador correctamente
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

                // Idempotencia: si ya existe proyecto con ese dealId, skip
                var exists = await _db.Proyectos.AnyAsync(p => p.HubspotDealId == dealId, ct);
                if (exists)
                {
                    skipped++;
                    maxClosed = Max(maxClosed, item.ClosedUtc);
                    continue;
                }

                // Obtener empresa asociada
                var companyId = await _hubspot.GetCompanyIdForDealAsync(dealId, ct);
                if (companyId is null)
                {
                    // No bloqueamos: creamos cliente placeholder con nombre del deal
                    var cliente = await GetOrCreatePlaceholderClienteAsync(deal, ct);
                    await CreateProyectoAsync(cliente.Id, deal, item.ClosedUtc!.Value, ct);
                    processed++;
                    maxClosed = Max(maxClosed, item.ClosedUtc);
                    continue;
                }

                var company = await _hubspot.GetCompanyAsync(companyId.Value, ct);
                var companyName = company?.Properties.TryGetValue("name", out var nm) == true ? (nm ?? "") : "";

                var clienteReal = await GetOrCreateClienteByHubspotCompanyIdAsync(companyId.Value.ToString(), companyName, ct);

                await CreateProyectoAsync(clienteReal.Id, deal, item.ClosedUtc!.Value, ct);
                processed++;
                maxClosed = Max(maxClosed, item.ClosedUtc);
            }

            // Avanzar marcador
            if (maxClosed.HasValue && maxClosed.Value > state.LastClosedDateProcessedUtc)
            {
                state.LastClosedDateProcessedUtc = maxClosed.Value;
                await _db.SaveChangesAsync(ct);
            }

            return (processed, skipped);
        }

        private static DateTime? TryGetClosedDateUtc(AvitalERP.Models.Hubspot.HubspotDeal deal)
        {
            if (!deal.Properties.TryGetValue("closedate", out var v) || string.IsNullOrWhiteSpace(v))
                return null;

            // HubSpot closedate suele venir en ms epoch (string)
            if (long.TryParse(v, out var ms))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
            }

            // fallback por si viniera ISO
            if (DateTimeOffset.TryParse(v, out var dto))
                return dto.UtcDateTime;

            return null;
        }

        private async Task<Cliente> GetOrCreateClienteByHubspotCompanyIdAsync(string hubspotCompanyId, string companyName, CancellationToken ct)
        {
            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.HubspotCompanyId == hubspotCompanyId, ct);
            if (cliente != null) return cliente;

            // Cliente rápido + pendiente de razón social
            cliente = new Cliente
            {
                HubspotCompanyId = hubspotCompanyId,
                NombreComercial = string.IsNullOrWhiteSpace(companyName) ? "(Sin nombre)" : companyName.Trim(),
                RazonSocial = "", // pendiente
                Email = "",
                Telefono = "",
                EsCliente = true,
                EsProveedor = false,
                Origen = "HubSpot",
                FechaRegistro = DateTime.Now
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync(ct);
            return cliente;
        }

        private async Task<Cliente> GetOrCreatePlaceholderClienteAsync(AvitalERP.Models.Hubspot.HubspotDeal deal, CancellationToken ct)
        {
            var dealName = deal.Properties.TryGetValue("dealname", out var n) ? (n ?? "") : "";
            dealName = string.IsNullOrWhiteSpace(dealName) ? "(Deal sin nombre)" : dealName.Trim();

            // Placeholder: HubspotCompanyId vacío => para revisar después
            var cliente = new Cliente
            {
                HubspotCompanyId = "",
                NombreComercial = dealName,
                RazonSocial = "",
                Email = "",
                Telefono = "",
                EsCliente = true,
                EsProveedor = false,
                Origen = "HubSpot",
                FechaRegistro = DateTime.Now
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync(ct);
            return cliente;
        }

        private async Task CreateProyectoAsync(int clienteId, AvitalERP.Models.Hubspot.HubspotDeal deal, DateTime closedUtc, CancellationToken ct)
        {
            var year = DateTime.Now.Year;
            var prefix = $"AVT-{year}-";

            // Folio: dentro de transacción serializable (simple y seguro)
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

            var amount = 0m;
            if (deal.Properties.TryGetValue("amount", out var am) && !string.IsNullOrWhiteSpace(am))
                decimal.TryParse(am, out amount);

            var payload = JsonSerializer.Serialize(deal);

            var proyecto = new Proyecto
            {
                Folio = folio,
                HubspotDealId = deal.Id,
                ClienteId = clienteId,
                NombreProyecto = dealName.Trim(),
                Monto = amount,
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

    // Estado de sincronización (tabla simple)
    public class HubspotSyncState
    {
        public int Id { get; set; } = 1;
        public DateTime LastClosedDateProcessedUtc { get; set; }
    }
}


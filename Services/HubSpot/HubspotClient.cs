using AvitalERP.Models.Hubspot;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AvitalERP.Services.Hubspot
{
    public class HubspotClient
    {
        private readonly HttpClient _http;
        private readonly HubspotOptions _opt;
        private static readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web);

        public HubspotClient(HttpClient http, IOptions<HubspotOptions> opt)
        {
            _http = http;
            _opt = opt.Value;

            _http.BaseAddress = new Uri("https://api.hubapi.com/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _opt.AccessToken);
        }

        public async Task<(string pipelineId, string closedWonStageId)> GetPipelineAndClosedWonStageIdsAsync(CancellationToken ct = default)
        {
            var resp = await _http.GetAsync("crm/v3/pipelines/deals", ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            var data = JsonSerializer.Deserialize<HubspotPipelinesResponse>(json, _jsonOpts)
                       ?? new HubspotPipelinesResponse();

            var pipeline = data.Results.FirstOrDefault(p => string.Equals(p.Label, _opt.PipelineLabel, StringComparison.OrdinalIgnoreCase))
                        ?? data.Results.FirstOrDefault();

            if (pipeline is null)
                throw new InvalidOperationException("No se encontró pipeline de deals en HubSpot.");

            var stage = pipeline.Stages.FirstOrDefault(s => string.Equals(s.Label, _opt.ClosedWonStageLabel, StringComparison.OrdinalIgnoreCase));
            if (stage is null)
                throw new InvalidOperationException($"No se encontró la etapa '{_opt.ClosedWonStageLabel}' en el pipeline '{pipeline.Label}'.");

            return (pipeline.Id, stage.Id);
        }

        // ✅ Correcto para HubSpot v3: SEARCH (POST)
        public async Task<List<HubspotDeal>> SearchClosedWonDealsAsync(string closedWonStageId, DateTime? closedAfterUtc, CancellationToken ct = default)
        {
            var results = new List<HubspotDeal>();
            string? after = null;

            // HubSpot usa epoch ms para fechas
            string? closedAfterMs = null;
            if (closedAfterUtc.HasValue)
            {
                var ms = new DateTimeOffset(DateTime.SpecifyKind(closedAfterUtc.Value, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
                closedAfterMs = ms.ToString();
            }

            do
            {
                var req = new HubspotSearchRequest
                {
                    Limit = _opt.PageSize <= 0 ? 50 : _opt.PageSize,
                    After = after,
                    Properties = new List<string> { "dealname", "dealstage", "amount", "closedate" },
                    FilterGroups = new List<HubspotFilterGroup>
                    {
                        new HubspotFilterGroup
                        {
                            Filters = new List<HubspotFilter>
                            {
                                new HubspotFilter { PropertyName = "dealstage", Operator = "EQ", Value = closedWonStageId }
                            }
                        }
                    }
                };

                if (!string.IsNullOrWhiteSpace(closedAfterMs))
                {
                    req.FilterGroups[0].Filters.Add(new HubspotFilter
                    {
                        PropertyName = "closedate",
                        Operator = "GT",
                        Value = closedAfterMs
                    });
                }

                var body = JsonSerializer.Serialize(req, _jsonOpts);
                var resp = await _http.PostAsync(
                    "crm/v3/objects/deals/search",
                    new StringContent(body, Encoding.UTF8, "application/json"),
                    ct);

                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync(ct);
                var data = JsonSerializer.Deserialize<HubspotDealSearchResponse>(json, _jsonOpts)
                           ?? new HubspotDealSearchResponse();

                results.AddRange(data.Results);
                after = data.Paging?.Next?.After;
            }
            while (!string.IsNullOrWhiteSpace(after));

            return results;
        }
        public async Task<long?> GetCompanyIdForDealAsync(string dealId, CancellationToken ct = default)
        {
            // v3 correcto:
            // /crm/v3/objects/deals/{dealId}/associations/companies
            var url = $"crm/v3/objects/deals/{dealId}/associations/companies";

            var resp = await _http.GetAsync(url, ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);

            // v3: { results:[ { id:"123" } ] }
            // v4: { results:[ { toObjectId:123 } ] }
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("results", out var results) ||
                results.ValueKind != JsonValueKind.Array ||
                results.GetArrayLength() == 0)
                return null;

            var first = results[0];

            if (first.TryGetProperty("id", out var idProp))
            {
                var idStr = idProp.ValueKind == JsonValueKind.String ? idProp.GetString() : idProp.ToString();
                if (long.TryParse(idStr, out var idLong))
                    return idLong;
            }

            if (first.TryGetProperty("toObjectId", out var toObjProp))
            {
                if (toObjProp.ValueKind == JsonValueKind.Number && toObjProp.TryGetInt64(out var idLong))
                    return idLong;

                var idStr = toObjProp.ValueKind == JsonValueKind.String ? toObjProp.GetString() : toObjProp.ToString();
                if (long.TryParse(idStr, out var idLong2))
                    return idLong2;
            }

            return null;
        }
        public async Task<HubspotCompanyResponse?> GetCompanyAsync(long companyId, CancellationToken ct = default)
        {
            // Traemos name + algunos campos típicos (puedes ampliar después)
            var url = $"crm/v3/objects/companies/{companyId}?properties=name,domain,website,phone";

            var resp = await _http.GetAsync(url, ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<HubspotCompanyResponse>(json, _jsonOpts);
        }


    }
}


using System.Text.Json.Serialization;

namespace AvitalERP.Models.Hubspot
{
    // Pipelines
    public class HubspotPipelinesResponse
    {
        [JsonPropertyName("results")]
        public List<HubspotPipeline> Results { get; set; } = new();
    }

    public class HubspotPipeline
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("label")]
        public string Label { get; set; } = "";

        [JsonPropertyName("stages")]
        public List<HubspotStage> Stages { get; set; } = new();
    }

    public class HubspotStage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("label")]
        public string Label { get; set; } = "";
    }

    // Deal search
    public class HubspotSearchRequest
    {
        [JsonPropertyName("filterGroups")]
        public List<HubspotFilterGroup> FilterGroups { get; set; } = new();

        [JsonPropertyName("properties")]
        public List<string> Properties { get; set; } = new();

        [JsonPropertyName("limit")]
        public int Limit { get; set; } = 50;

        [JsonPropertyName("after")]
        public string? After { get; set; }
    }

    public class HubspotFilterGroup
    {
        [JsonPropertyName("filters")]
        public List<HubspotFilter> Filters { get; set; } = new();
    }

    public class HubspotFilter
    {
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; } = "";

        [JsonPropertyName("operator")]
        public string Operator { get; set; } = "EQ";

        [JsonPropertyName("value")]
        public string Value { get; set; } = "";
    }

    public class HubspotDealSearchResponse
    {
        [JsonPropertyName("results")]
        public List<HubspotDeal> Results { get; set; } = new();

        [JsonPropertyName("paging")]
        public HubspotPaging? Paging { get; set; }
    }

    public class HubspotPaging
    {
        [JsonPropertyName("next")]
        public HubspotNext? Next { get; set; }
    }

    public class HubspotNext
    {
        [JsonPropertyName("after")]
        public string After { get; set; } = "";
    }

    public class HubspotDeal
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("properties")]
        public Dictionary<string, string?> Properties { get; set; } = new();
    }

    // Associations: deal -> companies
    public class HubspotAssociationsResponse
    {
        [JsonPropertyName("results")]
        public List<HubspotAssociation> Results { get; set; } = new();
    }

    public class HubspotAssociation
    {
        [JsonPropertyName("toObjectId")]
        public long ToObjectId { get; set; }
    }

    // Company
    public class HubspotCompanyResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("properties")]
        public Dictionary<string, string?> Properties { get; set; } = new();
    }
}

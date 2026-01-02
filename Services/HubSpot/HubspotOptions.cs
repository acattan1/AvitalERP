namespace AvitalERP.Services.Hubspot
{
    public class HubspotOptions
    {
        public string AccessToken { get; set; } = "";
        public string PipelineLabel { get; set; } = "Pipeline de ventas";
        public string ClosedWonStageLabel { get; set; } = "Cierre ganado";
        public int PageSize { get; set; } = 50;
    }
}


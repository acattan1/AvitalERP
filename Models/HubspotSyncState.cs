using System;

namespace AvitalERP.Models
{
    public class HubspotSyncState
    {
        public int Id { get; set; } = 1;

        // Usamos este marcador para el polling incremental
        public DateTime LastClosedDateProcessedUtc { get; set; } = DateTime.UtcNow.AddDays(-30);
    }
}


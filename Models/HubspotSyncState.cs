using System;

namespace AvitalERP.Models
{
    public class HubspotSyncState
    {
        public int Id { get; set; } = 1;
        public DateTime LastClosedDateProcessedUtc { get; set; }
    }
}


using System;
using System.ComponentModel.DataAnnotations;

namespace AvitalERP.Models
{
    public class HubspotCompanyLink
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string HubspotCompanyId { get; set; } = "";

        [StringLength(200)]
        public string HubspotCompanyName { get; set; } = "";

        // Cuando aún no está ligado en ERP, queda null
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvitalERP.Models
{
    public enum ProyectoEstado
    {
        Nuevo = 0,
        Planeacion = 1,
        Ejecucion = 2,
        Entrega = 3,
        Facturado = 4,
        Cobrado = 5,
        Cerrado = 6
    }

    public class Proyecto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Folio { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HubspotDealId { get; set; } = string.Empty;

        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [StringLength(50)]
        public string? HubspotCompanyId { get; set; }

        [StringLength(200)]
        public string? HubspotCompanyName { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreProyecto { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        public bool AplicaIVA { get; set; } = true;

        [Column(TypeName = "decimal(9,6)")]
        public decimal TasaIVA { get; set; } = 0.16m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal IvaImporte { get; set; }

        public bool AplicaRetencionISR { get; set; } = false;

        [Column(TypeName = "decimal(9,6)")]
        public decimal TasaRetencionISR { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RetencionISRImporte { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(10)]
        public string Moneda { get; set; } = "MXN";

        public ProyectoEstado Estado { get; set; } = ProyectoEstado.Nuevo;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string JsonPayloadOriginal { get; set; } = string.Empty;
    }
}

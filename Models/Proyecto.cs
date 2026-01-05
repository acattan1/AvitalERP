using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvitalERP.Models
{
    /// <summary>
    /// Estados de proyecto (guardados como string en BD para compatibilidad con datos existentes).
    /// </summary>
    public static class ProyectoEstados
    {
        public const string Nuevo = "Nuevo";
        public const string Planeacion = "Planeacion";
        public const string Ejecucion = "Ejecucion";
        public const string Entrega = "Entrega";
        public const string Facturado = "Facturado";
        public const string Cobrado = "Cobrado";
        public const string Cerrado = "Cerrado";
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
        public string HubspotCompanyId { get; set; } = string.Empty;

        [StringLength(200)]
        public string HubspotCompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string NombreProyecto { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        public bool AplicaIVA { get; set; } = true;

        [Column(TypeName = "decimal(9,4)")]
        public decimal TasaIVA { get; set; } = 0.16m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal IvaImporte { get; set; }

        public bool AplicaRetencionISR { get; set; } = false;

        [Column(TypeName = "decimal(9,4)")]
        public decimal TasaRetencionISR { get; set; } = 0.0125m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal RetencionISRImporte { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(10)]
        public string Moneda { get; set; } = "MXN";

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = ProyectoEstados.Nuevo;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // ===== Cierre / OneDrive =====
        [StringLength(200)]
        public string? CerradoPor { get; set; }

        public DateTime? FechaCierre { get; set; }

        [StringLength(1000)]
        public string? OneDriveFolderUrl { get; set; }

        public string JsonPayloadOriginal { get; set; } = string.Empty;
    }
}

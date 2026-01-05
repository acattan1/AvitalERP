using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvitalERP.Models
{
    public enum CfdiDestinoTipo
    {
        Proyecto = 0,
        Negocio = 1,
        Personal = 2
    }

    public class CfdiDocumento
    {
        public int Id { get; set; }

        // opcional: si el documento completo pertenece a un proyecto
        public int? ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        [StringLength(260)]
        public string FileNameXml { get; set; } = string.Empty;

        [StringLength(500)]
        public string StoredPathXml { get; set; } = string.Empty; // /uploads/.../file.xml

        [StringLength(260)]
        public string? FileNamePdf { get; set; }

        [StringLength(500)]
        public string? StoredPathPdf { get; set; }

        [StringLength(1000)]
        public string? OneDriveUrl { get; set; }

        [StringLength(64)]
        public string? UUID { get; set; }

        [StringLength(20)]
        public string? EmisorRfc { get; set; }

        [StringLength(200)]
        public string? EmisorNombre { get; set; }

        [StringLength(20)]
        public string? ReceptorRfc { get; set; }

        public DateTime? Fecha { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(10)]
        public string? Moneda { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public List<CfdiConcepto> Conceptos { get; set; } = new();
    }

    public class CfdiConcepto
    {
        public int Id { get; set; }

        public int CfdiDocumentoId { get; set; }
        public CfdiDocumento? CfdiDocumento { get; set; }

        [StringLength(20)]
        public string? ClaveProdServ { get; set; }

        [StringLength(50)]
        public string? NoIdentificacion { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,4)")]
        public decimal Cantidad { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ValorUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Importe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Iva { get; set; }

        public CfdiDestinoTipo DestinoTipo { get; set; } = CfdiDestinoTipo.Proyecto;

        public int? ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        [StringLength(120)]
        public string? Categoria { get; set; }
    }
}

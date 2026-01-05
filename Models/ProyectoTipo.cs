using System;
using System.ComponentModel.DataAnnotations;

namespace AvitalERP.Models
{
    public class ProyectoTipo
    {
        public int Id { get; set; }

        // Código corto para estabilidad (ej: CCTV, ACCESO, WIFI, VENTA_EQ, VENTA_REF, CABLEADO)
        [Required]
        [StringLength(40)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

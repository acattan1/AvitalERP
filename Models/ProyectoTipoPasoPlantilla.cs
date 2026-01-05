using System;
using System.ComponentModel.DataAnnotations;

namespace AvitalERP.Models
{
    public class ProyectoTipoPasoPlantilla
    {
        public int Id { get; set; }

        public int ProyectoTipoId { get; set; }
        public ProyectoTipo ProyectoTipo { get; set; } = default!;

        public int Orden { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(800)]
        public string? Descripcion { get; set; }

        public bool RequiereEvidencia { get; set; } = false;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

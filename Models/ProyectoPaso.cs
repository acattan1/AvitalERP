using System;
using System.ComponentModel.DataAnnotations;

namespace AvitalERP.Models
{
    public static class ProyectoPasoEstados
    {
        public const string Pendiente = "Pendiente";
        public const string EnProceso = "EnProceso";
        public const string Hecho = "Hecho";
        public const string Bloqueado = "Bloqueado";
    }

    public class ProyectoPaso
    {
        public int Id { get; set; }

        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; } = default!;

        public int? ProyectoTipoId { get; set; }
        public ProyectoTipo? ProyectoTipo { get; set; }

        public int Orden { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(800)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = ProyectoPasoEstados.Pendiente;

        public int? TecnicoId { get; set; }
        public Tecnico? Tecnico { get; set; }

        public DateTime? FechaObjetivo { get; set; }
        public DateTime? FechaHecho { get; set; }

        [StringLength(2000)]
        public string? Notas { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

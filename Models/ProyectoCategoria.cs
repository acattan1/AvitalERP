using System;
using System.ComponentModel.DataAnnotations;

namespace AvitalERP.Models
{
    public class ProyectoCategoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
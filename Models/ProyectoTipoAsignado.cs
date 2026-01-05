using System;

namespace AvitalERP.Models
{
    public class ProyectoTipoAsignado
    {
        public int Id { get; set; }

        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; } = default!;

        public int ProyectoTipoId { get; set; }
        public ProyectoTipo ProyectoTipo { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

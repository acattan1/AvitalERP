using System;

namespace AvitalERP.Models
{
    public class ProyectoCategoriaAsignada
    {
        public int Id { get; set; }

        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; } = default!;

        public int ProyectoCategoriaId { get; set; }
        public ProyectoCategoria ProyectoCategoria { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
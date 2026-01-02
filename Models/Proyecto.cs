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
        public string Folio { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string HubspotDealId { get; set; } = "";

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreProyecto { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(10)]
        public string Moneda { get; set; } = "MXN";

        public ProyectoEstado Estado { get; set; } = ProyectoEstado.Nuevo;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string JsonPayloadOriginal { get; set; } = "";
    }
}

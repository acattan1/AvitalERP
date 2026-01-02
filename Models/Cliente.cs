using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvitalERP.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El RFC es obligatorio")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener entre 12 y 13 caracteres")]
        public string Rfc { get; set; } = "";

        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(200, ErrorMessage = "La razón social no puede exceder 200 caracteres")]
        public string RazonSocial { get; set; } = "";

        [StringLength(200, ErrorMessage = "El nombre comercial no puede exceder 200 caracteres")]
        public string NombreComercial { get; set; } = ""; // ← Agregar = ""

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = "";

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; } = "";

        public string RegimenFiscal { get; set; } = "";

        [StringLength(5, ErrorMessage = "El código postal debe tener 5 dígitos")]
        public string CodigoPostal { get; set; } = "00000";

        public string UsoCfdi { get; set; } = "";

        // Clasificación
        public bool EsProveedor { get; set; }
        public bool EsCliente { get; set; } = true;

        [StringLength(50)]
        public string TipoProveedor { get; set; } = "N/A";

        // Metadata
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Origen { get; set; } = "Manual";

        public string IdFacturama { get; set; } = "";

        [StringLength(500)]
        public string Notas { get; set; } = "";

        public string HubspotCompanyId { get; set; } = "";

   
        // Propiedades calculadas
        [NotMapped]
        public string DisplayName =>
            !string.IsNullOrEmpty(NombreComercial) ? NombreComercial : RazonSocial;

        [NotMapped]
        public string TipoDisplay =>
            EsCliente && EsProveedor ? "Cliente/Proveedor" :
            EsCliente ? "Cliente" :
            EsProveedor ? "Proveedor" : "Sin clasificar";
    }
}

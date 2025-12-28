using System.Xml.Serialization;

namespace AvitalERP.Models
{
    [XmlRoot(ElementName = "Comprobante", Namespace = "http://www.sat.gob.mx/cfd/4")]
    public class Comprobante
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = "4.0";

        [XmlElement("Emisor")]
        public Emisor Emisor { get; set; } = new();

        [XmlElement("Receptor")]
        public Receptor Receptor { get; set; } = new();

        [XmlAttribute("Folio")]
        public string Folio { get; set; } = "";

        [XmlAttribute("Fecha")]
        public DateTime Fecha { get; set; }

        [XmlAttribute("Total")]
        public decimal Total { get; set; }

        [XmlAttribute("SubTotal")]
        public decimal SubTotal { get; set; }

        [XmlAttribute("Moneda")]
        public string Moneda { get; set; } = "MXN";
    }

    public class Emisor
    {
        [XmlAttribute("Rfc")]
        public string Rfc { get; set; } = "";

        [XmlAttribute("Nombre")]
        public string Nombre { get; set; } = "";

        [XmlAttribute("RegimenFiscal")]
        public string RegimenFiscal { get; set; } = "";
    }

    public class Receptor
    {
        [XmlAttribute("Rfc")]
        public string Rfc { get; set; } = "";

        [XmlAttribute("Nombre")]
        public string Nombre { get; set; } = "";

        [XmlAttribute("UsoCFDI")]
        public string UsoCFDI { get; set; } = "";

        [XmlAttribute("DomicilioFiscalReceptor")]
        public string DomicilioFiscalReceptor { get; set; } = "";
    }

    public class XmlImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Cliente? ClienteImportado { get; set; }
        public string? Rfc { get; set; }
        public string? RazonSocial { get; set; }
        public string? Tipo { get; set; } // "Cliente" o "Proveedor"
    }
}
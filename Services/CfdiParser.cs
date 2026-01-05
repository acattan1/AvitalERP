using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AvitalERP.Models;

namespace AvitalERP.Services
{
    public sealed class CfdiParseResult
    {
        public CfdiDocumento Doc { get; set; } = new();
        public List<CfdiConcepto> Conceptos { get; set; } = new();
    }

    public static class CfdiParser
    {
        public static Task<CfdiParseResult> ParseAsync(string xmlFilePath)
        {
            var xml = File.ReadAllText(xmlFilePath);
            var xdoc = XDocument.Parse(xml);

            var comprobante = xdoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Comprobante");
            if (comprobante == null)
                return Task.FromResult(new CfdiParseResult());

            var emisor = xdoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Emisor");
            var receptor = xdoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Receptor");
            var timbre = xdoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "TimbreFiscalDigital");

            string? GetAttr(XElement? el, string name) => el?.Attribute(name)?.Value;

            decimal TryDec(string? s)
            {
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
                if (decimal.TryParse(s, out d)) return d;
                return 0m;
            }

            DateTime? TryDate(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt)) return dt;
                if (DateTime.TryParse(s, out dt)) return dt;
                return null;
            }

            // Creamos doc y llenamos SOLO campos "seguros".
            // Si tu modelo tiene más campos, luego lo enriquecemos.
            var doc = new CfdiDocumento();

            // UUID (si tu modelo tiene una propiedad UUID/Uuid/FolioFiscal, etc. lo ajustamos luego)
            // Por ahora NO asignamos doc.Uuid para evitar CS0117.

            // Fecha: si tu modelo tiene DateTime, asignamos DateTime?.
            // Si tu modelo no tiene Fecha, quita esta parte.
            var fechaStr = GetAttr(comprobante, "Fecha");
            var fecha = TryDate(fechaStr);

            // Totales (estos casi seguro existen)
            // Ajusta si tu modelo usa otros nombres.
            // Ejemplo común: Subtotal, Total
            try
            {
                // Si existen estas propiedades, compila:
                doc.Subtotal = TryDec(GetAttr(comprobante, "SubTotal"));
                doc.Total = TryDec(GetAttr(comprobante, "Total"));
            }
            catch
            {
                // Si tus nombres son distintos, me dices y lo alineo
            }

            // Si tu modelo tiene campos DateTime? Fecha
            // (si no existe, comenta esta línea)
            try
            {
                doc.Fecha = fecha;
            }
            catch { }

            // Conceptos
            var conceptos = xdoc.Descendants()
                .Where(e => e.Name.LocalName == "Concepto")
                .Select(c => new CfdiConcepto
                {
                    ClaveProdServ = GetAttr(c, "ClaveProdServ") ?? "",
                    Descripcion = GetAttr(c, "Descripcion") ?? "",
                    Cantidad = TryDec(GetAttr(c, "Cantidad")),
                    ValorUnitario = TryDec(GetAttr(c, "ValorUnitario")),
                    Importe = TryDec(GetAttr(c, "Importe")),
                })
                .ToList();

            return Task.FromResult(new CfdiParseResult
            {
                Doc = doc,
                Conceptos = conceptos
            });
        }
    }
}

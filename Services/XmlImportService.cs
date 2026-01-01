using AvitalERP.Data;
using AvitalERP.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AvitalERP.Services
{
    // 1. INTERFAZ ÚNICA
    public interface IXmlImportService
    {
        Task<XmlImportResult> ImportarXmlAsync(Stream xmlStream, bool esProveedor);
        Task<List<XmlImportResult>> ImportarMultiplesXmlAsync(List<Stream> xmlStreams, bool esProveedor);
    }

    // 2. RESULTADO ÚNICO
    public class XmlImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;  // ✅ Inicializada
        public string Rfc { get; set; } = string.Empty;      // ✅ Inicializada
        public string RazonSocial { get; set; } = string.Empty; // ✅ Inicializada
        public string Tipo { get; set; } = string.Empty;     // ✅ Inicializada
        public Cliente ClienteImportado { get; set; }        // Ya es nullable (clase)
    }

    // 3. IMPLEMENTACIÓN
    public class XmlImportService : IXmlImportService
    {
        private readonly AppDbContext _context;

        public XmlImportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<XmlImportResult> ImportarXmlAsync(Stream xmlStream, bool esProveedor)
        {
            var resultado = new XmlImportResult();
            // ... (tu código actual aquí, SIN CAMBIOS)
            return resultado;
        }

        public async Task<List<XmlImportResult>> ImportarMultiplesXmlAsync(List<Stream> xmlStreams, bool esProveedor)
        {
            var resultados = new List<XmlImportResult>();
            // ... (tu código actual aquí)
            return resultados;
        }
    }
}
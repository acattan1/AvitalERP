using AvitalERP.Data;
using AvitalERP.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace AvitalERP.Services
{
    public interface IXmlImportService
    {
        Task<XmlImportResult> ImportarXmlAsync(Stream xmlStream, bool esProveedor);
        Task<List<XmlImportResult>> ImportarMultiplesXmlAsync(List<Stream> xmlStreams, bool esProveedor);
        Comprobante? LeerComprobante(Stream xmlStream);
    }

    public class XmlImportService : IXmlImportService
    {
        private readonly AppDbContext _context;

        public XmlImportService(AppDbContext context)
        {
            _context = context;
        }

        public Comprobante? LeerComprobante(Stream xmlStream)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Comprobante));
                xmlStream.Position = 0;
                return serializer.Deserialize(xmlStream) as Comprobante;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo XML: {ex.Message}");
                return null;
            }
        }

        public async Task<XmlImportResult> ImportarXmlAsync(Stream xmlStream, bool esProveedor)
        {
            var resultado = new XmlImportResult();

            try
            {
                var comprobante = LeerComprobante(xmlStream);
                if (comprobante == null)
                {
                    resultado.Message = "Archivo XML inválido o mal formado";
                    return resultado;
                }

                // Determinar si es cliente o proveedor según la lógica del negocio
                string rfc, razonSocial, regimenFiscal, usoCfdi;

                if (esProveedor)
                {
                    // Es proveedor: datos del emisor
                    rfc = comprobante.Emisor.Rfc;
                    razonSocial = comprobante.Emisor.Nombre;
                    regimenFiscal = comprobante.Emisor.RegimenFiscal;
                    usoCfdi = "N/A";
                }
                else
                {
                    // Es cliente: datos del receptor
                    rfc = comprobante.Receptor.Rfc;
                    razonSocial = comprobante.Receptor.Nombre;
                    regimenFiscal = "N/A";
                    usoCfdi = comprobante.Receptor.UsoCFDI;
                }

                // Validar que el RFC no esté vacío
                if (string.IsNullOrWhiteSpace(rfc))
                {
                    resultado.Message = "El RFC no puede estar vacío en el XML";
                    return resultado;
                }

                // Buscar si ya existe el cliente/proveedor
                var clienteExistente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Rfc == rfc);

                if (clienteExistente != null)
                {
                    // Actualizar datos existentes
                    clienteExistente.RazonSocial = razonSocial;
                    clienteExistente.RegimenFiscal = regimenFiscal;
                    clienteExistente.UsoCfdi = usoCfdi;

                    if (esProveedor)
                    {
                        clienteExistente.EsProveedor = true;
                        clienteExistente.TipoProveedor = "General";
                    }
                    else
                    {
                        clienteExistente.EsCliente = true;
                    }

                    clienteExistente.Origen = "Importado XML";

                    _context.Clientes.Update(clienteExistente);
                    await _context.SaveChangesAsync();

                    resultado.Success = true;
                    resultado.Message = "Cliente actualizado exitosamente";
                    resultado.ClienteImportado = clienteExistente;
                    resultado.Rfc = rfc;
                    resultado.RazonSocial = razonSocial;
                    resultado.Tipo = esProveedor ? "Proveedor" : "Cliente";
                }
                else
                {
                    // Crear nuevo cliente/proveedor
                    var nuevoCliente = new Cliente
                    {
                        Rfc = rfc,
                        RazonSocial = razonSocial,
                        NombreComercial = razonSocial,
                        RegimenFiscal = regimenFiscal,
                        UsoCfdi = usoCfdi,
                        EsProveedor = esProveedor,
                        EsCliente = !esProveedor,
                        TipoProveedor = esProveedor ? "General" : "N/A",
                        Origen = "Importado XML",
                        FechaRegistro = DateTime.Now
                    };

                    await _context.Clientes.AddAsync(nuevoCliente);
                    await _context.SaveChangesAsync();

                    resultado.Success = true;
                    resultado.Message = "Cliente creado exitosamente";
                    resultado.ClienteImportado = nuevoCliente;
                    resultado.Rfc = rfc;
                    resultado.RazonSocial = razonSocial;
                    resultado.Tipo = esProveedor ? "Proveedor" : "Cliente";
                }
            }
            catch (Exception ex)
            {
                resultado.Message = $"Error importando XML: {ex.Message}";
            }

            return resultado;
        }

        public async Task<List<XmlImportResult>> ImportarMultiplesXmlAsync(List<Stream> xmlStreams, bool esProveedor)
        {
            var resultados = new List<XmlImportResult>();

            foreach (var stream in xmlStreams)
            {
                var resultado = await ImportarXmlAsync(stream, esProveedor);
                resultados.Add(resultado);
            }

            return resultados;
        }
    }
}
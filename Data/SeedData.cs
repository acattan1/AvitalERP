using AvitalERP.Models;
using Microsoft.EntityFrameworkCore;

namespace AvitalERP.Data
{
    /// <summary>
    /// Seed liviano: crea catálogo mínimo (técnicos y tipos + plantillas) si aún no existe.
    /// Diseñado para NO romper el arranque.
    /// </summary>
    public static class SeedData
    {
        public static async Task EnsureSeededAsync(AppDbContext db)
        {
            // Seguridad: si la BD no existe o no está accesible, no detengas el arranque.
            try
            {
                if (!await db.Database.CanConnectAsync())
                    return;
            }
            catch { return; }

            // ===== Técnicos (placeholder) =====
            try
            {
                if (!await db.Tecnicos.AnyAsync())
                {
                    db.Tecnicos.AddRange(
                        new Tecnico { Nombre = "Técnico 1" },
                        new Tecnico { Nombre = "Técnico 2" }
                    );
                    await db.SaveChangesAsync();
                }
            }
            catch { /* si falta tabla por migración pendiente, no tumbar */ }

            // ===== Tipos de proyecto + plantillas MVP =====
            try
            {
                if (!await db.ProyectoCategorias.AnyAsync())
                {
                    var categorias = BuildDefaultCategorias();
                    db.ProyectoCategorias.AddRange(categorias);
                    await db.SaveChangesAsync();
                }

                if (!await db.ProyectoTipos.AnyAsync())
                {
                    var tipos = BuildDefaultTipos();
                    db.ProyectoTipos.AddRange(tipos);
                    await db.SaveChangesAsync();
                }

                if (!await db.ProyectoTipoPasoPlantillas.AnyAsync())
                {
                    var tipos = await db.ProyectoTipos.AsNoTracking().ToListAsync();
                    var plantillas = BuildDefaultPlantillas(tipos);
                    db.ProyectoTipoPasoPlantillas.AddRange(plantillas);
                    await db.SaveChangesAsync();
                }
            }
            catch { }
        }

        private static List<ProyectoTipo> BuildDefaultTipos()
        {
            return new List<ProyectoTipo>
            {
                new ProyectoTipo { Codigo = "CCTV", Nombre = "CCTV" },
                new ProyectoTipo { Codigo = "CONTROL_ACCESO", Nombre = "Control de acceso" },
                new ProyectoTipo { Codigo = "CABLEADO", Nombre = "Cableado estructurado" },
                new ProyectoTipo { Codigo = "WIFI", Nombre = "Red WiFi" },
                new ProyectoTipo { Codigo = "VENTA_EQUIPOS", Nombre = "Venta de equipos" },
                new ProyectoTipo { Codigo = "VENTA_REFACCIONES", Nombre = "Venta de refacciones" }
            };
        }

        private static List<ProyectoCategoria> BuildDefaultCategorias()
        {
            return new List<ProyectoCategoria>
            {
                new ProyectoCategoria { Nombre = "CCTV Wifi" },
                new ProyectoCategoria { Nombre = "CCTV Cableado" },
                new ProyectoCategoria { Nombre = "CCTV Movil" },
                new ProyectoCategoria { Nombre = "Revision de Camaras" },
                new ProyectoCategoria { Nombre = "Venta de Equipos" }
            };
        }

        private static List<ProyectoTipoPasoPlantilla> BuildDefaultPlantillas(List<ProyectoTipo> tipos)
        {
            var map = tipos.ToDictionary(t => t.Codigo, t => t.Id);

            var pasos = new List<ProyectoTipoPasoPlantilla>();

            void add(string codigo, int orden, string titulo, string? desc = null, bool evidencia = false)
            {
                if (!map.TryGetValue(codigo, out var tipoId)) return;
                pasos.Add(new ProyectoTipoPasoPlantilla
                {
                    ProyectoTipoId = tipoId,
                    Orden = orden,
                    Nombre = titulo,
                    Descripcion = desc,
                    RequiereEvidencia = evidencia
                });
            }

            // CCTV
            add("CCTV", 10, "Levantamiento / sitio", "Fotos, rutas de cableado, puntos de cámara", true);
            add("CCTV", 20, "Compra de materiales", "Cotización / OC / evidencia", true);
            add("CCTV", 30, "Instalación física", "Montaje cámaras / canalización", true);
            add("CCTV", 40, "Configuración", "NVR, red, app", true);
            add("CCTV", 50, "Entrega", "Checklist final y capacitación", true);
            add("CCTV", 60, "Facturación", "Factura emitida", true);
            add("CCTV", 70, "Cobranza", "Pago recibido", true);
            add("CCTV", 80, "Cierre", "Acta de cierre", true);

            // Control de acceso
            add("CONTROL_ACCESO", 10, "Levantamiento / requerimientos", null, true);
            add("CONTROL_ACCESO", 20, "Compra de materiales", null, true);
            add("CONTROL_ACCESO", 30, "Instalación", null, true);
            add("CONTROL_ACCESO", 40, "Configuración", null, true);
            add("CONTROL_ACCESO", 50, "Entrega", null, true);
            add("CONTROL_ACCESO", 60, "Facturación", null, true);
            add("CONTROL_ACCESO", 70, "Cobranza", null, true);
            add("CONTROL_ACCESO", 80, "Cierre", null, true);

            // Cableado
            add("CABLEADO", 10, "Levantamiento / planos", null, true);
            add("CABLEADO", 20, "Compra de materiales", null, true);
            add("CABLEADO", 30, "Tendido y ponchado", null, true);
            add("CABLEADO", 40, "Certificación / pruebas", null, true);
            add("CABLEADO", 50, "Entrega", null, true);
            add("CABLEADO", 60, "Facturación", null, true);
            add("CABLEADO", 70, "Cobranza", null, true);
            add("CABLEADO", 80, "Cierre", null, true);

            // WiFi
            add("WIFI", 10, "Site survey", null, true);
            add("WIFI", 20, "Compra de equipos", null, true);
            add("WIFI", 30, "Instalación APs", null, true);
            add("WIFI", 40, "Configuración (VLAN/SSID)", null, true);
            add("WIFI", 50, "Pruebas", null, true);
            add("WIFI", 60, "Entrega", null, true);
            add("WIFI", 70, "Facturación", null, true);
            add("WIFI", 80, "Cobranza", null, true);
            add("WIFI", 90, "Cierre", null, true);

            // Venta equipos / refacciones (más simple)
            add("VENTA_EQUIPOS", 10, "Cotización", null, false);
            add("VENTA_EQUIPOS", 20, "Compra / surtido", null, true);
            add("VENTA_EQUIPOS", 30, "Entrega", null, true);
            add("VENTA_EQUIPOS", 40, "Facturación", null, true);
            add("VENTA_EQUIPOS", 50, "Cobranza", null, true);
            add("VENTA_EQUIPOS", 60, "Cierre", null, true);

            add("VENTA_REFACCIONES", 10, "Cotización", null, false);
            add("VENTA_REFACCIONES", 20, "Compra / surtido", null, true);
            add("VENTA_REFACCIONES", 30, "Entrega", null, true);
            add("VENTA_REFACCIONES", 40, "Facturación", null, true);
            add("VENTA_REFACCIONES", 50, "Cobranza", null, true);
            add("VENTA_REFACCIONES", 60, "Cierre", null, true);

            return pasos;
        }
    }
}

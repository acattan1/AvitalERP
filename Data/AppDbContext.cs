using Microsoft.EntityFrameworkCore;
using AvitalERP.Models;

namespace AvitalERP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        // Aquí agregaremos más DbSet después: Proveedores, Facturas, etc.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Índice único para RFC (no puede haber RFC duplicados)
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Rfc)
                .IsUnique();

            // Configuración por defecto
            modelBuilder.Entity<Cliente>()
                .Property(c => c.FechaRegistro)
                .HasDefaultValueSql("GETDATE()");

            base.OnModelCreating(modelBuilder);
        }
    }
}
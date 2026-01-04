using AvitalERP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvitalERP.Data
{
    // IMPORTANTE: Identity debe usar TU AppUser aquí
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ===== DbSets del ERP =====
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Proyecto> Proyectos => Set<Proyecto>();

        // HubSpot
        public DbSet<HubspotSyncState> HubspotSyncStates => Set<HubspotSyncState>();
        public DbSet<HubspotCompanyLink> HubspotCompanyLinks => Set<HubspotCompanyLink>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Proyecto: HubspotDealId único (idempotencia)
            builder.Entity<Proyecto>()
                .HasIndex(p => p.HubspotDealId)
                .IsUnique();

            // Relación Proyecto -> Cliente (opcional)
            builder.Entity<Proyecto>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

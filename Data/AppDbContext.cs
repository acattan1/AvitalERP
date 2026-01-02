using AvitalERP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvitalERP.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;

        public DbSet<Proyecto> Proyectos { get; set; } = null!;
        public DbSet<HubspotSyncState> HubspotSyncStates { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Rfc)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .Property(c => c.FechaRegistro)
                .HasDefaultValueSql("GETDATE()");


            modelBuilder.Entity<Proyecto>()
    .HasIndex(p => p.HubspotDealId)
    .IsUnique();

            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => p.Folio)
                .IsUnique();

            modelBuilder.Entity<Proyecto>()
                .Property(p => p.Estado)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Si agregaste HubspotCompanyId en Cliente, agrega esto (ver siguiente punto)
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.HubspotCompanyId)
                .IsUnique()
                .HasFilter("[HubspotCompanyId] <> ''");



            modelBuilder.Entity<Cliente>()

                    .HasIndex(c => c.HubspotCompanyId)
    .IsUnique()
    .HasFilter("[HubspotCompanyId] <> ''");



        }
    }
}

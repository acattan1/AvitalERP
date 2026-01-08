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

        // Operación (MVP)
        public DbSet<Tecnico> Tecnicos => Set<Tecnico>();
        public DbSet<ProyectoCategoria> ProyectoCategorias => Set<ProyectoCategoria>();
        public DbSet<ProyectoCategoriaAsignada> ProyectoCategoriasAsignadas => Set<ProyectoCategoriaAsignada>();
        public DbSet<ProyectoTipo> ProyectoTipos => Set<ProyectoTipo>();
        public DbSet<ProyectoTipoPasoPlantilla> ProyectoTipoPasoPlantillas => Set<ProyectoTipoPasoPlantilla>();
        public DbSet<ProyectoTipoAsignado> ProyectoTiposAsignados => Set<ProyectoTipoAsignado>();
        public DbSet<ProyectoPaso> ProyectoPasos => Set<ProyectoPaso>();
        public DbSet<ProyectoPasoEvidencia> ProyectoPasoEvidencias => Set<ProyectoPasoEvidencia>();

        // Compras (CFDI)
        public DbSet<CfdiDocumento> CfdiDocumentos => Set<CfdiDocumento>();
        public DbSet<CfdiConcepto> CfdiConceptos => Set<CfdiConcepto>();

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

            builder.Entity<Proyecto>()
                .Property(p => p.Estado)
                .HasMaxLength(30);

            builder.Entity<CfdiConcepto>()
                .Property(c => c.DestinoTipo)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Relaciones Operación
            builder.Entity<ProyectoTipoAsignado>()
                .HasOne(x => x.Proyecto)
                .WithMany()
                .HasForeignKey(x => x.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProyectoCategoriaAsignada>()
                .HasOne(x => x.Proyecto)
                .WithMany()
                .HasForeignKey(x => x.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProyectoTipoAsignado>()
                .HasOne(x => x.ProyectoTipo)
                .WithMany()
                .HasForeignKey(x => x.ProyectoTipoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProyectoCategoriaAsignada>()
                .HasOne(x => x.ProyectoCategoria)
                .WithMany()
                .HasForeignKey(x => x.ProyectoCategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProyectoPaso>()
                .HasOne(p => p.Tecnico)
                .WithMany()
                .HasForeignKey(p => p.TecnicoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProyectoPasoEvidencia>()
                .HasOne(e => e.ProyectoPaso)
                .WithMany()
                .HasForeignKey(e => e.ProyectoPasoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Compras (CFDI)
            builder.Entity<CfdiDocumento>()
                .HasOne(d => d.Proyecto)
                .WithMany()
                .HasForeignKey(d => d.ProyectoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CfdiConcepto>()
                .HasOne(c => c.Proyecto)
                .WithMany()
                .HasForeignKey(c => c.ProyectoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Unicidad simple
            builder.Entity<ProyectoTipo>()
                .HasIndex(t => t.Codigo)
                .IsUnique();

            builder.Entity<ProyectoCategoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            builder.Entity<CfdiDocumento>()
                .HasIndex(d => d.UUID)
                .IsUnique(false);


        }
    }
}
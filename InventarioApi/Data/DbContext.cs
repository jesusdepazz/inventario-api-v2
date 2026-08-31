using InventarioApi.Models;
using InventarioApi.Models.Suministros;
using Inventory.Models;
using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data
{
    public class InventarioContext : DbContext
    {
        public InventarioContext(DbContextOptions<InventarioContext> options) : base(options) { }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<EmpleadoInfo> EmpleadosInfo { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }
        public DbSet<AsignacionComunal> AsignacionesComunales { get; set; }
        public DbSet<AsignacionComunalVersion> AsignacionComunalVersiones { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<HojaSolvencia> Solvencias { get; set; }
        public DbSet<HojaResponsabilidad> HojasResponsabilidad { get; set; }
        public DbSet<HojaEmpleado> HojaEmpleados { get; set; }
        public DbSet<HojaEquipo> HojaEquipos { get; set; }
        public DbSet<Traslado> Traslados { get; set; }
        public DbSet<TrasladoEquipo> TrasladoEquipos { get; set; }
        public DbSet<TrasladoEmpleadoEntrega> TrasladoEmpleadoEntregas { get; set; }
        public DbSet<TrasladoEmpleadoRecibe> TrasladoEmpleadoRecibes { get; set; }
        public DbSet<Suministro> Suministros { get; set; }
        public DbSet<EntradaSuministro> EntradaSuministros { get; set; }
        public DbSet<SalidaSuministro> SalidaSuministros { get; set; }
        public DbSet<BajaActivo> BajaActivos { get; set; }
        public DbSet<EmpleadoExterno> EmpleadosExternos { get; set; }
        public DbSet<TrasladoRetorno> TrasladoRetornos { get; set; }
        public DbSet<TrasladoRetornoEquipo> TrasladoRetornEquipos { get; set; }
        public DbSet<TrasladoRetornoEmpleado> TrasladoRetornoEmpleados { get; set; }
        public DbSet<HojaResponsabilidadVersion> HojaResponsabilidadVersiones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tablas de Softland — solo lectura, no pertenecen a esta BD
            modelBuilder.Entity<EmpleadoInfo>(entity =>
            {
                entity.ToTable("Empleado", t => t.ExcludeFromMigrations());
                entity.HasKey(e => e.Empleado);
            });

            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("departamento", t => t.ExcludeFromMigrations());
                entity.HasKey(d => d.Codigo);
            });

            modelBuilder.Entity<HojaEmpleado>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.EmpleadoId).IsRequired();

                entity.HasOne(h => h.HojaResponsabilidad)
                      .WithMany(h => h.Empleados)
                      .HasForeignKey(h => h.HojaResponsabilidadId);
            });

            modelBuilder.Entity<HojaEquipo>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Codificacion).IsRequired();

                entity.HasOne(h => h.HojaResponsabilidad)
                      .WithMany(h => h.Equipos)
                      .HasForeignKey(h => h.HojaResponsabilidadId);
            });

            modelBuilder.Entity<HojaSolvencia>()
            .HasOne(s => s.HojaResponsabilidad)
            .WithMany(h => h.Solvencias)
            .HasForeignKey(s => s.HojaResponsabilidadId);

            modelBuilder.Entity<HojaResponsabilidadVersion>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.ToTable("HojaResponsabilidadVersiones", t => t.ExcludeFromMigrations());
                entity.HasOne(v => v.HojaResponsabilidad)
                      .WithMany()
                      .HasForeignKey(v => v.HojaResponsabilidadId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AsignacionComunalVersion>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasOne(v => v.AsignacionComunal)
                      .WithMany()
                      .HasForeignKey(v => v.AsignacionComunalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Suministro>()
                .HasMany(s => s.Entradas)
                .WithOne(e => e.Suministro)
                .HasForeignKey(e => e.SuministroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Suministro>()
                .HasMany(s => s.Salidas)
                .WithOne(sal => sal.Suministro)
                .HasForeignKey(sal => sal.SuministroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrasladoRetorno>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasMany(t => t.Equipos)
                      .WithOne(d => d.TrasladoRetorno)
                      .HasForeignKey(d => d.TrasladoRetornoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TrasladoRetornoEquipo>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.Equipo)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.HasOne(d => d.TrasladoRetorno)
                      .WithMany(t => t.Equipos)
                      .HasForeignKey(d => d.TrasladoRetornoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TrasladoRetornoEmpleado>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EmpleadoId)
                      .IsRequired();

                entity.HasOne(e => e.TrasladoRetorno)
                      .WithMany(t => t.Empleados)
                      .HasForeignKey(e => e.TrasladoRetornoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Traslado>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasMany(t => t.Equipos)
                      .WithOne()
                      .HasForeignKey(e => e.TrasladoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.EmpleadoEntrega)
                      .WithOne()
                      .HasForeignKey<TrasladoEmpleadoEntrega>(e => e.TrasladoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.EmpleadoRecibe)
                      .WithOne()
                      .HasForeignKey<TrasladoEmpleadoRecibe>(e => e.TrasladoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

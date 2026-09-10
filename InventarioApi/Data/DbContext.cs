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

        public DbSet<MobiliarioEquipo> MobiliarioEquipos { get; set; }
        public DbSet<ReporteDanio> ReportesDanios { get; set; }

        // Módulo de Inmuebles
        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<InmuebleArchivo> InmuebleArchivos { get; set; }
        public DbSet<PolizaSeguro> PolizasSeguro { get; set; }

        // Módulo de Vehículos
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<HistorialReparacionVehiculo> HistorialReparaciones { get; set; }
        public DbSet<MantenimientoVehiculo> MantenimientosVehiculo { get; set; }
        public DbSet<AlertaServicioVehiculo> AlertasServicioVehiculo { get; set; }
        public DbSet<BitacoraFallaVehiculo> BitacorasFallasVehiculo { get; set; }
        public DbSet<PolizaSeguroVehiculo> PolizasSeguroVehiculo { get; set; }
        public DbSet<ReporteEstadoFisicoVehiculo> ReportesEstadoFisicoVehiculo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================== Configuración para OtrosActivosController / Equipos ==================
            modelBuilder.Entity<Equipo>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Índices únicos o de optimización basados en las validaciones del controlador (AnyAsync)
                entity.HasIndex(e => e.Codificacion).IsUnique();

                // Si la serie puede ser nula o vacía en algunos registros, se puede configurar adecuadamente, 
                // pero aquí dejamos el mapeo base y restricciones comunes.
                entity.Property(e => e.Codificacion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Serie).HasMaxLength(100);
            });

            // ================== Configuración Módulo Inmuebles ==================
            modelBuilder.Entity<Inmueble>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.HasMany(i => i.Archivos)
                      .WithOne(a => a.Inmueble)
                      .HasForeignKey(a => a.InmuebleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(i => i.Polizas)
                      .WithOne(p => p.Inmueble)
                      .HasForeignKey(p => p.InmuebleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración MobiliarioEquipo y ReporteDanio
            modelBuilder.Entity<MobiliarioEquipo>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasMany(m => m.ReportesDanios)
                      .WithOne(r => r.MobiliarioEquipo)
                      .HasForeignKey(r => r.MobiliarioEquipoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ================== Módulo de Vehículos ==================
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.HasIndex(v => v.Vin);
                entity.HasIndex(v => v.Placa);

                entity.Property(v => v.Vin).HasMaxLength(17);
                entity.Property(v => v.Placa).HasMaxLength(20);
                entity.Property(v => v.Color).HasMaxLength(50);
                entity.Property(v => v.TipoCombustible).HasMaxLength(30);

                entity.HasMany(v => v.HistorialReparaciones)
                      .WithOne(r => r.Vehiculo)
                      .HasForeignKey(r => r.VehiculoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.Mantenimientos)
                      .WithOne(m => m.Vehiculo)
                      .HasForeignKey(m => m.VehiculoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.AlertasServicio)
                      .WithOne(a => a.Vehiculo)
                      .HasForeignKey(a => a.VehiculoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.BitacoraFallas)
                      .WithOne(f => f.Vehiculo)
                      .HasForeignKey(f => f.VehiculoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.PolizasSeguro)
                      .WithOne(p => p.Vehiculo)
                      .HasForeignKey(p => p.VehiculoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.ReportesEstadoFisico)
                      .WithOne(r => r.Vehiculo)
                      .HasForeignKey(r => r.VehiculoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HistorialReparacionVehiculo>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Costo).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PolizaSeguroVehiculo>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Prima).HasColumnType("decimal(18,2)");
            });

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

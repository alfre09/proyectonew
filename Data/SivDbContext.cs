using Microsoft.EntityFrameworkCore;
using proyectonew.Models;

namespace proyectonew.Data
{
    public class SivDbContext : DbContext
    {
        public SivDbContext(DbContextOptions<SivDbContext> options) : base(options)
        {
        }

        // Tablas existentes
        public DbSet<Vuelo> Vuelo { get; set; }
        public DbSet<Aerolinea> Aerolineas { get; set; }
        public DbSet<Aeropuerto> Aeropuertos { get; set; }
        public DbSet<EstadoVuelo> EstadosVuelo { get; set; }

        // Nuevas tablas
        public DbSet<Seguimiento> Seguimientos { get; set; }
        public DbSet<CambioOperativo> CambiosOperativos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<HistorialEstadoVuelo> HistorialEstados { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vuelo>().ToTable("Vuelos");

            base.OnModelCreating(modelBuilder);
        }
    }
}
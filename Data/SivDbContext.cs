using Microsoft.EntityFrameworkCore;
using proyectonew.Models;

namespace proyectonew.Data
{
    public class SivDbContext : DbContext
    {
        public SivDbContext(DbContextOptions<SivDbContext> options) : base(options)
        {
        }

        public DbSet<Vuelo> Vuelo { get; set; }
        public DbSet<Aerolinea> Aerolineas { get; set; }
        public DbSet<Aeropuerto> Aeropuertos { get; set; }
        public DbSet<EstadoVuelo> EstadosVuelo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vuelo>().ToTable("Vuelos");
        }
    }
}

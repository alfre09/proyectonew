using Microsoft.EntityFrameworkCore;
using proyectonew.Models;
using System.Collections.Generic;

namespace proyectonew.Data
{
    public class SivDbContext : DbContext
    {
        public SivDbContext(DbContextOptions<SivDbContext> options) : base(options)
        {
        }

        public DbSet<Vuelo> Vuelos { get; set; }
        public DbSet<Aerolinea> Aerolineas { get; set; }
        public DbSet<Aeropuerto> Aeropuertos { get; set; }
        public DbSet<EstadoVuelo> EstadosVuelo { get; set; }
    }
}
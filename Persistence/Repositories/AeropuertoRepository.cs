using Microsoft.EntityFrameworkCore;
using proyectonew.Data;
using proyectonew.Models;
using proyectonew.Persistence.Base;
using proyectonew.Persistence.Interfaces;

namespace proyectonew.Persistence.Repositories
{
    public class AeropuertoRepository : BaseRepository<Aeropuerto>, IAeropuertoRepository
    {
        public AeropuertoRepository(SivDbContext context) : base(context)
        {
        }

        public async Task<Aeropuerto?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Codigo == codigo);
        }

        public async Task<List<Aeropuerto>> ObtenerPorPaisAsync(string pais)
        {
            return await _dbSet
                .Where(a => a.Pais == pais)
                .ToListAsync();
        }
    }
}

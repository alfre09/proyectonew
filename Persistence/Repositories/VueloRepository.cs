using Microsoft.EntityFrameworkCore;
using proyectonew.Data;
using proyectonew.Models;
using proyectonew.Persistence.Base;
using proyectonew.Persistence.Interfaces;

namespace proyectonew.Persistence.Repositories
{
    public class VueloRepository : BaseRepository<Vuelo>, IVueloRepository
    {
        public VueloRepository(SivDbContext context) : base(context)
        {
        }

        public async Task<List<Vuelo>> ObtenerPorAerolineaAsync(int aerolineaId)
        {
            return await _dbSet
                .Where(v => v.AerolineaId == aerolineaId)
                .ToListAsync();
        }

        public async Task<List<Vuelo>> ObtenerPorEstadoAsync(int estadoVueloId)
        {
            return await _dbSet
                .Where(v => v.EstadoVueloId == estadoVueloId)
                .ToListAsync();
        }

        public async Task<Vuelo?> ObtenerPorNumeroVueloAsync(string numeroVuelo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(v => v.NumeroVuelo == numeroVuelo);
        }
    }
}

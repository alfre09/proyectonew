using Microsoft.EntityFrameworkCore;
using proyectonew.Data;
using proyectonew.Models;
using proyectonew.Persistence.Base;
using proyectonew.Persistence.Interfaces;

namespace proyectonew.Persistence.Repositories
{
    public class AerolineaRepository : BaseRepository<Aerolinea>, IAerolineaRepository
    {
        public AerolineaRepository(SivDbContext context) : base(context)
        {
        }

        public async Task<Aerolinea?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Codigo == codigo);
        }
    }
}

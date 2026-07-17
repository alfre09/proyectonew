using Microsoft.EntityFrameworkCore;
using proyectonew.Data;
using proyectonew.Models;
using proyectonew.Persistence.Base;
using proyectonew.Persistence.Interfaces;

namespace proyectonew.Persistence.Repositories
{
    public class AuditoriaRepository : BaseRepository<Auditoria>, IAuditoriaRepository
    {
        public AuditoriaRepository(SivDbContext context) : base(context)
        {
        }

        public async Task<List<Auditoria>> ObtenerPorTablaAsync(string tabla)
        {
            return await _dbSet
                .Where(a => a.Tabla == tabla)
                .ToListAsync();
        }
    }
}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using proyectonew.Data;

namespace proyectonew.Persistence.Base
{
    /// <summary>
    /// Implementación genérica de las operaciones CRUD sobre el DbContext.
    /// Los repositorios concretos (VueloRepository, AerolineaRepository, etc.)
    /// heredan de esta clase y solo agregan las consultas específicas de su entidad.
    /// </summary>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly SivDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(SivDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> ObtenerPorIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> ObtenerTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<List<TEntity>> BuscarAsync(Expression<Func<TEntity, bool>> filtro)
        {
            return await _dbSet.Where(filtro).ToListAsync();
        }

        public virtual async Task<TEntity> AgregarAsync(TEntity entidad)
        {
            await _dbSet.AddAsync(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public virtual async Task ActualizarAsync(TEntity entidad)
        {
            _dbSet.Update(entidad);
            await _context.SaveChangesAsync();
        }

        public virtual async Task EliminarAsync(int id)
        {
            var entidad = await _dbSet.FindAsync(id);
            if (entidad != null)
            {
                _dbSet.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExisteAsync(Expression<Func<TEntity, bool>> filtro)
        {
            return await _dbSet.AnyAsync(filtro);
        }
    }
}

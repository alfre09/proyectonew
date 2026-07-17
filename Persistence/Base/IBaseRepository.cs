using System.Linq.Expressions;

namespace proyectonew.Persistence.Base
{
    /// <summary>
    /// Contrato genérico con las operaciones básicas de acceso a datos
    /// que cualquier repositorio de una entidad debe implementar.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad (Vuelo, Aerolinea, etc.)</typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> ObtenerPorIdAsync(int id);
        Task<List<TEntity>> ObtenerTodosAsync();
        Task<List<TEntity>> BuscarAsync(Expression<Func<TEntity, bool>> filtro);
        Task<TEntity> AgregarAsync(TEntity entidad);
        Task ActualizarAsync(TEntity entidad);
        Task EliminarAsync(int id);
        Task<bool> ExisteAsync(Expression<Func<TEntity, bool>> filtro);
    }
}

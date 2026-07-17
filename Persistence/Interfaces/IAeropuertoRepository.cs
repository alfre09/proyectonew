using proyectonew.Models;
using proyectonew.Persistence.Base;

namespace proyectonew.Persistence.Interfaces
{
    public interface IAeropuertoRepository : IBaseRepository<Aeropuerto>
    {
        Task<Aeropuerto?> ObtenerPorCodigoAsync(string codigo);
        Task<List<Aeropuerto>> ObtenerPorPaisAsync(string pais);
    }
}

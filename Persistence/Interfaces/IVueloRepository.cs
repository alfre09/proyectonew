using proyectonew.Models;
using proyectonew.Persistence.Base;

namespace proyectonew.Persistence.Interfaces
{
    public interface IVueloRepository : IBaseRepository<Vuelo>
    {
        Task<List<Vuelo>> ObtenerPorAerolineaAsync(int aerolineaId);
        Task<List<Vuelo>> ObtenerPorEstadoAsync(int estadoVueloId);
        Task<Vuelo?> ObtenerPorNumeroVueloAsync(string numeroVuelo);
    }
}

using proyectonew.Models;
using proyectonew.Persistence.Base;

namespace proyectonew.Persistence.Interfaces
{
    public interface IAerolineaRepository : IBaseRepository<Aerolinea>
    {
        Task<Aerolinea?> ObtenerPorCodigoAsync(string codigo);
    }
}

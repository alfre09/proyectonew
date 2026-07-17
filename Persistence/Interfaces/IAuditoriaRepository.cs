using proyectonew.Models;
using proyectonew.Persistence.Base;

namespace proyectonew.Persistence.Interfaces
{
    public interface IAuditoriaRepository : IBaseRepository<Auditoria>
    {
        Task<List<Auditoria>> ObtenerPorTablaAsync(string tabla);
    }
}

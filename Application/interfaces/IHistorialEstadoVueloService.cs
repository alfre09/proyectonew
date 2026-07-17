using proyectonew.Application.Dtos;

namespace proyectonew.Application.Interfaces
{
    public interface IHistorialEstadoVueloService
    {
        Task<List<HistorialEstadoVueloDto>> ObtenerPorVueloAsync(int vueloId);

        Task RegistrarCambioDeEstadoAsync(int vueloId, int estadoAnteriorId, int estadoNuevoId);
    }
}

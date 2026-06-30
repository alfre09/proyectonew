using proyectonew.Application.Dtos;

namespace proyectonew.Application.Interfaces
{
    public interface IVueloService
    {
        Task<List<VueloDto>> ObtenerTodosAsync();

        Task<VueloDto?> ObtenerPorIdAsync(int id);

        Task CrearAsync(VueloDto vueloDto);

        Task ActualizarAsync(VueloDto vueloDto);

        Task EliminarAsync(int id);
    }
}
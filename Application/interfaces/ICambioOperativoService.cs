using proyectonew.Application.Dtos;

namespace proyectonew.Application.Interfaces
{
    public interface ICambioOperativoService
    {
        Task<List<CambioOperativoDto>> ObtenerTodosAsync();
        Task<List<CambioOperativoDto>> ObtenerPorVueloAsync(int vueloId);
        Task<CambioOperativoDto> RegistrarCambioAsync(CambioOperativoDto dto);
    }
}

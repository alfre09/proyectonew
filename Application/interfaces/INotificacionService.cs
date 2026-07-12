using proyectonew.Application.Dtos;

namespace proyectonew.Application.Interfaces
{
    public interface INotificacionService
    {
        Task<List<NotificacionDto>> ObtenerTodosAsync();
        Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(string usuario);
        Task<List<NotificacionDto>> ObtenerPorVueloAsync(int vueloId);
        Task MarcarComoLeidaAsync(int notificacionId);

        // Usado internamente por CambioOperativoService cuando se registra un cambio.
        // Regla: "Solo los usuarios interesados en un vuelo deben recibir notificaciones relacionadas."
        Task GenerarNotificacionesPorCambioAsync(int vueloId, string mensaje);
    }
}

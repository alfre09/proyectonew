using proyectonew.Application.Dtos;

namespace proyectonew.Application.Interfaces
{
    public interface IAuditoriaService
    {
        // Regla de negocio: "Toda acción relevante debe quedar registrada"
        // (registro/modificación de vuelos, cambios operativos, seguimiento, notificaciones).
        Task RegistrarAsync(string accion, string tabla, string descripcion);

        Task<List<AuditoriaDto>> ObtenerTodosAsync();

        Task<List<AuditoriaDto>> ObtenerPorTablaAsync(string tabla);
    }
}

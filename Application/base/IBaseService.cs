namespace proyectonew.Application.Base
{
    public interface IBaseService<TDto>
    {
        Task<List<TDto>> ObtenerTodosAsync();
        Task<TDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(TDto dto);
        Task ActualizarAsync(TDto dto);
        Task EliminarAsync(int id);
    }
}
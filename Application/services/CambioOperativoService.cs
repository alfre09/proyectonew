using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class CambioOperativoService : ICambioOperativoService
    {
        private readonly SivDbContext _context;
        private readonly INotificacionService _notificacionService;
        private readonly IHistorialEstadoVueloService _historialService;

        private const string ESTADO_CANCELADO = "Cancelado";

        public CambioOperativoService(
            SivDbContext context,
            INotificacionService notificacionService,
            IHistorialEstadoVueloService historialService)
        {
            _context = context;
            _notificacionService = notificacionService;
            _historialService = historialService;
        }

        public async Task<List<CambioOperativoDto>> ObtenerTodosAsync()
        {
            return await _context.CambiosOperativos
                .OrderByDescending(c => c.FechaCambio)
                .Select(c => new CambioOperativoDto
                {
                    CambioOperativoId = c.CambioOperativoId,
                    VueloId = c.VueloId,
                    TipoCambio = c.TipoCambio,
                    Causa = c.Causa,
                    FechaCambio = c.FechaCambio
                })
                .ToListAsync();
        }

        public async Task<List<CambioOperativoDto>> ObtenerPorVueloAsync(int vueloId)
        {
            return await _context.CambiosOperativos
                .Where(c => c.VueloId == vueloId)
                .OrderByDescending(c => c.FechaCambio)
                .Select(c => new CambioOperativoDto
                {
                    CambioOperativoId = c.CambioOperativoId,
                    VueloId = c.VueloId,
                    TipoCambio = c.TipoCambio,
                    Causa = c.Causa,
                    FechaCambio = c.FechaCambio
                })
                .ToListAsync();
        }

        public async Task<CambioOperativoDto> RegistrarCambioAsync(CambioOperativoDto dto)
        {
            var vuelo = await _context.Vuelo
                .Include(v => v.EstadoVuelo)
                .FirstOrDefaultAsync(v => v.VueloId == dto.VueloId);

            if (vuelo == null)
                throw new InvalidOperationException($"No existe un vuelo con id {dto.VueloId}.");

            // Regla: "Un vuelo cancelado no puede continuar su ciclo operativo."
            if (vuelo.EstadoVuelo != null &&
                vuelo.EstadoVuelo.Nombre.Equals(ESTADO_CANCELADO, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "El vuelo ya está cancelado. No se pueden registrar más cambios operativos sobre él.");
            }

            if (string.IsNullOrWhiteSpace(dto.Causa))
                throw new InvalidOperationException("Todo cambio operativo debe tener una causa identificable.");

            int estadoAnteriorId = vuelo.EstadoVueloId;
            string valorAnterior;
            string valorNuevo;

            switch (dto.TipoCambio)
            {
                case "Retraso":
                case "Adelanto":
                    if (dto.NuevoHorario == null)
                        throw new InvalidOperationException("Debes indicar el nuevo horario para un retraso o adelanto.");

                    valorAnterior = vuelo.HorarioProgramado.ToString("g");
                    vuelo.HorarioProgramado = dto.NuevoHorario.Value;
                    valorNuevo = vuelo.HorarioProgramado.ToString("g");
                    break;

                case "CambioPuerta":
                    if (string.IsNullOrWhiteSpace(dto.NuevaPuerta))
                        throw new InvalidOperationException("Debes indicar la nueva puerta.");

                    valorAnterior = vuelo.Puerta ?? "(sin asignar)";
                    vuelo.Puerta = dto.NuevaPuerta;
                    valorNuevo = vuelo.Puerta;
                    break;

                case "Cancelacion":
                    if (dto.NuevoEstadoVueloId == null)
                        throw new InvalidOperationException("Debes indicar el id del estado 'Cancelado'.");

                    valorAnterior = vuelo.EstadoVuelo?.Nombre ?? vuelo.EstadoVueloId.ToString();
                    vuelo.EstadoVueloId = dto.NuevoEstadoVueloId.Value;
                    valorNuevo = "Cancelado";
                    break;

                default:
                    throw new InvalidOperationException(
                        "Tipo de cambio no reconocido. Usa: Retraso, Adelanto, CambioPuerta o Cancelacion.");
            }

            // Si el cambio implicó un nuevo estado del vuelo, se registra en el historial.
            if (dto.NuevoEstadoVueloId.HasValue && dto.NuevoEstadoVueloId.Value != estadoAnteriorId)
            {
                await _historialService.RegistrarCambioDeEstadoAsync(
                    vuelo.VueloId, estadoAnteriorId, dto.NuevoEstadoVueloId.Value);
            }

            var cambioOperativo = new CambioOperativo
            {
                VueloId = dto.VueloId,
                TipoCambio = dto.TipoCambio,
                Causa = dto.Causa,
                FechaCambio = DateTime.Now
            };

            _context.CambiosOperativos.Add(cambioOperativo);
            await _context.SaveChangesAsync();

            // Regla: "Los cambios relevantes en un vuelo pueden generar notificaciones"
            // y solo llegan a quienes siguen ese vuelo.
            var mensaje = $"El vuelo {vuelo.NumeroVuelo} tuvo un cambio ({dto.TipoCambio}): " +
                           $"{valorAnterior} → {valorNuevo}. Causa: {dto.Causa}";

            await _notificacionService.GenerarNotificacionesPorCambioAsync(vuelo.VueloId, mensaje);

            return new CambioOperativoDto
            {
                CambioOperativoId = cambioOperativo.CambioOperativoId,
                VueloId = cambioOperativo.VueloId,
                TipoCambio = cambioOperativo.TipoCambio,
                Causa = cambioOperativo.Causa,
                FechaCambio = cambioOperativo.FechaCambio
            };
        }
    }
}

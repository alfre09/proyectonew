using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Base;
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
        private readonly IAuditoriaService _auditoriaService;

        public CambioOperativoService(
            SivDbContext context,
            INotificacionService notificacionService,
            IHistorialEstadoVueloService historialService,
            IAuditoriaService auditoriaService)
        {
            _context = context;
            _notificacionService = notificacionService;
            _historialService = historialService;
            _auditoriaService = auditoriaService;
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

            if (string.IsNullOrWhiteSpace(dto.Causa))
                throw new InvalidOperationException("Todo cambio operativo debe tener una causa identificable.");

            // Regla: "Un vuelo cancelado no puede continuar su ciclo operativo"
            // (aplica también a un vuelo ya aterrizado: su ciclo también terminó).
            if (CicloEstadosVuelo.EsEstadoFinal(vuelo.EstadoVuelo?.Nombre ?? string.Empty))
                throw new InvalidOperationException(
                    $"El vuelo está en estado '{vuelo.EstadoVuelo?.Nombre}' y no se pueden registrar más cambios operativos sobre él.");

            int estadoAnteriorId = vuelo.EstadoVueloId;
            string valorAnterior;
            string valorNuevo;
            EstadoVuelo? estadoNuevo = null;

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

                case "CambioEstado":
                case "Cancelacion":
                    if (dto.NuevoEstadoVueloId == null)
                        throw new InvalidOperationException("Debes indicar el id del nuevo estado del vuelo.");

                    estadoNuevo = await _context.EstadosVuelo.FindAsync(dto.NuevoEstadoVueloId.Value);

                    if (estadoNuevo == null)
                        throw new InvalidOperationException($"No existe el estado con id {dto.NuevoEstadoVueloId}.");

                    // Regla: "No se puede avanzar a un estado sin cumplir las condiciones
                    // del estado anterior." Aquí se valida la secuencia completa, no solo cancelación.
                    if (dto.TipoCambio == "Cancelacion" &&
                        !estadoNuevo.Nombre.Equals(CicloEstadosVuelo.ESTADO_CANCELADO, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException(
                            $"Para el tipo 'Cancelacion' el nuevo estado debe ser '{CicloEstadosVuelo.ESTADO_CANCELADO}'.");

                    CicloEstadosVuelo.ValidarTransicion(vuelo.EstadoVuelo?.Nombre ?? string.Empty, estadoNuevo.Nombre);

                    valorAnterior = vuelo.EstadoVuelo?.Nombre ?? vuelo.EstadoVueloId.ToString();
                    vuelo.EstadoVueloId = estadoNuevo.EstadoVueloId;
                    valorNuevo = estadoNuevo.Nombre;
                    break;

                default:
                    throw new InvalidOperationException(
                        "Tipo de cambio no reconocido. Usa: Retraso, Adelanto, CambioPuerta, CambioEstado o Cancelacion.");
            }

            // Si el cambio implicó un nuevo estado del vuelo, se registra en el historial.
            if (estadoNuevo != null && estadoNuevo.EstadoVueloId != estadoAnteriorId)
            {
                await _historialService.RegistrarCambioDeEstadoAsync(
                    vuelo.VueloId, estadoAnteriorId, estadoNuevo.EstadoVueloId);
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

            await _auditoriaService.RegistrarAsync(
                "Registrar", "CambiosOperativos",
                $"Vuelo {vuelo.NumeroVuelo} (id {vuelo.VueloId}): {dto.TipoCambio} de '{valorAnterior}' a '{valorNuevo}'. Causa: {dto.Causa}");

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

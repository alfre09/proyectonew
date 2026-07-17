using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Base;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class VueloService : IVueloService
    {
        private readonly SivDbContext _context;
        private readonly IAuditoriaService _auditoriaService;

        public VueloService(SivDbContext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<VueloDto>> ObtenerTodosAsync()
        {
            return await _context.Vuelo
                .Select(v => new VueloDto
                {
                    VueloId = v.VueloId,
                    NumeroVuelo = v.NumeroVuelo,
                    AerolineaId = v.AerolineaId,
                    AeropuertoOrigenId = v.AeropuertoOrigenId,
                    AeropuertoDestinoId = v.AeropuertoDestinoId,
                    HorarioProgramado = v.HorarioProgramado,
                    Puerta = v.Puerta,
                    EstadoVueloId = v.EstadoVueloId,
                    FechaCreacion = v.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<VueloDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Vuelo
                .Where(v => v.VueloId == id)
                .Select(v => new VueloDto
                {
                    VueloId = v.VueloId,
                    NumeroVuelo = v.NumeroVuelo,
                    AerolineaId = v.AerolineaId,
                    AeropuertoOrigenId = v.AeropuertoOrigenId,
                    AeropuertoDestinoId = v.AeropuertoDestinoId,
                    HorarioProgramado = v.HorarioProgramado,
                    Puerta = v.Puerta,
                    EstadoVueloId = v.EstadoVueloId,
                    FechaCreacion = v.FechaCreacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task CrearAsync(VueloDto vueloDto)
        {
            await ValidarReferenciasAsync(vueloDto);

            // Regla: "No se permite la existencia de vuelos activos sin una programación válida."
            // Todo vuelo nuevo debe iniciar en el primer estado del ciclo operativo (Programado).
            var estadoInicial = await _context.EstadosVuelo.FindAsync(vueloDto.EstadoVueloId);

            if (estadoInicial == null)
                throw new InvalidOperationException($"No existe el estado de vuelo con id {vueloDto.EstadoVueloId}.");

            if (!estadoInicial.Nombre.Equals(CicloEstadosVuelo.Secuencia[0], StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Todo vuelo nuevo debe registrarse en estado '{CicloEstadosVuelo.Secuencia[0]}'.");

            if (string.IsNullOrWhiteSpace(vueloDto.NumeroVuelo))
                throw new InvalidOperationException("El vuelo debe tener un número de vuelo.");

            if (vueloDto.AeropuertoOrigenId == vueloDto.AeropuertoDestinoId)
                throw new InvalidOperationException("El aeropuerto de origen y destino no pueden ser el mismo.");

            var vuelo = new Vuelo
            {
                NumeroVuelo = vueloDto.NumeroVuelo,
                AerolineaId = vueloDto.AerolineaId,
                AeropuertoOrigenId = vueloDto.AeropuertoOrigenId,
                AeropuertoDestinoId = vueloDto.AeropuertoDestinoId,
                HorarioProgramado = vueloDto.HorarioProgramado,
                Puerta = vueloDto.Puerta,
                EstadoVueloId = vueloDto.EstadoVueloId,
                FechaCreacion = DateTime.Now
            };

            _context.Vuelo.Add(vuelo);
            await _context.SaveChangesAsync();

            vueloDto.VueloId = vuelo.VueloId;
            vueloDto.FechaCreacion = vuelo.FechaCreacion;

            await _auditoriaService.RegistrarAsync(
                "Crear", "Vuelos", $"Se registró el vuelo {vuelo.NumeroVuelo} (id {vuelo.VueloId}).");
        }

        public async Task ActualizarAsync(VueloDto vueloDto)
        {
            var vuelo = await _context.Vuelo.FindAsync(vueloDto.VueloId);

            if (vuelo == null)
                throw new InvalidOperationException($"No existe un vuelo con id {vueloDto.VueloId}.");

            await ValidarReferenciasAsync(vueloDto);

            if (vueloDto.AeropuertoOrigenId == vueloDto.AeropuertoDestinoId)
                throw new InvalidOperationException("El aeropuerto de origen y destino no pueden ser el mismo.");

            vuelo.NumeroVuelo = vueloDto.NumeroVuelo;
            vuelo.AerolineaId = vueloDto.AerolineaId;
            vuelo.AeropuertoOrigenId = vueloDto.AeropuertoOrigenId;
            vuelo.AeropuertoDestinoId = vueloDto.AeropuertoDestinoId;
            vuelo.HorarioProgramado = vueloDto.HorarioProgramado;
            vuelo.Puerta = vueloDto.Puerta;

            // El estado del vuelo NO se cambia desde aquí: eso es responsabilidad de
            // CambioOperativoService, que valida la transición y deja historial + notificaciones.
            // (Regla: "Toda modificación a la programación debe quedar registrada".)

            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Actualizar", "Vuelos", $"Se actualizó la programación del vuelo {vuelo.VueloId}.");
        }

        public async Task EliminarAsync(int id)
        {
            var vuelo = await _context.Vuelo.FindAsync(id);

            if (vuelo == null)
                return;

            _context.Vuelo.Remove(vuelo);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Eliminar", "Vuelos", $"Se eliminó el vuelo {vuelo.NumeroVuelo} (id {vuelo.VueloId}).");
        }

        private async Task ValidarReferenciasAsync(VueloDto dto)
        {
            if (!await _context.Aerolineas.AnyAsync(a => a.AerolineaId == dto.AerolineaId))
                throw new InvalidOperationException($"No existe la aerolínea con id {dto.AerolineaId}.");

            if (!await _context.Aeropuertos.AnyAsync(a => a.AeropuertoId == dto.AeropuertoOrigenId))
                throw new InvalidOperationException($"No existe el aeropuerto de origen con id {dto.AeropuertoOrigenId}.");

            if (!await _context.Aeropuertos.AnyAsync(a => a.AeropuertoId == dto.AeropuertoDestinoId))
                throw new InvalidOperationException($"No existe el aeropuerto de destino con id {dto.AeropuertoDestinoId}.");
        }
    }
}

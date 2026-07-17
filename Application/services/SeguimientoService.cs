using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class SeguimientoService : ISeguimientoService
    {
        private readonly SivDbContext _context;
        private readonly IAuditoriaService _auditoriaService;

        public SeguimientoService(SivDbContext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<SeguimientoDto>> ObtenerTodosAsync()
        {
            return await _context.Seguimientos
                .Select(s => new SeguimientoDto
                {
                    SeguimientoId = s.SeguimientoId,
                    Usuario = s.Usuario,
                    VueloId = s.VueloId,
                    FechaSeguimiento = s.FechaSeguimiento
                })
                .ToListAsync();
        }

        public async Task<SeguimientoDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Seguimientos
                .Where(s => s.SeguimientoId == id)
                .Select(s => new SeguimientoDto
                {
                    SeguimientoId = s.SeguimientoId,
                    Usuario = s.Usuario,
                    VueloId = s.VueloId,
                    FechaSeguimiento = s.FechaSeguimiento
                })
                .FirstOrDefaultAsync();
        }

        public async Task CrearAsync(SeguimientoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Usuario))
                throw new InvalidOperationException("El seguimiento debe estar asociado a un usuario.");

            // Regla: "El seguimiento de un vuelo debe estar asociado a un identificador válido del vuelo."
            var existeVuelo = await _context.Vuelo.AnyAsync(v => v.VueloId == dto.VueloId);

            if (!existeVuelo)
                throw new InvalidOperationException($"No existe un vuelo con id {dto.VueloId}.");

            var yaExiste = await _context.Seguimientos
                .AnyAsync(s => s.VueloId == dto.VueloId && s.Usuario == dto.Usuario);

            if (yaExiste)
                throw new InvalidOperationException("Este usuario ya está siguiendo este vuelo.");

            var seguimiento = new Seguimiento
            {
                Usuario = dto.Usuario,
                VueloId = dto.VueloId,
                FechaSeguimiento = DateTime.Now
            };

            _context.Seguimientos.Add(seguimiento);
            await _context.SaveChangesAsync();

            dto.SeguimientoId = seguimiento.SeguimientoId;
            dto.FechaSeguimiento = seguimiento.FechaSeguimiento;

            await _auditoriaService.RegistrarAsync(
                "Crear", "Seguimientos", $"El usuario {dto.Usuario} inició seguimiento del vuelo {dto.VueloId}.");
        }

        public async Task ActualizarAsync(SeguimientoDto dto)
        {
            var seguimiento = await _context.Seguimientos.FindAsync(dto.SeguimientoId);

            if (seguimiento == null)
                throw new InvalidOperationException($"No existe un seguimiento con id {dto.SeguimientoId}.");

            var existeVuelo = await _context.Vuelo.AnyAsync(v => v.VueloId == dto.VueloId);

            if (!existeVuelo)
                throw new InvalidOperationException($"No existe un vuelo con id {dto.VueloId}.");

            seguimiento.Usuario = dto.Usuario;
            seguimiento.VueloId = dto.VueloId;

            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Actualizar", "Seguimientos", $"Se actualizó el seguimiento {seguimiento.SeguimientoId}.");
        }

        public async Task EliminarAsync(int id)
        {
            var seguimiento = await _context.Seguimientos.FindAsync(id);

            if (seguimiento == null)
                return;

            _context.Seguimientos.Remove(seguimiento);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Eliminar", "Seguimientos",
                $"El usuario {seguimiento.Usuario} dejó de seguir el vuelo {seguimiento.VueloId}.");
        }
    }
}

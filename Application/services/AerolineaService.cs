using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class AerolineaService : IAerolineaService
    {
        private readonly SivDbContext _context;
        private readonly IAuditoriaService _auditoriaService;

        public AerolineaService(SivDbContext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<AerolineaDto>> ObtenerTodosAsync()
        {
            return await _context.Aerolineas
                .OrderBy(a => a.Nombre)
                .Select(a => new AerolineaDto
                {
                    AerolineaId = a.AerolineaId,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre
                })
                .ToListAsync();
        }

        public async Task<AerolineaDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Aerolineas
                .Where(a => a.AerolineaId == id)
                .Select(a => new AerolineaDto
                {
                    AerolineaId = a.AerolineaId,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre
                })
                .FirstOrDefaultAsync();
        }

        public async Task CrearAsync(AerolineaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                throw new InvalidOperationException("La aerolínea debe tener código y nombre.");

            var existeCodigo = await _context.Aerolineas
                .AnyAsync(a => a.Codigo.ToUpper() == dto.Codigo.ToUpper());

            if (existeCodigo)
                throw new InvalidOperationException($"Ya existe una aerolínea con el código '{dto.Codigo}'.");

            var aerolinea = new Aerolinea
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre
            };

            _context.Aerolineas.Add(aerolinea);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Crear", "Aerolineas", $"Se registró la aerolínea {aerolinea.Codigo} - {aerolinea.Nombre}.");
        }

        public async Task ActualizarAsync(AerolineaDto dto)
        {
            var aerolinea = await _context.Aerolineas.FindAsync(dto.AerolineaId);

            if (aerolinea == null)
                throw new InvalidOperationException($"No existe una aerolínea con id {dto.AerolineaId}.");

            aerolinea.Codigo = dto.Codigo;
            aerolinea.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Actualizar", "Aerolineas", $"Se actualizó la aerolínea {aerolinea.AerolineaId}.");
        }

        public async Task EliminarAsync(int id)
        {
            var aerolinea = await _context.Aerolineas.FindAsync(id);

            if (aerolinea == null)
                return;

            var tieneVuelos = await _context.Vuelo.AnyAsync(v => v.AerolineaId == id);

            if (tieneVuelos)
                throw new InvalidOperationException(
                    "No se puede eliminar la aerolínea porque tiene vuelos asociados.");

            _context.Aerolineas.Remove(aerolinea);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Eliminar", "Aerolineas", $"Se eliminó la aerolínea {aerolinea.Codigo} - {aerolinea.Nombre}.");
        }
    }
}

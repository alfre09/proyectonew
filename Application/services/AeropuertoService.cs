using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class AeropuertoService : IAeropuertoService
    {
        private readonly SivDbContext _context;
        private readonly IAuditoriaService _auditoriaService;

        public AeropuertoService(SivDbContext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<AeropuertoDto>> ObtenerTodosAsync()
        {
            return await _context.Aeropuertos
                .OrderBy(a => a.Nombre)
                .Select(a => new AeropuertoDto
                {
                    AeropuertoId = a.AeropuertoId,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    Ciudad = a.Ciudad,
                    Pais = a.Pais
                })
                .ToListAsync();
        }

        public async Task<AeropuertoDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Aeropuertos
                .Where(a => a.AeropuertoId == id)
                .Select(a => new AeropuertoDto
                {
                    AeropuertoId = a.AeropuertoId,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    Ciudad = a.Ciudad,
                    Pais = a.Pais
                })
                .FirstOrDefaultAsync();
        }

        public async Task CrearAsync(AeropuertoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                throw new InvalidOperationException("El aeropuerto debe tener código y nombre.");

            var existeCodigo = await _context.Aeropuertos
                .AnyAsync(a => a.Codigo.ToUpper() == dto.Codigo.ToUpper());

            if (existeCodigo)
                throw new InvalidOperationException($"Ya existe un aeropuerto con el código '{dto.Codigo}'.");

            var aeropuerto = new Aeropuerto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Ciudad = dto.Ciudad,
                Pais = dto.Pais
            };

            _context.Aeropuertos.Add(aeropuerto);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Crear", "Aeropuertos", $"Se registró el aeropuerto {aeropuerto.Codigo} - {aeropuerto.Nombre}.");
        }

        public async Task ActualizarAsync(AeropuertoDto dto)
        {
            var aeropuerto = await _context.Aeropuertos.FindAsync(dto.AeropuertoId);

            if (aeropuerto == null)
                throw new InvalidOperationException($"No existe un aeropuerto con id {dto.AeropuertoId}.");

            aeropuerto.Codigo = dto.Codigo;
            aeropuerto.Nombre = dto.Nombre;
            aeropuerto.Ciudad = dto.Ciudad;
            aeropuerto.Pais = dto.Pais;

            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Actualizar", "Aeropuertos", $"Se actualizó el aeropuerto {aeropuerto.AeropuertoId}.");
        }

        public async Task EliminarAsync(int id)
        {
            var aeropuerto = await _context.Aeropuertos.FindAsync(id);

            if (aeropuerto == null)
                return;

            var tieneVuelos = await _context.Vuelo
                .AnyAsync(v => v.AeropuertoOrigenId == id || v.AeropuertoDestinoId == id);

            if (tieneVuelos)
                throw new InvalidOperationException(
                    "No se puede eliminar el aeropuerto porque tiene vuelos asociados (como origen o destino).");

            _context.Aeropuertos.Remove(aeropuerto);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Eliminar", "Aeropuertos", $"Se eliminó el aeropuerto {aeropuerto.Codigo} - {aeropuerto.Nombre}.");
        }
    }
}

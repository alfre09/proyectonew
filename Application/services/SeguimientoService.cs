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

        public SeguimientoService(SivDbContext context)
        {
            _context = context;
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
            var seguimiento = new Seguimiento
            {
                Usuario = dto.Usuario,
                VueloId = dto.VueloId,
                FechaSeguimiento = DateTime.Now
            };

            _context.Seguimientos.Add(seguimiento);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(SeguimientoDto dto)
        {
            var seguimiento = await _context.Seguimientos.FindAsync(dto.SeguimientoId);

            if (seguimiento == null)
                return;

            seguimiento.Usuario = dto.Usuario;
            seguimiento.VueloId = dto.VueloId;

            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var seguimiento = await _context.Seguimientos.FindAsync(id);

            if (seguimiento == null)
                return;

            _context.Seguimientos.Remove(seguimiento);
            await _context.SaveChangesAsync();
        }
    }
}
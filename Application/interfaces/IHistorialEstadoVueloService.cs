using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class HistorialEstadoVueloService : IHistorialEstadoVueloService
    {
        private readonly SivDbContext _context;

        public HistorialEstadoVueloService(SivDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistorialEstadoVueloDto>> ObtenerPorVueloAsync(int vueloId)
        {
            var historial = await _context.HistorialEstados
                .Where(h => h.VueloId == vueloId)
                .OrderBy(h => h.FechaCambio)
                .ToListAsync();

            var estados = await _context.EstadosVuelo
                .ToDictionaryAsync(e => e.EstadoVueloId, e => e.Nombre);

            return historial.Select(h => new HistorialEstadoVueloDto
            {
                HistorialEstadoVueloId = h.HistorialEstadoVueloId,
                VueloId = h.VueloId,
                EstadoAnteriorId = h.EstadoAnteriorId,
                EstadoAnteriorNombre = estados.GetValueOrDefault(h.EstadoAnteriorId, "Desconocido"),
                EstadoNuevoId = h.EstadoNuevoId,
                EstadoNuevoNombre = estados.GetValueOrDefault(h.EstadoNuevoId, "Desconocido"),
                FechaCambio = h.FechaCambio
            }).ToList();
        }

        public async Task RegistrarCambioDeEstadoAsync(int vueloId, int estadoAnteriorId, int estadoNuevoId)
        {
            var historial = new HistorialEstadoVuelo
            {
                VueloId = vueloId,
                EstadoAnteriorId = estadoAnteriorId,
                EstadoNuevoId = estadoNuevoId,
                FechaCambio = DateTime.Now
            };

            _context.HistorialEstados.Add(historial);
            await _context.SaveChangesAsync();
        }
    }
}
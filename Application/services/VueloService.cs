using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class VueloService : IVueloService
    {
        private readonly SivDbContext _context;

        public VueloService(SivDbContext context)
        {
            _context = context;
        }

        public async Task<List<VueloDto>> ObtenerTodosAsync()
        {
            return await _context.Vuelos
                .Select(v => new VueloDto
                {
                    VueloId = v.VueloId,
                    NumeroVuelo = v.NumeroVuelo,
                    AerolineaId = v.AerolineaId,
                    AeropuertoOrigenId = v.AeropuertoOrigenId,
                    AeropuertoDestinoId = v.AeropuertoDestinoId,
                    HorarioProgramado = v.HorarioProgramado,
                    EstadoVueloId = v.EstadoVueloId,
                    FechaCreacion = v.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<VueloDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Vuelos
                .Where(v => v.VueloId == id)
                .Select(v => new VueloDto
                {
                    VueloId = v.VueloId,
                    NumeroVuelo = v.NumeroVuelo,
                    AerolineaId = v.AerolineaId,
                    AeropuertoOrigenId = v.AeropuertoOrigenId,
                    AeropuertoDestinoId = v.AeropuertoDestinoId,
                    HorarioProgramado = v.HorarioProgramado,
                    EstadoVueloId = v.EstadoVueloId,
                    FechaCreacion = v.FechaCreacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task CrearAsync(VueloDto vueloDto)
        {
            var vuelo = new Vuelo
            {
                NumeroVuelo = vueloDto.NumeroVuelo,
                AerolineaId = vueloDto.AerolineaId,
                AeropuertoOrigenId = vueloDto.AeropuertoOrigenId,
                AeropuertoDestinoId = vueloDto.AeropuertoDestinoId,
                HorarioProgramado = vueloDto.HorarioProgramado,
                EstadoVueloId = vueloDto.EstadoVueloId,
                FechaCreacion = DateTime.Now
            };

            _context.Vuelos.Add(vuelo);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(VueloDto vueloDto)
        {
            var vuelo = await _context.Vuelos.FindAsync(vueloDto.VueloId);

            if (vuelo == null)
                return;

            vuelo.NumeroVuelo = vueloDto.NumeroVuelo;
            vuelo.AerolineaId = vueloDto.AerolineaId;
            vuelo.AeropuertoOrigenId = vueloDto.AeropuertoOrigenId;
            vuelo.AeropuertoDestinoId = vueloDto.AeropuertoDestinoId;
            vuelo.HorarioProgramado = vueloDto.HorarioProgramado;
            vuelo.EstadoVueloId = vueloDto.EstadoVueloId;

            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var vuelo = await _context.Vuelos.FindAsync(id);

            if (vuelo == null)
                return;

            _context.Vuelos.Remove(vuelo);
            await _context.SaveChangesAsync();
        }
    }
}
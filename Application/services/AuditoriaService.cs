using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    // Regla de negocio: "Toda acción relevante debe quedar registrada" y
    // "Los registros deben permitir supervisión operativa, auditoría institucional y análisis histórico."
    // Este servicio centraliza esa trazabilidad para que no se duplique la lógica de auditoría
    // en cada componente que registra vuelos, cambios, seguimientos o notificaciones.
    public class AuditoriaService : IAuditoriaService
    {
        private readonly SivDbContext _context;

        public AuditoriaService(SivDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(string accion, string tabla, string descripcion)
        {
            var auditoria = new Auditoria
            {
                Accion = accion,
                Tabla = tabla,
                Descripcion = descripcion,
                Fecha = DateTime.Now
            };

            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditoriaDto>> ObtenerTodosAsync()
        {
            return await _context.Auditorias
                .OrderByDescending(a => a.Fecha)
                .Select(a => new AuditoriaDto
                {
                    AuditoriaId = a.AuditoriaId,
                    Accion = a.Accion,
                    Tabla = a.Tabla,
                    Descripcion = a.Descripcion,
                    Fecha = a.Fecha
                })
                .ToListAsync();
        }

        public async Task<List<AuditoriaDto>> ObtenerPorTablaAsync(string tabla)
        {
            return await _context.Auditorias
                .Where(a => a.Tabla == tabla)
                .OrderByDescending(a => a.Fecha)
                .Select(a => new AuditoriaDto
                {
                    AuditoriaId = a.AuditoriaId,
                    Accion = a.Accion,
                    Tabla = a.Tabla,
                    Descripcion = a.Descripcion,
                    Fecha = a.Fecha
                })
                .ToListAsync();
        }
    }
}

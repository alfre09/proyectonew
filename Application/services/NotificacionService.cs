using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;
using proyectonew.Models;

namespace proyectonew.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly SivDbContext _context;
        private readonly IAuditoriaService _auditoriaService;

        public NotificacionService(SivDbContext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<NotificacionDto>> ObtenerTodosAsync()
        {
            return await _context.Notificaciones
                .OrderByDescending(n => n.FechaEnvio)
                .Select(n => new NotificacionDto
                {
                    NotificacionId = n.NotificacionId,
                    VueloId = n.VueloId,
                    Usuario = n.Usuario,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    FechaEnvio = n.FechaEnvio
                })
                .ToListAsync();
        }

        public async Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(string usuario)
        {
            return await _context.Notificaciones
                .Where(n => n.Usuario == usuario)
                .OrderByDescending(n => n.FechaEnvio)
                .Select(n => new NotificacionDto
                {
                    NotificacionId = n.NotificacionId,
                    VueloId = n.VueloId,
                    Usuario = n.Usuario,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    FechaEnvio = n.FechaEnvio
                })
                .ToListAsync();
        }

        public async Task<List<NotificacionDto>> ObtenerPorVueloAsync(int vueloId)
        {
            return await _context.Notificaciones
                .Where(n => n.VueloId == vueloId)
                .OrderByDescending(n => n.FechaEnvio)
                .Select(n => new NotificacionDto
                {
                    NotificacionId = n.NotificacionId,
                    VueloId = n.VueloId,
                    Usuario = n.Usuario,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    FechaEnvio = n.FechaEnvio
                })
                .ToListAsync();
        }

        public async Task MarcarComoLeidaAsync(int notificacionId)
        {
            var notificacion = await _context.Notificaciones.FindAsync(notificacionId);

            if (notificacion == null)
                return;

            notificacion.Leida = true;
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Actualizar", "Notificaciones", $"Notificación {notificacion.NotificacionId} marcada como leída.");
        }

        public async Task GenerarNotificacionesPorCambioAsync(int vueloId, string mensaje)
        {
            // Regla: solo se notifica a los usuarios que siguen ese vuelo específico.
            var interesados = await _context.Seguimientos
                .Where(s => s.VueloId == vueloId)
                .Select(s => s.Usuario)
                .Distinct()
                .ToListAsync();

            if (!interesados.Any())
                return;

            var notificaciones = interesados.Select(usuario => new Notificacion
            {
                VueloId = vueloId,
                Usuario = usuario,
                Mensaje = mensaje,
                Leida = false,
                FechaEnvio = DateTime.Now
            });

            _context.Notificaciones.AddRange(notificaciones);
            await _context.SaveChangesAsync();

            await _auditoriaService.RegistrarAsync(
                "Generar", "Notificaciones",
                $"Se generaron {interesados.Count} notificación(es) para el vuelo {vueloId}.");
        }
    }
}

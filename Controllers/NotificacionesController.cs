using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        // GET: api/Notificaciones
        [HttpGet]
        public async Task<ActionResult<List<NotificacionDto>>> ObtenerTodos()
        {
            var notificaciones = await _notificacionService.ObtenerTodosAsync();
            return Ok(notificaciones);
        }

        // GET: api/Notificaciones/usuario/correo@ejemplo.com
        [HttpGet("usuario/{usuario}")]
        public async Task<ActionResult<List<NotificacionDto>>> ObtenerPorUsuario(string usuario)
        {
            var notificaciones = await _notificacionService.ObtenerPorUsuarioAsync(usuario);
            return Ok(notificaciones);
        }

        // GET: api/Notificaciones/vuelo/5
        [HttpGet("vuelo/{vueloId}")]
        public async Task<ActionResult<List<NotificacionDto>>> ObtenerPorVuelo(int vueloId)
        {
            var notificaciones = await _notificacionService.ObtenerPorVueloAsync(vueloId);
            return Ok(notificaciones);
        }

        // PUT: api/Notificaciones/5/leida
        [HttpPut("{id}/leida")]
        public async Task<ActionResult> MarcarComoLeida(int id)
        {
            await _notificacionService.MarcarComoLeidaAsync(id);
            return Ok(new { mensaje = "Notificación marcada como leída." });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CambiosOperativosController : ControllerBase
    {
        private readonly ICambioOperativoService _cambioOperativoService;

        public CambiosOperativosController(ICambioOperativoService cambioOperativoService)
        {
            _cambioOperativoService = cambioOperativoService;
        }

        // GET: api/CambiosOperativos
        [HttpGet]
        public async Task<ActionResult<List<CambioOperativoDto>>> ObtenerTodos()
        {
            var cambios = await _cambioOperativoService.ObtenerTodosAsync();
            return Ok(cambios);
        }

        // GET: api/CambiosOperativos/vuelo/5
        [HttpGet("vuelo/{vueloId}")]
        public async Task<ActionResult<List<CambioOperativoDto>>> ObtenerPorVuelo(int vueloId)
        {
            var cambios = await _cambioOperativoService.ObtenerPorVueloAsync(vueloId);
            return Ok(cambios);
        }

        // POST: api/CambiosOperativos
        // Registra un cambio (retraso, adelanto, cambio de puerta o cancelación),
        // actualiza el vuelo, guarda el historial de estado y genera notificaciones.
        [HttpPost]
        public async Task<ActionResult> RegistrarCambio([FromBody] CambioOperativoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultado = await _cambioOperativoService.RegistrarCambioAsync(dto);

                return StatusCode(201, new
                {
                    mensaje = "Cambio operativo registrado correctamente. Se notificó a los interesados.",
                    cambio = resultado
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguimientosController : ControllerBase
    {
        private readonly ISeguimientoService _seguimientoService;

        public SeguimientosController(ISeguimientoService seguimientoService)
        {
            _seguimientoService = seguimientoService;
        }

        // GET: api/Seguimientos
        [HttpGet]
        public async Task<ActionResult<List<SeguimientoDto>>> ObtenerTodos()
        {
            var seguimientos = await _seguimientoService.ObtenerTodosAsync();
            return Ok(seguimientos);
        }

        // GET: api/Seguimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SeguimientoDto>> ObtenerPorId(int id)
        {
            var seguimiento = await _seguimientoService.ObtenerPorIdAsync(id);

            if (seguimiento == null)
                return NotFound(new { mensaje = $"No se encontró el seguimiento con id {id}" });

            return Ok(seguimiento);
        }

        // POST: api/Seguimientos
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] SeguimientoDto seguimientoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _seguimientoService.CrearAsync(seguimientoDto);

                return StatusCode(201, new
                {
                    mensaje = "Seguimiento creado correctamente",
                    seguimiento = seguimientoDto
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/Seguimientos/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] SeguimientoDto seguimientoDto)
        {
            if (id != seguimientoDto.SeguimientoId)
                return BadRequest(new
                {
                    mensaje = "El id de la ruta no coincide con el id enviado"
                });

            var existente = await _seguimientoService.ObtenerPorIdAsync(id);

            if (existente == null)
                return NotFound(new
                {
                    mensaje = $"No se encontró el seguimiento con id {id}"
                });

            try
            {
                await _seguimientoService.ActualizarAsync(seguimientoDto);

                return Ok(new
                {
                    mensaje = "Seguimiento actualizado correctamente"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/Seguimientos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var existente = await _seguimientoService.ObtenerPorIdAsync(id);

            if (existente == null)
                return NotFound(new
                {
                    mensaje = $"No se encontró el seguimiento con id {id}"
                });

            await _seguimientoService.EliminarAsync(id);

            return Ok(new
            {
                mensaje = "Seguimiento eliminado correctamente"
            });
        }
    }
}
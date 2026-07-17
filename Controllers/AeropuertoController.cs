using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    // Catálogo de aeropuertos (origen/destino). Antes no existía ningún módulo para
    // administrarlo, aunque un vuelo lo requiere obligatoriamente.
    [ApiController]
    [Route("api/[controller]")]
    public class AeropuertoController : ControllerBase
    {
        private readonly IAeropuertoService _aeropuertoService;

        public AeropuertoController(IAeropuertoService aeropuertoService)
        {
            _aeropuertoService = aeropuertoService;
        }

        // GET: api/Aeropuerto
        [HttpGet]
        public async Task<ActionResult<List<AeropuertoDto>>> ObtenerTodos()
        {
            var aeropuertos = await _aeropuertoService.ObtenerTodosAsync();
            return Ok(aeropuertos);
        }

        // GET: api/Aeropuerto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AeropuertoDto>> ObtenerPorId(int id)
        {
            var aeropuerto = await _aeropuertoService.ObtenerPorIdAsync(id);

            if (aeropuerto == null)
                return NotFound(new { mensaje = $"No se encontró el aeropuerto con id {id}" });

            return Ok(aeropuerto);
        }

        // POST: api/Aeropuerto
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] AeropuertoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _aeropuertoService.CrearAsync(dto);
                return StatusCode(201, new { mensaje = "Aeropuerto creado correctamente", aeropuerto = dto });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/Aeropuerto/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] AeropuertoDto dto)
        {
            if (id != dto.AeropuertoId)
                return BadRequest(new { mensaje = "El id de la ruta no coincide con el id enviado" });

            try
            {
                await _aeropuertoService.ActualizarAsync(dto);
                return Ok(new { mensaje = "Aeropuerto actualizado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/Aeropuerto/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var existente = await _aeropuertoService.ObtenerPorIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = $"No se encontró el aeropuerto con id {id}" });

            try
            {
                await _aeropuertoService.EliminarAsync(id);
                return Ok(new { mensaje = "Aeropuerto eliminado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    // Catálogo de aerolíneas. Un vuelo debe estar asociado, como mínimo, a una aerolínea
    // (Reglas de Programación y Registro de Vuelos), por eso este catálogo debe existir
    // y poder consultarse antes de crear un vuelo.
    [ApiController]
    [Route("api/[controller]")]
    public class AerolineaController : ControllerBase
    {
        private readonly IAerolineaService _aerolineaService;

        public AerolineaController(IAerolineaService aerolineaService)
        {
            _aerolineaService = aerolineaService;
        }

        // GET: api/Aerolinea
        [HttpGet]
        public async Task<ActionResult<List<AerolineaDto>>> ObtenerTodos()
        {
            var aerolineas = await _aerolineaService.ObtenerTodosAsync();
            return Ok(aerolineas);
        }

        // GET: api/Aerolinea/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AerolineaDto>> ObtenerPorId(int id)
        {
            var aerolinea = await _aerolineaService.ObtenerPorIdAsync(id);

            if (aerolinea == null)
                return NotFound(new { mensaje = $"No se encontró la aerolínea con id {id}" });

            return Ok(aerolinea);
        }

        // POST: api/Aerolinea
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] AerolineaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _aerolineaService.CrearAsync(dto);
                return StatusCode(201, new { mensaje = "Aerolínea creada correctamente", aerolinea = dto });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/Aerolinea/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] AerolineaDto dto)
        {
            if (id != dto.AerolineaId)
                return BadRequest(new { mensaje = "El id de la ruta no coincide con el id enviado" });

            try
            {
                await _aerolineaService.ActualizarAsync(dto);
                return Ok(new { mensaje = "Aerolínea actualizada correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/Aerolinea/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var existente = await _aerolineaService.ObtenerPorIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = $"No se encontró la aerolínea con id {id}" });

            try
            {
                await _aerolineaService.EliminarAsync(id);
                return Ok(new { mensaje = "Aerolínea eliminada correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

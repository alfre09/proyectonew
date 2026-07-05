using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VuelosController : ControllerBase
    {
        private readonly IVueloService _vueloService;

        public VuelosController(IVueloService vueloService)
        {
            _vueloService = vueloService;
        }

        // GET: api/vuelos
        [HttpGet]
        public async Task<ActionResult<List<VueloDto>>> ObtenerTodos()
        {
            var vuelos = await _vueloService.ObtenerTodosAsync();
            //var vuelos = 
            return Ok(vuelos);
        }

        // GET: api/vuelos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VueloDto>> ObtenerPorId(int id)
        {
            var vuelo = await _vueloService.ObtenerPorIdAsync(id);

            if (vuelo == null)
                return NotFound(new { mensaje = $"No se encontró el vuelo con id {id}" });

            return Ok(vuelo);
        }

        // POST: api/vuelos
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] VueloDto vueloDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _vueloService.CrearAsync(vueloDto);
            return StatusCode(201, new { mensaje = "Vuelo creado correctamente", vuelo = vueloDto });
        }

        // PUT: api/vuelos/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] VueloDto vueloDto)
        {
            if (id != vueloDto.VueloId)
                return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del vuelo enviado" });

            var existente = await _vueloService.ObtenerPorIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = $"No se encontró el vuelo con id {id}" });

            await _vueloService.ActualizarAsync(vueloDto);
            return Ok(new { mensaje = "Vuelo actualizado correctamente" });
        }

        // DELETE: api/vuelos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var existente = await _vueloService.ObtenerPorIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = $"No se encontró el vuelo con id {id}" });

            await _vueloService.EliminarAsync(id);
            return Ok(new { mensaje = "Vuelo eliminado correctamente" });
        }
    }
}

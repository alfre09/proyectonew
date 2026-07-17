using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    // Regla de negocio: "Los registros deben permitir supervisión operativa,
    // auditoría institucional y análisis histórico." Este controlador expone esa trazabilidad.
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        // GET: api/Auditoria
        [HttpGet]
        public async Task<ActionResult<List<AuditoriaDto>>> ObtenerTodos()
        {
            var registros = await _auditoriaService.ObtenerTodosAsync();
            return Ok(registros);
        }

        // GET: api/Auditoria/tabla/Vuelos
        [HttpGet("tabla/{tabla}")]
        public async Task<ActionResult<List<AuditoriaDto>>> ObtenerPorTabla(string tabla)
        {
            var registros = await _auditoriaService.ObtenerPorTablaAsync(tabla);
            return Ok(registros);
        }
    }
}

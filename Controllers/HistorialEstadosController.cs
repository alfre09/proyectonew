using Microsoft.AspNetCore.Mvc;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistorialEstadosController : ControllerBase
    {
        private readonly IHistorialEstadoVueloService _historialService;

        public HistorialEstadosController(IHistorialEstadoVueloService historialService)
        {
            _historialService = historialService;
        }

        // GET: api/HistorialEstados/vuelo/5
        // Muestra la línea de tiempo de cambios de estado de un vuelo.
        [HttpGet("vuelo/{vueloId}")]
        public async Task<ActionResult<List<HistorialEstadoVueloDto>>> ObtenerPorVuelo(int vueloId)
        {
            var historial = await _historialService.ObtenerPorVueloAsync(vueloId);
            return Ok(historial);
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Pages.Vuelos
{
    public class IndexModel : PageModel
    {
        private readonly IVueloService _vueloService;

        public IndexModel(IVueloService vueloService)
        {
            _vueloService = vueloService;
        }

        public List<VueloDto> Vuelos { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vuelos = await _vueloService.ObtenerTodosAsync();
        }
    }
}
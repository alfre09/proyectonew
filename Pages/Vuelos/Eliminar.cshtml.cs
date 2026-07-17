using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;

namespace proyectonew.Pages.Vuelos
{
    public class EliminarModel : PageModel
    {
        private readonly IVueloService _vueloService;

        public EliminarModel(IVueloService vueloService)
        {
            _vueloService = vueloService;
        }

        [BindProperty]
        public VueloDto Vuelo { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vuelo = await _vueloService.ObtenerPorIdAsync(id);

            if (vuelo == null)
                return RedirectToPage("Index");

            Vuelo = vuelo;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _vueloService.EliminarAsync(id);
            return RedirectToPage("Index");
        }
    }
}

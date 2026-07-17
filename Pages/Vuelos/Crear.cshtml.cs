using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Base;
using proyectonew.Application.Dtos;
using proyectonew.Application.Interfaces;
using proyectonew.Data;

namespace proyectonew.Pages.Vuelos
{
    public class CrearModel : PageModel
    {
        private readonly IVueloService _vueloService;
        private readonly SivDbContext _context;

        public CrearModel(IVueloService vueloService, SivDbContext context)
        {
            _vueloService = vueloService;
            _context = context;
        }

        [BindProperty]
        public VueloDto Vuelo { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CargarListasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return Page();
            }

            // Regla: todo vuelo nuevo se registra en el primer estado del ciclo (Programado).
            var estadoProgramado = await _context.EstadosVuelo
                .FirstOrDefaultAsync(e => e.Nombre == CicloEstadosVuelo.Secuencia[0]);

            if (estadoProgramado == null)
            {
                ModelState.AddModelError(string.Empty,
                    $"No existe el estado '{CicloEstadosVuelo.Secuencia[0]}' en el catálogo. Reinicia la aplicación para sembrar los datos base.");
                await CargarListasAsync();
                return Page();
            }

            Vuelo.EstadoVueloId = estadoProgramado.EstadoVueloId;

            try
            {
                await _vueloService.CrearAsync(Vuelo);
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListasAsync();
                return Page();
            }
        }

        private async Task CargarListasAsync()
        {
            ViewData["Aerolineas"] = await _context.Aerolineas.ToListAsync();
            ViewData["Aeropuertos"] = await _context.Aeropuertos.ToListAsync();
            ViewData["Estados"] = await _context.EstadosVuelo.ToListAsync();
        }
    }
}

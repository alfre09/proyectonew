using proyectonew.Application.Interfaces;
using proyectonew.Application.Services;

namespace proyectonew.IOC.Dependencies
{
    public static class VueloDependency
    {
        public static IServiceCollection AddVueloDependencies(this IServiceCollection services)
        {
            services.AddScoped<IVueloService, VueloService>();
            services.AddScoped<ISeguimientoService, SeguimientoService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            services.AddScoped<IHistorialEstadoVueloService, HistorialEstadoVueloService>();
            services.AddScoped<ICambioOperativoService, CambioOperativoService>();
            services.AddScoped<IAerolineaService, AerolineaService>();
            services.AddScoped<IAeropuertoService, AeropuertoService>();
            services.AddScoped<IAuditoriaService, AuditoriaService>();

            return services;
        }
    }
}
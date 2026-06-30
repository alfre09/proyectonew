using proyectonew.Application.Interfaces;
using proyectonew.Application.Services;

namespace proyectonew.IOC.Dependencies
{
    public static class VueloDependency
    {
        public static IServiceCollection AddVueloDependencies(this IServiceCollection services)
        {
            services.AddScoped<IVueloService, VueloService>();
            return services;
        }
    }
}
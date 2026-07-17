using proyectonew.Persistence.Interfaces;
using proyectonew.Persistence.Repositories;

namespace proyectonew.IOC.Dependencies
{
    public static class PersistenceDependency
    {
        public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IVueloRepository, VueloRepository>();
            services.AddScoped<IAerolineaRepository, AerolineaRepository>();
            services.AddScoped<IAeropuertoRepository, AeropuertoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            return services;
        }
    }
}

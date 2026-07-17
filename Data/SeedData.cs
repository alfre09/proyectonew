using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Base;
using proyectonew.Models;

namespace proyectonew.Data
{
    // Siembra el catálogo mínimo para poder probar el SIV de inmediato:
    // - Estados de vuelo (deben coincidir con CicloEstadosVuelo.Secuencia).
    // - Un par de aerolíneas y aeropuertos de ejemplo.
    // Es idempotente: solo inserta lo que falte, así que es seguro que corra en cada arranque.
    public static class SeedData
    {
        public static async Task InicializarAsync(SivDbContext context)
        {
            await SembrarEstadosAsync(context);
            await SembrarAerolineasAsync(context);
            await SembrarAeropuertosAsync(context);
        }

        private static async Task SembrarEstadosAsync(SivDbContext context)
        {
            var nombresRequeridos = CicloEstadosVuelo.Secuencia
                .Append(CicloEstadosVuelo.ESTADO_CANCELADO);

            foreach (var nombre in nombresRequeridos)
            {
                var existe = await context.EstadosVuelo
                    .AnyAsync(e => e.Nombre == nombre);

                if (!existe)
                {
                    context.EstadosVuelo.Add(new EstadoVuelo { Nombre = nombre });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SembrarAerolineasAsync(SivDbContext context)
        {
            if (await context.Aerolineas.AnyAsync())
                return;

            context.Aerolineas.AddRange(
                new Aerolinea { Codigo = "AA", Nombre = "American Airlines" },
                new Aerolinea { Codigo = "LA", Nombre = "LATAM Airlines" },
                new Aerolinea { Codigo = "CM", Nombre = "Copa Airlines" },
                new Aerolinea { Codigo = "B6", Nombre = "JetBlue Airways" }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SembrarAeropuertosAsync(SivDbContext context)
        {
            if (await context.Aeropuertos.AnyAsync())
                return;

            context.Aeropuertos.AddRange(
                new Aeropuerto { Codigo = "SDQ", Nombre = "Aeropuerto Las Américas", Ciudad = "Santo Domingo", Pais = "República Dominicana" },
                new Aeropuerto { Codigo = "PUJ", Nombre = "Aeropuerto Internacional de Punta Cana", Ciudad = "Punta Cana", Pais = "República Dominicana" },
                new Aeropuerto { Codigo = "MIA", Nombre = "Aeropuerto Internacional de Miami", Ciudad = "Miami", Pais = "Estados Unidos" },
                new Aeropuerto { Codigo = "JFK", Nombre = "Aeropuerto John F. Kennedy", Ciudad = "Nueva York", Pais = "Estados Unidos" },
                new Aeropuerto { Codigo = "MAD", Nombre = "Aeropuerto Adolfo Suárez Madrid-Barajas", Ciudad = "Madrid", Pais = "España" }
            );

            await context.SaveChangesAsync();
        }
    }
}

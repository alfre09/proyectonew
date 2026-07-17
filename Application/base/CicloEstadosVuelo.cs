namespace proyectonew.Application.Base
{
    // Regla de negocio: "Todo vuelo debe manejar un conjunto finito de estados operativos.
    // Un estado determina qué acciones están permitidas. No se puede avanzar a un estado
    // sin cumplir las condiciones del estado anterior."
    //
    // Se centraliza aquí la secuencia válida de estados para que VueloService y
    // CambioOperativoService (Application/services) apliquen exactamente la misma regla
    // y no quede duplicada ni pueda desalinearse entre componentes.
    public static class CicloEstadosVuelo
    {
        // Orden obligatorio del ciclo normal de un vuelo.
        // Los nombres deben coincidir (sin distinguir mayúsculas/minúsculas) con los
        // registros sembrados en la tabla EstadosVuelo (ver SeedData en Program.cs).
        public static readonly string[] Secuencia =
        {
            "Programado",
            "Embarcando",
            "En Vuelo",
            "Aterrizado"
        };

        public const string ESTADO_CANCELADO = "Cancelado";

        public static bool EsEstadoFinal(string nombreEstado)
        {
            if (string.IsNullOrWhiteSpace(nombreEstado))
                return false;

            return nombreEstado.Equals(ESTADO_CANCELADO, StringComparison.OrdinalIgnoreCase)
                || nombreEstado.Equals(Secuencia[^1], StringComparison.OrdinalIgnoreCase);
        }

        // Lanza InvalidOperationException si la transición de estadoActual -> estadoNuevo
        // no respeta el ciclo de vida del vuelo.
        public static void ValidarTransicion(string estadoActual, string estadoNuevo)
        {
            if (string.IsNullOrWhiteSpace(estadoActual) || string.IsNullOrWhiteSpace(estadoNuevo))
                throw new InvalidOperationException("El estado del vuelo no es válido.");

            if (EsEstadoFinal(estadoActual))
                throw new InvalidOperationException(
                    $"El vuelo se encuentra en un estado final ('{estadoActual}') y no puede continuar su ciclo operativo.");

            // Cancelar es válido desde cualquier estado no final.
            if (estadoNuevo.Equals(ESTADO_CANCELADO, StringComparison.OrdinalIgnoreCase))
                return;

            int indiceActual = Array.FindIndex(
                Secuencia, s => s.Equals(estadoActual, StringComparison.OrdinalIgnoreCase));
            int indiceNuevo = Array.FindIndex(
                Secuencia, s => s.Equals(estadoNuevo, StringComparison.OrdinalIgnoreCase));

            if (indiceActual == -1 || indiceNuevo == -1)
                throw new InvalidOperationException(
                    $"Estado no reconocido en la secuencia operativa del vuelo ('{estadoActual}' -> '{estadoNuevo}').");

            if (indiceNuevo != indiceActual + 1)
                throw new InvalidOperationException(
                    $"No se puede pasar de '{estadoActual}' a '{estadoNuevo}' sin cumplir el estado anterior.");
        }
    }
}

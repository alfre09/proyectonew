namespace proyectonew.Application.Dtos
{
    public class CambioOperativoDto
    {
        public int CambioOperativoId { get; set; }

        public int VueloId { get; set; }

        // Valores esperados: "Retraso", "Adelanto", "CambioPuerta", "Cancelacion"
        public string TipoCambio { get; set; }

        public string Causa { get; set; }

        public DateTime FechaCambio { get; set; }

        // Solo se usa cuando TipoCambio es "Retraso" o "Adelanto"
        public DateTime? NuevoHorario { get; set; }

        // Solo se usa cuando TipoCambio es "CambioPuerta"
        public string? NuevaPuerta { get; set; }

        // Solo se usa cuando TipoCambio es "Cancelacion" (debe ser el Id del estado "Cancelado")
        public int? NuevoEstadoVueloId { get; set; }
    }
}

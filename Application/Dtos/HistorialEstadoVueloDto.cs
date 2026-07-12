namespace proyectonew.Application.Dtos
{
    public class HistorialEstadoVueloDto
    {
        public int HistorialEstadoVueloId { get; set; }
        public int VueloId { get; set; }
        public int EstadoAnteriorId { get; set; }
        public string EstadoAnteriorNombre { get; set; }
        public int EstadoNuevoId { get; set; }
        public string EstadoNuevoNombre { get; set; }
        public DateTime FechaCambio { get; set; }
    }
}

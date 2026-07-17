namespace proyectonew.Application.Dtos
{
    public class SeguimientoDto
    {
        public int SeguimientoId { get; set; }

        public string Usuario { get; set; }

        public int VueloId { get; set; }

        public DateTime FechaSeguimiento { get; set; }
    }
}
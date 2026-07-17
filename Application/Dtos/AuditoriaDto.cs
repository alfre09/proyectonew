namespace proyectonew.Application.Dtos
{
    public class AuditoriaDto
    {
        public int AuditoriaId { get; set; }

        public string Accion { get; set; }

        public string Tabla { get; set; }

        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }
    }
}

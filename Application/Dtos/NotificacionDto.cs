namespace proyectonew.Application.Dtos
{
    public class NotificacionDto
    {
        public int NotificacionId { get; set; }

        public int VueloId { get; set; }

        public string Usuario { get; set; }

        public string Mensaje { get; set; }

        public bool Leida { get; set; }

        public DateTime FechaEnvio { get; set; }
    }
}

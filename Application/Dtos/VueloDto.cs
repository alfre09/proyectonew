using System.ComponentModel.DataAnnotations;

namespace proyectonew.Application.Dtos
{
    public class VueloDto

    {
        [Key]
        public int VueloId { get; set; }

        public string NumeroVuelo { get; set; }

        public int AerolineaId { get; set; }

        public int AeropuertoOrigenId { get; set; }

        public int AeropuertoDestinoId { get; set; }

        public DateTime HorarioProgramado { get; set; }

        public int EstadoVueloId { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
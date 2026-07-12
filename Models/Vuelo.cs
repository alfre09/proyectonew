using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectonew.Models
{
    public class Vuelo
    {
        [Key]
        public int VueloId { get; set; }

        [Required]
        [MaxLength(20)]
        public string NumeroVuelo { get; set; }

        [Required]
        public int AerolineaId { get; set; }

        [ForeignKey(nameof(AerolineaId))]
        public Aerolinea Aerolinea { get; set; }

        [Required]
        public int AeropuertoOrigenId { get; set; }

        [ForeignKey(nameof(AeropuertoOrigenId))]
        public Aeropuerto AeropuertoOrigen { get; set; }

        [Required]
        public int AeropuertoDestinoId { get; set; }

        [ForeignKey(nameof(AeropuertoDestinoId))]
        public Aeropuerto AeropuertoDestino { get; set; }

        [Required]
        public DateTime HorarioProgramado { get; set; }

        [MaxLength(10)]
        public string? Puerta { get; set; }

        [Required]
        public int EstadoVueloId { get; set; }

        [ForeignKey(nameof(EstadoVueloId))]
        public EstadoVuelo EstadoVuelo { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
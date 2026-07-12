using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectonew.Models
{
    public class HistorialEstadoVuelo
    {
        [Key]
        public int HistorialEstadoVueloId { get; set; }

        [Required]
        public int VueloId { get; set; }

        [ForeignKey(nameof(VueloId))]
        public Vuelo Vuelo { get; set; }

        public int EstadoAnteriorId { get; set; }

        public int EstadoNuevoId { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.Now;
    }
}
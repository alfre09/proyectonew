using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectonew.Models
{
    public class Seguimiento
    {
        [Key]
        public int SeguimientoId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; }

        [Required]
        public int VueloId { get; set; }

        [ForeignKey(nameof(VueloId))]
        public Vuelo Vuelo { get; set; }

        public DateTime FechaSeguimiento { get; set; } = DateTime.Now;
    }
}
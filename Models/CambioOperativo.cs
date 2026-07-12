using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectonew.Models
{
    public class CambioOperativo
    {
        [Key]
        public int CambioOperativoId { get; set; }

        [Required]
        public int VueloId { get; set; }

        [ForeignKey(nameof(VueloId))]
        public Vuelo Vuelo { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoCambio { get; set; }

        [Required]
        [MaxLength(200)]
        public string Causa { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.Now;
    }
}
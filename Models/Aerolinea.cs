using System.ComponentModel.DataAnnotations;

namespace proyectonew.Models
{
    public class Aerolinea
    {
        [Key]
        public int AerolineaId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; }
    }
}
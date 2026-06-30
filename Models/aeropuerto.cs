using System.ComponentModel.DataAnnotations;

namespace proyectonew.Models
{
    public class Aeropuerto
    {
        [Key]
        public int AeropuertoId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Ciudad { get; set; }

        [Required]
        [MaxLength(100)]
        public string Pais { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace proyectonew.Models
{
    public class EstadoVuelo
    {
        [Key]
        public int EstadoVueloId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }
    }
}
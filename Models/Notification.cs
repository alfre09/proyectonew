using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectonew.Models
{
    public class Notificacion
    {
        [Key]
        public int NotificacionId { get; set; }

        [Required]
        public int VueloId { get; set; }

        [ForeignKey(nameof(VueloId))]
        public Vuelo Vuelo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public bool Leida { get; set; } = false;

        public DateTime FechaEnvio { get; set; } = DateTime.Now;
    }
}
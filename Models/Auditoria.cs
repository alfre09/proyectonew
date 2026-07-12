using System.ComponentModel.DataAnnotations;

namespace proyectonew.Models
{
    public class Auditoria
    {
        [Key]
        public int AuditoriaId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Accion { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tabla { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
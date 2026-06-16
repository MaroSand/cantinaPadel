using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        [Column("id_empleado")]
        public int IdEmpleado { get; set; }

        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("nombre_usuario")]
        [Required]
        public string NombreUsuario { get; set; }

        [Column("contrasena")]
        [Required]
        public string Contrasena { get; set; }

        [Column("rol")]
        public string Rol { get; set; } = "Empleado";

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navigation property — el JOIN se hace solo con .Include()
        [ForeignKey("IdPersona")]
        public Persona Persona { get; set; }
    }
}
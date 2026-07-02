using System;
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

        // Valida el formato de los campos propios del Empleado y delega en Persona lo que es de Persona.
        public void ValidarFormato(bool dniObligatorio = false)
        {
            if (Persona == null)
                throw new ArgumentException("Los datos personales del empleado no pueden estar vacíos.");

            Persona.ValidarDatosComunes(dniObligatorio);

            NombreUsuario = NombreUsuario?.Trim() ?? string.Empty;
            Contrasena = Contrasena?.Trim() ?? string.Empty;
            Rol = Rol?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(NombreUsuario))
                throw new ArgumentException("El campo Nombre de Usuario es obligatorio.");

            if (NombreUsuario.Length > 50)
                throw new ArgumentException("El Nombre de Usuario no puede exceder los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(Contrasena))
                throw new ArgumentException("El campo Contraseña es obligatorio.");

            if (Contrasena.Length > 9)
                throw new ArgumentException("La Contraseña no puede exceder los 9 caracteres.");

            if (string.IsNullOrWhiteSpace(Rol))
                throw new ArgumentException("Debe seleccionar un Rol válido para el empleado.");
        }
    }
}
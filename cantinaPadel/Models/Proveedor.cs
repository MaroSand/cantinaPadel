using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("Proveedores")]
    public class Proveedor
    {
        [Key]
        [Column("id_proveedor")]
        public int IdProveedor { get; set; }

        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("nombre_empresa")]
        public string NombreEmpresa { get; set; }

        [ForeignKey("IdPersona")]
        public Persona Persona { get; set; }

        // Valida atributo propio
        public void ValidarNombreEmpresa()
        {
            NombreEmpresa = NombreEmpresa?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(NombreEmpresa))
                throw new ArgumentException("El nombre de la empresa es obligatorio.");
        }
    }
}
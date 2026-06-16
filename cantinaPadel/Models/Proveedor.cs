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

        [Column("razon_social")]
        public string? RazonSocial { get; set; }

        [Column("cuit")]
        public string? Cuit { get; set; }

        [Column("condicion_iva")]
        public string? CondicionIva { get; set; }

        [ForeignKey("IdPersona")]
        public Persona Persona { get; set; }
    }
}
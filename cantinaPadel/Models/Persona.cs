using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("Personas")]
    public class Persona
    {
        [Key]
        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("nombre")]
        [Required]
        public string Nombre { get; set; }

        [Column("apellido")]
        [Required]
        public string Apellido { get; set; }

        [Column("dni")]
        public string? Dni { get; set; }
        
        [Column("cuit")]
        public string? Cuit { get; set; }
        
        [Column("condicion_iva")]
        public string? CondicionIva { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_alta")]
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        [Column("es_cliente")]
        public bool EsCliente { get; set; } = false;

        [Column("es_empleado")]
        public bool EsEmpleado { get; set; } = false;

        [Column("es_proveedor")]
        public bool EsProveedor { get; set; } = false;

        // @OneToOne
        public Empleado?   Empleado   { get; set; }
        public Cliente?    Cliente    { get; set; }
        public Proveedor?  Proveedor  { get; set; }
    }
}
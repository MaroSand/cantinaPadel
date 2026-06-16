using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("email")]
        [Required]
        public string Email { get; set; }

        [Column("saldo_cuenta_corriente")]
        public decimal SaldoCuentaCorriente { get; set; } = 0;

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [ForeignKey("IdPersona")]
        public Persona Persona { get; set; }
    }
}
using System;
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
        public decimal SaldoCuentaCorriente { get; set; }

        [ForeignKey("IdPersona")]
        public Persona Persona { get; set; }

        // Valida la estructura/formato del propio Cliente (y delega en Persona lo que es de Persona).
        public void Validar()
        {
            if (Persona == null)
                throw new ArgumentException("Los datos de la persona son obligatorios.");

            Persona.ValidarDatosComunes();

            Email = Email?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentException("El Email es obligatorio.");

            if (!EsEmailValido(Email))
                throw new ArgumentException("El Email ingresado no es válido.");
        }

        public static bool EsEmailValido(string email)
        {
            try
            {
                var direccion = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
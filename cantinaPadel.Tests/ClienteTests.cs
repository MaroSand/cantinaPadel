using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // Modelo: Cliente - Validar valida los campos propios del cliente (email) y delega en Persona lo que es
    // de Persona (nombre, apellido, dni, cuit, etc)
    [TestClass]
    public class ClienteModelTests
    {
        private static Cliente CrearClienteValido()
        {
            return new Cliente
            {
                Email = "jperez@gmail.com",
                Persona = new Persona
                {
                    Nombre = "Juan",
                    Apellido = "Perez",
                    Dni = "30111222",
                    Cuit = "20-30111222-0"
                }
            };
        }

        [TestMethod]
        public void Validar_PersonaNula_LanzaArgumentException()
        {
            // Sin Persona no hay a quién delegar la validación de nombre/apellido/dni/cuit, así que debe fallar
            var cliente = CrearClienteValido();
            cliente.Persona = null!;

            Assert.ThrowsExactly<ArgumentException>(() => cliente.Validar());
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Validar_EmailVacio_LanzaArgumentException(string? valor)
        {
            // El Email es obligatorio para el Cliente, sin importar si Persona es válida
            var cliente = CrearClienteValido();
            cliente.Email = valor!;

            Assert.ThrowsExactly<ArgumentException>(() => cliente.Validar());
        }

        [TestMethod]
        [DataRow("correo-invalido")]
        [DataRow("juan@")]
        [DataRow("@gmail.com")]
        public void Validar_EmailInvalido_LanzaArgumentException(string valor)
        {
            // Formatos que no cumplen con MailAddress deben rechazarse vía EsEmailValido
            var cliente = CrearClienteValido();
            cliente.Email = valor;

            Assert.ThrowsExactly<ArgumentException>(() => cliente.Validar());
        }

        [TestMethod]
        public void Validar_EmailConEspaciosAlrededor_RecortaYAcepta()
        {
            // Validar() debe hacer Trim() del Email antes de validarlo y dejarlo guardado sin espacios
            var cliente = CrearClienteValido();
            cliente.Email = "  jperez@gmail.com  ";

            cliente.Validar();

            Assert.AreEqual("jperez@gmail.com", cliente.Email);
        }

        [TestMethod]
        public void Validar_TodoValido_NoLanzaExcepcion()
        {
            // Caso "feliz": email y datos de persona pasan juntos la validación
            var cliente = CrearClienteValido();

            cliente.Validar();
        }
    }
}
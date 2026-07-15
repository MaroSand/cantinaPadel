using cantinaPadel.Models;
using cantinaPadel.BLL;

namespace cantinaPadel.Tests
{
    [TestClass]
    public class ProveedorModelTests
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void ValidarNombreEmpresa_Invalido_LanzaArgumentException(string valor)
        {
            var proveedor = new Proveedor { NombreEmpresa = valor };

            Assert.ThrowsExactly<ArgumentException>(() => proveedor.ValidarNombreEmpresa());
        }

        [TestMethod]
        public void ValidarNombreEmpresa_ConEspaciosAlrededor_RecortaYAcepta()
        {
            var proveedor = new Proveedor { NombreEmpresa = "  Repuestos Padel SA  " };

            proveedor.ValidarNombreEmpresa();

            Assert.AreEqual("Repuestos Padel SA", proveedor.NombreEmpresa);
        }
    }

    // ------------------------------------------------------------------
    // Modelo: Persona - validaciones de campos obligatorios usadas
    // por el alta/modificación de Proveedor
    // ------------------------------------------------------------------
    [TestClass]
    public class PersonaValidationTests
    {
        [TestMethod]
        [DataRow("")] // vacío
        [DataRow("Juan123")] // con números
        public void ValidarNombre_Invalido_LanzaArgumentException(string nombre) =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarNombre(nombre));

        [TestMethod]
        public void ValidarApellido_Vacio_LanzaArgumentException() =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarApellido("   "));

        [DataTestMethod]
        [DataRow("20123456789")] // sin guiones
        [DataRow("20-1234-5")] // dígitos incompletos
        public void ValidarCuit_Invalido_LanzaArgumentException(string cuit) =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarCuit(cuit));

        [TestMethod]
        public void ValidarTelefono_FormatoInvalido_LanzaArgumentException() =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarTelefono("abc"));

        [TestMethod]
        public void ValidarDatosComunes_TodoValido_NoLanzaExcepcion()
        {
            // Caso "feliz": nombre, apellido, cuit y telefono válidos
            // pasan juntos la validación.
            var persona = new Persona
            {
                Nombre = "Carlos",
                Apellido = "Gomez",
                Cuit = "20-30111222-5",
                Telefono = "3624123456"
            };

            persona.ValidarDatosComunes();
        }

        [TestMethod]
        public void ValidarDatosComunes_NombreFaltante_LanzaArgumentException()
        {
            var persona = new Persona { Nombre = "", Apellido = "Gomez" };

            Assert.ThrowsExactly<ArgumentException>(() => persona.ValidarDatosComunes());
        }
    }

    [TestClass]
    public class LogicaProveedorValidarTests
    {
        private readonly LogicaProveedor _logica = new LogicaProveedor();

        [TestMethod]
        public void Validar_PersonaNula_LanzaArgumentException()
        {
            var proveedor = new Proveedor { NombreEmpresa = "Empresa X" };

            Assert.ThrowsExactly<ArgumentException>(() => _logica.Validar(null, proveedor));
        }

        [TestMethod]
        public void Validar_ProveedorNulo_LanzaArgumentException()
        {
            var persona = new Persona { Nombre = "Ana", Apellido = "Suarez" };

            Assert.ThrowsExactly<ArgumentException>(() => _logica.Validar(persona, null));
        }

        [TestMethod]
        public void Validar_NombreEmpresaVacio_LanzaArgumentException()
        {
            var persona = new Persona { Nombre = "Ana", Apellido = "Suarez" };
            var proveedor = new Proveedor { NombreEmpresa = "" };

            Assert.ThrowsExactly<ArgumentException>(() => _logica.Validar(persona, proveedor));
        }

        [TestMethod]
        public void Validar_TodoValido_NoLanzaExcepcion()
        {
            var persona = new Persona { Nombre = "Ana", Apellido = "Suarez", Cuit = "20-12345678-9" };
            var proveedor = new Proveedor { NombreEmpresa = "Empresa X" };

            _logica.Validar(persona, proveedor);
        }
    }
}
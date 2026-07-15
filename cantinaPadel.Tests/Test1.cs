using Moq;
using cantinaPadel.Models;
using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;

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
        [DataRow("")]           // vacío
        [DataRow("Juan123")]    // con números
        public void ValidarNombre_Invalido_LanzaArgumentException(string nombre) =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarNombre(nombre));

        [TestMethod]
        public void ValidarNombre_Valido_NoLanzaExcepcion() =>
            Persona.ValidarNombre("Maria");

        [TestMethod]
        public void ValidarApellido_Vacio_LanzaArgumentException() =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarApellido("   "));

        [TestMethod]
        [DataRow("20123456789")]  // sin guiones
        [DataRow("20-1234-5")]    // dígitos incompletos
        public void ValidarCuit_FormatoInvalido_LanzaArgumentException(string cuit) =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarCuit(cuit));

        [TestMethod]
        public void ValidarCuit_FormatoValidoONulo_NoLanzaExcepcion()
        {
            Persona.ValidarCuit("20-12345678-9");
            Persona.ValidarCuit(null); // el CUIT no es obligatorio
        }

        [TestMethod]
        public void ValidarDni_FueraDeRangoDeDigitos_LanzaArgumentException() =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarDni("123"));

        [TestMethod]
        public void ValidarTelefono_FormatoInvalido_LanzaArgumentException() =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarTelefono("abc"));

        [TestMethod]
        public void ValidarCondicionIva_Invalida_LanzaArgumentException() =>
            Assert.ThrowsExactly<ArgumentException>(() => Persona.ValidarCondicionIva("Condicion Inventada"));

        [TestMethod]
        public void ValidarDatosComunes_TodoValido_NoLanzaExcepcion()
        {
            var persona = new Persona
            {
                Nombre = "Carlos",
                Apellido = "Gomez",
                Cuit = "20-30111222-5"
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
            var persona = new Persona { Nombre = "Ana", Apellido = "Diaz" };

            Assert.ThrowsExactly<ArgumentException>(() => _logica.Validar(persona, null));
        }

        [TestMethod]
        public void Validar_NombreEmpresaVacio_LanzaArgumentException()
        {
            var persona = new Persona { Nombre = "Ana", Apellido = "Diaz" };
            var proveedor = new Proveedor { NombreEmpresa = "" };

            Assert.ThrowsExactly<ArgumentException>(() => _logica.Validar(persona, proveedor));
        }

        [TestMethod]
        public void Validar_DatosDePersonaInvalidos_LanzaArgumentException()
        {
            var persona = new Persona { Nombre = "", Apellido = "Diaz" };
            var proveedor = new Proveedor { NombreEmpresa = "Empresa X" };

            Assert.ThrowsExactly<ArgumentException>(() => _logica.Validar(persona, proveedor));
        }

        [TestMethod]
        public void Validar_TodoValido_NoLanzaExcepcion()
        {
            var persona = new Persona { Nombre = "Ana", Apellido = "Diaz", Cuit = "20-12345678-9" };
            var proveedor = new Proveedor { NombreEmpresa = "Empresa X" };

            _logica.Validar(persona, proveedor);
        }
    }
    
    [TestClass]
    public class LogicaProveedorMockedTests
    {
        [TestMethod]
        public void Agregar_DatosValidos_InvocaRepositorioAgregar()
        {
            var repoMock = new Mock<IProveedorRepository>();
            var logica = new LogicaProveedor(repoMock.Object);
            var persona = new Persona { Nombre = "Ana", Apellido = "Diaz" };
            var proveedor = new Proveedor { NombreEmpresa = "Empresa X" };

            logica.Agregar(persona, proveedor);

            repoMock.Verify(r => r.Agregar(persona, proveedor), Times.Once);
        }

        [TestMethod]
        public void Agregar_DatosInvalidos_NoInvocaRepositorio()
        {
            var repoMock = new Mock<IProveedorRepository>();
            var logica = new LogicaProveedor(repoMock.Object);
            var persona = new Persona { Nombre = "", Apellido = "Diaz" }; // nombre inválido
            var proveedor = new Proveedor { NombreEmpresa = "Empresa X" };

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(persona, proveedor));
            repoMock.Verify(r => r.Agregar(It.IsAny<Persona>(), It.IsAny<Proveedor>()), Times.Never);
        }

        [TestMethod]
        public void BajaLogica_InvocaRepositorioConElIdCorrecto()
        {
            var repoMock = new Mock<IProveedorRepository>();
            var logica = new LogicaProveedor(repoMock.Object);

            logica.BajaLogica(7);

            repoMock.Verify(r => r.BajaLogica(7), Times.Once);
        }

        [TestMethod]
        public void Buscar_TerminoVacio_InvocaObtenerTodos()
        {
            var repoMock = new Mock<IProveedorRepository>();
            repoMock.Setup(r => r.ObtenerTodos()).Returns(new List<Proveedor>());
            var logica = new LogicaProveedor(repoMock.Object);

            logica.Buscar("");

            repoMock.Verify(r => r.ObtenerTodos(), Times.Once);
            repoMock.Verify(r => r.BuscarPorNombreOCuit(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Buscar_ConTermino_InvocaBuscarPorNombreOCuit()
        {
            var repoMock = new Mock<IProveedorRepository>();
            repoMock.Setup(r => r.BuscarPorNombreOCuit("20-1")).Returns(new List<Proveedor>());
            var logica = new LogicaProveedor(repoMock.Object);

            logica.Buscar("20-1");

            repoMock.Verify(r => r.BuscarPorNombreOCuit("20-1"), Times.Once);
            repoMock.Verify(r => r.ObtenerTodos(), Times.Never);
        }
    }
}
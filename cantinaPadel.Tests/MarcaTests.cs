using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // Modelo: Marcas - Validar() normaliza el nombre (Trim) y chequea que sea obligatorio y no supere la longitud máxima
    [TestClass]
    public class MarcaModelTests
    {
        private static Marca CrearMarcaValida()
        {
            return new Marca
            {
                Nombre = "Wilson",
                Activa = true
            };
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Validar_NombreVacio_LanzaArgumentException(string? valor)
        {
            var marca = CrearMarcaValida();
            marca.Nombre = valor!;

            Assert.ThrowsExactly<ArgumentException>(() => marca.Validar());
        }

        [TestMethod]
        public void Validar_NombreExcedeLongitud_LanzaArgumentException()
        {
            var marca = CrearMarcaValida();
            marca.Nombre = new string('a', 26);

            Assert.ThrowsExactly<ArgumentException>(() => marca.Validar());
        }

        [TestMethod]
        public void Validar_ConEspaciosAlrededor_RecortaYAcepta()
        {
            var marca = CrearMarcaValida();
            marca.Nombre = "  Wilson  ";

            marca.Validar();

            Assert.AreEqual("Wilson", marca.Nombre);
        }

        [TestMethod]
        public void Validar_TodoValido_NoLanzaExcepcion()
        {
            // Caso feliz: el nombre pasa la validación sin problemas
            var marca = CrearMarcaValida();

            marca.Validar();
        }

        [TestMethod]
        public void Normalizar_NombreNulo_LoConvierteEnCadenaVacia()
        {
            var marca = CrearMarcaValida();
            marca.Nombre = null!;

            marca.Normalizar();

            Assert.AreEqual(string.Empty, marca.Nombre);
        }
    }
}
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // Modelo: Categoria - Validar() se encarga de normalizar el nombre (Trim) y chequear las reglas: nombre obligatorio, longitud máxima,
    // sin números y porcentaje de ganancia no negativo
    [TestClass]
    public class CategoriaModelTests
    {
        private static Categoria CrearCategoriaValida()
        {
            return new Categoria
            {
                Nombre = "Bebidas",
                Activa = true,
                PorcentajeGanancia = 30m
            };
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Validar_NombreVacio_LanzaArgumentException(string? valor)
        {
            var categoria = CrearCategoriaValida();
            categoria.Nombre = valor!;

            Assert.ThrowsExactly<ArgumentException>(() => categoria.Validar());
        }

        [TestMethod]
        public void Validar_NombreExcedeLongitud_LanzaArgumentException()
        {
            var categoria = CrearCategoriaValida();
            categoria.Nombre = new string('a', 26); // límite es 25

            Assert.ThrowsExactly<ArgumentException>(() => categoria.Validar());
        }

        [TestMethod]
        [DataRow("Bebidas2")]
        [DataRow("Snack s1")]
        [DataRow("123")]
        public void Validar_NombreConNumeros_LanzaArgumentException(string valor)
        {
            var categoria = CrearCategoriaValida();
            categoria.Nombre = valor;

            Assert.ThrowsExactly<ArgumentException>(() => categoria.Validar());
        }

        [TestMethod]
        [DataRow(-0.01)]
        [DataRow(-100)]
        public void Validar_PorcentajeGananciaNegativo_LanzaArgumentException(double valor)
        {
            var categoria = CrearCategoriaValida();
            categoria.PorcentajeGanancia = (decimal)valor;

            Assert.ThrowsExactly<ArgumentException>(() => categoria.Validar());
        }

        [TestMethod]
        public void Validar_PorcentajeGananciaEnCero_NoLanzaExcepcion()
        {
            // Cero es un valor límite válido, no negativo
            var categoria = CrearCategoriaValida();
            categoria.PorcentajeGanancia = 0m;

            categoria.Validar();
        }

        [TestMethod]
        public void Validar_ConEspaciosAlrededor_RecortaYAcepta()
        {
            var categoria = CrearCategoriaValida();
            categoria.Nombre = "  Bebidas  ";

            categoria.Validar();

            Assert.AreEqual("Bebidas", categoria.Nombre);
        }

        [TestMethod]
        public void Validar_TodoValido_NoLanzaExcepcion()
        {
            // Caso "feliz": nombre y porcentaje de ganancia pasan juntos la validación
            var categoria = CrearCategoriaValida();

            categoria.Validar();
        }

        [TestMethod]
        public void Normalizar_NombreNulo_LoConvierteEnCadenaVacia()
        {
            var categoria = CrearCategoriaValida();
            categoria.Nombre = null!;

            categoria.Normalizar();

            Assert.AreEqual(string.Empty, categoria.Nombre);
        }
    }
}
using cantinaPadel.BLL;
using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace cantinaPadel.Tests
{
    // Pruebas unitarias de la clase Cliente
    [TestClass]
    public class ActualizacionPreciosTests
    {
        [TestMethod]
        public void ConfirmarActualizacion_ConPreciosNuevos_ActualizaSoloLosProductosIncluidos()
        {
            var repo = new ProductoRepositoryFake();
            repo.Cargar(
                new Producto { IdProducto = 1, PrecioVenta = 100m },
                new Producto { IdProducto = 2, PrecioVenta = 200m },
                new Producto { IdProducto = 3, PrecioVenta = 300m });
            var logica = new LogicaProducto(repo);
            var preciosFinales = new Dictionary<int, decimal> { { 1, 110m }, { 3, 330m } };

            logica.ConfirmarActualizacion(preciosFinales);

            Assert.AreEqual(110m, repo.ObtenerPorId(1)!.PrecioVenta);
            Assert.AreEqual(200m, repo.ObtenerPorId(2)!.PrecioVenta);
            Assert.AreEqual(330m, repo.ObtenerPorId(3)!.PrecioVenta);
        }

        [TestMethod]
        public void ConfirmarActualizacion_DiccionarioVacio_NoModificaNingunProducto()
        {
            var repo = new ProductoRepositoryFake();
            repo.Cargar(
                new Producto { IdProducto = 1, PrecioVenta = 100m },
                new Producto { IdProducto = 2, PrecioVenta = 200m });
            var logica = new LogicaProducto(repo);

            logica.ConfirmarActualizacion(new Dictionary<int, decimal>());

            Assert.AreEqual(100m, repo.ObtenerPorId(1)!.PrecioVenta);
            Assert.AreEqual(200m, repo.ObtenerPorId(2)!.PrecioVenta);
        }

        [TestMethod]
        public void ConfirmarActualizacion_ConIdInexistente_NoLanzaExcepcionYNoAfectaOtros()
        {
            var repo = new ProductoRepositoryFake();
            repo.Cargar(
                new Producto { IdProducto = 1, PrecioVenta = 100m },
                new Producto { IdProducto = 2, PrecioVenta = 200m });
            var logica = new LogicaProducto(repo);
            var preciosFinales = new Dictionary<int, decimal> { { 1, 110m }, { 999, 500m } };

            logica.ConfirmarActualizacion(preciosFinales);

            Assert.AreEqual(110m, repo.ObtenerPorId(1)!.PrecioVenta);
            Assert.AreEqual(200m, repo.ObtenerPorId(2)!.PrecioVenta);
        }
    }
}
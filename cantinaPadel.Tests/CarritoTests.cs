using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // BLL: LogicaCarrito  (Pantalla de Ventas con Carrito)
    // Agregar/quitar productos,/ actualizar cantidades, validar stock disponible y calcular el total
    [TestClass]
    public class LogicaCarritoTests
    {
        private static Producto CrearProducto(
            int id = 1,
            string nombre = "Gaseosa 500ml",
            decimal precioVenta = 100m,
            int stock = 10,
            bool activo = true)
            => new Producto
            {
                IdProducto = id,
                Nombre = nombre,
                PrecioVenta = precioVenta,
                StockActual = stock,
                Activo = activo
            };

        // AgregarProducto

        [TestMethod]
        public void AgregarProducto_ProductoNuevo_LoAgregaConLaCantidadIndicada()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 10);

            carrito.AgregarProducto(producto, 3);

            Assert.HasCount(1, carrito.Items);
            Assert.AreEqual(3, carrito.Items[0].Cantidad);
        }

        [TestMethod]
        public void AgregarProducto_SinIndicarCantidad_AgregaUno()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 10);

            carrito.AgregarProducto(producto);

            Assert.AreEqual(1, carrito.Items[0].Cantidad);
        }

        [TestMethod]
        public void AgregarProducto_YaEstabaEnElCarrito_SumaALaCantidadExistente()
        {
            // Ej: se escanea dos veces el mismo código de barras
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 10);

            carrito.AgregarProducto(producto, 2);
            carrito.AgregarProducto(producto, 3);

            Assert.HasCount(1, carrito.Items);
            Assert.AreEqual(5, carrito.Items[0].Cantidad);
        }

        [TestMethod]
        public void AgregarProducto_CantidadSuperaElStockDisponible_LanzaArgumentException()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 5);

            Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(producto, 6));
        }

        [TestMethod]
        public void AgregarProducto_LaSumaConLoYaAgregadoSuperaElStock_LanzaArgumentException()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 5);
            carrito.AgregarProducto(producto, 3);

            // 3 ya en el carrito + 3 nuevos = 6, supera el stock de 5
            Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(producto, 3));

            // La excepción no debe haber modificado la cantidad ya cargada
            Assert.AreEqual(3, carrito.Items[0].Cantidad);
        }

        [TestMethod]
        public void AgregarProducto_ProductoInactivo_LanzaArgumentException()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(activo: false);

            Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(producto, 1));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void AgregarProducto_CantidadCeroONegativa_LanzaArgumentException(int cantidad)
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto();

            Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(producto, cantidad));
        }

        [TestMethod]
        public void AgregarProducto_ProductoNulo_LanzaArgumentException()
        {
            var carrito = new LogicaCarrito();

            Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(null!, 1));
        }

        // ActualizarCantidad

        [TestMethod]
        public void ActualizarCantidad_ProductoEnElCarrito_CambiaLaCantidad()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 10);
            carrito.AgregarProducto(producto, 2);

            carrito.ActualizarCantidad(producto.IdProducto, 7);

            Assert.AreEqual(7, carrito.Items[0].Cantidad);
        }

        [TestMethod]
        public void ActualizarCantidad_SuperaElStockDisponible_LanzaArgumentException()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto(stock: 5);
            carrito.AgregarProducto(producto, 2);

            Assert.ThrowsExactly<ArgumentException>(() => carrito.ActualizarCantidad(producto.IdProducto, 6));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void ActualizarCantidad_CeroONegativa_QuitaElProductoDelCarrito(int cantidadNueva)
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto();
            carrito.AgregarProducto(producto, 2);

            carrito.ActualizarCantidad(producto.IdProducto, cantidadNueva);

            Assert.HasCount(0, carrito.Items);
        }

        [TestMethod]
        public void ActualizarCantidad_ProductoQueNoEstaEnElCarrito_LanzaArgumentException()
        {
            var carrito = new LogicaCarrito();

            Assert.ThrowsExactly<ArgumentException>(() => carrito.ActualizarCantidad(999, 1));
        }

        // QuitarProducto / Vaciar

        [TestMethod]
        public void QuitarProducto_ProductoEnElCarrito_LoElimina()
        {
            var carrito = new LogicaCarrito();
            var producto = CrearProducto();
            carrito.AgregarProducto(producto, 2);

            carrito.QuitarProducto(producto.IdProducto);

            Assert.HasCount(0, carrito.Items);
        }

        [TestMethod]
        public void QuitarProducto_ProductoQueNoEstaEnElCarrito_NoLanzaExcepcion()
        {
            var carrito = new LogicaCarrito();

            carrito.QuitarProducto(999);
        }

        [TestMethod]
        public void Vaciar_ConVariosProductos_DejaElCarritoVacio()
        {
            var carrito = new LogicaCarrito();
            carrito.AgregarProducto(CrearProducto(id: 1), 1);
            carrito.AgregarProducto(CrearProducto(id: 2), 1);

            carrito.Vaciar();

            Assert.HasCount(0, carrito.Items);
        }

        // Total / CantidadItems

        [TestMethod]
        public void Total_SumaLosSubtotalesConIva()
        {
            var carrito = new LogicaCarrito();
            // PrecioConIva = PrecioVenta * 1.21, redondeado a 2 decimales
            carrito.AgregarProducto(CrearProducto(id: 1, precioVenta: 100m, stock: 10), 2); // 121.00 c/u -> 242.00
            carrito.AgregarProducto(CrearProducto(id: 2, precioVenta: 50m, stock: 10), 3);  // 60.50 c/u  -> 181.50

            Assert.AreEqual(423.50m, carrito.Total);
        }

        [TestMethod]
        public void CantidadItems_SumaLasCantidadesDeTodosLosProductos()
        {
            var carrito = new LogicaCarrito();
            carrito.AgregarProducto(CrearProducto(id: 1), 2);
            carrito.AgregarProducto(CrearProducto(id: 2), 5);

            Assert.AreEqual(7, carrito.CantidadItems);
        }

        [TestMethod]
        public void Total_CarritoVacio_EsCero()
        {
            var carrito = new LogicaCarrito();

            Assert.AreEqual(0m, carrito.Total);
        }
    }
}
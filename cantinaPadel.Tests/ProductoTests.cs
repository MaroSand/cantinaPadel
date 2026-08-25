using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    [TestClass]
    public class ProductoModelTests
    {
        [TestMethod]
        public void StockBajo_CuandoStockActualEsIgualAlMinimo_DevuelveTrue()
        {
            var producto = new Producto { StockActual = 5, StockMinimo = 5 };

            Assert.IsTrue(producto.StockBajo);
        }
    }

    [TestClass]
    public class LogicaProductoTests
    {
        [TestMethod]
        public void CalcularPrecioConIva_RedondeaADosDecimales()
        {
            var logica = new LogicaProducto(new ProductoRepositoryFake());

            decimal resultado = logica.CalcularPrecioConIva(99.99m);

            Assert.AreEqual(120.99m, resultado);
        }

        [TestMethod]
        public void CalcularPrecioVenta_ConCategoriaValida_UsaPorcentajeDeGanancia()
        {
            var repo = new ProductoRepositoryFake { PorcentajeGananciaCategoria = 35m };
            var logica = new LogicaProducto(repo);

            decimal resultado = logica.CalcularPrecioVenta(100m, 1);

            Assert.AreEqual(135m, resultado);
        }

        [TestMethod]
        public void Agregar_ProductoValido_GuardaProductoConNombreNormalizadoYPrecioVentaCalculado()
        {
            var repo = new ProductoRepositoryFake { PorcentajeGananciaCategoria = 25m };
            var logica = new LogicaProducto(repo);
            var producto = CrearProductoValido(nombre: "  Pelotas tubo x3  ", codigoBarras: "  ABC123  ");

            logica.Agregar(producto);

            Assert.AreSame(producto, repo.ProductoAgregado);
            Assert.AreEqual("Pelotas tubo x3", producto.Nombre);
            Assert.AreEqual("ABC123", producto.CodigoBarras);
            Assert.AreEqual(125m, producto.PrecioVenta);
        }

        [TestMethod]
        public void Agregar_SinCodigoBarras_GeneraCodigoUnicoAntesDeGuardar()
        {
            var repo = new ProductoRepositoryFake();
            var logica = new LogicaProducto(repo);
            var producto = CrearProductoValido(codigoBarras: null);

            logica.Agregar(producto);

            Assert.IsFalse(string.IsNullOrWhiteSpace(producto.CodigoBarras));
            Assert.IsTrue(producto.CodigoBarras!.StartsWith("20"));
            Assert.AreEqual(20, producto.CodigoBarras.Length);
            Assert.AreSame(producto, repo.ProductoAgregado);
        }

        [TestMethod]
        public void Agregar_CodigoBarrasDuplicado_LanzaArgumentExceptionYNoGuarda()
        {
            var repo = new ProductoRepositoryFake();
            repo.Cargar(new Producto { IdProducto = 10, CodigoBarras = "ABC123" });
            var logica = new LogicaProducto(repo);
            var producto = CrearProductoValido(codigoBarras: " ABC123 ");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(producto));
            Assert.IsNull(repo.ProductoAgregado);
        }

        [TestMethod]
        public void Modificar_MismoCodigoBarrasDelProductoActual_NoLoTomaComoDuplicado()
        {
            var repo = new ProductoRepositoryFake();
            repo.Cargar(new Producto { IdProducto = 5, CodigoBarras = "ABC123" });
            var logica = new LogicaProducto(repo);
            var producto = CrearProductoValido(idProducto: 5, codigoBarras: "ABC123");

            logica.Modificar(producto);

            Assert.AreSame(producto, repo.ProductoModificado);
            Assert.IsTrue(repo.ConsultasCodigo.Any(c => c.Codigo == "ABC123" && c.IdExcluir == 5));
        }

        [TestMethod]
        public void Modificar_CodigoBarrasDeOtroProducto_LanzaArgumentExceptionYNoModifica()
        {
            var repo = new ProductoRepositoryFake();
            repo.Cargar(new Producto { IdProducto = 7, CodigoBarras = "ABC123" });
            var logica = new LogicaProducto(repo);
            var producto = CrearProductoValido(idProducto: 5, codigoBarras: "ABC123");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Modificar(producto));
            Assert.IsNull(repo.ProductoModificado);
        }

        [DataTestMethod]
        [DataRow("   ", "COD123", 1, 100, 2, DisplayName = "Sin nombre")]
        [DataRow("Paleta control", "COD123", 0, 100, 2, DisplayName = "Sin categoría")]
        [DataRow("Paleta control", "COD123", 1, 0, 2, DisplayName = "Precio de costo en cero")]
        [DataRow("Paleta control", "COD123", 1, 100, -1, DisplayName = "Stock mínimo negativo")]
        public void Agregar_ConDatosInvalidos_LanzaArgumentException(
            string nombre, string codigoBarras, int idCategoria, double precioCosto, int stockMinimo)
        {
            var logica = new LogicaProducto(new ProductoRepositoryFake());
            var producto = CrearProductoValido(
                nombre: nombre,
                codigoBarras: codigoBarras,
                idCategoria: idCategoria,
                precioCosto: (decimal)precioCosto,
                stockMinimo: stockMinimo);

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(producto));
        }
        
        private static Producto CrearProductoValido(
            int idProducto = 1,
            string nombre = "Paleta control",
            string? codigoBarras = "COD123",
            int idCategoria = 1,
            decimal precioCosto = 100m,
            int stockMinimo = 2)
        {
            return new Producto
            {
                IdProducto = idProducto,
                Nombre = nombre,
                CodigoBarras = codigoBarras,
                IdCategoria = idCategoria,
                PrecioCosto = precioCosto,
                StockActual = 10,
                StockMinimo = stockMinimo,
                Activo = true
            };
        }
    }

    internal sealed class ProductoRepositoryFake : IProductoRepository
    {
        private readonly List<Producto> _productos = new();

        public decimal PorcentajeGananciaCategoria { get; set; } = 30m;
        public Producto? ProductoAgregado { get; private set; }
        public Producto? ProductoModificado { get; private set; }
        public int? IdBajaLogica { get; private set; }
        public List<(string Codigo, int? IdExcluir)> ConsultasCodigo { get; } = new();

        public void Cargar(params Producto[] productos) => _productos.AddRange(productos);

        public List<Producto> ObtenerTodos(bool? activo = true) => _productos
            .Where(p => !activo.HasValue || p.Activo == activo.Value)
            .ToList();

        public List<Producto> Buscar(string? texto, int? idCategoria, int? idMarca, bool? activo = true)
        {
            return ObtenerTodos(activo)
                .Where(p => string.IsNullOrWhiteSpace(texto) || p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .Where(p => !idCategoria.HasValue || p.IdCategoria == idCategoria.Value)
                .Where(p => !idMarca.HasValue || p.IdMarca == idMarca.Value)
                .ToList();
        }

        public Producto? ObtenerPorCodigoBarras(string codigoBarras) => _productos
            .FirstOrDefault(p => p.Activo && p.CodigoBarras == codigoBarras);

        public Producto? ObtenerPorId(int idProducto) => _productos
            .FirstOrDefault(p => p.IdProducto == idProducto);

        public decimal ObtenerPorcentajeGananciaCategoria(int idCategoria) => PorcentajeGananciaCategoria;

        public bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null)
        {
            ConsultasCodigo.Add((codigoBarras, idProductoExcluir));

            return _productos.Any(p =>
                p.CodigoBarras == codigoBarras &&
                (!idProductoExcluir.HasValue || p.IdProducto != idProductoExcluir.Value));
        }

        public void Agregar(Producto producto)
        {
            ProductoAgregado = producto;
            _productos.Add(producto);
        }

        public void Modificar(Producto producto)
        {
            ProductoModificado = producto;
        }

        public void BajaLogica(int idProducto)
        {
            IdBajaLogica = idProducto;
        }

        public List<Producto> ObtenerPorCriterio(int? idCategoria, int? idMarca, int? idProducto)
        {
            return _productos
                .Where(p => !idCategoria.HasValue || p.IdCategoria == idCategoria.Value)
                .Where(p => !idMarca.HasValue || p.IdMarca == idMarca.Value)
                .Where(p => !idProducto.HasValue || p.IdProducto == idProducto.Value)
                .ToList();
        }

        public void ActualizarPrecios(Dictionary<int, decimal> preciosNuevos)
        {
            foreach (var producto in _productos)
            {
                if (preciosNuevos.TryGetValue(producto.IdProducto, out decimal precioNuevo))
                    producto.PrecioVenta = precioNuevo;
            }
        }
    }
}
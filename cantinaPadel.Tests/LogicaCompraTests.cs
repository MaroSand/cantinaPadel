using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

// BLL: LogicaCompra - alta, listado y anulación de compras a proveedores.
[TestClass]
public class LogicaCompraTests
{
    private const int IdProveedorValido = 1;
    private const int IdEmpleadoValido = 5;

    // Fakes

    private sealed class CompraRepositoryFake : ICompraRepository
    {
        public Compra? Registrada { get; private set; }
        public IReadOnlyDictionary<int, decimal>? PreciosVentaRegistrados { get; private set; }
        public (int? IdProveedor, DateTime? Desde, DateTime? Hasta, string? Producto, bool IncluirAnuladas)? UltimaBusqueda { get; private set; }
        public int? IdCompraAnulada { get; private set; }

        public Compra Registrar(Compra compra, IReadOnlyDictionary<int, decimal> preciosVentaNuevos)
        {
            compra.IdCompra = 1;
            Registrada = compra;
            PreciosVentaRegistrados = preciosVentaNuevos;
            return compra;
        }

        public List<Compra> Buscar(int? idProveedor, DateTime? desde, DateTime? hasta, string? producto, bool incluirAnuladas = false)
        {
            UltimaBusqueda = (idProveedor, desde, hasta, producto, incluirAnuladas);
            return new List<Compra>();
        }

        public Compra? ObtenerPorId(int idCompra) => Registrada;

        public Compra Anular(int idCompra)
        {
            IdCompraAnulada = idCompra;
            if (Registrada == null || Registrada.IdCompra != idCompra)
                throw new InvalidOperationException("La compra seleccionada ya no existe.");
            Registrada.Anulada = true;
            return Registrada;
        }
    }

    private sealed class ProveedorRepositoryFake : IProveedorRepository
    {
        private readonly List<Proveedor> _proveedores;
        public ProveedorRepositoryFake(params Proveedor[] proveedores) => _proveedores = proveedores.ToList();

        public List<Proveedor> ObtenerTodos() => _proveedores;
        public List<Proveedor> BuscarPorNombreOCuit(string termino) => throw new NotSupportedException();
        public Proveedor? ObtenerPorId(int idProveedor) => _proveedores.FirstOrDefault(p => p.IdProveedor == idProveedor);
        public void Agregar(Persona persona, Proveedor proveedor) => throw new NotSupportedException();
        public void Modificar(Persona persona, Proveedor proveedor) => throw new NotSupportedException();
        public void BajaLogica(int idProveedor) => throw new NotSupportedException();
    }

    private sealed class ProductoRepositoryFake : IProductoRepository
    {
        private readonly List<Producto> _productos;
        public ProductoRepositoryFake(params Producto[] productos) => _productos = productos.ToList();

        public Producto? ObtenerPorId(int idProducto) => _productos.FirstOrDefault(p => p.IdProducto == idProducto);

        public List<Producto> ObtenerTodos(bool? activo = true) => throw new NotSupportedException();
        public List<Producto> Buscar(string? texto, int? idCategoria, int? idMarca, bool? activo = true) => throw new NotSupportedException();
        public Producto? ObtenerPorCodigoBarras(string codigoBarras) => throw new NotSupportedException();
        public decimal ObtenerPorcentajeGananciaCategoria(int idCategoria) => throw new NotSupportedException();
        public bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null) => throw new NotSupportedException();
        public void Agregar(Producto producto) => throw new NotSupportedException();
        public void Modificar(Producto producto) => throw new NotSupportedException();
        public void BajaLogica(int idProducto) => throw new NotSupportedException();
        public List<Producto> ObtenerPorCriterio(int? idCategoria, int? idMarca, int? idProducto) => throw new NotSupportedException();
        public void ActualizarPrecios(Dictionary<int, decimal> preciosNuevos) => throw new NotSupportedException();
    }

    // ---------- Datos de prueba ----------

    private static Proveedor CrearProveedor(int id = IdProveedorValido, bool activo = true, string empresa = "Distribuidora Bebidas S.A.")
        => new()
        {
            IdProveedor = id,
            NombreEmpresa = empresa,
            Persona = new Persona { Nombre = "Juan", Apellido = "Pérez", Activo = activo }
        };

    private static Producto CrearProducto(int id, string nombre = "Coca Cola 500ml", bool activo = true)
    {
        decimal costoInicial = id == 2 ? 400m : 800m;
        decimal precioVentaInicial = id == 2 ? 520m : 1040m;
        int stockInicial = id == 2 ? 60 : 48;

        return new Producto
        {
            IdProducto = id,
            IdCategoria = 1,
            Nombre = nombre,
            Activo = activo,
            PrecioCosto = costoInicial,
            PrecioVenta = precioVentaInicial,
            StockActual = stockInicial,
            IdProveedor = 1,
            Categoria = new Categoria { IdCategoria = 1, Nombre = "Bebidas", PorcentajeGanancia = 30m }
        };
    }

    private static (LogicaCompra Logica, CompraRepositoryFake Repo) CrearLogica(
        Proveedor? proveedor = null, Producto[]? productos = null, DateTime? ahora = null)
    {
        var repo = new CompraRepositoryFake();
        var logica = new LogicaCompra(
            repo,
            new ProveedorRepositoryFake(proveedor ?? CrearProveedor()),
            new ProductoRepositoryFake(productos ?? new[] { CrearProducto(1), CrearProducto(2, "Agua") }),
            ahora == null ? null : () => ahora.Value);
        return (logica, repo);
    }

    private static List<ItemCompra> UnaLinea(int idProducto = 1, int cantidad = 10, decimal costo = 800m)
        => new() { new ItemCompra(CrearProducto(idProducto), cantidad, costo) };

    // RegistrarCompra

    [TestMethod]
    public void RegistrarCompra_Valida_GuardaConUsuarioActivoFechaHoraYTotal()
    {
        var ahora = DateTime.Now;
        var coca = CrearProducto(1);
        var agua = CrearProducto(2, "Agua Mineral Sin Gas 500ml");
        agua.PrecioCosto = 400m;
        agua.PrecioVenta = 520m;
        agua.StockActual = 60;
        var (logica, repo) = CrearLogica(productos: new[] { coca, agua }, ahora: ahora);
        var items = new List<ItemCompra>
        {
            new(coca, 10, 800m),
            new(agua, 5, 400m)
        };

        var compra = logica.RegistrarCompra(IdProveedorValido, ahora.Date, "0001-00001234", items, IdEmpleadoValido);

        Assert.AreSame(compra, repo.Registrada);
        Assert.AreEqual(IdProveedorValido, compra.IdProveedor);
        Assert.AreEqual(IdEmpleadoValido, compra.IdEmpleado);
        Assert.AreEqual("0001-00001234", compra.NumeroComprobante);
        Assert.AreEqual(ahora.Date + ahora.TimeOfDay, compra.FechaCompra);
        Assert.IsFalse(compra.Anulada);
        Assert.AreEqual(10000m, compra.Total);
        Assert.HasCount(2, compra.Detalles);
        Assert.AreEqual(8000m, compra.Detalles[0].Subtotal);
        Assert.AreEqual(2000m, compra.Detalles[1].Subtotal);
        // El detalle guarda solo el id del producto, no el objeto (para que EF no intente insertarlo)
        Assert.IsTrue(compra.Detalles.All(d => d.Producto == null));
    }

    [TestMethod]
    public void RegistrarCompra_FechaPosteriorAHoy_LanzaArgumentExceptionYNoGuarda()
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today.AddDays(1), null, UnaLinea(), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_SinProductos_LanzaArgumentExceptionYNoGuarda()
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, new List<ItemCompra>(), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_SinProveedor_LanzaArgumentException()
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(0, DateTime.Today, null, UnaLinea(), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_ProveedorInactivo_LanzaArgumentException()
    {
        var (logica, repo) = CrearLogica(proveedor: CrearProveedor(activo: false));

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_SinEmpleadoAutenticado_LanzaArgumentException()
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(), 0));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    [DataRow(0, "800")]
    [DataRow(10, "0")]
    public void RegistrarCompra_CantidadOCostoMenorOIgualACero_LanzaArgumentExceptionYNoGuarda(int cantidad, string costo)
    {
        var (logica, repo) = CrearLogica();
        var items = UnaLinea(cantidad: cantidad, costo: decimal.Parse(costo, System.Globalization.CultureInfo.InvariantCulture));

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, items, IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_ProductosRepetidos_SeGuardanEnUnaSolaLineaConLaCantidadSumada()
    {
        var (logica, repo) = CrearLogica();
        var items = new List<ItemCompra>
        {
            new(CrearProducto(1), 10, 800m),
            new(CrearProducto(2, "Agua"), 3, 400m),
            new(CrearProducto(1), 5, 800m)
        };

        var compra = logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, items, IdEmpleadoValido);

        Assert.HasCount(2, compra.Detalles);
        var coca = compra.Detalles.Single(d => d.IdProducto == 1);
        Assert.AreEqual(15, coca.Cantidad);
        Assert.AreEqual(12000m, coca.Subtotal);
        Assert.AreEqual(13200m, compra.Total);
        Assert.IsNotNull(repo.Registrada);
    }

    [TestMethod]
    public void PrevisualizarImpactoCompra_CalculaStockCostoProveedorYPrecioNuevo()
    {
        var producto = CrearProducto(1);
        producto.StockActual = 12;
        producto.PrecioCosto = 800m;
        producto.PrecioVenta = 1040m;
        var (logica, _) = CrearLogica(productos: new[] { producto });

        var preview = logica.PrevisualizarImpactoCompra(
            IdProveedorValido, UnaLinea(idProducto: 1, cantidad: 5, costo: 1000m));

        var impacto = preview.Productos.Single();
        Assert.AreEqual(12, impacto.StockActual);
        Assert.AreEqual(17, impacto.StockNuevo);
        Assert.AreEqual(800m, impacto.CostoActual);
        Assert.AreEqual(1000m, impacto.CostoCompra);
        Assert.AreEqual(1040m, impacto.PrecioVentaActual);
        Assert.AreEqual(1300m, impacto.PrecioVentaNuevo);
        Assert.IsTrue(impacto.RecalculaPrecio);
        Assert.AreEqual(1300m, preview.PreciosVentaNuevos[1]);
        Assert.AreEqual("Distribuidora Bebidas S.A. - Pérez Juan", impacto.ProveedorNuevo);
    }

    [TestMethod]
    public void PrevisualizarImpactoCompra_StockPositivoYCostoMenorOIgual_ConservaPrecioVenta()
    {
        var producto = CrearProducto(1);
        producto.StockActual = 48;
        producto.PrecioCosto = 800m;
        producto.PrecioVenta = 1040m;
        var (logica, _) = CrearLogica(productos: new[] { producto });

        var preview = logica.PrevisualizarImpactoCompra(
            IdProveedorValido, new[] { new ItemCompra(producto, 2, 700m) });

        var impacto = preview.Productos.Single();
        Assert.IsFalse(impacto.RecalculaPrecio);
        Assert.IsNull(impacto.PrecioVentaNuevo);
        Assert.AreEqual(1040m, impacto.PrecioVentaActual);
        Assert.IsFalse(preview.PreciosVentaNuevos.ContainsKey(1));
    }

    [TestMethod]
    public void PrevisualizarImpactoCompra_StockCero_RecalculaAunqueElCostoSeaMenor()
    {
        var producto = CrearProducto(1);
        producto.StockActual = 0;
        producto.PrecioCosto = 800m;
        producto.PrecioVenta = 1040m;
        var (logica, _) = CrearLogica(productos: new[] { producto });

        var preview = logica.PrevisualizarImpactoCompra(
            IdProveedorValido, new[] { new ItemCompra(producto, 2, 700m) });

        var impacto = preview.Productos.Single();
        Assert.IsTrue(impacto.RecalculaPrecio);
        Assert.AreEqual(910m, impacto.PrecioVentaNuevo);
        Assert.AreEqual(910m, preview.PreciosVentaNuevos[1]);
    }

    [TestMethod]
    [DataRow("Varios")]
    [DataRow("Sin Categoria")]
    public void PrevisualizarImpactoCompra_CategoriaDePrecioManual_ConservaPrecio(string categoria)
    {
        var producto = CrearProducto(1, "Artículo varios");
        producto.Categoria.Nombre = categoria;
        producto.PrecioVenta = 1040m;
        var (logica, _) = CrearLogica(productos: new[] { producto });

        var preview = logica.PrevisualizarImpactoCompra(
            IdProveedorValido, UnaLinea(idProducto: 1, cantidad: 2, costo: 900m));

        var impacto = preview.Productos.Single();
        Assert.IsFalse(impacto.RecalculaPrecio);
        Assert.IsNull(impacto.PrecioVentaNuevo);
        Assert.AreEqual(1040m, impacto.PrecioVentaActual);
        Assert.IsFalse(preview.PreciosVentaNuevos.ContainsKey(1));
    }

    [TestMethod]
    public void RegistrarCompra_PasaLosPreciosCalculadosAlRepositorio()
    {
        var producto = CrearProducto(1);
        producto.Categoria.PorcentajeGanancia = 30m;
        var (logica, repo) = CrearLogica(productos: new[] { producto });

        logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null,
            UnaLinea(idProducto: 1, cantidad: 3, costo: 1000m), IdEmpleadoValido);

        Assert.AreEqual(1300m, repo.PreciosVentaRegistrados![1]);
    }

    [TestMethod]
    public void RegistrarCompra_ProductoInexistenteOInactivo_LanzaArgumentExceptionYNoGuarda()
    {
        var (logica, repo) = CrearLogica(productos: new[] { CrearProducto(1, activo: false) });

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(1), IdEmpleadoValido));
        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(77), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    // Proveedores

    [TestMethod]
    public void ObtenerProveedoresActivos_NoIncluyeLosDadosDeBaja_YArmaElTexto()
    {
        var repo = new CompraRepositoryFake();
        var logica = new LogicaCompra(
            repo,
            new ProveedorRepositoryFake(CrearProveedor(1, activo: true, empresa: "Padel Supplies SRL"), CrearProveedor(2, activo: false, empresa: "Vieja SA")),
            new ProductoRepositoryFake());

        var activos = logica.ObtenerProveedoresActivos();
        var todos = logica.ObtenerTodosLosProveedores();

        Assert.HasCount(1, activos);
        Assert.AreEqual(1, activos[0].IdProveedor);
        Assert.AreEqual("Padel Supplies SRL - Pérez Juan", activos[0].Texto);
        Assert.HasCount(2, todos);
    }

    // Buscar

    [TestMethod]
    public void Buscar_PasaLosFiltrosAlRepositorio()
    {
        var (logica, repo) = CrearLogica();
        var desde = new DateTime(2026, 10, 1);
        var hasta = new DateTime(2026, 10, 19);

        logica.Buscar(IdProveedorValido, desde, hasta, "coca", incluirAnuladas: true);

        var busqueda = repo.UltimaBusqueda!.Value;
        Assert.AreEqual(IdProveedorValido, busqueda.IdProveedor);
        Assert.AreEqual(desde, busqueda.Desde);
        Assert.AreEqual(hasta, busqueda.Hasta);
        Assert.AreEqual("coca", busqueda.Producto);
        Assert.IsTrue(busqueda.IncluirAnuladas);
    }

    [TestMethod]
    public void Buscar_PorDefectoNoTraeAnuladas()
    {
        var (logica, repo) = CrearLogica();

        logica.Buscar(null, null, null, null);

        Assert.IsFalse(repo.UltimaBusqueda!.Value.IncluirAnuladas);
    }

    [TestMethod]
    public void Buscar_DesdePosteriorAHasta_LanzaArgumentException()
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.Buscar(null, new DateTime(2026, 10, 20), new DateTime(2026, 10, 1), null));
        Assert.IsNull(repo.UltimaBusqueda);
    }

    [TestMethod]
    public void AnularCompra_Administrador_DelegaAlRepositorio()
    {
        var repo = new CompraRepositoryFake();
        var logica = new LogicaCompra(repo,
            new ProveedorRepositoryFake(CrearProveedor()),
            new ProductoRepositoryFake(CrearProducto(1)),
            rolActual: () => "Admin");
        logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(), IdEmpleadoValido);

        var anulada = logica.AnularCompra(1);

        Assert.IsTrue(anulada.Anulada);
        Assert.AreEqual(1, repo.IdCompraAnulada);
    }

    [TestMethod]
    public void AnularCompra_Empleado_LanzaYNoLlegaAlRepositorio()
    {
        var repo = new CompraRepositoryFake();
        var logica = new LogicaCompra(repo,
            new ProveedorRepositoryFake(CrearProveedor()),
            new ProductoRepositoryFake(CrearProducto(1)),
            rolActual: () => "Empleado");

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.AnularCompra(1));
        Assert.IsNull(repo.IdCompraAnulada);
    }
}

using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

// BLL: LogicaCompra - alta y listado de compras a proveedores (US-25)
[TestClass]
public class LogicaCompraTests
{
    private const int IdProveedorValido = 3;
    private const int IdEmpleadoValido = 5;

    // ---------- Fakes ----------

    private sealed class CompraRepositoryFake : ICompraRepository
    {
        public Compra? Registrada { get; private set; }
        public (int? IdProveedor, DateTime? Desde, DateTime? Hasta, string? Producto, bool IncluirAnuladas)? UltimaBusqueda { get; private set; }

        public Compra Registrar(Compra compra)
        {
            compra.IdCompra = 1;
            Registrada = compra;
            return compra;
        }

        public List<Compra> Buscar(int? idProveedor, DateTime? desde, DateTime? hasta, string? producto, bool incluirAnuladas = false)
        {
            UltimaBusqueda = (idProveedor, desde, hasta, producto, incluirAnuladas);
            return new List<Compra>();
        }

        public Compra? ObtenerPorId(int idCompra) => Registrada;
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
        => new() { IdProducto = id, Nombre = nombre, Activo = activo };

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

    // ---------- RegistrarCompra ----------

    [TestMethod]
    public void RegistrarCompra_Valida_GuardaConUsuarioActivoFechaHoraYTotal()
    {
        var ahora = DateTime.Now;
        var (logica, repo) = CrearLogica(ahora: ahora);
        var items = new List<ItemCompra>
        {
            new(CrearProducto(1), 10, 800m),
            new(CrearProducto(2, "Agua"), 5, 400.50m)
        };

        var compra = logica.RegistrarCompra(IdProveedorValido, ahora.Date, "0001-00001234", items, IdEmpleadoValido);

        Assert.AreSame(compra, repo.Registrada);
        Assert.AreEqual(IdProveedorValido, compra.IdProveedor);
        Assert.AreEqual(IdEmpleadoValido, compra.IdEmpleado);
        Assert.AreEqual("0001-00001234", compra.NumeroComprobante);
        Assert.AreEqual(ahora.Date + ahora.TimeOfDay, compra.FechaCompra);
        Assert.IsFalse(compra.Anulada);
        Assert.AreEqual(10002.50m, compra.Total);
        Assert.HasCount(2, compra.Detalles);
        Assert.AreEqual(8000m, compra.Detalles[0].Subtotal);
        Assert.AreEqual(2002.50m, compra.Detalles[1].Subtotal);
        // El detalle guarda solo el id del producto, no el objeto (para que EF no intente insertarlo)
        Assert.IsTrue(compra.Detalles.All(d => d.Producto == null));
    }

    [TestMethod]
    public void RegistrarCompra_FechaElegidaDeDiasAtras_ConservaEsaFechaConLaHoraActual()
    {
        var ahora = DateTime.Now;
        var (logica, _) = CrearLogica(ahora: ahora);
        var fecha = ahora.Date.AddDays(-3);

        var compra = logica.RegistrarCompra(IdProveedorValido, fecha, null, UnaLinea(), IdEmpleadoValido);

        Assert.AreEqual(fecha + ahora.TimeOfDay, compra.FechaCompra);
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
    public void RegistrarCompra_ProveedorInexistente_LanzaArgumentException()
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(999, DateTime.Today, null, UnaLinea(), IdEmpleadoValido));
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
    [DataRow(-1, "800")]
    [DataRow(10, "0")]
    [DataRow(10, "-5")]
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
    public void RegistrarCompra_ProductoInexistenteOInactivo_LanzaArgumentExceptionYNoGuarda()
    {
        var (logica, repo) = CrearLogica(productos: new[] { CrearProducto(1, activo: false) });

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(1), IdEmpleadoValido));
        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, UnaLinea(77), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_ComprobanteVacio_SeGuardaComoNulo()
    {
        var (logica, _) = CrearLogica();

        var compra = logica.RegistrarCompra(IdProveedorValido, DateTime.Today, "   ", UnaLinea(), IdEmpleadoValido);

        Assert.IsNull(compra.NumeroComprobante);
    }

    [TestMethod]
    public void RegistrarCompra_ComprobanteConEspaciosEnLosBordes_SeGuardaSinEllos()
    {
        var (logica, _) = CrearLogica();

        var compra = logica.RegistrarCompra(IdProveedorValido, DateTime.Today, "  A 0001-123 ", UnaLinea(), IdEmpleadoValido);

        Assert.AreEqual("A 0001-123", compra.NumeroComprobante);
    }

    [TestMethod]
    [DataRow("0001_123")]
    [DataRow("fact*123")]
    [DataRow("1234567890123456789012345678901")] // 31 caracteres
    public void RegistrarCompra_ComprobanteInvalido_LanzaArgumentExceptionYNoGuarda(string comprobante)
    {
        var (logica, repo) = CrearLogica();

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, comprobante, UnaLinea(), IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    [TestMethod]
    public void RegistrarCompra_TotalQueSuperaElLimiteDeLaColumna_LanzaArgumentException()
    {
        var (logica, repo) = CrearLogica();
        var items = UnaLinea(cantidad: DetalleCompra.CantidadMaxima, costo: 99_999_999m);

        Assert.ThrowsExactly<ArgumentException>(() =>
            logica.RegistrarCompra(IdProveedorValido, DateTime.Today, null, items, IdEmpleadoValido));
        Assert.IsNull(repo.Registrada);
    }

    // ---------- Proveedores ----------

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

    // ---------- Buscar ----------

    [TestMethod]
    public void Buscar_PasaLosFiltrosAlRepositorio()
    {
        var (logica, repo) = CrearLogica();
        var desde = new DateTime(2026, 10, 1);
        var hasta = new DateTime(2026, 10, 19);

        logica.Buscar(3, desde, hasta, "coca", incluirAnuladas: true);

        var busqueda = repo.UltimaBusqueda!.Value;
        Assert.AreEqual(3, busqueda.IdProveedor);
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
    public void Buscar_MismoDiaEnDesdeYHasta_EsValido()
    {
        var (logica, _) = CrearLogica();

        var resultado = logica.Buscar(null, new DateTime(2026, 10, 19, 15, 0, 0), new DateTime(2026, 10, 19, 8, 0, 0), null);

        Assert.IsNotNull(resultado);
    }
}
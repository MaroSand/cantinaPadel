using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Opción de proveedor para combos y autocompletado: el texto que se ve y el id que se usa para filtrar o guardar
public sealed record ProveedorOpcion(int IdProveedor, string Texto)
{
    public override string ToString() => Texto;
}

public sealed record ImpactoProductoCompra(
    int IdProducto,
    string Nombre,
    int Cantidad,
    int StockActual,
    int StockNuevo,
    decimal CostoActual,
    decimal CostoCompra,
    string Categoria,
    decimal PrecioVentaActual,
    decimal? PrecioVentaNuevo,
    bool RecalculaPrecio,
    string? MotivoPrecioSinCambio,
    string ProveedorActual,
    string ProveedorNuevo);

public sealed record PrevisualizacionImpactoCompra(
    IReadOnlyList<ImpactoProductoCompra> Productos,
    IReadOnlyDictionary<int, decimal> PreciosVentaNuevos);

public class LogicaCompra
{
    private readonly ICompraRepository _compras;
    private readonly IProveedorRepository _proveedores;
    private readonly IProductoRepository _productos;
    private readonly Func<DateTime> _ahora;
    private readonly Func<string?> _rolActual;

    public LogicaCompra() : this(new CompraRepository(), new ProveedorRepository(), new ProductoRepository()) { }

    // "ahora" se puede reemplazar en las pruebas para no depender del reloj real
    public LogicaCompra(ICompraRepository compras, IProveedorRepository proveedores, IProductoRepository productos,
        Func<DateTime>? ahora = null, Func<string?>? rolActual = null)
    {
        _compras = compras;
        _proveedores = proveedores;
        _productos = productos;
        _ahora = ahora ?? (() => DateTime.Now);
        _rolActual = rolActual ?? (() => global::cantinaPadel.Sesion.Rol);
    }

    // Proveedores activos: los que se pueden elegir al cargar una compra nueva
    public List<ProveedorOpcion> ObtenerProveedoresActivos()
        => _proveedores.ObtenerTodos()
            .Where(p => p.Persona != null && p.Persona.Activo)
            .Select(AOpcion)
            .OrderBy(o => o.Texto)
            .ToList();

    // Todos los proveedores, también los dados de baja: sirve para filtrar el listado, porque puede haber compras históricas de un proveedor que ya no está activo
    public List<ProveedorOpcion> ObtenerTodosLosProveedores()
        => _proveedores.ObtenerTodos()
            .Select(AOpcion)
            .OrderBy(o => o.Texto)
            .ToList();

    private static ProveedorOpcion AOpcion(Proveedor p)
    {
        string empresa = p.NombreEmpresa?.Trim() ?? string.Empty;
        string contacto = p.Persona == null ? string.Empty : $"{p.Persona.Apellido} {p.Persona.Nombre}".Trim();

        string texto = (empresa.Length, contacto.Length) switch
        {
            ( > 0, > 0) => $"{empresa} - {contacto}",
            ( > 0, _) => empresa,
            _ => contacto
        };

        return new ProveedorOpcion(p.IdProveedor, texto);
    }

    // Registra la compra con el usuario activo (idEmpleado) y la fecha/hora del momento: la fecha elegida en pantalla + la hora actual.
    // Valida todo antes de guardar y nada se guarda si algo falla (el repositorio usa una sola transacción)
    public Compra RegistrarCompra(int idProveedor, DateTime fecha, string? numeroComprobante, IReadOnlyCollection<ItemCompra> items, int idEmpleado)
    {
        if (idProveedor <= 0)
            throw new ArgumentException("Seleccione un proveedor.");

        if (items == null || items.Count == 0)
            throw new ArgumentException("La compra debe tener al menos un producto.");

        if (idEmpleado <= 0)
            throw new ArgumentException("No hay un empleado autenticado para registrar la compra.");

        var proveedor = _proveedores.ObtenerPorId(idProveedor)
            ?? throw new ArgumentException("El proveedor seleccionado no existe.");
        if (proveedor.Persona == null || !proveedor.Persona.Activo)
            throw new ArgumentException("El proveedor seleccionado está inactivo.");

        var ahora = _ahora();
        if (fecha.Date > ahora.Date)
            throw new ArgumentException("La fecha de la compra no puede ser posterior a hoy.");

        // Se recalcula la vista previa al confirmar para guardar los precios calculados por la BLL.
        var impacto = PrevisualizarImpactoCompra(idProveedor, items);
        var lineas = AgruparLineas(items);

        var detalles = new List<DetalleCompra>();
        foreach (var linea in lineas)
        {
            detalles.Add(new DetalleCompra
            {
                IdProducto = linea.IdProducto,
                Cantidad = linea.Cantidad,
                PrecioUnitario = linea.Costo,
                Subtotal = DetalleCompra.CalcularSubtotal(linea.Cantidad, linea.Costo)
            });
        }

        var compra = new Compra
        {
            IdProveedor = idProveedor,
            IdEmpleado = idEmpleado,
            FechaCompra = fecha.Date + ahora.TimeOfDay,
            NumeroComprobante = numeroComprobante,
            Total = decimal.Round(detalles.Sum(d => d.Subtotal), 2),
            Detalles = detalles
        };

        compra.Validar();
        return _compras.Registrar(compra, impacto.PreciosVentaNuevos);
    }

    // Calcula y devuelve el impacto que se mostrará antes de confirmar la compra.
    // El precio se recalcula si el stock previo era cero o el nuevo costo supera al costo registrado.
    // Las categorías de precio manual actualizan stock, costo y proveedor, pero conservan su precio de venta.
    public PrevisualizacionImpactoCompra PrevisualizarImpactoCompra(int idProveedor, IReadOnlyCollection<ItemCompra> items)
    {
        if (idProveedor <= 0)
            throw new ArgumentException("Seleccione un proveedor.");
        if (items == null || items.Count == 0)
            throw new ArgumentException("La compra debe tener al menos un producto.");

        var proveedor = _proveedores.ObtenerPorId(idProveedor)
            ?? throw new ArgumentException("El proveedor seleccionado no existe.");
        if (proveedor.Persona == null || !proveedor.Persona.Activo)
            throw new ArgumentException("El proveedor seleccionado está inactivo.");

        var preciosVenta = new Dictionary<int, decimal>();
        var productos = new List<ImpactoProductoCompra>();
        var proveedorNuevo = AOpcion(proveedor).Texto;

        foreach (var linea in AgruparLineas(items))
        {
            var producto = _productos.ObtenerPorId(linea.IdProducto);
            if (producto == null || !producto.Activo)
                throw new ArgumentException($"El producto '{linea.Nombre}' ya no está disponible.");

            DetalleCompra.ValidarCantidad(linea.Cantidad);
            DetalleCompra.ValidarCosto(linea.Costo);
            var stockNuevo = CalculadorCompra.CalcularStockNuevo(producto.StockActual, linea.Cantidad);

            if (producto.Categoria == null)
                throw new InvalidOperationException($"No se pudo cargar la categoría del producto '{producto.Nombre}'.");

            bool precioManual = CalculadorCompra.EsPrecioVentaManual(producto.Categoria.Nombre);
            bool recalculaPrecio = !precioManual &&
                                   (producto.StockActual == 0 || linea.Costo > producto.PrecioCosto);
            decimal? precioVentaNuevo = null;
            if (recalculaPrecio)
            {
                precioVentaNuevo = CalculadorCompra.CalcularPrecioVentaBase(
                    linea.Costo, producto.Categoria.PorcentajeGanancia);
                preciosVenta.Add(producto.IdProducto, precioVentaNuevo.Value);
            }

            var proveedorActual = producto.IdProveedor.HasValue
                ? _proveedores.ObtenerPorId(producto.IdProveedor.Value)
                : null;

            productos.Add(new ImpactoProductoCompra(
                producto.IdProducto,
                producto.Nombre,
                linea.Cantidad,
                producto.StockActual,
                stockNuevo,
                producto.PrecioCosto,
                linea.Costo,
                producto.Categoria.Nombre,
                producto.PrecioVenta,
                precioVentaNuevo,
                recalculaPrecio,
                recalculaPrecio
                    ? null
                    : precioManual
                        ? "precio manual"
                        : "el costo nuevo no supera el registrado y había stock",
                proveedorActual == null ? "Sin proveedor" : AOpcion(proveedorActual).Texto,
                proveedorNuevo));
        }

        return new PrevisualizacionImpactoCompra(productos, preciosVenta);
    }

    private static List<LineaCompra> AgruparLineas(IReadOnlyCollection<ItemCompra> items)
        => items
            .GroupBy(i => i.Producto.IdProducto)
            .Select(g => new LineaCompra(
                g.Key,
                g.First().Producto.Nombre,
                g.Sum(i => i.Cantidad),
                g.Last().PrecioCostoUnitario))
            .ToList();

    private sealed record LineaCompra(int IdProducto, string Nombre, int Cantidad, decimal Costo);

    // Listado de compras. Todos los filtros son opcionales y se combinan (AND). Por defecto no trae las anuladas
    public List<Compra> Buscar(int? idProveedor, DateTime? desde, DateTime? hasta, string? producto, bool incluirAnuladas = false)
    {
        if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
            throw new ArgumentException("La fecha \"desde\" no puede ser posterior a la fecha \"hasta\".");

        return _compras.Buscar(idProveedor, desde, hasta, producto, incluirAnuladas);
    }

    public Compra? ObtenerPorId(int idCompra) => _compras.ObtenerPorId(idCompra);

    public Compra AnularCompra(int idCompra)
    {
        if (idCompra <= 0)
            throw new ArgumentException("Seleccione una compra válida.", nameof(idCompra));

        if (!string.Equals(_rolActual(), "Admin", StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Solo un administrador puede anular compras.");

        return _compras.Anular(idCompra);
    }
}

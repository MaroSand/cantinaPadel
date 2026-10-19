using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Opción de proveedor para combos y autocompletado: el texto que se ve y el id que se usa para filtrar o guardar
public sealed record ProveedorOpcion(int IdProveedor, string Texto)
{
    public override string ToString() => Texto;
}

public class LogicaCompra
{
    private readonly ICompraRepository _compras;
    private readonly IProveedorRepository _proveedores;
    private readonly IProductoRepository _productos;
    private readonly Func<DateTime> _ahora;

    public LogicaCompra() : this(new CompraRepository(), new ProveedorRepository(), new ProductoRepository()) { }

    // "ahora" se puede reemplazar en las pruebas para no depender del reloj real
    public LogicaCompra(ICompraRepository compras, IProveedorRepository proveedores, IProductoRepository productos, Func<DateTime>? ahora = null)
    {
        _compras = compras;
        _proveedores = proveedores;
        _productos = productos;
        _ahora = ahora ?? (() => DateTime.Now);
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

        // Aunque la pantalla ya evita repetidos, se agrupa por las dudas: un producto = una línea, con las cantidades sumadas
        // (si vinieran costos distintos, queda el de la última línea, igual que en LogicaCarritoCompra)
        var lineas = items
            .GroupBy(i => i.Producto.IdProducto)
            .Select(g => new { IdProducto = g.Key, Cantidad = g.Sum(i => i.Cantidad), Costo = g.Last().PrecioCostoUnitario })
            .ToList();

        var detalles = new List<DetalleCompra>();
        foreach (var linea in lineas)
        {
            // Solo se valida que el producto exista y esté activo. En el detalle se guarda únicamente su id (no el objeto Producto)
            var producto = _productos.ObtenerPorId(linea.IdProducto);
            if (producto == null || !producto.Activo)
                throw new ArgumentException($"El producto '{items.First(i => i.Producto.IdProducto == linea.IdProducto).Producto.Nombre}' ya no está disponible.");

            // Se validan cantidad y costo antes de calcular el subtotal para que el mensaje sea el de la regla incumplida
            DetalleCompra.ValidarCantidad(linea.Cantidad);
            DetalleCompra.ValidarCosto(linea.Costo);

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
        return _compras.Registrar(compra);
    }

    // Listado de compras. Todos los filtros son opcionales y se combinan (AND). Por defecto no trae las anuladas
    public List<Compra> Buscar(int? idProveedor, DateTime? desde, DateTime? hasta, string? producto, bool incluirAnuladas = false)
    {
        if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
            throw new ArgumentException("La fecha \"desde\" no puede ser posterior a la fecha \"hasta\".");

        return _compras.Buscar(idProveedor, desde, hasta, producto, incluirAnuladas);
    }

    public Compra? ObtenerPorId(int idCompra) => _compras.ObtenerPorId(idCompra);
}
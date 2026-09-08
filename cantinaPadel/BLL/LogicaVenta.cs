using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public sealed class PagoVenta
{
    public decimal Efectivo { get; init; }
    public decimal Transferencia { get; init; }
    public decimal Total => Efectivo + Transferencia;

    public string FormaPago => (Efectivo > 0, Transferencia > 0) switch
    {
        (true, false) => "Efectivo",
        (false, true) => "Transferencia",
        (true, true) => "Mixto",
        _ => string.Empty
    };
}

public class LogicaVenta
{
    private const decimal TasaIva = 0.21m;
    private readonly IVentaRepository _ventas;
    private readonly IClienteRepository _clientes;
    private readonly ICajaRepository _cajas;

    public LogicaVenta() : this(new VentaRepository(), new ClienteRepository(), new CajaRepository()) { }

    public LogicaVenta(IVentaRepository ventas, IClienteRepository clientes, ICajaRepository cajas)
    {
        _ventas = ventas;
        _clientes = clientes;
        _cajas = cajas;
    }

    public List<Cliente> BuscarClientes(string texto) => _clientes.Buscar(texto ?? string.Empty);

    public Cliente ObtenerConsumidorFinal()
    {
        var cliente = _clientes.ObtenerTodos().FirstOrDefault(c => c.Persona.Activo &&
            string.Equals($"{c.Persona.Nombre} {c.Persona.Apellido}".Trim(), "Consumidor Final", StringComparison.OrdinalIgnoreCase));
        return cliente ?? throw new InvalidOperationException("No existe un cliente activo llamado 'Consumidor Final'. Debe crearlo antes de registrar ventas sin cliente.");
    }

    public Venta ConfirmarVenta(IReadOnlyCollection<ItemCarrito> items, Cliente? clienteSeleccionado, PagoVenta pago, int idEmpleado)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("El carrito está vacío.");
        if (idEmpleado <= 0)
            throw new ArgumentException("No hay un empleado autenticado para registrar la venta.");
        if (pago == null || pago.Efectivo < 0 || pago.Transferencia < 0 || string.IsNullOrEmpty(pago.FormaPago))
            throw new ArgumentException("Ingrese un importe válido en efectivo y/o transferencia.");

        decimal total = Math.Round(items.Sum(i => i.Subtotal), 2);
        if (pago.Total != total)
            throw new ArgumentException($"El pago debe coincidir con el total a cobrar ({total:C2}).");

        var cliente = clienteSeleccionado ?? ObtenerConsumidorFinal();
        if (!cliente.Persona.Activo)
            throw new ArgumentException("El cliente seleccionado está inactivo.");
        var caja = _cajas.ObtenerCajaAbierta(idEmpleado)
            ?? throw new InvalidOperationException("No hay una caja abierta para el empleado actual.");

        // El precio de carrito ya incluye IVA; se descompone para mantener las columnas de ventas consistentes.
        decimal subtotal = Math.Round(total / (1 + TasaIva), 2);
        var venta = new Venta
        {
            IdCaja = caja.IdCaja,
            IdCliente = cliente.IdCliente,
            IdEmpleado = idEmpleado,
            FechaVenta = DateTime.Now,
            Subtotal = subtotal,
            Iva = total - subtotal,
            Total = total,
            FormaPago = pago.FormaPago
        };
        var detalles = items.Select(i => new DetalleVenta
        {
            IdProducto = i.Producto.IdProducto,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario,
            Subtotal = i.Subtotal,
            Pagado = true
        }).ToList();

        return _ventas.Registrar(venta, detalles);
    }

    public void ActualizarTipoComprobante(int idVenta, TipoComprobante tipo)
        => _ventas.ActualizarTipoComprobante(idVenta, tipo == TipoComprobante.FacturaA ? "A" : "B");
}

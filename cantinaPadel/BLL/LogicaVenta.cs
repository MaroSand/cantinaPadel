using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public enum MetodoPago
{
    Efectivo,
    Transferencia,
    Tarjeta,
    BilleteraVirtual,
    CuentaCorriente
}

public sealed class PagoVenta
{
    public MetodoPago Metodo { get; init; }

    public string FormaPago => Metodo switch
    {
        MetodoPago.Efectivo => "Efectivo",
        MetodoPago.Transferencia => "Transferencia",
        MetodoPago.Tarjeta => "Tarjeta",
        MetodoPago.BilleteraVirtual => "Billetera Virtual",
        MetodoPago.CuentaCorriente => "Cuenta Corriente",
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
        if (pago == null)
            throw new ArgumentException("Seleccione un método de pago.");
        if (pago.Metodo == MetodoPago.CuentaCorriente && clienteSeleccionado == null)
            throw new ArgumentException("Cuenta Corriente requiere seleccionar un cliente.");

        decimal total = Math.Round(items.Sum(i => i.Subtotal), 2);

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
        // cada unidad vendida es su propia fila de detalles_venta (ya
        // no existe "cantidad"). Si se paga con Cuenta Corriente, las filas
        // quedan sin pagar (Pagado = false) hasta que el cliente las cancele
        // desde la pantalla de Cuenta Corriente; con cualquier otro método
        // quedan pagadas en el momento.
        bool pagadoAlMomento = pago.Metodo != MetodoPago.CuentaCorriente;
        var detalles = items
            .SelectMany(i => Enumerable.Range(0, i.Cantidad).Select(_ => new DetalleVenta
            {
                IdProducto = i.Producto.IdProducto,
                PrecioUnitario = i.PrecioUnitario,
                Subtotal = i.PrecioUnitario,
                Pagado = pagadoAlMomento
            }))
            .ToList();

        return _ventas.Registrar(venta, detalles, cliente.IdCliente);
    }

    public void ActualizarTipoComprobante(int idVenta, TipoComprobante tipo)
        => _ventas.ActualizarTipoComprobante(idVenta, tipo == TipoComprobante.FacturaA ? "A" : "B");
}
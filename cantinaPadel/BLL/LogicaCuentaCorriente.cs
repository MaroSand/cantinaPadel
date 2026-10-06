using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public class LogicaCuentaCorriente
{
    private readonly ICuentaCorrienteRepository _cuentaCorriente;
    private readonly IClienteRepository _clientes;
    private readonly ICajaRepository _cajas;

    public LogicaCuentaCorriente()
        : this(new CuentaCorrienteRepository(), new ClienteRepository(), new CajaRepository()) { }

    public LogicaCuentaCorriente(ICuentaCorrienteRepository cuentaCorriente, IClienteRepository clientes, ICajaRepository cajas)
    {
        _cuentaCorriente = cuentaCorriente;
        _clientes = clientes;
        _cajas = cajas;
    }

    // Busca por dni, apellido o nombre
    public List<Cliente> BuscarClientes(string texto) => _clientes.Buscar(texto ?? string.Empty);

    public List<ItemDeudaCliente> ObtenerPendientes(int idCliente) => _cuentaCorriente.ObtenerPendientes(idCliente);

    public ResumenCuentaCorriente ObtenerResumen(int idCliente) => _cuentaCorriente.ObtenerResumen(idCliente);

    // Formas de pago con las que se puede cobrar una deuda de cuenta corriente: son las mismas del Punto de Venta (MetodoPago), menos
    // Cuenta Corriente (una deuda no se puede pagar con más deuda)
    public static readonly MetodoPago[] MetodosPagoDeCobro =
    {
        MetodoPago.Efectivo,
        MetodoPago.Transferencia,
        MetodoPago.Tarjeta,
        MetodoPago.BilleteraVirtual
    };

    // Cobra la deuda usando el mismo PagoVenta que el Punto de Venta: el nombre de la forma de pago (p. ej. BilleteraVirtual -> "MercadoPago")
    // sale de PagoVenta.FormaPago, así caja y reportes ven exactamente los mismos valores que en una venta común
    public ResultadoPagoCuentaCorriente RegistrarPago(Cliente cliente, decimal monto, int idEmpleado, PagoVenta pago)
    {
        if (pago == null)
            throw new ArgumentException("Seleccione una forma de pago.");
        if (!MetodosPagoDeCobro.Contains(pago.Metodo))
            throw new ArgumentException("La deuda de cuenta corriente no se puede pagar con Cuenta Corriente. Seleccione otra forma de pago.");

        return RegistrarPago(cliente, monto, idEmpleado, pago.FormaPago);
    }

    public ResultadoPagoCuentaCorriente RegistrarPago(
        Cliente cliente,
        decimal monto,
        int idEmpleado,
        string tipoPago = MovimientoCuentaCorriente.TipoPagoEfectivo)
    {
        if (cliente == null)
            throw new ArgumentException("Debe seleccionar un cliente.");
        if (monto <= 0)
            throw new ArgumentException("El monto a cobrar debe ser mayor a cero.");
        if (!EsTipoPagoValido(tipoPago))
            throw new ArgumentException("Seleccione un tipo de pago válido.");

        var caja = _cajas.ObtenerCajaAbierta(idEmpleado)
            ?? throw new InvalidOperationException("No hay una caja abierta para el empleado actual.");

        return _cuentaCorriente.RegistrarPago(cliente.IdCliente, monto, caja.IdTurnoCaja, idEmpleado, tipoPago);
    }

    public static bool EsTipoPagoValido(string? tipoPago)
        => tipoPago is MovimientoCuentaCorriente.TipoPagoEfectivo
            or MovimientoCuentaCorriente.TipoPagoTransferencia
            or MovimientoCuentaCorriente.TipoPagoTarjeta
            or MovimientoCuentaCorriente.TipoPagoMercadoPago;
}

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

    // Busca por DNI, apellido o nombre (reutiliza la misma búsqueda que ya
    // usa el resto de la aplicación para clientes).
    public List<Cliente> BuscarClientes(string texto) => _clientes.Buscar(texto ?? string.Empty);

    public List<ItemDeudaCliente> ObtenerPendientes(int idCliente) => _cuentaCorriente.ObtenerPendientes(idCliente);

    public ResumenCuentaCorriente ObtenerResumen(int idCliente) => _cuentaCorriente.ObtenerResumen(idCliente);

    public ResultadoPagoCuentaCorriente RegistrarPago(Cliente cliente, decimal monto, int idEmpleado)
    {
        if (cliente == null)
            throw new ArgumentException("Debe seleccionar un cliente.");
        if (monto <= 0)
            throw new ArgumentException("El monto a cobrar debe ser mayor a cero.");

        var caja = _cajas.ObtenerCajaAbierta(idEmpleado)
            ?? throw new InvalidOperationException("No hay una caja abierta para el empleado actual.");

        return _cuentaCorriente.RegistrarPago(cliente.IdCliente, monto, caja.IdCaja, idEmpleado);
    }
}

using cantinaPadel.BLL;

namespace cantinaPadel.DAL.Repositories;

public interface ICuentaCorrienteRepository
{
    // Productos pendientes de pago de un cliente, del más viejo al más nuevo.
    List<ItemDeudaCliente> ObtenerPendientes(int idCliente);

    // Aplica "monto" (más el saldo a favor que ya tuviera el cliente) a los
    // productos pendientes más viejos, marcando como pagados los que se
    // alcanzan a cubrir completos. Devuelve el detalle de lo aplicado.
    ResultadoPagoCuentaCorriente RegistrarPago(int idCliente, decimal monto, int idCaja, int idEmpleado);
}

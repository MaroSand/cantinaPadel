using cantinaPadel.BLL;

namespace cantinaPadel.DAL.Repositories;

public interface ICuentaCorrienteRepository
{
    // Productos pendientes de pago de un cliente, del más viejo al más nuevo
    List<ItemDeudaCliente> ObtenerPendientes(int idCliente);

    // Deuda pendiente y crédito actual del cliente, leídos de la base
    ResumenCuentaCorriente ObtenerResumen(int idCliente);

    // Aplica "monto" (más el crédito que ya tuviera el cliente) a los productos pendientes más viejos, marcando como pagados solo los que se
    // alcanzan a cubrir completos. Rechaza montos mayores a la deuda. Devuelve el detalle de lo aplicado
    ResultadoPagoCuentaCorriente RegistrarPago(int idCliente, decimal monto, int idCaja, int idEmpleado);
}

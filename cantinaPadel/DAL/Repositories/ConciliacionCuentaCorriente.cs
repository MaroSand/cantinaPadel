using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

// Unidad impaga (fila de detalles_venta) junto con los datos para mostrarla.
internal sealed record PendienteCliente(DetalleVenta Detalle, ItemDeudaCliente Item);

// Punto único donde se aplica el crédito de un cliente a sus unidades
// impagas. Lo usan el registro de pagos, las ventas a cuenta corriente y los
// cambios de precio, así el estado de "pagado" y el saldo del cliente nunca
// quedan desincronizados.
//
// Ninguno de estos métodos guarda cambios: el llamador es dueño del
// SaveChanges y de la transacción.
internal static class ConciliacionCuentaCorriente
{
    // Unidades impagas del cliente, de la más vieja a la más nueva (FIFO).
    public static List<PendienteCliente> ConsultarPendientes(AppDbContext ctx, int idCliente)
    {
        var filas = (from d in ctx.DetallesVenta
                     join v in ctx.Ventas on d.IdVenta equals v.IdVenta
                     join p in ctx.Productos on d.IdProducto equals (int?)p.IdProducto
                     where v.IdCliente == idCliente && !d.Pagado
                     orderby v.FechaVenta, d.IdDetalle
                     select new { Detalle = d, v.IdVenta, v.FechaVenta, NombreProducto = p.Nombre })
            .ToList();

        return filas
            .Select(f => new PendienteCliente(f.Detalle, new ItemDeudaCliente
            {
                IdDetalle = f.Detalle.IdDetalle,
                IdVenta = f.IdVenta,
                FechaVenta = f.FechaVenta,
                NombreProducto = f.NombreProducto,
                Monto = f.Detalle.Subtotal
            }))
            .ToList();
    }

    // Marca como pagadas las unidades que el crédito del cliente cubre por
    // completo y deja en su saldo lo que sobra.
    public static (List<ItemDeudaCliente> Saldados, List<ItemDeudaCliente> Pendientes) Aplicar(
        Cliente cliente, IReadOnlyList<PendienteCliente> pendientes)
    {
        var aplicacion = CalculadorCuentaCorriente.AplicarCredito(
            pendientes.Select(p => p.Item.Monto).ToList(),
            Math.Max(cliente.SaldoCuentaCorriente, 0m));

        foreach (var pendiente in pendientes.Take(aplicacion.CantidadSaldada))
            pendiente.Detalle.Pagado = true;

        cliente.SaldoCuentaCorriente = aplicacion.CreditoRemanente;

        return (
            pendientes.Take(aplicacion.CantidadSaldada).Select(p => p.Item).ToList(),
            pendientes.Skip(aplicacion.CantidadSaldada).Select(p => p.Item).ToList());
    }

    // Reaplica el crédito de los clientes indicados. Se usa cuando cambió la
    // deuda por fuera de un pago (nueva venta, cambio de precio) y el crédito
    // que ya tenían puede alcanzar ahora para saldar una unidad.
    public static void ConciliarClientes(AppDbContext ctx, IEnumerable<int> idsClientes)
    {
        var ids = idsClientes.Distinct().ToList();
        if (ids.Count == 0) return;

        var conCredito = ctx.Clientes
            .Where(c => ids.Contains(c.IdCliente) && c.SaldoCuentaCorriente > 0)
            .ToList();

        foreach (var cliente in conCredito)
            Aplicar(cliente, ConsultarPendientes(ctx, cliente.IdCliente));
    }
}

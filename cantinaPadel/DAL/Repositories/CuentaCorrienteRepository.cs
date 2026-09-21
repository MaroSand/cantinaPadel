using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

public class CuentaCorrienteRepository : ICuentaCorrienteRepository
{
    public List<ItemDeudaCliente> ObtenerPendientes(int idCliente)
    {
        using var ctx = new AppDbContext();
        return ConsultarPendientes(ctx, idCliente);
    }

    public ResultadoPagoCuentaCorriente RegistrarPago(int idCliente, decimal monto, int idCaja, int idEmpleado)
    {
        if (monto <= 0)
            throw new ArgumentException("El monto a cobrar debe ser mayor a cero.");

        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        var cliente = ctx.Clientes.Find(idCliente)
            ?? throw new InvalidOperationException("No se encontró el cliente.");

        // Detalles pendientes, de más viejo a más nuevo (FIFO): lo lógico es
        // saldar primero lo que se debe hace más tiempo.
        var pendientes = ctx.DetallesVenta
            .Join(ctx.Ventas, d => d.IdVenta, v => v.IdVenta, (d, v) => new { Detalle = d, Venta = v })
            .Where(x => x.Venta.IdCliente == idCliente && x.Detalle.IdProducto != null && !x.Detalle.Pagado)
            .OrderBy(x => x.Venta.FechaVenta).ThenBy(x => x.Detalle.IdDetalle)
            .ToList();

        // El saldo a favor que ya tuviera el cliente se suma al pago
        // recibido, así un pago chico puede terminar de cubrir un producto
        // gracias a un crédito previo.
        decimal disponible = monto + Math.Max(cliente.SaldoCuentaCorriente, 0m);

        var resultado = new ResultadoPagoCuentaCorriente { MontoRecibido = monto };

        foreach (var x in pendientes)
        {
            var item = new ItemDeudaCliente
            {
                IdDetalle = x.Detalle.IdDetalle,
                IdVenta = x.Venta.IdVenta,
                FechaVenta = x.Venta.FechaVenta,
                Monto = x.Detalle.Subtotal
            };

            if (disponible >= x.Detalle.Subtotal)
            {
                x.Detalle.Pagado = true;
                disponible -= x.Detalle.Subtotal;
                resultado.ItemsPagados.Add(item);
            }
            else
            {
                resultado.ItemsPendientes.Add(item);
            }
        }

        // Lo que sobra (no alcanzó para cubrir el próximo producto completo,
        // o ya no quedan productos pendientes) queda como saldo a favor.
        cliente.SaldoCuentaCorriente = disponible;
        resultado.SaldoFavorResultante = disponible;

        ctx.MovimientosCuentaCorriente.Add(new MovimientoCuentaCorriente
        {
            IdCliente = idCliente,
            IdCaja = idCaja,
            IdEmpleado = idEmpleado,
            Fecha = DateTime.Now,
            Tipo = MovimientoCuentaCorriente.TipoPago,
            Monto = monto,
            SaldoPosterior = cliente.SaldoCuentaCorriente
        });

        ctx.SaveChanges();

        // Nombres de producto para el resultado (para no dejar el join de
        // productos activo mientras se resuelve la lógica de arriba).
        CompletarNombres(ctx, resultado);

        transaccion.Commit();
        return resultado;
    }

    private static List<ItemDeudaCliente> ConsultarPendientes(AppDbContext ctx, int idCliente)
    {
        var pendientes = (from d in ctx.DetallesVenta
                           join v in ctx.Ventas on d.IdVenta equals v.IdVenta
                           join p in ctx.Productos on d.IdProducto equals (int?)p.IdProducto
                           where v.IdCliente == idCliente && !d.Pagado && d.IdProducto != null
                           orderby v.FechaVenta, d.IdDetalle
                           select new ItemDeudaCliente
                           {
                               IdDetalle = d.IdDetalle,
                               IdVenta = v.IdVenta,
                               FechaVenta = v.FechaVenta,
                               NombreProducto = p.Nombre,
                               Monto = d.Subtotal
                           }).ToList();
        return pendientes;
    }

    private static void CompletarNombres(AppDbContext ctx, ResultadoPagoCuentaCorriente resultado)
    {
        var idsDetalle = resultado.ItemsPagados.Select(i => i.IdDetalle)
            .Concat(resultado.ItemsPendientes.Select(i => i.IdDetalle))
            .ToList();
        if (idsDetalle.Count == 0) return;

        var nombresPorDetalle = (from d in ctx.DetallesVenta
                                  join p in ctx.Productos on d.IdProducto equals (int?)p.IdProducto
                                  where idsDetalle.Contains(d.IdDetalle)
                                  select new { d.IdDetalle, p.Nombre })
            .ToDictionary(x => x.IdDetalle, x => x.Nombre);

        foreach (var item in resultado.ItemsPagados.Concat(resultado.ItemsPendientes))
            if (nombresPorDetalle.TryGetValue(item.IdDetalle, out var nombre))
                item.NombreProducto = nombre;
    }
}

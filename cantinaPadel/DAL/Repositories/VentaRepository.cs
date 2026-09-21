using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class VentaRepository : IVentaRepository
{
    public Venta Registrar(Venta venta, IReadOnlyCollection<DetalleVenta> detalles, int idCliente)
    {
        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        // US-16: ya no hay "cantidad" por fila; se agrupa por producto para
        // saber cuántas unidades se están vendiendo de cada uno.
        var cantidadPorProducto = detalles
            .GroupBy(d => d.IdProducto!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var idsProductos = cantidadPorProducto.Keys.ToList();
        var productos = ctx.Productos.Where(p => idsProductos.Contains(p.IdProducto)).ToDictionary(p => p.IdProducto);

        foreach (var (idProducto, cantidad) in cantidadPorProducto)
        {
            if (!productos.TryGetValue(idProducto, out var producto) || !producto.Activo)
                throw new InvalidOperationException("Uno de los productos ya no se encuentra disponible.");
            if (producto.StockActual < cantidad)
                throw new InvalidOperationException($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.StockActual}.");
        }

        foreach (var (idProducto, cantidad) in cantidadPorProducto)
            productos[idProducto].StockActual -= cantidad;

        ctx.Ventas.Add(venta);
        ctx.SaveChanges();

        foreach (var detalle in detalles)
            detalle.IdVenta = venta.IdVenta;
        ctx.DetallesVenta.AddRange(detalles);
        ctx.SaveChanges();

        // Si la venta quedó a Cuenta Corriente, se deja un registro de
        // auditoría (Cargo). La deuda en sí vive en las filas de
        // detalles_venta con Pagado = false. Si el cliente ya tenía crédito
        // suficiente para cubrir alguna unidad, se aplica en el momento.
        if (venta.FormaPago == "Cuenta Corriente")
        {
            ConciliacionCuentaCorriente.ConciliarClientes(ctx, new[] { idCliente });
            var cliente = ctx.Clientes.Find(idCliente);
            ctx.MovimientosCuentaCorriente.Add(new MovimientoCuentaCorriente
            {
                IdCliente = idCliente,
                IdCaja = venta.IdCaja,
                IdVenta = venta.IdVenta,
                IdEmpleado = venta.IdEmpleado,
                Fecha = venta.FechaVenta,
                Tipo = MovimientoCuentaCorriente.TipoCargo,
                Monto = venta.Total,
                SaldoPosterior = cliente?.SaldoCuentaCorriente ?? 0m
            });
            ctx.SaveChanges();
        }

        transaccion.Commit();
        return venta;
    }

    public void ActualizarTipoComprobante(int idVenta, string tipoComprobante)
    {
        using var ctx = new AppDbContext();
        var venta = ctx.Ventas.Find(idVenta) ?? throw new InvalidOperationException("No se encontró la venta registrada.");
        venta.TipoComprobante = tipoComprobante;
        ctx.SaveChanges();
    }
}

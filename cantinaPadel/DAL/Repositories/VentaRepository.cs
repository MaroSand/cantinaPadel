using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class VentaRepository : IVentaRepository
{
    public Venta Registrar(Venta venta, IReadOnlyCollection<DetalleVenta> detalles)
    {
        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        var idsProductos = detalles.Select(d => d.IdProducto!.Value).Distinct().ToList();
        var productos = ctx.Productos.Where(p => idsProductos.Contains(p.IdProducto)).ToDictionary(p => p.IdProducto);

        foreach (var detalle in detalles)
        {
            if (!productos.TryGetValue(detalle.IdProducto!.Value, out var producto) || !producto.Activo)
                throw new InvalidOperationException("Uno de los productos ya no se encuentra disponible.");
            if (producto.StockActual < detalle.Cantidad)
                throw new InvalidOperationException($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.StockActual}.");
        }

        foreach (var detalle in detalles)
            productos[detalle.IdProducto!.Value].StockActual -= detalle.Cantidad;

        ctx.Ventas.Add(venta);
        ctx.SaveChanges();

        foreach (var detalle in detalles)
            detalle.IdVenta = venta.IdVenta;
        ctx.DetallesVenta.AddRange(detalles);
        ctx.SaveChanges();
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

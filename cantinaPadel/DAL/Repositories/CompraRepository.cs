using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;
using cantinaPadel.BLL;

namespace cantinaPadel.DAL.Repositories;

public class CompraRepository : ICompraRepository
{
    public Compra Registrar(Compra compra, IReadOnlyDictionary<int, decimal> preciosVentaNuevos)
    {
        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        var cantidadesPorProducto = compra.Detalles.ToDictionary(d => d.IdProducto, d => d.Cantidad);
        var idsProducto = cantidadesPorProducto.Keys.ToList();
        var productos = ctx.Productos
            .Include(p => p.Categoria)
            .Where(p => idsProducto.Contains(p.IdProducto))
            .ToList();

        if (productos.Count != idsProducto.Count)
            throw new InvalidOperationException("Uno o más productos de la compra ya no existen.");

        var productosPorId = productos.ToDictionary(p => p.IdProducto);
        foreach (var detalle in compra.Detalles)
        {
            var producto = productosPorId[detalle.IdProducto];
            if (!producto.Activo)
                throw new InvalidOperationException($"El producto '{producto.Nombre}' ya no está disponible.");

            // Se valida dentro de la transacción por si el stock cambió después de mostrar la vista previa.
            _ = CalcularStockNuevo(producto.StockActual, detalle.Cantidad);
            detalle.CostoAnterior = producto.PrecioCosto;
            detalle.PrecioVentaAnterior = producto.PrecioVenta;
            detalle.IdProveedorAnterior = producto.IdProveedor;
        }

        // Compra, detalles y actualización de productos se confirman o revierten como una sola operación.
        ctx.Compras.Add(compra);
        ctx.SaveChanges();

        var productosConCambioDePrecio = new List<Producto>();
        foreach (var detalle in compra.Detalles)
        {
            var producto = productosPorId[detalle.IdProducto];
            producto.StockActual = CalcularStockNuevo(producto.StockActual, detalle.Cantidad);
            producto.PrecioCosto = detalle.PrecioUnitario;
            producto.IdProveedor = compra.IdProveedor;

            if (preciosVentaNuevos.TryGetValue(producto.IdProducto, out var precioNuevo))
            {
                if (precioNuevo < 0m || precioNuevo > Producto.PrecioVentaMaximo ||
                    decimal.Round(precioNuevo, 2) != precioNuevo)
                    throw new InvalidOperationException($"El precio calculado de '{producto.Nombre}' no es válido.");

                if (producto.PrecioVenta != precioNuevo)
                    productosConCambioDePrecio.Add(producto);
                producto.PrecioVenta = precioNuevo;
            }
        }

        ctx.SaveChanges();
        if (productosConCambioDePrecio.Count > 0)
            ProductoRepository.ReajustarDeudaPendientePorCambioDePrecio(ctx, productosConCambioDePrecio);

        transaccion.Commit();
        return compra;
    }

    public Compra Anular(int idCompra)
    {
        if (idCompra <= 0)
            throw new ArgumentException("Seleccione una compra válida.", nameof(idCompra));

        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        var compra = ctx.Compras
            .Include(c => c.Detalles)
            .FirstOrDefault(c => c.IdCompra == idCompra)
            ?? throw new InvalidOperationException("La compra seleccionada ya no existe.");

        if (compra.Anulada)
            throw new InvalidOperationException("La compra ya está anulada.");

        var cantidadesPorProducto = compra.Detalles
            .GroupBy(d => d.IdProducto)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));
        var idsProducto = cantidadesPorProducto.Keys.ToList();
        var productos = ctx.Productos
            .Where(p => idsProducto.Contains(p.IdProducto))
            .ToList();

        if (productos.Count != idsProducto.Count)
            throw new InvalidOperationException("No se puede anular la compra porque uno de sus productos ya no existe.");

        var productosPorId = productos.ToDictionary(p => p.IdProducto);
        var stockRevertido = new Dictionary<int, int>();
        var estadoProductoRestaurado = new Dictionary<int, (decimal Costo, int? IdProveedor)>();
        var preciosVentaRestaurados = new Dictionary<int, decimal>();

        foreach (var (idProducto, cantidad) in cantidadesPorProducto)
        {
            var producto = productosPorId[idProducto];
            stockRevertido[idProducto] = CalculadorCompra.CalcularStockLuegoDeAnular(producto.StockActual, cantidad);

            // Si quedan compras activas, prevalece la más reciente. Si no, se recupera
            // el estado anterior a la primera compra registrada para ese producto.
            var ultimaCompraActiva = (
                from detalle in ctx.DetallesCompra
                join compraActiva in ctx.Compras on detalle.IdCompra equals compraActiva.IdCompra
                where detalle.IdProducto == idProducto &&
                      compraActiva.IdCompra != idCompra &&
                      !compraActiva.Anulada
                orderby compraActiva.FechaCompra descending, compraActiva.IdCompra descending
                select new { detalle.PrecioUnitario, compraActiva.IdProveedor, compraActiva.IdCompra, compraActiva.FechaCompra })
                .FirstOrDefault();

            // Si la compra anulada es la más reciente, se restaura exactamente el precio
            // que tenía el producto justo antes de registrarla. Si hay una compra activa
            // posterior, esa compra sigue determinando el precio vigente.
            bool hayCompraPosteriorActiva = ultimaCompraActiva != null &&
                (ultimaCompraActiva.FechaCompra > compra.FechaCompra ||
                 (ultimaCompraActiva.FechaCompra == compra.FechaCompra && ultimaCompraActiva.IdCompra > compra.IdCompra));

            if (!hayCompraPosteriorActiva)
            {
                var detalleAnulado = compra.Detalles.First(d => d.IdProducto == idProducto);
                if (detalleAnulado.PrecioVentaAnterior == null)
                {
                    throw new InvalidOperationException(
                        $"No se puede anular la compra porque no se guardó el precio de venta anterior del producto '{productosPorId[idProducto].Nombre}'. Esta compra es anterior a la actualización; restaure el precio manualmente y vuelva a intentar.");
                }

                preciosVentaRestaurados[idProducto] = detalleAnulado.PrecioVentaAnterior.Value;
            }

            if (ultimaCompraActiva != null)
            {
                estadoProductoRestaurado[idProducto] =
                    (ultimaCompraActiva.PrecioUnitario, ultimaCompraActiva.IdProveedor);
                continue;
            }

            var estadoAnteriorPrimeraCompra = (
                from detalle in ctx.DetallesCompra
                join compraHistorica in ctx.Compras on detalle.IdCompra equals compraHistorica.IdCompra
                where detalle.IdProducto == idProducto
                orderby compraHistorica.FechaCompra, compraHistorica.IdCompra
                select new { detalle.CostoAnterior, detalle.IdProveedorAnterior })
                .FirstOrDefault();

            if (estadoAnteriorPrimeraCompra?.CostoAnterior == null)
            {
                throw new InvalidOperationException(
                    $"No se puede anular la compra porque el historial anterior del producto '{producto.Nombre}' no tiene guardado su costo previo. Actualice ese dato antes de volver a intentar.");
            }

            estadoProductoRestaurado[idProducto] =
                (estadoAnteriorPrimeraCompra.CostoAnterior.Value, estadoAnteriorPrimeraCompra.IdProveedorAnterior);
        }

        var productosConCambioDePrecio = new List<Producto>();
        foreach (var (idProducto, stockNuevo) in stockRevertido)
        {
            var producto = productosPorId[idProducto];
            producto.StockActual = stockNuevo;
            producto.PrecioCosto = estadoProductoRestaurado[idProducto].Costo;
            producto.IdProveedor = estadoProductoRestaurado[idProducto].IdProveedor;

            if (preciosVentaRestaurados.TryGetValue(idProducto, out var precioVentaRestaurado) &&
                producto.PrecioVenta != precioVentaRestaurado)
            {
                producto.PrecioVenta = precioVentaRestaurado;
                productosConCambioDePrecio.Add(producto);
            }
        }

        compra.Anulada = true;
        ctx.SaveChanges();
        if (productosConCambioDePrecio.Count > 0)
            ProductoRepository.ReajustarDeudaPendientePorCambioDePrecio(ctx, productosConCambioDePrecio);

        transaccion.Commit();
        return compra;
    }

    private static int CalcularStockNuevo(int stockActual, int cantidad)
    {
        try
        {
            return checked(stockActual + cantidad);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException("El stock resultante supera el máximo permitido.");
        }
    }

    public List<Compra> Buscar(int? idProveedor, DateTime? desde, DateTime? hasta, string? producto, bool incluirAnuladas = false)
    {
        using var ctx = new AppDbContext();

        IQueryable<Compra> query = ctx.Compras
            .Include(c => c.Proveedor).ThenInclude(p => p!.Persona)
            .Include(c => c.Empleado).ThenInclude(e => e!.Persona);

        if (!incluirAnuladas)
            query = query.Where(c => !c.Anulada);

        if (idProveedor.HasValue)
            query = query.Where(c => c.IdProveedor == idProveedor.Value);

        // El rango es inclusivo en ambos extremos: "hasta" abarca todo ese día, hasta las 23:59:59
        if (desde.HasValue)
        {
            DateTime inicio = desde.Value.Date;
            query = query.Where(c => c.FechaCompra >= inicio);
        }

        if (hasta.HasValue)
        {
            DateTime finExclusivo = hasta.Value.Date.AddDays(1);
            query = query.Where(c => c.FechaCompra < finExclusivo);
        }

        if (!string.IsNullOrWhiteSpace(producto))
        {
            string texto = producto.Trim().ToLower();
            query = query.Where(c => c.Detalles.Any(d =>
                d.Producto != null &&
                (d.Producto.Nombre.ToLower().Contains(texto) ||
                 (d.Producto.CodigoBarras != null && d.Producto.CodigoBarras.Contains(texto)))));
        }

        return query
            .OrderByDescending(c => c.FechaCompra)
            .ThenByDescending(c => c.IdCompra)
            .ToList();
    }

    public Compra? ObtenerPorId(int idCompra)
    {
        using var ctx = new AppDbContext();
        return ctx.Compras
            .Include(c => c.Proveedor).ThenInclude(p => p!.Persona)
            .Include(c => c.Empleado).ThenInclude(e => e!.Persona)
            .Include(c => c.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefault(c => c.IdCompra == idCompra);
    }
}

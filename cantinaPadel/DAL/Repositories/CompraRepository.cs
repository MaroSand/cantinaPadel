using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class CompraRepository : ICompraRepository
{
    public Compra Registrar(Compra compra)
    {
        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        // EF inserta primero la compra (genera id_compra) y después sus detalles, que viajan dentro de compra.Detalles
        ctx.Compras.Add(compra);
        ctx.SaveChanges();

        // US-26: el impacto de la compra en stock, costo y proveedor de cada producto se agrega acá, dentro de esta misma transacción
        transaccion.Commit();
        return compra;
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
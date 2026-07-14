using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        public List<Producto> ObtenerTodos()
        {
            using var ctx = new AppDbContext();
            return ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToList();
        }
        
        public List<Producto> Buscar(string? texto, int? idCategoria, int? idMarca)
        {
            using var ctx = new AppDbContext();

            IQueryable<Producto> query = ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                texto = texto.Trim().ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(texto) ||
                    p.CodigoBarras.Contains(texto));
            }

            if (idCategoria.HasValue)
                query = query.Where(p => p.IdCategoria == idCategoria.Value);

            if (idMarca.HasValue)
                query = query.Where(p => p.IdMarca == idMarca.Value);

            return query.OrderBy(p => p.Nombre).ToList();
        }

        // Devuelve todos los productos que cumplen con los criterios de categoría, marca y producto.
        public List<Producto> ObtenerPorCriterio(int? idCategoria, int? idMarca, int? idProducto)
        {
            using var ctx = new AppDbContext();

            IQueryable<Producto> query = ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo);

            if (idProducto.HasValue)
                query = query.Where(p => p.IdProducto == idProducto.Value);

            if (idCategoria.HasValue)
                query = query.Where(p => p.IdCategoria == idCategoria.Value);

            if (idMarca.HasValue)
                query = query.Where(p => p.IdMarca == idMarca.Value);

            return query.OrderBy(p => p.Nombre).ToList();
        }

        public void ActualizarPreciosMasivo(List<int> idsProductos, decimal porcentaje)
        {
            if (idsProductos == null || idsProductos.Count == 0) return;

            using var ctx = new AppDbContext();
            decimal factor = 1 + (porcentaje / 100m);
            ctx.Productos
                .Where(p => idsProductos.Contains(p.IdProducto))
                .ExecuteUpdate(setters => setters
                    .SetProperty(p => p.PrecioVenta, p => Math.Round(p.PrecioVenta * factor, 2)));
        }

        // Usado por el lector de código de barras: escaneás y busca al toque
        public Producto? ObtenerPorCodigoBarras(string codigoBarras)
        {
            using var ctx = new AppDbContext();
            return ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .FirstOrDefault(p => p.CodigoBarras == codigoBarras && p.Activo);
        }

        public Producto? ObtenerPorId(int idProducto)
        {
            using var ctx = new AppDbContext();
            return ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .FirstOrDefault(p => p.IdProducto == idProducto);
        }

        // Valida unicidad del código de barras antes de guardar.
        public bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null)
        {
            using var ctx = new AppDbContext();
            return ctx.Productos.Any(p =>
                p.CodigoBarras == codigoBarras &&
                p.IdProducto   != (idProductoExcluir ?? 0));
        }

        public void Agregar(Producto producto)
        {
            using var ctx = new AppDbContext();
            ctx.Productos.Add(producto);
            ctx.SaveChanges();
        }

        public void Modificar(Producto producto)
        {
            using var ctx = new AppDbContext();
            ctx.Productos.Update(producto);
            ctx.SaveChanges();
        }

        // Baja lógica: no se borra un producto, se desactiva.
        public void BajaLogica(int idProducto)
        {
            using var ctx = new AppDbContext();
            var producto = ctx.Productos.Find(idProducto);
            if (producto == null) return;

            producto.Activo = false;
            ctx.SaveChanges();
        }
    }
}
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        public List<Producto> ObtenerTodos(bool? activo = true)
        {
            using var ctx = new AppDbContext();
            IQueryable<Producto> query = ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor)
                    .ThenInclude(p => p.Persona);

            if (activo.HasValue)
                query = query.Where(p => p.Activo == activo.Value);

            return query.OrderBy(p => p.Nombre).ToList();
        }

        public List<Producto> Buscar(string? texto, int? idCategoria, int? idMarca, bool? activo = true)
        {
            using var ctx = new AppDbContext();

            IQueryable<Producto> query = ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor)
                    .ThenInclude(p => p.Persona);

            if (activo.HasValue)
                query = query.Where(p => p.Activo == activo.Value);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                texto = texto.Trim().ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(texto) ||
                    (p.CodigoBarras != null && p.CodigoBarras.Contains(texto)));
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

        // Actualiza cada producto a su precio final indicado en el diccionario.
        // A diferencia del viejo esquema (un solo % para todos, resuelto con
        // ExecuteUpdate), acá cada fila puede traer un valor distinto, así
        // que se trackean las entidades y se guardan con SaveChanges.
        public void ActualizarPrecios(Dictionary<int, decimal> preciosNuevos)
        {
            if (preciosNuevos == null || preciosNuevos.Count == 0) return;

            using var ctx = new AppDbContext();
            using var transaccion = ctx.Database.BeginTransaction();
            var ids = preciosNuevos.Keys.ToList();

            var productos = ctx.Productos
                .Where(p => ids.Contains(p.IdProducto))
                .ToList();

            foreach (var producto in productos)
            {
                if (preciosNuevos.TryGetValue(producto.IdProducto, out var precioNuevo))
                    producto.PrecioVenta = precioNuevo;
            }

            ctx.SaveChanges();
            ReajustarDeudaPendientePorCambioDePrecio(ctx, productos);
            transaccion.Commit();
        }

        // Si un producto que todavía tiene unidades impagas (cuenta corriente)
        // cambia de precio, todas esas unidades pasan al precio nuevo, ya sea
        // que el precio suba o baje. Las unidades pagadas por completo no se
        // tocan: son historial de una venta ya cobrada.
        //
        // Después de reprecificar se reaplica el crédito de los clientes
        // afectados, porque con un precio menor el crédito que ya tenían puede
        // alcanzar para saldar una unidad.
        //
        // Guarda los cambios con SaveChanges; el llamador maneja la transacción.
        private static void ReajustarDeudaPendientePorCambioDePrecio(AppDbContext ctx, IReadOnlyCollection<Producto> productosActualizados)
        {
            if (productosActualizados.Count == 0) return;

            var precioConIvaPorProducto = productosActualizados.ToDictionary(p => p.IdProducto, p => p.PrecioConIva);
            var idsProductos = precioConIvaPorProducto.Keys.ToList();

            var detallesPendientes = (from d in ctx.DetallesVenta
                                      join v in ctx.Ventas on d.IdVenta equals v.IdVenta
                                      where d.IdProducto != null && idsProductos.Contains(d.IdProducto.Value) && !d.Pagado
                                      select new { Detalle = d, v.IdCliente })
                .ToList();

            foreach (var pendiente in detallesPendientes)
            {
                decimal nuevoPrecio = precioConIvaPorProducto[pendiente.Detalle.IdProducto!.Value];
                pendiente.Detalle.PrecioUnitario = nuevoPrecio;
                pendiente.Detalle.Subtotal = nuevoPrecio; // 1 fila = 1 unidad
            }
            ctx.SaveChanges();

            ConciliacionCuentaCorriente.ConciliarClientes(ctx, detallesPendientes.Select(x => x.IdCliente));
            ctx.SaveChanges();
        }

        // Usado por el lector de código de barras: escaneás y busca al toque
        public Producto? ObtenerPorCodigoBarras(string codigoBarras)
        {
            using var ctx = new AppDbContext();
            return ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor)
                    .ThenInclude(p => p.Persona)
                .FirstOrDefault(p => p.CodigoBarras == codigoBarras && p.Activo);
        }

        public Producto? ObtenerPorId(int idProducto)
        {
            using var ctx = new AppDbContext();
            return ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.Proveedor)
                    .ThenInclude(p => p.Persona)
                .FirstOrDefault(p => p.IdProducto == idProducto);
        }


        public decimal ObtenerPorcentajeGananciaCategoria(int idCategoria)
        {
            using var ctx = new AppDbContext();
            return ctx.Categorias
                .Where(c => c.IdCategoria == idCategoria)
                .Select(c => c.PorcentajeGanancia)
                .FirstOrDefault();
        }

        // Valida unicidad del código de barras antes de guardar.
        public bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null)
        {
            using var ctx = new AppDbContext();

            if (!idProductoExcluir.HasValue)
                return ctx.Productos.Any(p => p.CodigoBarras == codigoBarras);

            int idExcluir = idProductoExcluir.Value;
            return ctx.Productos.Any(p =>
                p.CodigoBarras == codigoBarras &&
                p.IdProducto != idExcluir);
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
            using var transaccion = ctx.Database.BeginTransaction();
            ctx.Productos.Update(producto);
            ctx.SaveChanges();
            ReajustarDeudaPendientePorCambioDePrecio(ctx, new[] { producto });
            transaccion.Commit();
        }

        // Baja/alta lógica: no se borra un producto, se alterna su estado.
        public void BajaLogica(int idProducto)
        {
            using var ctx = new AppDbContext();
            var producto = ctx.Productos.Find(idProducto);
            if (producto == null) return;

            producto.Activo = !producto.Activo;
            ctx.SaveChanges();
        }
    }
}
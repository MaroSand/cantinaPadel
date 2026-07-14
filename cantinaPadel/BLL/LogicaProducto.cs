using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.BLL
{
    public class LogicaProducto
    {
        private readonly IProductoRepository _repo;

        public LogicaProducto()
        {
            _repo = new ProductoRepository();
        }

        public List<Producto> ObtenerTodos() => _repo.ObtenerTodos();

        public List<Producto> Buscar(string? texto, int? idCategoria, int? idMarca)
            => _repo.Buscar(texto, idCategoria, idMarca);

        public Producto? ObtenerPorId(int id) => _repo.ObtenerPorId(id);

        public Producto? ObtenerPorCodigoBarras(string codigo)
            => _repo.ObtenerPorCodigoBarras(codigo);

        // Alta
        public (bool ok, string error) Agregar(Producto producto)
        {
            var validacion = Validar(producto, esAlta: true);
            if (!validacion.ok) return validacion;

            _repo.Agregar(producto);
            return (true, string.Empty);
        }

        // Modificación
        public (bool ok, string error) Modificar(Producto producto)
        {
            var validacion = Validar(producto, esAlta: false);
            if (!validacion.ok) return validacion;

            _repo.Modificar(producto);
            return (true, string.Empty);
        }

        public void BajaLogica(int idProducto) => _repo.BajaLogica(idProducto);

        public decimal CalcularPrecioConIva(decimal precioBase)
            => Math.Round(precioBase * 1.21m, 2);

        // Helpers
        public List<Categoria> ObtenerCategoriasActivas()
        {
            using var ctx = new DAL.AppDbContext();
            return ctx.Categorias.Where(c => c.Activa).OrderBy(c => c.Nombre).ToList();
        }

        public List<Marca> ObtenerMarcasActivas()
        {
            using var ctx = new DAL.AppDbContext();
            return ctx.Marcas.Where(m => m.Activa).OrderBy(m => m.Nombre).ToList();
        }

        public List<Proveedor> ObtenerProveedoresActivos()
        {
            using var ctx = new DAL.AppDbContext();
            return ctx.Proveedores
                .Include(p => p.Persona)
                .Where(p => p.Persona.Activo)
                .OrderBy(p => p.Persona.Apellido)
                .ToList();
        }

        // Validaciones
        private (bool ok, string error) Validar(Producto producto, bool esAlta)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return (false, "El nombre es obligatorio.");

            // El código de barras NO es obligatorio. Si viene, validamos unicidad más abajo.

            if (producto.IdCategoria <= 0)
                return (false, "Debe seleccionar una categoría.");

            if (producto.PrecioVenta <= 0)
                return (false, "El precio de venta debe ser mayor a cero.");

            if (producto.PrecioCosto < 0)
                return (false, "El precio de costo no puede ser negativo.");

            if (producto.StockMinimo < 0)
                return (false, "El stock mínimo no puede ser negativo.");

            // Unicidad del código de barras. En modificación, excluimos
            // el propio producto para no chocar contra sí mismo.
            int? idExcluir = esAlta ? null : producto.IdProducto;
            if (!string.IsNullOrWhiteSpace(producto.CodigoBarras) && _repo.ExisteCodigoBarras(producto.CodigoBarras!, idExcluir))
                return (false, $"Ya existe un producto con el código de barras '{producto.CodigoBarras}'.");

            return (true, string.Empty);
        }
    }
}
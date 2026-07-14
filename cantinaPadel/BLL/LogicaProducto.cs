using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.BLL
{


    public class ProductoPrecioPreview
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioActual { get; set; }
        public decimal PrecioNuevo { get; set; }
        // Tildado en la grilla: indica si este producto se incluye al confirmar.
        // Por default viene tildado (true) al previsualizar.
        public bool Aplicar { get; set; } = true;
        // No se muestra en la grilla (no es columna). Se usa para que el
        // recálculo automático por porcentaje NO pise un precio que el
        // usuario ya tipeó a mano en la celda "Precio Nuevo".
        public bool EditadoManualmente { get; set; } = false;
    }
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

        // Búsqueda única para FrmActualizacionPrecios: combina texto libre
        // (nombre, código, categoría o marca) con los combos de categoría y
        // marca, todo en simultáneo (AND). Reemplaza a los antiguos
        // PrevisualizarActualizacion / PrevisualizarPorTexto: ya no hace
        // falta elegir un "criterio" único, se usa lo que el usuario haya
        // completado de cada filtro.
        public List<ProductoPrecioPreview> BuscarParaActualizacion(string? texto, int? idCategoria, int? idMarca, decimal porcentaje)
        {
            using var ctx = new DAL.AppDbContext();

            IQueryable<Producto> query = ctx.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var t = texto.Trim().ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(t) ||
                    (p.CodigoBarras != null && p.CodigoBarras.Contains(t)) ||
                    p.Categoria.Nombre.ToLower().Contains(t) ||
                    (p.Marca != null && p.Marca.Nombre.ToLower().Contains(t)));
            }

            if (idCategoria.HasValue)
                query = query.Where(p => p.IdCategoria == idCategoria.Value);

            if (idMarca.HasValue)
                query = query.Where(p => p.IdMarca == idMarca.Value);

            var productos = query.OrderBy(p => p.Nombre).ToList();
            decimal factor = 1 + (porcentaje / 100m);

            return productos.Select(p => new ProductoPrecioPreview
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                PrecioActual = p.PrecioVenta,
                PrecioNuevo = Math.Round(p.PrecioVenta * factor, 2)
            }).ToList();
        }

        // Confirma los precios finales tal cual quedaron en la grilla del
        // usuario (ya sea calculados por porcentaje o tipeados a mano),
        // no un porcentaje único para todos.
        public void ConfirmarActualizacion(Dictionary<int, decimal> preciosFinales)
            => _repo.ActualizarPrecios(preciosFinales);

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
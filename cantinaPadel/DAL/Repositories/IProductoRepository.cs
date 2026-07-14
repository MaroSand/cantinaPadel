using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IProductoRepository
    {
        List<Producto> ObtenerTodos();

        // Un solo método de búsqueda "todo en uno". Cada parámetro es opcional
        // (nullable). Si viene null, ese filtro simplemente no se aplica.
        List<Producto> Buscar(string? texto, int? idCategoria, int? idMarca);

        Producto? ObtenerPorCodigoBarras(string codigoBarras);
        Producto? ObtenerPorId(int idProducto);
        bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null);
        void Agregar(Producto producto);
        void Modificar(Producto producto);
        void BajaLogica(int idProducto);

        // Devuelve todos los productos que cumplen con los criterios de categoría, marca y producto.
        List<Producto> ObtenerPorCriterio(int? idCategoria, int? idMarca, int? idProducto);

        // Actualiza el precio de venta de cada producto al valor final indicado
        // en el diccionario (idProducto -> precioNuevo). Reemplaza al viejo
        // esquema de "un solo porcentaje para todos", porque ahora cada fila
        // puede tener un precio distinto (por % o editado a mano).
        void ActualizarPrecios(Dictionary<int, decimal> preciosNuevos);
    }
}
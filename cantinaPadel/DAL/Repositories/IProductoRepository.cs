using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IProductoRepository
    {
        List<Producto> ObtenerTodos();
        List<Producto> BuscarPorNombre(string nombre);
        List<Producto> BuscarPorIdProducto(int idProducto);
        List<Producto> BuscarPorMarca(int idMarca);
        Producto? ObtenerPorCodigoBarras(string codigoBarras);
        Producto? ObtenerPorId(int idProducto);
        bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null);
        void Agregar(Producto producto);
        void Modificar(Producto producto);
        void BajaLogica(int idProducto);
    }
}
using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

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

    public List<Producto> BuscarPorNombre(string nombre)
    {
        throw new NotImplementedException();
    }

    public List<Producto> BuscarPorIdProducto(int idProducto)
    {
        throw new NotImplementedException();
    }

    public List<Producto> BuscarPorMarca(int idMarca)
    {
        throw new NotImplementedException();
    }

    public Producto? ObtenerPorCodigoBarras(string codigoBarras)
    {
        throw new NotImplementedException();
    }

    public Producto? ObtenerPorId(int idProducto)
    {
        throw new NotImplementedException();
    }

    public bool ExisteCodigoBarras(string codigoBarras, int? idProductoExcluir = null)
    {
        throw new NotImplementedException();
    }

    public void Agregar(Producto producto)
    {
        throw new NotImplementedException();
    }

    public void Modificar(Producto producto)
    {
        throw new NotImplementedException();
    }

    public void BajaLogica(int idProducto)
    {
        throw new NotImplementedException();
    }
}
using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

public interface IProveedorRepositorio
{
    List<Proveedor> ObtenerTodos();
    List<Proveedor> BuscarPorNombreOCuit(string termino);
    Proveedor? ObtenerPorId(int idProveedor);
    void Agregar(Persona persona, Proveedor proveedor);
    void Modificar(Persona persona, Proveedor proveedor);
    void BajaLogica(int idProveedor);
}
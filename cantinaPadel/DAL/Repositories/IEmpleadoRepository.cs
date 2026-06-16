using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IEmpleadoRepository
    {
        Empleado? ObtenerPorCredenciales(string nombreUsuario, string contrasena);
    }
}
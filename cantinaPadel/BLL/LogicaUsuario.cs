using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaUsuario
    {
        private readonly IEmpleadoRepository _repo;

        public LogicaUsuario()
        {
            _repo = new EmpleadoRepository();
        }

        public bool ValidarCredenciales(string usuario, string contrasena,
            out int idUsuario, out string? rol)
        {
            idUsuario = 0;
            rol       = null;

            Empleado? empleado = _repo.ObtenerPorCredenciales(usuario, contrasena);

            if (empleado == null)
                return false;

            idUsuario = empleado.IdEmpleado;
            rol       = empleado.Rol;
            return true;
        }
    }
}
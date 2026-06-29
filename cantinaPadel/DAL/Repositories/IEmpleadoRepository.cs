using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IEmpleadoRepository
    {
        Empleado? ObtenerPorCredenciales(string nombreUsuario, string contrasena);

        // Métodos para CRUD Empleado
        List<Empleado> ObtenerTodos();
        void Guardar(Empleado empleado);
        bool ExisteUsuario(string nombreUsuario, int idEmpleadoExcluir);
        bool ExisteDni(string dni, int idPersonaExcluir);
    }
}

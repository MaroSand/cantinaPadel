using System;
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

        public bool ValidarCredenciales(string usuario, string contrasena, out int idUsuario, out string? rol)
        {
            idUsuario = 0;
            rol = null;

            try
            {
                // Busca en la base de datos usando el método del repositorio
                Empleado? empleado = _repo.ObtenerPorCredenciales(usuario, contrasena);

                if (empleado == null)
                    return false;

                // Si lo encuentra, asigna las variables de salida para la sesión
                idUsuario = empleado.IdEmpleado;
                rol = empleado.Rol;
                return true;
            }
            catch (Exception)
            {
                // Si falla la conexión a la base de datos, manejamos el error de forma segura
                return false;
            }
        }
    }
}
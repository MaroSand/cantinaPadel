using System;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    // Resultado detallado del intento de login, para que la UI pueda mostrar un mensaje claro y específico según lo que falló
    public enum ResultadoLogin
    {
        Ok,
        UsuarioNoEncontrado,
        ContrasenaIncorrecta,
        ErrorConexion
    }

    public class LogicaUsuario
    {
        private readonly IEmpleadoRepository _repo;

        public LogicaUsuario()
        {
            _repo = new EmpleadoRepository();
        }

        public ResultadoLogin ValidarCredenciales(string usuario, string contrasena, out int idUsuario, out string? rol)
        {
            idUsuario = 0;
            rol = null;

            try
            {
                // Primero se busca solo por usuario, para poder distinguir "no existe ese usuario" de "la contraseña incorrecta"
                Empleado? empleado = _repo.ObtenerPorUsuario(usuario);

                if (empleado == null)
                    return ResultadoLogin.UsuarioNoEncontrado;

                if (empleado.Contrasena != contrasena)
                    return ResultadoLogin.ContrasenaIncorrecta;

                // Credenciales correctas: asigna las variables de salida para la sesión
                idUsuario = empleado.IdEmpleado;
                rol = empleado.Rol;
                return ResultadoLogin.Ok;
            }
            catch (Exception)
            {
                // Si falla la conexión a la base de datos, se maneja el error de forma segura
                return ResultadoLogin.ErrorConexion;
            }
        }
    }
}
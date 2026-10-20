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
        UsuarioInhabilitado,
        ErrorConexion
    }

    public class LogicaUsuario
    {
        private readonly IEmpleadoRepository _repo;

        public LogicaUsuario()
            : this(new EmpleadoRepository())
        {
        }

        public LogicaUsuario(IEmpleadoRepository repo)
        {
            _repo = repo;
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

                // Credenciales correctas pero el empleado fue dado de baja: no puede ingresar, y se informa distinto a un error de credenciales
                // Se evalúa después de la contraseña para no revelar el estado de la cuenta a quien no la conoce
                if (!empleado.Activo)
                    return ResultadoLogin.UsuarioInhabilitado;

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

        // Devuelve "Nombre Apellido" del empleado con ese usuario, o null si no se puede obtener (el login no debe fallar por esto)
        public string? ObtenerNombreCompleto(string usuario)
        {
            try
            {
                Empleado? empleado = _repo.ObtenerPorUsuario(usuario);
                if (empleado?.Persona == null)
                    return null;

                return $"{empleado.Persona.Nombre} {empleado.Persona.Apellido}".Trim();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
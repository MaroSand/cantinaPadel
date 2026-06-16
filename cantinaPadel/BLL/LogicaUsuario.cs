using System;

namespace cantinaPadel.BLL
{
    public class LogicaUsuario
    {
        public bool ValidarCredenciales(string usuario, string dni, out int idUsuario, out string rol)
        {
            // Inicializamos los parámetros de salida por defecto
            idUsuario = 0;
            rol = null;

            // CREDENCIALES DE PRUEBA

            // Prueba para Administrador
            if (usuario == "admin" && dni == "12345678")
            {
                idUsuario = 1;
                rol = "Administrador";
                return true;
            }

            // Prueba para Cajero (Empleado)
            // Probar con usuario: cajero y contraseña: 87654321
            if (usuario == "cajero" && dni == "87654321")
            {
                idUsuario = 2;
                rol = "Cajero";
                return true;
            }

            // Si no coincide con ninguno, rebota
            return false;
        }
    }
}
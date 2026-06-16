using System;

namespace cantinaPadel
{
    // Al ser static, no hace falta instanciarla con new. Se usa directo en todo el proyecto.
    public static class Sesion
    {
        // Propiedades globales obligatorias de la subtarea
        public static int IdUsuario { get; set; }
        public static string Rol { get; set; }

        // Método opcional para limpiar los datos al cerrar sesión
        public static void CerrarSesion()
        {
            IdUsuario = 0;
            Rol = null;
        }
    }
}
using System;
using System.Windows.Forms;

namespace cantinaPadel
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Ciclo Login → Menú principal. Al cerrar sesión se vuelve al Login en vez de terminar el programa
            while (true)
            {
                // El Login se muestra como diálogo: el código se detiene acá hasta que se cierra
                using (var login = new FrmLogin())
                {
                    // Si el usuario cerró el Login con la cruz, el programa termina sin abrir nada
                    if (login.ShowDialog() != DialogResult.OK)
                        break;
                }

                // Credenciales correctas: arranca la aplicación real con el formulario principal.
                // Application.Run no devuelve el control hasta que se cierra FrmMain
                using (var principal = new FrmMain())
                {
                    Application.Run(principal);
                }

                // "Cerrar sesión" limpia la sesión antes de cerrar FrmMain y vuelve al Login
                // Si FrmMain se cerró con la cruz de la ventana, la sesión sigue activa y el programa termina
                if (Sesion.Activa)
                    break;
            }
        }
    }
}
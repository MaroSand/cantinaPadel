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

            // Se instancia el formulario de Login
            FrmLogin login = new FrmLogin();

            // Se muestra el Login como una ventana de diálogo flotante
            // Esto detiene el código acá hasta que el Login se cierre
            if (login.ShowDialog() == DialogResult.OK)
            {
                // Si el Login devuelve "OK" (credenciales correctas), arranca la aplicación real con el formulario principal
                Application.Run(new FrmMain());
            }
            else
            {
                // Si el usuario cerró la cruz, el programa termina limpiamente sin abrir nada
                Application.Exit();
            }
        }
    }
}
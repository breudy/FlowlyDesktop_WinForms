using System;
using System.Windows.Forms;

namespace FlowlyDesktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Primero el login; si entra bien se abre el menú principal.
            // Si en el menú se pulsa "Cerrar sesión" se vuelve al login; si se cierra la ventana, termina la app.
            bool seguir = true;
            while (seguir)
            {
                LoginForm login = new LoginForm();
                if (login.ShowDialog() != DialogResult.OK)
                {
                    seguir = false;
                }
                else
                {
                    MainForm principal = new MainForm();
                    principal.ShowDialog();
                    seguir = principal.CerrarSesion;
                }
            }
        }
    }
}

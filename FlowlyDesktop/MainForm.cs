using System;
using System.Drawing;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Ventana principal: menú lateral que se puede plegar con la hamburguesa (como en el wireframe de Figma)
    // y, a la derecha, la pantalla que toque en cada momento.
    public class MainForm : Form
    {
        public bool CerrarSesion = false;

        private Panel panelMenu;
        private Panel panelCabecera;
        private Panel panelContenido;
        private Label lblTitulo;
        private Label lblUsuario;
        private Button btnHamburguesa;
        private Button[] botones;
        private string[] textos = { "Proyectos", "Usuarios", "Equipos", "Roles", "Informes", "Cerrar sesión" };

        private bool menuAbierto = true;
        private Form pantallaActual = null;

        public MainForm()
        {
            Ui.ConfigurarPantalla(this, "Flowly · Administración");
            this.ClientSize = new Size(1200, 720);
            this.MinimumSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            Construir();
            Abrir(new ProyectosForm(), "Proyectos");
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            // ----- Menú lateral -----
            panelMenu = new Panel();
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Width = 210;
            panelMenu.BackColor = Ui.ColorOscuro;

            btnHamburguesa = new Button();
            btnHamburguesa.Text = "☰";
            btnHamburguesa.Font = new Font("Segoe UI Symbol", 16F);
            btnHamburguesa.ForeColor = Color.White;
            btnHamburguesa.FlatStyle = FlatStyle.Flat;
            btnHamburguesa.FlatAppearance.BorderSize = 0;
            btnHamburguesa.Left = 0;
            btnHamburguesa.Top = 8;
            btnHamburguesa.Width = 56;
            btnHamburguesa.Height = 44;
            btnHamburguesa.Cursor = Cursors.Hand;
            btnHamburguesa.Click += new EventHandler(btnHamburguesa_Click);
            panelMenu.Controls.Add(btnHamburguesa);

            botones = new Button[textos.Length];
            for (int i = 0; i < textos.Length; i++)
            {
                Button b = new Button();
                b.Tag = i;
                b.Left = 0;
                b.Top = 64 + i * 48;
                b.Width = 210;
                b.Height = 44;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.ForeColor = Color.White;
                b.BackColor = Ui.ColorOscuro;
                b.Font = Ui.Fuente;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.Padding = new Padding(18, 0, 0, 0);
                b.Cursor = Cursors.Hand;
                b.Click += new EventHandler(botonMenu_Click);
                botones[i] = b;
                panelMenu.Controls.Add(b);
            }
            ActualizarTextosMenu();

            // ----- Cabecera -----
            panelCabecera = new Panel();
            panelCabecera.Dock = DockStyle.Top;
            panelCabecera.Height = 64;
            panelCabecera.BackColor = Color.White;

            lblTitulo = new Label();
            lblTitulo.Font = Ui.FuenteTitulo;
            lblTitulo.ForeColor = Ui.ColorOscuro;
            lblTitulo.AutoSize = true;
            lblTitulo.Left = 20;
            lblTitulo.Top = 15;

            lblUsuario = new Label();
            lblUsuario.AutoSize = false;
            lblUsuario.Dock = DockStyle.Right;
            lblUsuario.Width = 300;
            lblUsuario.TextAlign = ContentAlignment.MiddleRight;
            lblUsuario.Padding = new Padding(0, 0, 20, 0);
            lblUsuario.ForeColor = Color.FromArgb(100, 116, 139);
            if (Api.Sesion != null)
            {
                lblUsuario.Text = Api.Sesion.Nombre + " · Administrador";
            }

            panelCabecera.Controls.Add(lblTitulo);
            panelCabecera.Controls.Add(lblUsuario);

            // ----- Contenido -----
            panelContenido = new Panel();
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.BackColor = Ui.ColorFondo;

            // El orden importa: primero lo que rellena (Fill) y después lo que se pega a los bordes.
            this.Controls.Add(panelContenido);
            this.Controls.Add(panelCabecera);
            this.Controls.Add(panelMenu);
        }

        // Muestra "Proyectos" con el nombre completo o solo la inicial si el menú está plegado.
        private void ActualizarTextosMenu()
        {
            for (int i = 0; i < botones.Length; i++)
            {
                if (menuAbierto)
                {
                    botones[i].Text = textos[i];
                }
                else
                {
                    botones[i].Text = textos[i].Substring(0, 1);
                }
            }
        }

        private void btnHamburguesa_Click(object sender, EventArgs e)
        {
            menuAbierto = !menuAbierto;

            if (menuAbierto)
            {
                panelMenu.Width = 210;
            }
            else
            {
                panelMenu.Width = 56;
            }

            for (int i = 0; i < botones.Length; i++)
            {
                botones[i].Width = panelMenu.Width;
            }
            ActualizarTextosMenu();
        }

        private void botonMenu_Click(object sender, EventArgs e)
        {
            Button pulsado = (Button)sender;
            int numero = (int)pulsado.Tag;

            if (numero == 0)
            {
                Abrir(new ProyectosForm(), "Proyectos");
            }
            else if (numero == 1)
            {
                Abrir(new UsuariosForm(), "Usuarios");
            }
            else if (numero == 2)
            {
                Abrir(new EquiposForm(), "Equipos");
            }
            else if (numero == 3)
            {
                Abrir(new RolesForm(), "Roles");
            }
            else if (numero == 4)
            {
                Abrir(new InformesForm(0), "Informes");
            }
            else
            {
                if (Ui.Confirmar("¿Quieres cerrar la sesión?"))
                {
                    AuthApi.Logout();
                    this.CerrarSesion = true;
                    this.Close();
                }
            }
        }

        // Cambia la pantalla que se ve a la derecha.
        private void Abrir(Form pantalla, string titulo)
        {
            if (pantallaActual != null)
            {
                pantallaActual.Close();
                pantallaActual.Dispose();
            }

            pantallaActual = pantalla;

            // Esta parte es de información externa que no sale en los vídeos
            // (un Form normal es una ventana; con TopLevel = false se puede meter dentro de un Panel como si fuera un control)
            pantalla.TopLevel = false;
            pantalla.FormBorderStyle = FormBorderStyle.None;
            pantalla.Dock = DockStyle.Fill;

            panelContenido.Controls.Add(pantalla);
            lblTitulo.Text = titulo;
            pantalla.Show();
        }
    }
}

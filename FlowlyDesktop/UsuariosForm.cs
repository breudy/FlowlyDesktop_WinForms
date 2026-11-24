using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Lista de usuarios: crear, editar, activar / desactivar y cambiar contraseña.
    public class UsuariosForm : Form
    {
        private List<Usuario> todos = new List<Usuario>();
        private BindingList<Usuario> lista = new BindingList<Usuario>();

        private DataGridView tabla;
        private TextBox txtBuscar;
        private Button btnActivar;

        public UsuariosForm()
        {
            Ui.ConfigurarPantalla(this, "Usuarios");
            Construir();
            this.Load += new EventHandler(UsuariosForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            FlowLayoutPanel barra = Ui.BarraBotones();

            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar:";
            lblBuscar.AutoSize = true;
            lblBuscar.Margin = new Padding(0, 6, 6, 0);

            txtBuscar = new TextBox();
            txtBuscar.Width = 240;
            txtBuscar.Margin = new Padding(0, 3, 24, 0);
            txtBuscar.TextChanged += new EventHandler(txtBuscar_TextChanged);

            Button btnNuevo = Ui.Boton("+ Nuevo usuario", 140);
            btnNuevo.Click += new EventHandler(btnNuevo_Click);
            Button btnEditar = Ui.BotonSecundario("Editar", 80);
            btnEditar.Click += new EventHandler(btnEditar_Click);
            Button btnPassword = Ui.BotonSecundario("Cambiar contraseña", 160);
            btnPassword.Click += new EventHandler(btnPassword_Click);
            btnActivar = Ui.BotonSecundario("Activar / Desactivar", 160);
            btnActivar.Click += new EventHandler(btnActivar_Click);

            barra.Controls.Add(lblBuscar);
            barra.Controls.Add(txtBuscar);
            barra.Controls.Add(btnNuevo);
            barra.Controls.Add(btnEditar);
            barra.Controls.Add(btnPassword);
            barra.Controls.Add(btnActivar);

            tabla = Ui.Tabla();
            tabla.Dock = DockStyle.Fill;
            Ui.Col(tabla, "Nombre", "NombreCompleto", 25);
            Ui.Col(tabla, "Email", "Email", 28);
            Ui.Col(tabla, "Rol", "RolEtiqueta", 15);
            Ui.Col(tabla, "€/hora", "TarifaHora", 10, "N2");
            Ui.Col(tabla, "Estado", "Estado", 12);
            Ui.Col(tabla, "Equipos", "Grupos", 25);
            tabla.DataSource = lista;
            tabla.CellDoubleClick += new DataGridViewCellEventHandler(tabla_CellDoubleClick);

            this.Controls.Add(tabla);
            this.Controls.Add(barra);
        }

        private void UsuariosForm_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                todos = UsuariosApi.GetTodos();
                Filtrar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void Filtrar()
        {
            string texto = txtBuscar.Text.Trim().ToLower();

            lista.Clear();
            for (int i = 0; i < todos.Count; i++)
            {
                string datos = (todos[i].NombreCompleto + " " + todos[i].Email).ToLower();
                if (datos.IndexOf(texto) >= 0)
                {
                    lista.Add(todos[i]);
                }
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            Filtrar();
        }

        private Usuario ElegirUsuario()
        {
            Usuario u = Ui.Seleccionado(tabla) as Usuario;
            if (u == null)
            {
                Ui.Aviso("Selecciona primero un usuario de la tabla.");
            }
            return u;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            UsuarioEditForm dialogo = new UsuarioEditForm(null);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Usuario u = ElegirUsuario();
            if (u == null)
            {
                return;
            }

            UsuarioEditForm dialogo = new UsuarioEditForm(u);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void btnPassword_Click(object sender, EventArgs e)
        {
            Usuario u = ElegirUsuario();
            if (u == null)
            {
                return;
            }

            PasswordForm dialogo = new PasswordForm(u);
            dialogo.ShowDialog();
        }

        private void btnActivar_Click(object sender, EventArgs e)
        {
            Usuario u = ElegirUsuario();
            if (u == null)
            {
                return;
            }

            if (Api.Sesion != null && u.IdUsuario == Api.Sesion.IdUsuario)
            {
                Ui.Aviso("No puedes desactivar tu propio usuario.");
                return;
            }

            bool nuevoEstado = !u.Activo;
            string accion = "desactivar";
            if (nuevoEstado)
            {
                accion = "activar";
            }

            if (!Ui.Confirmar("¿Quieres " + accion + " a " + u.NombreCompleto + "?"))
            {
                return;
            }

            try
            {
                UsuariosApi.CambiarActivo(u.IdUsuario, nuevoEstado);
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void tabla_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEditar_Click(sender, e);
            }
        }
    }
}

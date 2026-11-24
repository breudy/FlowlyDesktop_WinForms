using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Roles: los roles los define la base de datos (no se crean desde aquí).
    // A la izquierda se ven los roles y a la derecha los usuarios que tiene el rol seleccionado.
    public class RolesForm : Form
    {
        private List<Usuario> usuarios = new List<Usuario>();
        private BindingList<Rol> roles = new BindingList<Rol>();
        private BindingList<Usuario> deRol = new BindingList<Usuario>();

        private DataGridView tablaRoles;
        private DataGridView tablaUsuarios;

        public RolesForm()
        {
            Ui.ConfigurarPantalla(this, "Roles");
            Construir();
            this.Load += new EventHandler(RolesForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            Label nota = new Label();
            nota.Dock = DockStyle.Top;
            nota.Height = 44;
            nota.Padding = new Padding(16, 12, 0, 0);
            nota.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            nota.Text = "Para cambiar el rol de una persona, edítala en la pantalla Usuarios.";

            tablaRoles = Ui.Tabla();
            tablaRoles.Dock = DockStyle.Left;
            tablaRoles.Width = 460;
            Ui.Col(tablaRoles, "Rol", "Etiqueta", 35);
            Ui.Col(tablaRoles, "Descripción", "Descripcion", 65);
            tablaRoles.DataSource = roles;
            tablaRoles.SelectionChanged += new EventHandler(tablaRoles_SelectionChanged);

            tablaUsuarios = Ui.Tabla();
            tablaUsuarios.Dock = DockStyle.Fill;
            Ui.Col(tablaUsuarios, "Usuario", "NombreCompleto", 40);
            Ui.Col(tablaUsuarios, "Email", "Email", 40);
            Ui.Col(tablaUsuarios, "Estado", "Estado", 20);
            tablaUsuarios.DataSource = deRol;

            this.Controls.Add(tablaUsuarios);
            this.Controls.Add(tablaRoles);
            this.Controls.Add(nota);
        }

        private void RolesForm_Load(object sender, EventArgs e)
        {
            try
            {
                usuarios = UsuariosApi.GetTodos();
                List<Rol> lista = RolesApi.GetTodos();
                for (int i = 0; i < lista.Count; i++)
                {
                    roles.Add(lista[i]);
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void tablaRoles_SelectionChanged(object sender, EventArgs e)
        {
            deRol.Clear();
            Rol seleccionado = Ui.Seleccionado(tablaRoles) as Rol;
            if (seleccionado == null)
            {
                return;
            }

            for (int i = 0; i < usuarios.Count; i++)
            {
                if (usuarios[i].IdRol == seleccionado.IdRol)
                {
                    deRol.Add(usuarios[i]);
                }
            }
        }
    }
}

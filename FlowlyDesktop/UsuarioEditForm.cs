using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Crear usuario / editar usuario (null = usuario nuevo). La contraseña solo se pide al crear.
    public class UsuarioEditForm : Form
    {
        private Usuario usuario;

        private TextBox txtNombre;
        private TextBox txtApellidos;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private NumericUpDown numTarifa;
        private ComboBox cmbRol;

        public UsuarioEditForm(Usuario usuario)
        {
            this.usuario = usuario;

            string titulo = "Crear usuario";
            if (usuario != null)
            {
                titulo = "Editar usuario";
            }
            Ui.ConfigurarDialogo(this, titulo, 500, 400);
            Construir();
            this.Load += new EventHandler(UsuarioEditForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            txtNombre = new TextBox();
            txtApellidos = new TextBox();
            txtEmail = new TextBox();
            txtPassword = new TextBox();
            txtPassword.UseSystemPasswordChar = true;

            numTarifa = new NumericUpDown();
            numTarifa.DecimalPlaces = 2;
            numTarifa.Minimum = 0;
            numTarifa.Maximum = 1000;
            numTarifa.Value = 0;

            cmbRol = new ComboBox();
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;

            Ui.Fila(this, "Nombre", txtNombre, 20);
            Ui.Fila(this, "Apellidos", txtApellidos, 60);
            Ui.Fila(this, "Email", txtEmail, 100);
            if (usuario == null)
            {
                Ui.Fila(this, "Contraseña", txtPassword, 140);
            }
            Ui.Fila(this, "Tarifa (€/hora)", numTarifa, 180);
            Ui.Fila(this, "Rol", cmbRol, 220);

            Button btnGuardar = Ui.Boton("Guardar", 120);
            btnGuardar.Left = 230;
            btnGuardar.Top = 330;
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            Button btnCancelar = Ui.BotonSecundario("Cancelar", 120);
            btnCancelar.Left = 360;
            btnCancelar.Top = 330;
            btnCancelar.Click += new EventHandler(btnCancelar_Click);

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
            this.AcceptButton = btnGuardar;
        }

        private void UsuarioEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<Rol> roles = RolesApi.GetTodos();
                cmbRol.DataSource = roles;
                cmbRol.DisplayMember = "Etiqueta";
                cmbRol.ValueMember = "IdRol";

                if (usuario != null)
                {
                    txtNombre.Text = usuario.Nombre;
                    txtApellidos.Text = usuario.Apellidos;
                    txtEmail.Text = usuario.Email;
                    numTarifa.Value = usuario.TarifaHora;
                    cmbRol.SelectedValue = usuario.IdRol;
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Trim() == "" || txtApellidos.Text.Trim() == "" || txtEmail.Text.Trim() == "")
            {
                Ui.Aviso("Nombre, apellidos y email son obligatorios.");
                return;
            }
            if (txtEmail.Text.IndexOf("@") < 0)
            {
                Ui.Aviso("El email no tiene un formato correcto.");
                return;
            }
            if (usuario == null && txtPassword.Text.Length < 6)
            {
                Ui.Aviso("La contraseña debe tener al menos 6 caracteres.");
                return;
            }
            if (cmbRol.SelectedValue == null)
            {
                Ui.Aviso("Hay que elegir un rol.");
                return;
            }

            int idRol = (int)cmbRol.SelectedValue;

            try
            {
                if (usuario == null)
                {
                    UsuariosApi.Insert(txtNombre.Text.Trim(), txtApellidos.Text.Trim(), txtEmail.Text.Trim(),
                                       txtPassword.Text, numTarifa.Value, idRol);
                }
                else
                {
                    UsuariosApi.Update(usuario.IdUsuario, txtNombre.Text.Trim(), txtApellidos.Text.Trim(),
                                       txtEmail.Text.Trim(), numTarifa.Value, idRol);
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

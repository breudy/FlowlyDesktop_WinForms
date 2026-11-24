using System;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Ventana pequeña para cambiar la contraseña de un usuario.
    public class PasswordForm : Form
    {
        private Usuario usuario;
        private TextBox txtPassword;
        private TextBox txtRepetir;

        // Esta parte es de información externa que no sale en los vídeos
        // (la ventana se monta por código en vez de con el diseñador)
        public PasswordForm(Usuario usuario)
        {
            this.usuario = usuario;
            Ui.ConfigurarDialogo(this, "Cambiar contraseña", 500, 230);

            Label quien = new Label();
            quien.Text = "Usuario: " + usuario.NombreCompleto;
            quien.AutoSize = true;
            quien.Left = 20;
            quien.Top = 20;
            quien.Font = Ui.FuenteNegrita;

            txtPassword = new TextBox();
            txtPassword.UseSystemPasswordChar = true;
            txtRepetir = new TextBox();
            txtRepetir.UseSystemPasswordChar = true;

            Ui.Fila(this, "Nueva contraseña", txtPassword, 60);
            Ui.Fila(this, "Repetir contraseña", txtRepetir, 100);

            Button btnGuardar = Ui.Boton("Guardar", 120);
            btnGuardar.Left = 230;
            btnGuardar.Top = 165;
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            Button btnCancelar = Ui.BotonSecundario("Cancelar", 120);
            btnCancelar.Left = 360;
            btnCancelar.Top = 165;
            btnCancelar.Click += new EventHandler(btnCancelar_Click);

            this.Controls.Add(quien);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
            this.AcceptButton = btnGuardar;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length < 6)
            {
                Ui.Aviso("La contraseña debe tener al menos 6 caracteres.");
                return;
            }
            if (txtPassword.Text != txtRepetir.Text)
            {
                Ui.Aviso("Las dos contraseñas no coinciden.");
                return;
            }

            try
            {
                UsuariosApi.CambiarPassword(usuario.IdUsuario, txtPassword.Text);
                Ui.Aviso("Contraseña cambiada.");
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

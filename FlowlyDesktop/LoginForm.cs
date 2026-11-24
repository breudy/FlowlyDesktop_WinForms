using System;
using System.Drawing;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Pantalla de login. Solo deja entrar a los administradores (la app de escritorio es de administración).
    public class LoginForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnEntrar;
        private Label lblError;

        public LoginForm()
        {
            Ui.ConfigurarDialogo(this, "Flowly · Iniciar sesión", 420, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            Construir();
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            Label titulo = new Label();
            titulo.Text = "Flowly";
            titulo.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            titulo.ForeColor = Ui.ColorPrincipal;
            titulo.AutoSize = true;
            titulo.Left = 40;
            titulo.Top = 24;

            Label subtitulo = new Label();
            subtitulo.Text = "Panel de administración";
            subtitulo.ForeColor = Color.FromArgb(100, 116, 139);
            subtitulo.AutoSize = true;
            subtitulo.Left = 44;
            subtitulo.Top = 78;

            Label lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.AutoSize = true;
            lblEmail.Left = 44;
            lblEmail.Top = 120;

            txtEmail = new TextBox();
            txtEmail.Left = 44;
            txtEmail.Top = 142;
            txtEmail.Width = 332;

            Label lblPassword = new Label();
            lblPassword.Text = "Contraseña";
            lblPassword.AutoSize = true;
            lblPassword.Left = 44;
            lblPassword.Top = 184;

            txtPassword = new TextBox();
            txtPassword.Left = 44;
            txtPassword.Top = 206;
            txtPassword.Width = 332;
            txtPassword.UseSystemPasswordChar = true;

            lblError = new Label();
            lblError.ForeColor = Ui.ColorPeligro;
            lblError.Left = 44;
            lblError.Top = 244;
            lblError.Width = 332;
            lblError.Height = 40;

            btnEntrar = Ui.Boton("Entrar", 332);
            btnEntrar.Left = 44;
            btnEntrar.Top = 296;
            btnEntrar.Height = 40;
            btnEntrar.Click += new EventHandler(btnEntrar_Click);

            this.Controls.Add(titulo);
            this.Controls.Add(subtitulo);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblError);
            this.Controls.Add(btnEntrar);

            // Pulsar Intro equivale a pulsar "Entrar".
            this.AcceptButton = btnEntrar;
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (txtEmail.Text.Trim() == "" || txtPassword.Text == "")
            {
                lblError.Text = "Escribe el email y la contraseña.";
                return;
            }

            try
            {
                AuthApi.Login(txtEmail.Text, txtPassword.Text);

                if (!AuthApi.EsAdministrador())
                {
                    AuthApi.Logout();
                    lblError.Text = "Esta aplicación es solo para administradores. Usa la app móvil.";
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }
}

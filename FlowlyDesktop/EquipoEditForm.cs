using System;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Crear equipo / editar equipo (null = equipo nuevo).
    public class EquipoEditForm : Form
    {
        private Equipo equipo;
        private TextBox txtNombre;
        private TextBox txtDescripcion;

        // Esta parte es de información externa que no sale en los vídeos
        // (la ventana se monta por código en vez de con el diseñador)
        public EquipoEditForm(Equipo equipo)
        {
            this.equipo = equipo;

            string titulo = "Crear equipo";
            if (equipo != null)
            {
                titulo = "Editar equipo";
            }
            Ui.ConfigurarDialogo(this, titulo, 500, 270);

            txtNombre = new TextBox();
            txtDescripcion = new TextBox();
            txtDescripcion.Multiline = true;
            txtDescripcion.Height = 80;

            if (equipo != null)
            {
                txtNombre.Text = equipo.Nombre;
                txtDescripcion.Text = equipo.Descripcion;
            }

            Ui.Fila(this, "Nombre", txtNombre, 20);
            Ui.Fila(this, "Descripción", txtDescripcion, 60);

            Button btnGuardar = Ui.Boton("Guardar", 120);
            btnGuardar.Left = 230;
            btnGuardar.Top = 205;
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            Button btnCancelar = Ui.BotonSecundario("Cancelar", 120);
            btnCancelar.Left = 360;
            btnCancelar.Top = 205;
            btnCancelar.Click += new EventHandler(btnCancelar_Click);

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Trim() == "")
            {
                Ui.Aviso("El nombre del equipo es obligatorio.");
                return;
            }

            try
            {
                if (equipo == null)
                {
                    EquiposApi.Insert(txtNombre.Text.Trim(), txtDescripcion.Text.Trim());
                }
                else
                {
                    EquiposApi.Update(equipo.IdEquipo, txtNombre.Text.Trim(), txtDescripcion.Text.Trim());
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

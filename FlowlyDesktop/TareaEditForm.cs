using System;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Crear tarea / subtarea (tarea == null) o editar una tarea existente.
    public class TareaEditForm : Form
    {
        private int idProyecto;
        private int? idPadre;
        private Tarea tarea;

        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private CheckBox chkFecha;
        private DateTimePicker dtpFecha;
        private NumericUpDown numHoras;
        private ComboBox cmbEstado;

        public TareaEditForm(int idProyecto, int? idPadre, Tarea tarea)
        {
            this.idProyecto = idProyecto;
            this.idPadre = idPadre;
            this.tarea = tarea;

            string titulo = "Crear tarea";
            if (tarea != null)
            {
                titulo = "Editar tarea";
            }
            else if (idPadre != null)
            {
                titulo = "Crear subtarea";
            }
            Ui.ConfigurarDialogo(this, titulo, 500, 420);
            Construir();
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            txtNombre = new TextBox();
            txtDescripcion = new TextBox();
            txtDescripcion.Multiline = true;
            txtDescripcion.Height = 80;

            chkFecha = new CheckBox();
            chkFecha.Text = "Tiene fecha límite";
            chkFecha.CheckedChanged += new EventHandler(chkFecha_CheckedChanged);

            dtpFecha = new DateTimePicker();
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Enabled = false;

            numHoras = new NumericUpDown();
            numHoras.DecimalPlaces = 1;
            numHoras.Minimum = 0;
            numHoras.Maximum = 10000;
            numHoras.Increment = 0.5M;

            cmbEstado = new ComboBox();
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            for (int i = 0; i < EstadosTarea.TODOS.Length; i++)
            {
                cmbEstado.Items.Add(EstadosTarea.Texto(EstadosTarea.TODOS[i]));
            }
            cmbEstado.SelectedIndex = 0;

            Ui.Fila(this, "Nombre", txtNombre, 20);
            Ui.Fila(this, "Descripción", txtDescripcion, 60);
            Ui.Fila(this, "", chkFecha, 160);
            Ui.Fila(this, "Fecha límite", dtpFecha, 195);
            Ui.Fila(this, "Horas estimadas", numHoras, 240);

            // El estado solo se puede cambiar al editar (una tarea nueva siempre empieza en Pendiente).
            if (tarea != null)
            {
                Ui.Fila(this, "Estado", cmbEstado, 285);

                txtNombre.Text = tarea.Nombre;
                txtDescripcion.Text = tarea.Descripcion;
                numHoras.Value = tarea.HorasEstimadas;
                if (tarea.FechaLimite != null)
                {
                    chkFecha.Checked = true;
                    dtpFecha.Value = tarea.FechaLimite.Value;
                }
                for (int i = 0; i < EstadosTarea.TODOS.Length; i++)
                {
                    if (EstadosTarea.TODOS[i] == tarea.Estado)
                    {
                        cmbEstado.SelectedIndex = i;
                    }
                }
            }

            Button btnGuardar = Ui.Boton("Guardar", 120);
            btnGuardar.Left = 230;
            btnGuardar.Top = 350;
            btnGuardar.Click += new EventHandler(btnGuardar_Click);

            Button btnCancelar = Ui.BotonSecundario("Cancelar", 120);
            btnCancelar.Left = 360;
            btnCancelar.Top = 350;
            btnCancelar.Click += new EventHandler(btnCancelar_Click);

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private void chkFecha_CheckedChanged(object sender, EventArgs e)
        {
            dtpFecha.Enabled = chkFecha.Checked;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Trim() == "")
            {
                Ui.Aviso("El nombre de la tarea es obligatorio.");
                return;
            }

            DateTime? fecha = null;
            if (chkFecha.Checked)
            {
                fecha = dtpFecha.Value.Date;
            }

            try
            {
                if (tarea == null)
                {
                    TareasApi.Insert(idProyecto, idPadre, txtNombre.Text.Trim(), txtDescripcion.Text.Trim(),
                                     fecha, numHoras.Value);
                }
                else
                {
                    string estado = EstadosTarea.TODOS[cmbEstado.SelectedIndex];
                    TareasApi.Update(tarea.IdTarea, txtNombre.Text.Trim(), txtDescripcion.Text.Trim(),
                                     fecha, numHoras.Value, estado);
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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Crear proyecto / editar proyecto (si se le pasa null es un proyecto nuevo).
    public class ProyectoEditForm : Form
    {
        private Proyecto proyecto;

        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private DateTimePicker dtpInicio;
        private CheckBox chkFin;
        private DateTimePicker dtpFin;
        private ComboBox cmbEquipo;

        public ProyectoEditForm(Proyecto proyecto)
        {
            this.proyecto = proyecto;

            string titulo = "Crear proyecto";
            if (proyecto != null)
            {
                titulo = "Editar proyecto";
            }
            Ui.ConfigurarDialogo(this, titulo, 500, 420);
            Construir();
            this.Load += new EventHandler(ProyectoEditForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            txtNombre = new TextBox();
            txtDescripcion = new TextBox();
            txtDescripcion.Multiline = true;
            txtDescripcion.Height = 80;
            txtDescripcion.ScrollBars = ScrollBars.Vertical;

            dtpInicio = new DateTimePicker();
            dtpInicio.Format = DateTimePickerFormat.Short;

            chkFin = new CheckBox();
            chkFin.Text = "Tiene fecha de fin";
            chkFin.CheckedChanged += new EventHandler(chkFin_CheckedChanged);

            dtpFin = new DateTimePicker();
            dtpFin.Format = DateTimePickerFormat.Short;
            dtpFin.Enabled = false;

            cmbEquipo = new ComboBox();
            cmbEquipo.DropDownStyle = ComboBoxStyle.DropDownList;

            Ui.Fila(this, "Nombre", txtNombre, 20);
            Ui.Fila(this, "Descripción", txtDescripcion, 60);
            Ui.Fila(this, "Fecha de inicio", dtpInicio, 160);
            Ui.Fila(this, "", chkFin, 200);
            Ui.Fila(this, "Fecha de fin", dtpFin, 235);
            Ui.Fila(this, "Equipo", cmbEquipo, 280);

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
            this.AcceptButton = btnGuardar;
        }

        private void ProyectoEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<Equipo> equipos = EquiposApi.GetTodos();
                cmbEquipo.DataSource = equipos;
                cmbEquipo.DisplayMember = "Nombre";
                cmbEquipo.ValueMember = "IdEquipo";

                if (proyecto != null)
                {
                    txtNombre.Text = proyecto.Nombre;
                    txtDescripcion.Text = proyecto.Descripcion;
                    dtpInicio.Value = proyecto.FechaInicio;
                    if (proyecto.FechaFin != null)
                    {
                        chkFin.Checked = true;
                        dtpFin.Value = proyecto.FechaFin.Value;
                    }
                    cmbEquipo.SelectedValue = proyecto.IdEquipo;
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void chkFin_CheckedChanged(object sender, EventArgs e)
        {
            dtpFin.Enabled = chkFin.Checked;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Trim() == "")
            {
                Ui.Aviso("El nombre del proyecto es obligatorio.");
                return;
            }
            if (cmbEquipo.SelectedValue == null)
            {
                Ui.Aviso("Hay que elegir un equipo. Si no hay ninguno, créalo antes en la pantalla Equipos.");
                return;
            }

            DateTime? fin = null;
            if (chkFin.Checked)
            {
                fin = dtpFin.Value.Date;
                if (fin.Value < dtpInicio.Value.Date)
                {
                    Ui.Aviso("La fecha de fin no puede ser anterior a la de inicio.");
                    return;
                }
            }

            int idEquipo = (int)cmbEquipo.SelectedValue;

            try
            {
                if (proyecto == null)
                {
                    ProyectosApi.Insert(txtNombre.Text.Trim(), txtDescripcion.Text.Trim(),
                                        dtpInicio.Value.Date, fin, idEquipo);
                }
                else
                {
                    ProyectosApi.Update(proyecto.IdProyecto, txtNombre.Text.Trim(), txtDescripcion.Text.Trim(),
                                        dtpInicio.Value.Date, fin, idEquipo);
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

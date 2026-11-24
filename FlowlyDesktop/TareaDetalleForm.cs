using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Detalle de una tarea: usuarios asignados (con horas) y tareas previas (dependencias).
    public class TareaDetalleForm : Form
    {
        private int idTarea;
        private int idProyecto;

        private BindingList<Asignacion> asignados = new BindingList<Asignacion>();
        private BindingList<Tarea> previas = new BindingList<Tarea>();

        private Label lblTitulo;
        private DataGridView tablaAsignados;
        private DataGridView tablaPrevias;
        private ComboBox cmbUsuarios;
        private NumericUpDown numHoras;
        private ComboBox cmbTareas;

        public TareaDetalleForm(int idTarea, int idProyecto)
        {
            this.idTarea = idTarea;
            this.idProyecto = idProyecto;

            Ui.ConfigurarPantalla(this, "Asignaciones y dependencias");
            this.ClientSize = new System.Drawing.Size(900, 640);
            this.StartPosition = FormStartPosition.CenterParent;
            Construir();
            this.Load += new EventHandler(TareaDetalleForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            lblTitulo = new Label();
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Height = 48;
            lblTitulo.Font = Ui.FuenteTitulo;
            lblTitulo.Padding = new Padding(16, 10, 0, 0);

            // ----- Mitad de arriba: asignaciones -----
            Panel arriba = new Panel();
            arriba.Dock = DockStyle.Top;
            arriba.Height = 280;

            Label lblA = new Label();
            lblA.Text = "Usuarios asignados";
            lblA.Dock = DockStyle.Top;
            lblA.Height = 30;
            lblA.Font = Ui.FuenteNegrita;
            lblA.Padding = new Padding(16, 8, 0, 0);

            FlowLayoutPanel barraA = Ui.BarraBotones();
            cmbUsuarios = new ComboBox();
            cmbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUsuarios.Width = 220;
            cmbUsuarios.Margin = new Padding(0, 3, 8, 0);

            Label lblH = new Label();
            lblH.Text = "Horas:";
            lblH.AutoSize = true;
            lblH.Margin = new Padding(0, 6, 4, 0);

            numHoras = new NumericUpDown();
            numHoras.DecimalPlaces = 1;
            numHoras.Minimum = 0;
            numHoras.Maximum = 10000;
            numHoras.Increment = 0.5M;
            numHoras.Width = 70;
            numHoras.Margin = new Padding(0, 3, 8, 0);

            Button btnAsignar = Ui.Boton("Asignar", 80);
            btnAsignar.Click += new EventHandler(btnAsignar_Click);
            Button btnHoras = Ui.BotonSecundario("Cambiar horas", 120);
            btnHoras.Click += new EventHandler(btnHoras_Click);
            Button btnQuitarUsuario = Ui.BotonSecundario("Quitar", 80);
            btnQuitarUsuario.Click += new EventHandler(btnQuitarUsuario_Click);

            barraA.Controls.Add(cmbUsuarios);
            barraA.Controls.Add(lblH);
            barraA.Controls.Add(numHoras);
            barraA.Controls.Add(btnAsignar);
            barraA.Controls.Add(btnHoras);
            barraA.Controls.Add(btnQuitarUsuario);

            tablaAsignados = Ui.Tabla();
            tablaAsignados.Dock = DockStyle.Fill;
            Ui.Col(tablaAsignados, "Usuario", "Usuario", 60);
            Ui.Col(tablaAsignados, "Horas asignadas", "HorasAsignadas", 30, "N1");
            tablaAsignados.DataSource = asignados;

            arriba.Controls.Add(tablaAsignados);
            arriba.Controls.Add(barraA);
            arriba.Controls.Add(lblA);

            // ----- Mitad de abajo: dependencias -----
            Panel abajo = new Panel();
            abajo.Dock = DockStyle.Fill;

            Label lblD = new Label();
            lblD.Text = "Tareas que deben terminar antes (dependencias)";
            lblD.Dock = DockStyle.Top;
            lblD.Height = 30;
            lblD.Font = Ui.FuenteNegrita;
            lblD.Padding = new Padding(16, 8, 0, 0);

            FlowLayoutPanel barraD = Ui.BarraBotones();
            cmbTareas = new ComboBox();
            cmbTareas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTareas.Width = 300;
            cmbTareas.Margin = new Padding(0, 3, 8, 0);
            Button btnAnadirPrevia = Ui.Boton("Añadir", 80);
            btnAnadirPrevia.Click += new EventHandler(btnAnadirPrevia_Click);
            Button btnQuitarPrevia = Ui.BotonSecundario("Quitar", 80);
            btnQuitarPrevia.Click += new EventHandler(btnQuitarPrevia_Click);
            barraD.Controls.Add(cmbTareas);
            barraD.Controls.Add(btnAnadirPrevia);
            barraD.Controls.Add(btnQuitarPrevia);

            tablaPrevias = Ui.Tabla();
            tablaPrevias.Dock = DockStyle.Fill;
            Ui.Col(tablaPrevias, "Tarea previa", "Nombre", 60);
            Ui.Col(tablaPrevias, "Estado", "EstadoTexto", 20);
            tablaPrevias.DataSource = previas;

            abajo.Controls.Add(tablaPrevias);
            abajo.Controls.Add(barraD);
            abajo.Controls.Add(lblD);

            this.Controls.Add(abajo);
            this.Controls.Add(arriba);
            this.Controls.Add(lblTitulo);
        }

        private void TareaDetalleForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<Usuario> usuarios = UsuariosApi.GetTodos();
                cmbUsuarios.DataSource = usuarios;
                cmbUsuarios.DisplayMember = "NombreCompleto";
                cmbUsuarios.ValueMember = "IdUsuario";

                // Las demás tareas del proyecto (menos esta) son las candidatas a ser previas.
                Proyecto p = ProyectosApi.GetPorId(idProyecto);
                List<Tarea> otras = new List<Tarea>();
                if (p.Tareas != null)
                {
                    for (int i = 0; i < p.Tareas.Count; i++)
                    {
                        if (p.Tareas[i].IdTarea != idTarea)
                        {
                            otras.Add(p.Tareas[i]);
                        }
                    }
                }
                cmbTareas.DataSource = otras;
                cmbTareas.DisplayMember = "Nombre";
                cmbTareas.ValueMember = "IdTarea";
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                Tarea t = TareasApi.GetPorId(idTarea);
                lblTitulo.Text = t.Nombre;

                asignados.Clear();
                if (t.Asignados != null)
                {
                    for (int i = 0; i < t.Asignados.Count; i++)
                    {
                        asignados.Add(t.Asignados[i]);
                    }
                }

                previas.Clear();
                if (t.TareasPrevias != null)
                {
                    for (int i = 0; i < t.TareasPrevias.Count; i++)
                    {
                        previas.Add(t.TareasPrevias[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            if (cmbUsuarios.SelectedValue == null)
            {
                return;
            }

            try
            {
                TareasApi.Asignar(idTarea, (int)cmbUsuarios.SelectedValue, numHoras.Value);
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnHoras_Click(object sender, EventArgs e)
        {
            Asignacion a = Ui.Seleccionado(tablaAsignados) as Asignacion;
            if (a == null)
            {
                Ui.Aviso("Selecciona primero un usuario asignado.");
                return;
            }

            try
            {
                TareasApi.CambiarHorasAsignadas(idTarea, a.IdUsuario, numHoras.Value);
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnQuitarUsuario_Click(object sender, EventArgs e)
        {
            Asignacion a = Ui.Seleccionado(tablaAsignados) as Asignacion;
            if (a == null)
            {
                Ui.Aviso("Selecciona primero un usuario asignado.");
                return;
            }

            try
            {
                TareasApi.QuitarAsignacion(idTarea, a.IdUsuario);
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnAnadirPrevia_Click(object sender, EventArgs e)
        {
            if (cmbTareas.SelectedValue == null)
            {
                return;
            }

            try
            {
                TareasApi.AnadirDependencia(idTarea, (int)cmbTareas.SelectedValue);
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnQuitarPrevia_Click(object sender, EventArgs e)
        {
            Tarea previa = Ui.Seleccionado(tablaPrevias) as Tarea;
            if (previa == null)
            {
                Ui.Aviso("Selecciona primero una tarea previa.");
                return;
            }

            try
            {
                TareasApi.QuitarDependencia(idTarea, previa.IdTarea);
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }
    }
}

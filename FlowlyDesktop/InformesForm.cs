using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Informes de un proyecto: resumen con barras por estado, costes por persona y tareas críticas.
    // Si se crea con idProyecto = 0 se empieza sin proyecto elegido.
    public class InformesForm : Form
    {
        private int idInicial;

        private BindingList<ResumenTarea> filasResumen = new BindingList<ResumenTarea>();
        private BindingList<CosteUsuario> filasCostes = new BindingList<CosteUsuario>();
        private BindingList<TareaCritica> filasCriticas = new BindingList<TareaCritica>();

        private ComboBox cmbProyecto;
        private Label lblAvance;
        private Label lblHoras;
        private Label lblTotalCoste;
        private ProgressBar barraPendientes;
        private ProgressBar barraEnCurso;
        private ProgressBar barraFinalizadas;
        private Label lblPendientes;
        private Label lblEnCurso;
        private Label lblFinalizadas;
        private DataGridView tablaResumen;
        private DataGridView tablaCostes;
        private DataGridView tablaCriticas;

        public InformesForm(int idProyecto)
        {
            this.idInicial = idProyecto;
            Ui.ConfigurarPantalla(this, "Informes");
            this.StartPosition = FormStartPosition.CenterParent;
            Construir();
            this.Load += new EventHandler(InformesForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            // ----- Selector de proyecto -----
            FlowLayoutPanel arriba = Ui.BarraBotones();
            Label lblProyecto = new Label();
            lblProyecto.Text = "Proyecto:";
            lblProyecto.AutoSize = true;
            lblProyecto.Margin = new Padding(0, 6, 6, 0);

            cmbProyecto = new ComboBox();
            cmbProyecto.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProyecto.Width = 320;
            cmbProyecto.Margin = new Padding(0, 3, 8, 0);

            Button btnGenerar = Ui.Boton("Generar informe", 150);
            btnGenerar.Click += new EventHandler(btnGenerar_Click);

            arriba.Controls.Add(lblProyecto);
            arriba.Controls.Add(cmbProyecto);
            arriba.Controls.Add(btnGenerar);

            // ----- Pestañas -----
            TabControl pestanas = new TabControl();
            pestanas.Dock = DockStyle.Fill;
            pestanas.Font = Ui.Fuente;

            TabPage pagResumen = new TabPage("Resumen");
            TabPage pagCostes = new TabPage("Costes");
            TabPage pagCriticas = new TabPage("Tareas críticas");
            pagResumen.BackColor = Ui.ColorFondo;
            pagCostes.BackColor = Ui.ColorFondo;
            pagCriticas.BackColor = Ui.ColorFondo;

            // --- Resumen ---
            Panel cabeceraResumen = new Panel();
            cabeceraResumen.Dock = DockStyle.Top;
            cabeceraResumen.Height = 150;
            cabeceraResumen.BackColor = System.Drawing.Color.White;

            lblAvance = new Label();
            lblAvance.Left = 16;
            lblAvance.Top = 10;
            lblAvance.AutoSize = true;
            lblAvance.Font = Ui.FuenteNegrita;
            lblAvance.Text = "Elige un proyecto y pulsa Generar informe.";

            lblHoras = new Label();
            lblHoras.Left = 16;
            lblHoras.Top = 36;
            lblHoras.AutoSize = true;

            lblPendientes = new Label();
            lblEnCurso = new Label();
            lblFinalizadas = new Label();
            barraPendientes = CrearBarra(cabeceraResumen, lblPendientes, "Pendientes", 66);
            barraEnCurso = CrearBarra(cabeceraResumen, lblEnCurso, "En curso", 92);
            barraFinalizadas = CrearBarra(cabeceraResumen, lblFinalizadas, "Finalizadas", 118);

            cabeceraResumen.Controls.Add(lblAvance);
            cabeceraResumen.Controls.Add(lblHoras);

            tablaResumen = Ui.Tabla();
            tablaResumen.Dock = DockStyle.Fill;
            Ui.Col(tablaResumen, "Tarea", "Tarea", 40);
            Ui.Col(tablaResumen, "Estado", "EstadoTexto", 15);
            Ui.Col(tablaResumen, "Fecha límite", "FechaLimite", 15, "dd/MM/yyyy");
            Ui.Col(tablaResumen, "Horas est.", "HorasEstimadas", 12, "N1");
            Ui.Col(tablaResumen, "Horas reg.", "HorasRegistradas", 12, "N1");
            Ui.Col(tablaResumen, "Desviación", "Desviacion", 12, "N1");
            tablaResumen.DataSource = filasResumen;

            pagResumen.Controls.Add(tablaResumen);
            pagResumen.Controls.Add(cabeceraResumen);

            // --- Costes ---
            lblTotalCoste = new Label();
            lblTotalCoste.Dock = DockStyle.Top;
            lblTotalCoste.Height = 44;
            lblTotalCoste.Font = Ui.FuenteNegrita;
            lblTotalCoste.Padding = new Padding(16, 12, 0, 0);

            tablaCostes = Ui.Tabla();
            tablaCostes.Dock = DockStyle.Fill;
            Ui.Col(tablaCostes, "Persona", "Usuario", 40);
            Ui.Col(tablaCostes, "Horas", "HorasTexto", 20);
            Ui.Col(tablaCostes, "Tarifa", "TarifaTexto", 20);
            Ui.Col(tablaCostes, "Coste", "CosteTexto", 20);
            tablaCostes.DataSource = filasCostes;

            pagCostes.Controls.Add(tablaCostes);
            pagCostes.Controls.Add(lblTotalCoste);

            // --- Tareas críticas ---
            tablaCriticas = Ui.Tabla();
            tablaCriticas.Dock = DockStyle.Fill;
            Ui.Col(tablaCriticas, "Tarea", "Tarea", 50);
            Ui.Col(tablaCriticas, "Estado", "EstadoTexto", 20);
            Ui.Col(tablaCriticas, "Tareas que bloquea", "TareasBloqueadas", 20);
            tablaCriticas.DataSource = filasCriticas;
            pagCriticas.Controls.Add(tablaCriticas);

            pestanas.TabPages.Add(pagResumen);
            pestanas.TabPages.Add(pagCostes);
            pestanas.TabPages.Add(pagCriticas);

            this.Controls.Add(pestanas);
            this.Controls.Add(arriba);
        }

        // Crea una fila "etiqueta + barra de progreso + número" dentro del panel.
        private ProgressBar CrearBarra(Panel panel, Label numero, string texto, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.Left = 16;
            etiqueta.Top = y + 2;
            etiqueta.Width = 90;

            ProgressBar barra = new ProgressBar();
            barra.Left = 110;
            barra.Top = y;
            barra.Width = 400;
            barra.Height = 20;
            barra.Minimum = 0;
            barra.Maximum = 100;

            numero.Left = 520;
            numero.Top = y + 2;
            numero.AutoSize = true;
            numero.Text = "0";

            panel.Controls.Add(etiqueta);
            panel.Controls.Add(barra);
            panel.Controls.Add(numero);
            return barra;
        }

        private void InformesForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<Proyecto> proyectos = ProyectosApi.GetTodos();
                cmbProyecto.DataSource = proyectos;
                cmbProyecto.DisplayMember = "Nombre";
                cmbProyecto.ValueMember = "IdProyecto";

                if (idInicial != 0)
                {
                    cmbProyecto.SelectedValue = idInicial;
                    Generar();
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Generar();
        }

        private void Generar()
        {
            if (cmbProyecto.SelectedValue == null)
            {
                Ui.Aviso("Elige primero un proyecto.");
                return;
            }
            int id = (int)cmbProyecto.SelectedValue;

            try
            {
                InformeResumen resumen = InformesApi.GetResumen(id);
                InformeCostes costes = InformesApi.GetCostes(id);
                List<TareaCritica> criticas = InformesApi.GetTareasCriticas(id);

                // Resumen
                lblAvance.Text = resumen.Proyecto + " · " + resumen.AvanceTexto;
                lblHoras.Text = "Horas estimadas: " + resumen.HorasEstimadas.ToString("N1")
                              + "   ·   Horas registradas: " + resumen.HorasRegistradas.ToString("N1");

                PonerBarra(barraPendientes, lblPendientes, resumen.Pendientes, resumen.TotalTareas);
                PonerBarra(barraEnCurso, lblEnCurso, resumen.EnCurso, resumen.TotalTareas);
                PonerBarra(barraFinalizadas, lblFinalizadas, resumen.Finalizadas, resumen.TotalTareas);

                filasResumen.Clear();
                if (resumen.Tareas != null)
                {
                    for (int i = 0; i < resumen.Tareas.Count; i++)
                    {
                        filasResumen.Add(resumen.Tareas[i]);
                    }
                }

                // Costes
                lblTotalCoste.Text = "Total: " + costes.TotalHoras.ToString("N1") + " horas  ·  "
                                   + costes.TotalCoste.ToString("N2") + " €";
                filasCostes.Clear();
                if (costes.Personas != null)
                {
                    for (int i = 0; i < costes.Personas.Count; i++)
                    {
                        filasCostes.Add(costes.Personas[i]);
                    }
                }

                // Tareas críticas
                filasCriticas.Clear();
                for (int i = 0; i < criticas.Count; i++)
                {
                    filasCriticas.Add(criticas[i]);
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        // Rellena una barra con el porcentaje que representa "cantidad" sobre "total".
        private void PonerBarra(ProgressBar barra, Label numero, int cantidad, int total)
        {
            int porcentaje = 0;
            if (total > 0)
            {
                porcentaje = cantidad * 100 / total;
            }
            barra.Value = porcentaje;
            numero.Text = cantidad + " de " + total;
        }
    }
}

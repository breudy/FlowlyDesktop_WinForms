using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Fila que se enseña en la tabla: envuelve un Proyecto y añade el estado calculado por las fechas.
    public class FilaProyecto
    {
        public Proyecto Datos;

        public FilaProyecto(Proyecto datos)
        {
            this.Datos = datos;
        }

        public int IdProyecto { get { return Datos.IdProyecto; } }
        public string Nombre { get { return Datos.Nombre; } }
        public string Equipo { get { return Datos.Equipo; } }
        public string Fechas { get { return Datos.FechasTexto; } }
        public int NumTareas { get { return Datos.NumTareas; } }

        public string Estado
        {
            get
            {
                DateTime hoy = DateTime.Today;
                if (Datos.FechaFin != null && Datos.FechaFin.Value.Date < hoy)
                {
                    return "Finalizado";
                }
                if (Datos.FechaInicio.Date > hoy)
                {
                    return "Planificado";
                }
                return "En curso";
            }
        }
    }

    // Dashboard de proyectos: estadísticas, buscador, filtro por estado y acciones.
    public class ProyectosForm : Form
    {
        private List<Proyecto> todos = new List<Proyecto>();
        private BindingList<FilaProyecto> filas = new BindingList<FilaProyecto>();

        private DataGridView tabla;
        private TextBox txtBuscar;
        private ComboBox cmbEstado;
        private Label lblTotal;
        private Label lblEnCurso;
        private Label lblFinalizados;
        private Label lblTareas;

        public ProyectosForm()
        {
            Ui.ConfigurarPantalla(this, "Proyectos");
            Construir();
            this.Load += new EventHandler(ProyectosForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            // ----- Estadísticas -----
            FlowLayoutPanel estadisticas = new FlowLayoutPanel();
            estadisticas.Dock = DockStyle.Top;
            estadisticas.Height = 100;
            estadisticas.Padding = new Padding(16, 12, 16, 10);
            estadisticas.Controls.Add(Ui.Tarjeta("Proyectos", out lblTotal));
            estadisticas.Controls.Add(Ui.Tarjeta("En curso", out lblEnCurso));
            estadisticas.Controls.Add(Ui.Tarjeta("Finalizados", out lblFinalizados));
            estadisticas.Controls.Add(Ui.Tarjeta("Tareas en total", out lblTareas));

            // ----- Buscador y filtro -----
            FlowLayoutPanel filtros = Ui.BarraBotones();

            txtBuscar = new TextBox();
            txtBuscar.Width = 240;
            txtBuscar.Margin = new Padding(0, 3, 12, 0);
            txtBuscar.TextChanged += new EventHandler(Filtro_Cambiado);

            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar:";
            lblBuscar.AutoSize = true;
            lblBuscar.Margin = new Padding(0, 6, 6, 0);

            cmbEstado = new ComboBox();
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.Width = 150;
            cmbEstado.Margin = new Padding(0, 3, 24, 0);
            cmbEstado.Items.Add("Todos");
            cmbEstado.Items.Add("En curso");
            cmbEstado.Items.Add("Planificado");
            cmbEstado.Items.Add("Finalizado");
            cmbEstado.SelectedIndex = 0;
            cmbEstado.SelectedIndexChanged += new EventHandler(Filtro_Cambiado);

            Button btnNuevo = Ui.Boton("+ Nuevo proyecto", 150);
            btnNuevo.Click += new EventHandler(btnNuevo_Click);
            Button btnEditar = Ui.BotonSecundario("Editar", 80);
            btnEditar.Click += new EventHandler(btnEditar_Click);
            Button btnTareas = Ui.BotonSecundario("Tareas", 80);
            btnTareas.Click += new EventHandler(btnTareas_Click);
            Button btnInforme = Ui.BotonSecundario("Informe", 80);
            btnInforme.Click += new EventHandler(btnInforme_Click);
            Button btnEliminar = Ui.BotonPeligro("Eliminar", 90);
            btnEliminar.Click += new EventHandler(btnEliminar_Click);

            filtros.Controls.Add(lblBuscar);
            filtros.Controls.Add(txtBuscar);
            filtros.Controls.Add(cmbEstado);
            filtros.Controls.Add(btnNuevo);
            filtros.Controls.Add(btnEditar);
            filtros.Controls.Add(btnTareas);
            filtros.Controls.Add(btnInforme);
            filtros.Controls.Add(btnEliminar);

            // ----- Tabla -----
            tabla = Ui.Tabla();
            tabla.Dock = DockStyle.Fill;
            Ui.Col(tabla, "Proyecto", "Nombre", 30);
            Ui.Col(tabla, "Equipo", "Equipo", 20);
            Ui.Col(tabla, "Fechas", "Fechas", 25);
            Ui.Col(tabla, "Tareas", "NumTareas", 10);
            Ui.Col(tabla, "Estado", "Estado", 15);
            tabla.DataSource = filas;
            tabla.CellDoubleClick += new DataGridViewCellEventHandler(tabla_CellDoubleClick);

            // El orden de Add importa con Dock: primero Fill, luego los Top de abajo hacia arriba.
            this.Controls.Add(tabla);
            this.Controls.Add(filtros);
            this.Controls.Add(estadisticas);
        }

        private void ProyectosForm_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                todos = ProyectosApi.GetTodos();
                Filtrar();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        // Rellena la tabla con los proyectos que cumplen el texto y el estado elegidos, y recalcula estadísticas.
        private void Filtrar()
        {
            string texto = txtBuscar.Text.Trim().ToLower();
            string estado = cmbEstado.SelectedItem.ToString();

            int enCurso = 0;
            int finalizados = 0;
            int tareas = 0;

            filas.Clear();
            for (int i = 0; i < todos.Count; i++)
            {
                FilaProyecto fila = new FilaProyecto(todos[i]);

                tareas = tareas + fila.NumTareas;
                if (fila.Estado == "En curso")
                {
                    enCurso++;
                }
                if (fila.Estado == "Finalizado")
                {
                    finalizados++;
                }

                bool coincideTexto = fila.Nombre.ToLower().IndexOf(texto) >= 0;
                bool coincideEstado = estado == "Todos" || fila.Estado == estado;
                if (coincideTexto && coincideEstado)
                {
                    filas.Add(fila);
                }
            }

            lblTotal.Text = todos.Count.ToString();
            lblEnCurso.Text = enCurso.ToString();
            lblFinalizados.Text = finalizados.ToString();
            lblTareas.Text = tareas.ToString();
        }

        private void Filtro_Cambiado(object sender, EventArgs e)
        {
            Filtrar();
        }

        private FilaProyecto ElegirProyecto()
        {
            FilaProyecto fila = Ui.Seleccionado(tabla) as FilaProyecto;
            if (fila == null)
            {
                Ui.Aviso("Selecciona primero un proyecto de la tabla.");
            }
            return fila;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            ProyectoEditForm dialogo = new ProyectoEditForm(null);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            FilaProyecto fila = ElegirProyecto();
            if (fila == null)
            {
                return;
            }

            try
            {
                Proyecto completo = ProyectosApi.GetPorId(fila.IdProyecto);
                ProyectoEditForm dialogo = new ProyectoEditForm(completo);
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    Cargar();
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnTareas_Click(object sender, EventArgs e)
        {
            FilaProyecto fila = ElegirProyecto();
            if (fila == null)
            {
                return;
            }

            TareasForm tareas = new TareasForm(fila.IdProyecto, fila.Nombre);
            tareas.ShowDialog();
            Cargar();
        }

        private void btnInforme_Click(object sender, EventArgs e)
        {
            FilaProyecto fila = ElegirProyecto();
            if (fila == null)
            {
                return;
            }

            InformesForm informe = new InformesForm(fila.IdProyecto);
            informe.ShowDialog();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            FilaProyecto fila = ElegirProyecto();
            if (fila == null)
            {
                return;
            }

            if (!Ui.Confirmar("¿Eliminar el proyecto \"" + fila.Nombre + "\" y todas sus tareas?"))
            {
                return;
            }

            try
            {
                ProyectosApi.Delete(fila.IdProyecto);
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
                btnTareas_Click(sender, e);
            }
        }
    }
}

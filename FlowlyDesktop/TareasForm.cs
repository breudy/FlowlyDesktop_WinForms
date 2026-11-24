using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Gestión de tareas de un proyecto: crear, subtareas, editar, eliminar y abrir asignaciones / dependencias.
    public class TareasForm : Form
    {
        private int idProyecto;
        private string nombreProyecto;
        private BindingList<Tarea> tareas = new BindingList<Tarea>();
        private DataGridView tabla;

        public TareasForm(int idProyecto, string nombreProyecto)
        {
            this.idProyecto = idProyecto;
            this.nombreProyecto = nombreProyecto;

            Ui.ConfigurarPantalla(this, "Tareas · " + nombreProyecto);
            this.StartPosition = FormStartPosition.CenterParent;
            Construir();
            this.Load += new EventHandler(TareasForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            FlowLayoutPanel barra = Ui.BarraBotones();
            Button btnNueva = Ui.Boton("+ Nueva tarea", 130);
            btnNueva.Click += new EventHandler(btnNueva_Click);
            Button btnSubtarea = Ui.BotonSecundario("+ Subtarea", 100);
            btnSubtarea.Click += new EventHandler(btnSubtarea_Click);
            Button btnEditar = Ui.BotonSecundario("Editar", 80);
            btnEditar.Click += new EventHandler(btnEditar_Click);
            Button btnDetalle = Ui.BotonSecundario("Asignaciones y dependencias", 220);
            btnDetalle.Click += new EventHandler(btnDetalle_Click);
            Button btnEliminar = Ui.BotonPeligro("Eliminar", 90);
            btnEliminar.Click += new EventHandler(btnEliminar_Click);

            barra.Controls.Add(btnNueva);
            barra.Controls.Add(btnSubtarea);
            barra.Controls.Add(btnEditar);
            barra.Controls.Add(btnDetalle);
            barra.Controls.Add(btnEliminar);

            tabla = Ui.Tabla();
            tabla.Dock = DockStyle.Fill;
            Ui.Col(tabla, "Tarea", "NombreConNivel", 40);
            Ui.Col(tabla, "Estado", "EstadoTexto", 15);
            Ui.Col(tabla, "Fecha límite", "FechaLimiteTexto", 15);
            Ui.Col(tabla, "Horas estimadas", "HorasEstimadas", 15, "N1");
            tabla.DataSource = tareas;
            tabla.CellDoubleClick += new DataGridViewCellEventHandler(tabla_CellDoubleClick);

            this.Controls.Add(tabla);
            this.Controls.Add(barra);
        }

        private void TareasForm_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                Proyecto p = ProyectosApi.GetPorId(idProyecto);
                tareas.Clear();
                if (p.Tareas != null)
                {
                    // Primero las tareas principales y, justo debajo de cada una, sus subtareas.
                    for (int i = 0; i < p.Tareas.Count; i++)
                    {
                        if (p.Tareas[i].IdTareaPadre == null)
                        {
                            tareas.Add(p.Tareas[i]);
                            for (int j = 0; j < p.Tareas.Count; j++)
                            {
                                if (p.Tareas[j].IdTareaPadre == p.Tareas[i].IdTarea)
                                {
                                    tareas.Add(p.Tareas[j]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private Tarea ElegirTarea()
        {
            Tarea t = Ui.Seleccionado(tabla) as Tarea;
            if (t == null)
            {
                Ui.Aviso("Selecciona primero una tarea de la tabla.");
            }
            return t;
        }

        private void btnNueva_Click(object sender, EventArgs e)
        {
            TareaEditForm dialogo = new TareaEditForm(idProyecto, null, null);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void btnSubtarea_Click(object sender, EventArgs e)
        {
            Tarea padre = ElegirTarea();
            if (padre == null)
            {
                return;
            }
            if (padre.IdTareaPadre != null)
            {
                Ui.Aviso("Una subtarea no puede tener más subtareas. Elige la tarea principal.");
                return;
            }

            TareaEditForm dialogo = new TareaEditForm(idProyecto, padre.IdTarea, null);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Tarea t = ElegirTarea();
            if (t == null)
            {
                return;
            }

            TareaEditForm dialogo = new TareaEditForm(idProyecto, null, t);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                Cargar();
            }
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            Tarea t = ElegirTarea();
            if (t == null)
            {
                return;
            }

            TareaDetalleForm dialogo = new TareaDetalleForm(t.IdTarea, idProyecto);
            dialogo.ShowDialog();
            Cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Tarea t = ElegirTarea();
            if (t == null)
            {
                return;
            }

            if (!Ui.Confirmar("¿Eliminar la tarea \"" + t.Nombre + "\" (y sus subtareas)?"))
            {
                return;
            }

            try
            {
                TareasApi.Delete(t.IdTarea);
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
                btnDetalle_Click(sender, e);
            }
        }
    }
}

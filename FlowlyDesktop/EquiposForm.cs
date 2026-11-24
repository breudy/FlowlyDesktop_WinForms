using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FlowlyDesktop.Models;

namespace FlowlyDesktop
{
    // Equipos: lista a la izquierda y miembros del equipo seleccionado a la derecha.
    public class EquiposForm : Form
    {
        private BindingList<Equipo> equipos = new BindingList<Equipo>();
        private BindingList<Usuario> miembros = new BindingList<Usuario>();

        private DataGridView tablaEquipos;
        private DataGridView tablaMiembros;
        private ComboBox cmbUsuarios;
        private Label lblMiembros;

        public EquiposForm()
        {
            Ui.ConfigurarPantalla(this, "Equipos");
            Construir();
            this.Load += new EventHandler(EquiposForm_Load);
        }

        // Esta parte es de información externa que no sale en los vídeos
        // (la pantalla se monta por código en vez de con el diseñador)
        private void Construir()
        {
            // ----- Parte izquierda: equipos -----
            Panel izquierda = new Panel();
            izquierda.Dock = DockStyle.Left;
            izquierda.Width = 520;

            FlowLayoutPanel barraEquipos = Ui.BarraBotones();
            Button btnNuevo = Ui.Boton("+ Nuevo equipo", 130);
            btnNuevo.Click += new EventHandler(btnNuevo_Click);
            Button btnEditar = Ui.BotonSecundario("Editar", 80);
            btnEditar.Click += new EventHandler(btnEditar_Click);
            Button btnEliminar = Ui.BotonPeligro("Eliminar", 90);
            btnEliminar.Click += new EventHandler(btnEliminar_Click);
            barraEquipos.Controls.Add(btnNuevo);
            barraEquipos.Controls.Add(btnEditar);
            barraEquipos.Controls.Add(btnEliminar);

            tablaEquipos = Ui.Tabla();
            tablaEquipos.Dock = DockStyle.Fill;
            Ui.Col(tablaEquipos, "Equipo", "Nombre", 40);
            Ui.Col(tablaEquipos, "Miembros", "NumMiembros", 20);
            Ui.Col(tablaEquipos, "Proyectos", "NumProyectos", 20);
            tablaEquipos.DataSource = equipos;
            tablaEquipos.SelectionChanged += new EventHandler(tablaEquipos_SelectionChanged);

            izquierda.Controls.Add(tablaEquipos);
            izquierda.Controls.Add(barraEquipos);

            // ----- Parte derecha: miembros -----
            Panel derecha = new Panel();
            derecha.Dock = DockStyle.Fill;

            lblMiembros = new Label();
            lblMiembros.Dock = DockStyle.Top;
            lblMiembros.Height = 40;
            lblMiembros.Font = Ui.FuenteNegrita;
            lblMiembros.Padding = new Padding(16, 12, 0, 0);
            lblMiembros.Text = "Miembros";

            FlowLayoutPanel barraMiembros = Ui.BarraBotones();
            cmbUsuarios = new ComboBox();
            cmbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUsuarios.Width = 250;
            cmbUsuarios.Margin = new Padding(0, 3, 8, 0);
            Button btnAnadir = Ui.Boton("Añadir", 90);
            btnAnadir.Click += new EventHandler(btnAnadir_Click);
            Button btnQuitar = Ui.BotonSecundario("Quitar", 90);
            btnQuitar.Click += new EventHandler(btnQuitar_Click);
            barraMiembros.Controls.Add(cmbUsuarios);
            barraMiembros.Controls.Add(btnAnadir);
            barraMiembros.Controls.Add(btnQuitar);

            tablaMiembros = Ui.Tabla();
            tablaMiembros.Dock = DockStyle.Fill;
            Ui.Col(tablaMiembros, "Nombre", "NombreCompleto", 40);
            Ui.Col(tablaMiembros, "Email", "Email", 40);
            Ui.Col(tablaMiembros, "Rol", "RolEtiqueta", 25);
            tablaMiembros.DataSource = miembros;

            derecha.Controls.Add(tablaMiembros);
            derecha.Controls.Add(barraMiembros);
            derecha.Controls.Add(lblMiembros);

            this.Controls.Add(derecha);
            this.Controls.Add(izquierda);
        }

        private void EquiposForm_Load(object sender, EventArgs e)
        {
            try
            {
                List<Usuario> usuarios = UsuariosApi.GetTodos();
                cmbUsuarios.DataSource = usuarios;
                cmbUsuarios.DisplayMember = "NombreCompleto";
                cmbUsuarios.ValueMember = "IdUsuario";
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
            CargarEquipos();
        }

        private void CargarEquipos()
        {
            try
            {
                int idSeleccionado = 0;
                Equipo actual = Ui.Seleccionado(tablaEquipos) as Equipo;
                if (actual != null)
                {
                    idSeleccionado = actual.IdEquipo;
                }

                List<Equipo> lista = EquiposApi.GetTodos();
                equipos.Clear();
                for (int i = 0; i < lista.Count; i++)
                {
                    equipos.Add(lista[i]);
                }

                // Volver a seleccionar el mismo equipo que estaba elegido.
                for (int i = 0; i < tablaEquipos.Rows.Count; i++)
                {
                    Equipo e = tablaEquipos.Rows[i].DataBoundItem as Equipo;
                    if (e != null && e.IdEquipo == idSeleccionado)
                    {
                        tablaEquipos.ClearSelection();
                        tablaEquipos.Rows[i].Selected = true;
                    }
                }
                CargarMiembros();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void CargarMiembros()
        {
            miembros.Clear();
            Equipo seleccionado = Ui.Seleccionado(tablaEquipos) as Equipo;
            if (seleccionado == null)
            {
                lblMiembros.Text = "Miembros";
                return;
            }

            lblMiembros.Text = "Miembros de " + seleccionado.Nombre;
            try
            {
                Equipo completo = EquiposApi.GetPorId(seleccionado.IdEquipo);
                if (completo.Miembros != null)
                {
                    for (int i = 0; i < completo.Miembros.Count; i++)
                    {
                        miembros.Add(completo.Miembros[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void tablaEquipos_SelectionChanged(object sender, EventArgs e)
        {
            CargarMiembros();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            EquipoEditForm dialogo = new EquipoEditForm(null);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                CargarEquipos();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Equipo seleccionado = Ui.Seleccionado(tablaEquipos) as Equipo;
            if (seleccionado == null)
            {
                Ui.Aviso("Selecciona primero un equipo.");
                return;
            }

            EquipoEditForm dialogo = new EquipoEditForm(seleccionado);
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                CargarEquipos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Equipo seleccionado = Ui.Seleccionado(tablaEquipos) as Equipo;
            if (seleccionado == null)
            {
                Ui.Aviso("Selecciona primero un equipo.");
                return;
            }

            if (!Ui.Confirmar("¿Eliminar el equipo \"" + seleccionado.Nombre + "\"?"))
            {
                return;
            }

            try
            {
                EquiposApi.Delete(seleccionado.IdEquipo);
                CargarEquipos();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnAnadir_Click(object sender, EventArgs e)
        {
            Equipo seleccionado = Ui.Seleccionado(tablaEquipos) as Equipo;
            if (seleccionado == null)
            {
                Ui.Aviso("Selecciona primero un equipo.");
                return;
            }
            if (cmbUsuarios.SelectedValue == null)
            {
                return;
            }

            try
            {
                EquiposApi.AnadirMiembro(seleccionado.IdEquipo, (int)cmbUsuarios.SelectedValue);
                CargarEquipos();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            Equipo equipo = Ui.Seleccionado(tablaEquipos) as Equipo;
            Usuario miembro = Ui.Seleccionado(tablaMiembros) as Usuario;
            if (equipo == null || miembro == null)
            {
                Ui.Aviso("Selecciona un equipo y uno de sus miembros.");
                return;
            }

            try
            {
                EquiposApi.QuitarMiembro(equipo.IdEquipo, miembro.IdUsuario);
                CargarEquipos();
            }
            catch (Exception ex)
            {
                Ui.Error(ex);
            }
        }
    }
}

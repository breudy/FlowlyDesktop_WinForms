using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowlyDesktop
{
    // Clase de ayuda para no repetir el mismo código de controles en todos los formularios.
    // Los colores están aquí arriba: si el manual de identidad usa otros, se cambian solo en este sitio.
    // Esta parte es de información externa que no sale en los vídeos
    // (esta clase construye los controles por código y usa FlowLayoutPanel, que no sale en los vídeos)
    public static class Ui
    {
        public static readonly Color ColorPrincipal = Color.FromArgb(37, 99, 235);
        public static readonly Color ColorOscuro = Color.FromArgb(30, 41, 59);
        public static readonly Color ColorFondo = Color.FromArgb(244, 246, 250);
        public static readonly Color ColorPeligro = Color.FromArgb(220, 38, 38);

        public static readonly Font Fuente = new Font("Segoe UI", 10F);
        public static readonly Font FuenteNegrita = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font FuenteTitulo = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font FuenteNumero = new Font("Segoe UI", 20F, FontStyle.Bold);

        // ---------- Mensajes ----------

        public static void Error(Exception ex)
        {
            MessageBox.Show(ex.Message, "Flowly", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Aviso(string texto)
        {
            MessageBox.Show(texto, "Flowly", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirmar(string texto)
        {
            DialogResult r = MessageBox.Show(texto, "Flowly", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return r == DialogResult.Yes;
        }

        // ---------- Formularios ----------

        // Configura un formulario que se abre como ventana de diálogo (crear / editar).
        public static void ConfigurarDialogo(Form f, string titulo, int ancho, int alto)
        {
            f.Text = titulo;
            f.ClientSize = new Size(ancho, alto);
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.StartPosition = FormStartPosition.CenterParent;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            f.BackColor = Color.White;
            f.Font = Fuente;
        }

        // Configura un formulario de pantalla completa que se muestra dentro del MainForm.
        public static void ConfigurarPantalla(Form f, string titulo)
        {
            f.Text = titulo;
            f.ClientSize = new Size(1000, 640);
            f.BackColor = ColorFondo;
            f.Font = Fuente;
        }

        // Pone una etiqueta a la izquierda y el control a la derecha, en la altura y.
        public static void Fila(Control padre, string texto, Control control, int y)
        {
            Label l = new Label();
            l.Text = texto;
            l.AutoSize = true;
            l.Left = 20;
            l.Top = y + 4;
            l.Font = Fuente;

            control.Left = 170;
            control.Top = y;
            control.Width = 300;
            control.Font = Fuente;

            padre.Controls.Add(l);
            padre.Controls.Add(control);
        }

        // ---------- Controles ----------

        public static Button Boton(string texto, int ancho)
        {
            Button b = new Button();
            b.Text = texto;
            b.Width = ancho;
            b.Height = 34;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = ColorPrincipal;
            b.ForeColor = Color.White;
            b.Font = Fuente;
            b.Cursor = Cursors.Hand;
            b.Margin = new Padding(0, 0, 8, 0);
            return b;
        }

        public static Button BotonSecundario(string texto, int ancho)
        {
            Button b = Boton(texto, ancho);
            b.BackColor = Color.White;
            b.ForeColor = ColorOscuro;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            return b;
        }

        public static Button BotonPeligro(string texto, int ancho)
        {
            Button b = Boton(texto, ancho);
            b.BackColor = ColorPeligro;
            return b;
        }

        // Fila horizontal que se pega arriba y donde se van colocando botones uno detrás de otro.
        public static FlowLayoutPanel BarraBotones()
        {
            FlowLayoutPanel p = new FlowLayoutPanel();
            p.Dock = DockStyle.Top;
            p.Height = 52;
            p.Padding = new Padding(16, 9, 16, 9);
            p.BackColor = ColorFondo;
            return p;
        }

        // Tarjeta con un número grande y un título pequeño (estadísticas del dashboard).
        public static Panel Tarjeta(string titulo, out Label valor)
        {
            Panel p = new Panel();
            p.Width = 200;
            p.Height = 78;
            p.BackColor = Color.White;
            p.Margin = new Padding(0, 0, 12, 0);

            Label t = new Label();
            t.Text = titulo;
            t.Left = 14;
            t.Top = 8;
            t.AutoSize = true;
            t.ForeColor = Color.FromArgb(100, 116, 139);
            t.Font = Fuente;

            valor = new Label();
            valor.Text = "0";
            valor.Left = 12;
            valor.Top = 30;
            valor.AutoSize = true;
            valor.ForeColor = ColorPrincipal;
            valor.Font = FuenteNumero;

            p.Controls.Add(t);
            p.Controls.Add(valor);
            return p;
        }

        // ---------- DataGridView ----------

        public static DataGridView Tabla()
        {
            DataGridView t = new DataGridView();
            t.AutoGenerateColumns = false;
            t.ReadOnly = true;
            t.AllowUserToAddRows = false;
            t.AllowUserToDeleteRows = false;
            t.AllowUserToResizeRows = false;
            t.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            t.MultiSelect = false;
            t.RowHeadersVisible = false;
            t.BackgroundColor = Color.White;
            t.BorderStyle = BorderStyle.None;
            t.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            t.EnableHeadersVisualStyles = false;
            t.ColumnHeadersDefaultCellStyle.BackColor = ColorOscuro;
            t.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            t.ColumnHeadersDefaultCellStyle.Font = FuenteNegrita;
            t.ColumnHeadersHeight = 34;
            t.RowTemplate.Height = 30;
            t.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            t.DefaultCellStyle.SelectionForeColor = ColorOscuro;
            t.Font = Fuente;
            return t;
        }

        // Añade una columna de texto enlazada a una propiedad del objeto.
        public static void Col(DataGridView t, string titulo, string propiedad, int peso)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.Name = propiedad;
            c.HeaderText = titulo;
            c.DataPropertyName = propiedad;
            c.FillWeight = peso;
            t.Columns.Add(c);
        }

        // Igual que Col pero con formato (por ejemplo "dd/MM/yyyy" o "N2").
        public static void Col(DataGridView t, string titulo, string propiedad, int peso, string formato)
        {
            Col(t, titulo, propiedad, peso);
            t.Columns[propiedad].DefaultCellStyle.Format = formato;
        }

        // Devuelve el objeto de la fila seleccionada, o null si no hay ninguna.
        public static object Seleccionado(DataGridView t)
        {
            if (t.SelectedRows.Count == 0)
            {
                return null;
            }
            return t.SelectedRows[0].DataBoundItem;
        }
    }
}

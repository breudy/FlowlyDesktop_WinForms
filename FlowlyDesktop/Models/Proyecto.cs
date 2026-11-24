using System;
using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public class Proyecto
    {
        public int IdProyecto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int IdEquipo { get; set; }
        public string Equipo { get; set; }
        public int IdCreador { get; set; }
        public string Creador { get; set; }
        public int NumTareas { get; set; }

        public List<Tarea> Tareas { get; set; }

        public string FechasTexto
        {
            get
            {
                string fin = "—";
                if (FechaFin != null)
                {
                    fin = FechaFin.Value.ToString("dd/MM/yyyy");
                }
                return FechaInicio.ToString("dd/MM/yyyy") + " – " + fin;
            }
        }
    }
}
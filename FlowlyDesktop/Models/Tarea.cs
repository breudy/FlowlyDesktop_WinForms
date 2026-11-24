using System;
using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public class Tarea
    {
        public int IdTarea { get; set; }
        public int IdProyecto { get; set; }
        public int? IdTareaPadre { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaLimite { get; set; }
        public decimal HorasEstimadas { get; set; }

        public string Proyecto { get; set; }
        public string TareaPadre { get; set; }
        public decimal? HorasRegistradas { get; set; }
        public List<Tarea> Subtareas { get; set; }
        public List<Asignacion> Asignados { get; set; }
        public List<Tarea> TareasPrevias { get; set; }

        public string EstadoTexto
        {
            get { return EstadosTarea.Texto(Estado); }
        }

        public string NombreConNivel
        {
            get
            {
                if (IdTareaPadre != null)
                {
                    return "   ↳ " + Nombre;
                }
                return Nombre;
            }
        }

        public string FechaLimiteTexto
        {
            get
            {
                if (FechaLimite == null)
                {
                    return "—";
                }
                return FechaLimite.Value.ToString("dd/MM/yyyy");
            }
        }
    }
}
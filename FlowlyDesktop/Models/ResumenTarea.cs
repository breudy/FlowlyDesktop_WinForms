using System;

namespace FlowlyDesktop.Models
{
    public class ResumenTarea
    {
        public int IdTarea { get; set; }
        public int? IdTareaPadre { get; set; }
        public string Tarea { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaLimite { get; set; }
        public decimal HorasEstimadas { get; set; }
        public decimal HorasRegistradas { get; set; }


        public string EstadoTexto
        {
            get { return EstadosTarea.Texto(Estado); }
        }

        public decimal Desviacion
        {
            get { return HorasRegistradas - HorasEstimadas; }
        }
    }
}
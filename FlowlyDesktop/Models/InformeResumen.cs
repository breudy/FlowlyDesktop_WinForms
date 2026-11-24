using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public class InformeResumen
    {
        public int IdProyecto { get; set; }
        public string Proyecto { get; set; }
        public int TotalTareas { get; set; }
        public int Pendientes { get; set; }
        public int EnCurso { get; set; }
        public int Finalizadas { get; set; }
        public decimal PorcentajeFinalizado { get; set; }
        public decimal HorasEstimadas { get; set; }
        public decimal HorasRegistradas { get; set; }
        public List<ResumenTarea> Tareas { get; set; }

        public string AvanceTexto
        {
            get
            {
                return Finalizadas + " de " + TotalTareas + " tareas (" + PorcentajeFinalizado.ToString("0.#") + " %)";
            }
        }
    }
}
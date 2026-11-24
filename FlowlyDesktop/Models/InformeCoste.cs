using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public class InformeCostes
    {
        public int IdProyecto { get; set; }
        public string Proyecto { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalCoste { get; set; }
        public List<CosteUsuario> Personas { get; set; }
    }
}
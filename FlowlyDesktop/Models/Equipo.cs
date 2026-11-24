using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public class Equipo
    {
        public int IdEquipo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int NumMiembros { get; set; }
        public int NumProyectos { get; set; }

        public List<Usuario> Miembros { get; set; }
        public List<Proyecto> Proyectos { get; set; }
    }
}
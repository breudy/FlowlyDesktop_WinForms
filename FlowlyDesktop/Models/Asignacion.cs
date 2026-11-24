namespace FlowlyDesktop.Models
{
    public class Asignacion
    {
        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public decimal? HorasAsignadas { get; set; }
    }
}
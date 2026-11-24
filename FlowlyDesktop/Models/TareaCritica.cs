namespace FlowlyDesktop.Models
{
    public class TareaCritica
    {
        public int IdTarea { get; set; }
        public int IdProyecto { get; set; }
        public string Tarea { get; set; }
        public string Estado { get; set; }
        public int TareasBloqueadas { get; set; }

        public string EstadoTexto
        {
            get { return EstadosTarea.Texto(Estado); }
        }
    }
}
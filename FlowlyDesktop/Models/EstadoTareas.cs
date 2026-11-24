namespace FlowlyDesktop.Models
{
    public static class EstadosTarea
    {
        public const string PENDIENTE = "PENDIENTE";
        public const string EN_CURSO = "EN_CURSO";
        public const string FINALIZADA = "FINALIZADA";

        public static readonly string[] TODOS = { PENDIENTE, EN_CURSO, FINALIZADA };

        public static string Texto(string estado)
        {
            switch (estado)
            {
                case PENDIENTE:
                    return "Pendiente";
                case EN_CURSO:
                    return "En curso";
                case FINALIZADA:
                    return "Finalizada";
                default:
                    return estado;
            }
        }
    }
}
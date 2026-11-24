namespace FlowlyDesktop.Models
{
    public class CosteUsuario
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public decimal Horas { get; set; }
        public decimal TarifaHora { get; set; }
        public decimal Coste { get; set; }

        public string HorasTexto
        {
            get { return Horas.ToString("0.##") + " h"; }
        }

        public string TarifaTexto
        {
            get { return TarifaHora.ToString("0.00") + " €/h"; }
        }

        public string CosteTexto
        {
            get { return Coste.ToString("0.00") + " €"; }
        }
    }
}
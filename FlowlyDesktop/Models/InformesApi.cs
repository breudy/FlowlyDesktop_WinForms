using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public static class InformesApi
    {
        public static InformeCostes GetCostes(int idProyecto)
        {
            return Api.Get<InformeCostes>("api/informes/proyectos/" + idProyecto + "/costes");
        }

        public static InformeResumen GetResumen(int idProyecto)
        {
            return Api.Get<InformeResumen>("api/informes/proyectos/" + idProyecto + "/resumen");
        }

        public static List<TareaCritica> GetTareasCriticas(int? idProyecto)
        {
            string ruta = "api/informes/tareas-criticas";
            if (idProyecto != null)
            {
                ruta = ruta + "?idProyecto=" + idProyecto.Value;
            }
            return Api.Get<List<TareaCritica>>(ruta);
        }
    }
}
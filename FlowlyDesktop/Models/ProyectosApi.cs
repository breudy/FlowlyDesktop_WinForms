using System;
using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public static class ProyectosApi
    {
        public static List<Proyecto> GetTodos()
        {
            return Api.Get<List<Proyecto>>("api/proyectos");
        }

        public static Proyecto GetPorId(int id)
        {
            return Api.Get<Proyecto>("api/proyectos/" + id);
        }

        public static int Insert(string nombre, string descripcion, DateTime fechaInicio,
                                 DateTime? fechaFin, int idEquipo)
        {
            Proyecto creado = Api.Post<Proyecto>("api/proyectos", new
            {
                nombre = nombre,
                descripcion = descripcion,
                fechaInicio = fechaInicio.Date,
                fechaFin = fechaFin,
                idEquipo = idEquipo
            });
            return creado.IdProyecto;
        }

        public static string Update(int id, string nombre, string descripcion, DateTime fechaInicio,
                                    DateTime? fechaFin, int idEquipo)
        {
            return Api.Put("api/proyectos/" + id, new
            {
                nombre = nombre,
                descripcion = descripcion,
                fechaInicio = fechaInicio.Date,
                fechaFin = fechaFin,
                idEquipo = idEquipo
            });
        }

        public static string Delete(int id)
        {
            return Api.Delete("api/proyectos/" + id);
        }
    }
}
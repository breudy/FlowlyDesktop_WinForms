using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public static class EquiposApi
    {
        public static List<Equipo> GetTodos()
        {
            return Api.Get<List<Equipo>>("api/equipos");
        }

        public static Equipo GetPorId(int id)
        {
            return Api.Get<Equipo>("api/equipos/" + id);
        }

        public static int Insert(string nombre, string descripcion)
        {
            Equipo creado = Api.Post<Equipo>("api/equipos", new
            {
                nombre = nombre,
                descripcion = descripcion
            });
            return creado.IdEquipo;
        }

        public static string Update(int id, string nombre, string descripcion)
        {
            return Api.Put("api/equipos/" + id, new
            {
                nombre = nombre,
                descripcion = descripcion
            });
        }

        public static string Delete(int id)
        {
            return Api.Delete("api/equipos/" + id);
        }

        public static string AnadirMiembro(int idEquipo, int idUsuario)
        {
            return Api.Post("api/equipos/" + idEquipo + "/miembros/" + idUsuario, null);
        }

        public static string QuitarMiembro(int idEquipo, int idUsuario)
        {
            return Api.Delete("api/equipos/" + idEquipo + "/miembros/" + idUsuario);
        }
    }
}
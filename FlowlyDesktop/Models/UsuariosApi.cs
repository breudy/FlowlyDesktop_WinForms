using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public static class UsuariosApi
    {
        public static List<Usuario> GetTodos()
        {
            return Api.Get<List<Usuario>>("api/usuarios");
        }

        public static Usuario GetPorId(int id)
        {
            return Api.Get<Usuario>("api/usuarios/" + id);
        }

        public static int Insert(string nombre, string apellidos, string email, string password,
                                 decimal tarifaHora, int idRol)
        {
            Usuario creado = Api.Post<Usuario>("api/usuarios", new
            {
                nombre = nombre,
                apellidos = apellidos,
                email = email,
                password = password,
                tarifaHora = tarifaHora,
                idRol = idRol
            });
            return creado.IdUsuario;
        }

        public static string Update(int id, string nombre, string apellidos, string email,
                                    decimal tarifaHora, int idRol)
        {
            return Api.Put("api/usuarios/" + id, new
            {
                nombre = nombre,
                apellidos = apellidos,
                email = email,
                tarifaHora = tarifaHora,
                idRol = idRol
            });
        }

        public static string CambiarPassword(int id, string password)
        {
            return Api.Put("api/usuarios/" + id + "/password", new { password = password });
        }

        public static string CambiarActivo(int id, bool activo)
        {
            return Api.Put("api/usuarios/" + id + "/activo", new { activo = activo });
        }
    }
}
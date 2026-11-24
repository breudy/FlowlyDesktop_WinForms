using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public static class RolesApi
    {
        public static List<Rol> GetTodos()
        {
            return Api.Get<List<Rol>>("api/roles");
        }
    }
}
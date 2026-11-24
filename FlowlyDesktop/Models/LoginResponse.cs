using System;

namespace FlowlyDesktop.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime Expira { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}
namespace FlowlyDesktop.Models
{
    public static class AuthApi
    {
        public static LoginResponse Login(string email, string password)
        {
            LoginResponse datos = Api.Post<LoginResponse>("api/auth/login",
                new { email = email.Trim(), password = password });

            Api.Token = datos.Token;
            Api.Sesion = datos;
            return datos;
        }

        public static bool EsAdministrador()
        {
            return Api.Sesion != null && Api.Sesion.Rol == Roles.ADMINISTRADOR;
        }

        public static void Logout()
        {
            Api.Token = "";
            Api.Sesion = null;
        }
    }
}
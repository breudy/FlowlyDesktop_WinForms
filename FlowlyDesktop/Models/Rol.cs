namespace FlowlyDesktop.Models
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string Etiqueta
        {
            get
            {
                if (Nombre == Roles.ADMINISTRADOR)
                {
                    return "Administrador";
                }
                if (Nombre == Roles.TECNICO)
                {
                    return "Usuario técnico";
                }
                return Nombre;
            }
        }
    }
}
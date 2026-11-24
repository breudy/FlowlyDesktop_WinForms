using System.Collections.Generic;

namespace FlowlyDesktop.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public decimal TarifaHora { get; set; }
        public bool Activo { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; }

        public List<Equipo> Equipos { get; set; }


        public string NombreCompleto
        {
            get { return Nombre + " " + Apellidos; }
        }

        public string Estado
        {
            get
            {
                if (Activo)
                {
                    return "Activo";
                }
                return "Desactivado";
            }
        }

        public string RolEtiqueta
        {
            get
            {
                if (Rol == Roles.ADMINISTRADOR)
                {
                    return "Administrador";
                }
                return "Usuario técnico";
            }
        }

        public string Grupos
        {
            get
            {
                if (Equipos == null || Equipos.Count == 0)
                {
                    return "—";
                }

                string texto = "";
                for (int i = 0; i < Equipos.Count; i++)
                {
                    if (i > 0)
                    {
                        texto = texto + ", ";
                    }
                    texto = texto + Equipos[i].Nombre;
                }
                return texto;
            }
        }
    }
}
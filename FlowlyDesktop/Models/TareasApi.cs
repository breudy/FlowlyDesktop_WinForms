using System;

namespace FlowlyDesktop.Models
{
    public static class TareasApi
    {
        public static Tarea GetPorId(int id)
        {
            return Api.Get<Tarea>("api/tareas/" + id);
        }

        public static int Insert(int idProyecto, int? idTareaPadre, string nombre, string descripcion,
                                 DateTime? fechaLimite, decimal horasEstimadas)
        {
            Tarea creada = Api.Post<Tarea>("api/tareas", new
            {
                idProyecto = idProyecto,
                idTareaPadre = idTareaPadre,
                nombre = nombre,
                descripcion = descripcion,
                fechaLimite = fechaLimite,
                horasEstimadas = horasEstimadas
            });
            return creada.IdTarea;
        }

        public static string Update(int id, string nombre, string descripcion, DateTime? fechaLimite,
                                    decimal horasEstimadas, string estado)
        {
            return Api.Put("api/tareas/" + id, new
            {
                nombre = nombre,
                descripcion = descripcion,
                fechaLimite = fechaLimite,
                horasEstimadas = horasEstimadas,
                estado = estado
            });
        }

        public static string Delete(int id)
        {
            return Api.Delete("api/tareas/" + id);
        }

        public static string Asignar(int idTarea, int idUsuario, decimal? horasAsignadas)
        {
            return Api.Post("api/tareas/" + idTarea + "/asignaciones", new
            {
                idUsuario = idUsuario,
                horasAsignadas = horasAsignadas
            });
        }

        public static string CambiarHorasAsignadas(int idTarea, int idUsuario, decimal? horasAsignadas)
        {
            return Api.Put("api/tareas/" + idTarea + "/asignaciones/" + idUsuario, new
            {
                horasAsignadas = horasAsignadas
            });
        }

        public static string QuitarAsignacion(int idTarea, int idUsuario)
        {
            return Api.Delete("api/tareas/" + idTarea + "/asignaciones/" + idUsuario);
        }

        public static string AnadirDependencia(int idTarea, int idTareaPrevia)
        {
            return Api.Post("api/tareas/" + idTarea + "/dependencias", new
            {
                idTareaPrevia = idTareaPrevia
            });
        }

        public static string QuitarDependencia(int idTarea, int idTareaPrevia)
        {
            return Api.Delete("api/tareas/" + idTarea + "/dependencias/" + idTareaPrevia);
        }
    }
}
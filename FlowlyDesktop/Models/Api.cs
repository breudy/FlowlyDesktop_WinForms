using System;
using System.IO;
using System.Net;
using System.Text;
// Esta parte es de información externa que no sale en los vídeos
using System.Text.Json;

namespace FlowlyDesktop.Models
{
    public class ApiException : Exception
    {
        public int Codigo { get; set; }

        public ApiException(string mensaje, int codigo) : base(mensaje)
        {
            this.Codigo = codigo;
        }
    }

    // Esta parte es de información externa que no sale en los vídeos
    // (llamar a una API web desde el formulario con WebClient y leer el JSON con System.Text.Json no se explica en los vídeos)
    public static class Api
    {
        public const string URL_API = "https://localhost:7189/";

        public static string Token = "";

        public static LoginResponse Sesion = null;

        private static JsonSerializerOptions opciones = CrearOpciones();

        public static T Get<T>(string ruta)
        {
            string json = Enviar("GET", ruta, null);
            return JsonSerializer.Deserialize<T>(json, opciones);
        }

        public static T Post<T>(string ruta, object cuerpo)
        {
            string json = Enviar("POST", ruta, cuerpo);
            return JsonSerializer.Deserialize<T>(json, opciones);
        }

        public static string Post(string ruta, object cuerpo)
        {
            return LeerMensaje(Enviar("POST", ruta, cuerpo));
        }

        public static string Put(string ruta, object cuerpo)
        {
            return LeerMensaje(Enviar("PUT", ruta, cuerpo));
        }

        public static string Delete(string ruta)
        {
            return LeerMensaje(Enviar("DELETE", ruta, null));
        }

        private static string Enviar(string metodo, string ruta, object cuerpo)
        {
            WebClient cliente = new WebClient();
            cliente.Encoding = Encoding.UTF8;
            cliente.Headers[HttpRequestHeader.ContentType] = "application/json";

            if (Token != "")
            {
                cliente.Headers[HttpRequestHeader.Authorization] = "Bearer " + Token;
            }

            string url = URL_API + ruta;

            try
            {
                if (metodo == "GET")
                {
                    return cliente.DownloadString(url);
                }

                string textoCuerpo = "";
                if (cuerpo != null)
                {
                    textoCuerpo = JsonSerializer.Serialize(cuerpo, opciones);
                }
                return cliente.UploadString(url, metodo, textoCuerpo);
            }
            catch (WebException ex)
            {
                HttpWebResponse respuesta = ex.Response as HttpWebResponse;

                if (respuesta == null)
                {
                    throw new ApiException("No se puede conectar con la API (" + URL_API + "). Comprueba que está arrancada.", 0);
                }

                int codigo = (int)respuesta.StatusCode;

                StreamReader lector = new StreamReader(respuesta.GetResponseStream());
                string textoError = lector.ReadToEnd();
                lector.Close();

                string mensaje = LeerMensaje(textoError);
                if (mensaje == "")
                {
                    if (codigo == 401)
                    {
                        mensaje = "No has iniciado sesión o tu sesión ha caducado.";
                    }
                    else if (codigo == 403)
                    {
                        mensaje = "No tienes permiso para hacer esto.";
                    }
                    else
                    {
                        mensaje = "Error " + codigo + " al llamar a la API.";
                    }
                }
                throw new ApiException(mensaje, codigo);
            }
            finally
            {
                cliente.Dispose();
            }
        }

        private static string LeerMensaje(string json)
        {
            try
            {
                RespuestaMensaje r = JsonSerializer.Deserialize<RespuestaMensaje>(json, opciones);
                if (r != null && r.Mensaje != null)
                {
                    return r.Mensaje;
                }
            }
            catch (JsonException)
            {
            }
            return "";
        }

        private static JsonSerializerOptions CrearOpciones()
        {
            JsonSerializerOptions o = new JsonSerializerOptions();
            o.PropertyNameCaseInsensitive = true;
            o.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            return o;
        }

        private class RespuestaMensaje
        {
            public string Mensaje { get; set; }
        }
    }
}
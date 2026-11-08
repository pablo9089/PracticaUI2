using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PracticaUI2
{
    //ERROR GUARDA EL MENSAJE QUE MANDO LA API T CONDIGO HTTP (401, 409)
    public class ApiException : Exception
    {
        public HttpStatusCode Codigo { get; }
        public ApiException(string mensaje, HttpStatusCode codigo) : base(mensaje) => Codigo = codigo;

    }
    public static class ApiService
    {
        private static readonly string BaseURL = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5080/"
            : "http://localhost:5080";

        //HttpClient: el cartero que lleva y trae mensajes de la API

        private static readonly HttpClient Http = new()
        {
            BaseAddress = new Uri(BaseURL),
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static string? token;
        public static bool HaySesion => token is not null;

        //Llama a POST /api/auth/login. Si los datos son correctos, guarda el token
        public static async Task LoginAsync(string correo, string clave)
        {
            var resp = await Http.PostAsJsonAsync("/api/auth/login", new { correo, clave });
            await RevisarAsync(resp);

            var datos = await resp.Content.ReadFromJsonAsync<TokenRespuesta>();
            token = datos!.token;
        }
        private static async Task RevisarAsync(HttpResponseMessage resp)
        {
            if (resp.IsSuccessStatusCode) return;
            string mensaje = resp.StatusCode == HttpStatusCode.Unauthorized
                ? "Sesion no valida o vencida"
                : "La API respondio con un error";

            try
            {
                //La API manda el mensaje
                var texto = (await resp.Content.ReadFromJsonAsync<ErrorRespuesta>())?.Mensaje;
                if (!string.IsNullOrWhiteSpace(texto)) mensaje = texto;
            }
            catch
            {

            }
            throw new ApiException(mensaje, resp.StatusCode);


        }

        private record TokenRespuesta(string token);
        // CS1061: "ApiService.TokenRespuesta" no contiene una definición para "token" ni un método de extensión accesible "token" que acepte un primer argumento del tipo "ApiService.TokenRespuesta"
        // CS1061: "ApiService.TokenRespuesta" no contiene una definición para "token" ni un método de extensión accesible "token" que acepte un primer argumento del tipo "ApiService.TokenRespuesta"
        private record ErrorRespuesta(string? Mensaje);
    }
}

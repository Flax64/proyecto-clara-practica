using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace backend_CLARA
{
    /// <summary>
    /// Agrega cabeceras de seguridad HTTP a todas las respuestas de la API.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;

            // Evita que el navegador "adivine" el tipo de archivo (MIME sniffing)
            headers["X-Content-Type-Options"] = "nosniff";

            // Impide que la página se cargue dentro de un iframe (clickjacking)
            headers["X-Frame-Options"] = "DENY";

            // Filtro XSS antiguo. Hoy se recomienda "0" y usar CSP.
            // Si tu revisor pide el valor clásico, cámbialo por: "1; mode=block"
            headers["X-XSS-Protection"] = "0";

            // No enviar la URL de origen (el enlace de recuperación lleva un token en la URL)
            headers["Referrer-Policy"] = "no-referrer";

            // HSTS: solo tiene efecto sobre HTTPS, los navegadores lo ignoran en HTTP.
            // 31536000 segundos = 1 año.
            if (context.Request.IsHttps)
            {
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            }

            // CSP: se omite para la documentación (Scalar / OpenAPI), porque su interfaz
            // carga scripts y estilos que una política estricta bloquearía.
            // 'unsafe-inline' en style-src es necesario por la página de confirmación
            // de "verificar-clic", que trae su <style> dentro del HTML.
            // Si algún día usas scripts de Google en una página web, agrega
            // https://apis.google.com a script-src.
            var ruta = context.Request.Path;
            bool esDocumentacion = ruta.StartsWithSegments("/scalar") || ruta.StartsWithSegments("/openapi");

            if (!esDocumentacion)
            {
                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "script-src 'self'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data:; " +
                    "frame-ancestors 'none'";
            }

            return _next(context);
        }
    }
}
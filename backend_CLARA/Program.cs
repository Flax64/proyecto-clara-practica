using backend_CLARA;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.WebHost.UseUrls("http://*:5133");

// AQUÍ VA EL CORS (ANTES DEL BUILD)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================================
// RATE LIMITING
// Política "limite-auth": 5 peticiones por IP cada 6 minutos.
// Al pasarse del límite se responde 429 hasta que la ventana se reinicia,
// así que el bloqueo máximo es de 6 minutos.
// ==========================================
const int MINUTOS_VENTANA = 6;
const int MAX_INTENTOS = 5;

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("limite-auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = MAX_INTENTOS,
                Window = TimeSpan.FromMinutes(MINUTOS_VENTANA),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    // Respuesta en JSON con la misma forma { "error": "..." } que usa el resto de la API,
    // para que la app de escritorio la muestre sin cambios.
    options.OnRejected = async (context, cancellationToken) =>
    {
        int minutos = MINUTOS_VENTANA;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers["Retry-After"] = ((int)retryAfter.TotalSeconds).ToString();
            minutos = (int)Math.Ceiling(retryAfter.TotalMinutes);
        }

        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = $"Demasiados intentos. Intenta de nuevo en {minutos} minuto(s)." },
            cancellationToken);
    };
});

// ==========================================
// EL HORNEADO (Punto de no retorno)
var app = builder.Build();
// ==========================================

// Cabeceras de seguridad (debe ir antes del resto para cubrir todas las respuestas)
app.UseMiddleware<SecurityHeadersMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

// ESTO VA DESPUÉS DEL BUILD
app.UseCors("PermitirTodo");

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
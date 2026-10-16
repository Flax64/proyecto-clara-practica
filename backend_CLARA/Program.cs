using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.WebHost.UseUrls("http://*:5133");

// 👇 AQUÍ VA EL CORS (ANTES DEL BUILD) 👇
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
// EL HORNEADO (Punto de no retorno)
var app = builder.Build();
// ==========================================


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

// ESTO VA DESPUÉS DEL BUILD 
app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();
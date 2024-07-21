using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.EndPoints;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;

var builder = WebApplication.CreateBuilder(args);
string AllowedHosts = builder.Configuration.GetValue<string>("AllowedHosts")!;
#region Servicios
// Aqu� van todos los servicios que se van a utilizar

//Configuracion de CORS
builder.Services.AddCors(options =>
{
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.WithOrigins(AllowedHosts)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
});

//Configuraci�n de Cach� del lado del servidor
builder.Services.AddOutputCache();

//Configuraci�n de Swagger para documentaci�n de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuraci�n de Repositorios
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();


#endregion

var app = builder.Build();

#region Middlewares 
// Aqu� van todos los middlewares que se van a utilizar
// Middleware para documentaci�n de la API
if(app.Environment.IsDevelopment()) // Solo se activa en ambiente de desarrollo
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware para aplicar politica de CORS
app.UseCors();

// Middleware para cach� del lado del servidor
app.UseOutputCache();

// Middleware Endpoints
app.MapGroup("/generos").MapGeneros();



#endregion Middlewares

app.Run();



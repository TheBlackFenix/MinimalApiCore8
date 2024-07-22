using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using WebApiNet8.EndPoints;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;
using WebApiNet8.Servicios;

var builder = WebApplication.CreateBuilder(args);
string AllowedHosts = builder.Configuration.GetValue<string>("AllowedHosts")!;
#region Servicios
// Aquí van todos los servicios que se van a utilizar

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

//Configuración de Caché del lado del servidor
builder.Services.AddOutputCache();

//Configuración de Swagger para documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuración de Repositorios
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
//Servicio para almacenar archivos en Azure
//builder.Services.AddScoped<IAlmacenarArchivos, AlmacenadorArchivosAzure>();
//Servicio para almacenar archivos en Local
builder.Services.AddScoped<IAlmacenarArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();
// Se agrega el servicio de AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

#endregion

var app = builder.Build();

#region Middlewares 
// Aquí van todos los middlewares que se van a utilizar
// Middleware para documentación de la API
if(app.Environment.IsDevelopment()) // Solo se activa en ambiente de desarrollo
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Midelware para usar archivos estáticos
app.UseStaticFiles();
// Middleware para aplicar politica de CORS
app.UseCors();

// Middleware para caché del lado del servidor
app.UseOutputCache();

// Middleware Endpoints
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores().DisableAntiforgery();



#endregion Middlewares

app.Run();



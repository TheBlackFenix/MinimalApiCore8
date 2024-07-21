using Microsoft.AspNetCore.Cors;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;

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

// Middleware para aplicar politica de CORS
app.UseCors();

// Middleware para caché del lado del servidor
app.UseOutputCache();

// Middleware Endpoints
app.MapGet("/generos", [EnableCors(policyName:"AllowAll")]() =>
{
    var generos = new List<Genero>
    {
        new Genero { IdGenero = 1, NombreGenero = "Comedia" },
        new Genero { IdGenero = 2, NombreGenero = "Drama" },
        new Genero { IdGenero = 3, NombreGenero = "Acción" },
        new Genero { IdGenero = 4, NombreGenero = "Romance" },
        new Genero { IdGenero = 5, NombreGenero = "Ciencia Ficción" }
    };
    return generos;
}).CacheOutput(c=> c.Expire(TimeSpan.FromSeconds(15))); // Se Agrega cache de 15 segundos

app.MapPost("/generos", async (Genero genero, IRepositorioGeneros repositorioGeneros) =>
{
    var id = await repositorioGeneros.CrearGenero(genero);
    return TypedResults.Created($"/generos/{id}", genero);
});
#endregion

app.Run();

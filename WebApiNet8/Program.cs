using Microsoft.AspNetCore.Cors;
using WebApiNet8.Entidades;

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


#endregion

var app = builder.Build();

#region Middlewares 
// Aquí van todos los middlewares que se van a utilizar

// Middleware para aplicar politica de CORS
app.UseCors();

// Middleware Endpoints
app.MapGet("/Generos", [EnableCors(policyName:"AllowAll")]() =>
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
});


#endregion

app.Run();

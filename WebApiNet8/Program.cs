using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using WebApiNet8.EndPoints;
using WebApiNet8.Entidades;
using WebApiNet8.Repositorios;
using WebApiNet8.Servicios;
using WebApiNet8.Utilidades;

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
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();
builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
//Servicio para almacenar archivos en Azure
builder.Services.AddScoped<IAlmacenarArchivos, AlmacenadorArchivosAzure>();
//Servicio para almacenar archivos en Local
//builder.Services.AddScoped<IAlmacenarArchivos, Almacen

//Servicio para detalle de problemas
builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();
// Se agrega el servicio de AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// Se agrega el servicio de FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Se agrega el servicio de autenticaci�n y autorizaci�n
builder.Services.AddAuthentication().AddJwtBearer(opciones=>
 opciones.TokenValidationParameters = new TokenValidationParameters
 {
     ValidateIssuer = false,
     ValidateAudience = false,
     ValidateLifetime = true,
     ValidateIssuerSigningKey = true,
     //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(),
     IssuerSigningKeys = Llaves.ObtenerLlaves(builder.Configuration),
     ClockSkew = TimeSpan.Zero
 });
builder.Services.AddAuthorization();

builder.Services.AddTransient<IUserStore<IdentityUser>, UsuarioStore>();
builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddTransient<SignInManager<IdentityUser>>();

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
// Middleware para manejo de problemas
app.UseExceptionHandler(excepcionHadlerapp =>
    excepcionHadlerapp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        var error = new LogError();
        error.MensajeError = exception.Message;
        error.StackTrace = exception.StackTrace;
        error.Fecha = DateTime.Now;
        var repositorioErrores = context.RequestServices.GetRequiredService<IRepositorioErrores>();
        await repositorioErrores.CrearLogError(error);


    }));
app.UseStatusCodePages();

// Midelware para usar archivos est�ticos
app.UseStaticFiles();
// Middleware para aplicar politica de CORS
app.UseCors();

// Middleware para cach� del lado del servidor
app.UseOutputCache();

//Middlewate para autenticaci�n y autorizaci�n
app.UseAuthorization();

// Middleware Endpoints
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores().DisableAntiforgery();
app.MapGroup("/peliculas").MapPeliculas().DisableAntiforgery();
app.MapGroup("/peliculas/{idPelicula:int}/comentarios").MapComentarios();


#endregion Middlewares

app.Run();



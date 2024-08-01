using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly string? connectionString;

        public readonly HttpContext httpContext;

        public RepositorioPeliculas(IConfiguration configuration, IHttpContextAccessor httpContextAccesor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            httpContext = httpContextAccesor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacion)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var peliculas = await conexion.QueryAsync<Pelicula>("sp_Pelicula_ObtenerTodos",
                                                                    new { paginacion.Pagina, paginacion.RegistrosPorPagina },
                                                                    commandType: CommandType.StoredProcedure);
                var cantidadPeliculas = await conexion.QuerySingleAsync<int>("sp_Pelicula_Cantidad", commandType: CommandType.StoredProcedure);
                httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidadPeliculas.ToString());
                return peliculas.ToList();

            }
        }

        public async Task<Pelicula?> ObtenerPeliculaPorId(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var pelicula = await conexion.QueryFirstOrDefaultAsync<Pelicula>("sp_Pelicula_ObtenerPorId", new { IdPelicula = id }, commandType: CommandType.StoredProcedure);
                return pelicula;
            }
        }

        public async Task<int> CrearPelicula(Pelicula pelicula)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var idPelicula = await conexion.QuerySingleAsync<int>("sp_Pelicula_Crear",
                                                                new
                                                                {
                                                                    pelicula.TituloPelicula,
                                                                    pelicula.EnCines,
                                                                    pelicula.FechaLanzamiento,
                                                                    pelicula.Poster
                                                                }, commandType: CommandType.StoredProcedure);
                pelicula.IdPelicula = idPelicula;
                return idPelicula;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var existe = await conexion.QuerySingleAsync<bool>("sp_Pelicula_Existe", new { IdPelicula = id }, commandType: CommandType.StoredProcedure);
                return existe;
            }
        }

        public async Task ActualizarPelicula(Pelicula pelicula)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Pelicula_ActualizarPorId",
                                            new
                                            {
                                                pelicula.IdPelicula,
                                                pelicula.TituloPelicula,
                                                pelicula.EnCines,
                                                pelicula.FechaLanzamiento,
                                                pelicula.Poster
                                            }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task EliminarPelicula(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Pelicula_EliminarPorId", new { IdPelicula = id }, commandType: CommandType.StoredProcedure);
            }
        }


    }
}

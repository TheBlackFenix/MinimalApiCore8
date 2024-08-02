using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiNet8.DTOs.Actores;
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
                using(var multi = await conexion.QueryMultipleAsync("sp_Pelicula_ObtenerPorId", new { IdPelicula = id }, commandType: CommandType.StoredProcedure))
                {
                    var pelicula = await multi.ReadFirstAsync<Pelicula>();
                    if (pelicula != null)
                    {
                        var comentarios = await multi.ReadAsync<Comentario>();
                        var generos = await multi.ReadAsync<Genero>();
                        var actores = await multi.ReadAsync<ActorPeliculaDTO>();
                        
                        pelicula.Comentarios = comentarios.ToList();
                        foreach (var genero in generos)
                        {
                            pelicula.GenerosPeliculas.Add(new GeneroPelicula { IdGenero = genero.IdGenero, Genero = genero });
                        }
                        foreach (var actor in actores)
                        {
                            pelicula.ActoresPeliculas.Add(new ActorPelicula { IdActor = actor.IdActor, Personaje = actor.Personaje, Actor = new Actor { NombreActor = actor.NombreActor } });
                        }
                    }
                    return pelicula;
                }
                
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

        public async Task AsignarGeneros(int id, List<int> generosIds)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            generosIds.ForEach(id => dt.Rows.Add(id));

            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Pelicula_AsignarGeneros", new { IdPelicula = id, GenerosIds = dt }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i-1].Orden = i ;
            }
            var dt = new DataTable();
            dt.Columns.Add("IdActor", typeof(int));
            dt.Columns.Add("Personaje", typeof(string));
            dt.Columns.Add("Orden", typeof(int));
            actores.ForEach(actor => dt.Rows.Add(actor.IdActor, actor.Personaje, actor.Orden));

            using(var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Pelicula_AsignarActores",
                    new { IdPelicula = id, Actores = dt }, 
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}

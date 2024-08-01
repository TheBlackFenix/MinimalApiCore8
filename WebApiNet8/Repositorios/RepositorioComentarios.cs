using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiNet8.DTOs.Paginacion;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly string? connectionString;

        public RepositorioComentarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Comentario>> ObtenerTodos(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var comentarios = await conexion.QueryAsync<Comentario>("sp_Comentario_ObtenerTodos",
                                                                    new { IdPelicula = id },                
                                                                    commandType: CommandType.StoredProcedure);

                return comentarios.ToList();
            }
        }

        public async Task<Comentario?> ObtenerComentarioPorId(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var pelicula = await conexion.QueryFirstOrDefaultAsync<Comentario>("sp_Comentario_ObtenerPorId", new { IdComentario = id }, commandType: CommandType.StoredProcedure);
                return pelicula;
            }
        }

        public async Task<int> CrearComentario(Comentario comentario)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var idComentario = await conexion.QuerySingleAsync<int>("sp_Comentario_Crear",
                                                                new
                                                                {
                                                                    comentario.Cuerpo,
                                                                    comentario.IdPelicula,
                                                                }, commandType: CommandType.StoredProcedure);
                comentario.IdComentario = idComentario;
                return idComentario;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var existe = await conexion.QuerySingleAsync<bool>("sp_Comentario_Existe", new { IdComentario = id }, commandType: CommandType.StoredProcedure);
                return existe;
            }
        }

        public async Task ActualizarComentario(Comentario comentario)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Comentario_ActualizarPorId",
                                            new
                                            {
                                                comentario.IdComentario,
                                                comentario.Cuerpo,
                                                comentario.IdPelicula,
                                            }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task EliminarComentario(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Comentario_EliminarPorId", new { IdComentario = id }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

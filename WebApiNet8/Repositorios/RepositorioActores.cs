using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiNet8.DTOs.Actores;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly string connectionString;
        public RepositorioActores(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Actor>> ObtenerTodos()
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var actores = await conexion.QueryAsync<Actor>("sp_Actor_ObtenerTodos", commandType: CommandType.StoredProcedure);
                return actores.ToList();
            }

        }
        public async Task<Actor> ObtenerPorId(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var actor = await conexion.QueryFirstOrDefaultAsync<Actor>("sp_Actor_ObtenerPorId", new { IdActor = id }, commandType: CommandType.StoredProcedure);
                return actor;
            }
        }
        public async Task<bool> Existe(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var existe = await conexion.QueryFirstOrDefaultAsync<bool>("sp_Actor_Existe", new { IdActor = id }, commandType: CommandType.StoredProcedure);
                return existe;
            }
        }
        public async Task<int> CrearActor(Actor actor)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var id = await conexion.QuerySingleAsync<int>("sp_Actor_Crear",
                                                                new { NombreActor = actor.NombreActor, FechaNacimiento = actor.FechaNacimiento, Foto = actor.Foto },
                                                                commandType: CommandType.StoredProcedure);
                return id;
            }

        }

        public async Task ActualizarActor(Actor actor)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Actor_ActualizarPorId", actor, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task EliminarActor(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("sp_Actor_EliminarPorId", new { IdActor = id }, commandType: CommandType.StoredProcedure);
            }
        }


    }
}

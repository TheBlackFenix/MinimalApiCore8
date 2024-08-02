using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly string? _connectionString;
        public RepositorioGeneros(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        public async Task<int> CrearGenero(Genero genero)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(@"sp_Genero_Crear", new {NombreGenero = genero.NombreGenero}, commandType: CommandType.StoredProcedure);
                genero.IdGenero = id;
                return id;
            }
        }

        public async Task<List<Genero>> ObtenerGeneros()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<Genero>("sp_Genero_ObtenerTodos", commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public async Task<Genero?> ObtenerGeneroPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Genero>("sp_Genero_ObtenerPorId", new { IdGenero = id }, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<bool>(@"sp_Genero_Existe", new { IdGenero = id }, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task ActualizarGenero(Genero genero)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(@"sp_Genero_ActualizarPorId", genero, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task EliminarGenero(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(@"sp_Genero_EliminarPorId", new {IdGenero = id}, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<List<int>> Existen(List<int> ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            ids.ForEach(id => dt.Rows.Add(id));
            using (var conexion = new SqlConnection(_connectionString))
            {
                var result = await conexion.QueryAsync<int>("sp_Genero_ObtenerVariosPorId", new { GenerosIds = dt }, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        public async Task<bool> Existe(int idGenero, string nombreGenero)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection
                    .QuerySingleAsync<bool>(@"sp_Genero_ExistePorIdYNombre",
                        new { IdGenero = idGenero, NombreGenero = nombreGenero },
                        commandType: CommandType.StoredProcedure);

                return result;
            }
        }
    }
}

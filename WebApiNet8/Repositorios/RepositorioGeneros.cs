using Dapper;
using Microsoft.Data.SqlClient;
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
                var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Generos (NombreGenero) VALUES (@NombreGenero) SELECT SCOPE_IDENTITY()", genero);
                genero.IdGenero = id;
                return id;
            }
        }

        public async Task<List<Genero>> ObtenerGeneros()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync<Genero>("SELECT IdGenero, NombreGenero FROM Generos WHERE Activo = 1 ORDER BY NombreGenero ASC");

                return result.ToList();
            }
        }

        public async Task<Genero?> ObtenerGeneroPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Genero>("SELECT IdGenero, NombreGenero FROM Generos WHERE IdGenero = @IdGenero AND Activo = 1", new { IdGenero = id });

                return result;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<bool>(@"IF EXISTS (SELECT 1 FROM Generos WHERE IdGenero = @IdGenero AND Activo = 1) 
                                                                                    SELECT 1 
                                                                                ELSE 
                                                                                    SELECT 0", new { IdGenero = id });

                return result;
            }
        }

        public async Task ActualizarGenero(Genero genero)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(@"UPDATE Generos SET NombreGenero = @NombreGenero WHERE  IdGenero = @IdGenero AND Activo = 1", genero);

            }
        }

        public async Task EliminarGenero(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(@"UPDATE Generos SET Activo = 0 WHERE IdGenero = @IdGenero", new {IdGenero = id});
            }
        }
    }
}

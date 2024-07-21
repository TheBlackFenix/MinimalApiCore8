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
    }
}

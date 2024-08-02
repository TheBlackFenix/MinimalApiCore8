using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiNet8.Entidades;

namespace WebApiNet8.Repositorios
{
    public class RepositorioErrores : IRepositorioErrores
    {
        private readonly string? connectionString;

        public RepositorioErrores(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task CrearLogError(LogError error)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("log_Errores_Crear",
                    new { error.MensajeError, error.StackTrace, error.Fecha },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}

using Microsoft.Data.SqlClient;
using System.Data;

namespace Academic.Infrastructure.Persistence
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CrearConexion()
        {
            return new SqlConnection(_connectionString);
        }
    }
}

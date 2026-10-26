
using Oracle.ManagedDataAccess.Client;

namespace PacientesApi.Data
{
    public class OracleConnectionFactory
    {
        private readonly string _connectionString;

        public OracleConnectionFactory(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("Oracle")
                ?? throw new InvalidOperationException(
                    "A connection string Oracle não foi configurada."
                );
        }

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }
}


using OEE.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace OEE.Infrastructure.Dapper.ConnectionFactory
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString;

        public ConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public IDbConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}

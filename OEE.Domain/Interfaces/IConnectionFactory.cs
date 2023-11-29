using System.Data;

namespace OEE.Domain.Interfaces
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection();
    }
}

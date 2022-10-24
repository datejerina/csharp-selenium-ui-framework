using System.Data;
using System.Data.Common;

namespace AutomationFW.Common.DataAccess.Interfaces
{
    public interface IDbConnectionHandler
    {
        string DbType { get; set; }
        DbCommand GetNewDbCommand(string commandText, DbConnection connection);
        DbConnection GetNewDbConnection();
        void InitializeDbConnection();
        bool IsConnectionStringMissingInfo();
        int PerformNonQuery(DbCommand dbCommand, int timeout = 60);
        int PerformNonQuery(string commandText, int timeout = 60);
        DataTable PerformQuery(DbCommand dbCommand, int timeout = 60);
        DataTable PerformQuery(string commandText, int timeout = 60);
    }
}
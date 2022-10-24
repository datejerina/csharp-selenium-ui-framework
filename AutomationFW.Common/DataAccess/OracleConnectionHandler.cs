using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace AutomationFW.Common.DataAccess
{
    public class OracleConnectionHandler : DbConnectionHandlerBase
    {
        public override string DbType { get; set; } = "Oracle";

        public OracleConnectionHandler(JObject jsonAccess) : base(jsonAccess)
        {
        }

        public override DbCommand GetNewDbCommand(string commandText, DbConnection connection)
        {
            return new OracleCommand(commandText, (OracleConnection)connection);
        }

        public override DbConnection GetNewDbConnection()
        {
            var conString = new OracleConnectionStringBuilder
            {
                // Resulting connection string format should be like so for conString.DataSource:
                // "database.url:port/databasename"
                DataSource = _jsonAccess.SelectToken("DataSource").ToString(),
                Password = _jsonAccess.SelectToken("Password").ToString(),
                UserID = _jsonAccess.SelectToken("UserID").ToString()
            };

            return new OracleConnection(conString.ConnectionString);
        }

        public override bool IsConnectionStringMissingInfo()
        {
            return _jsonAccess == null
                    || _jsonAccess.SelectToken("DataSource") == null
                    || _jsonAccess.SelectToken("Password") == null
                    || _jsonAccess.SelectToken("UserID") == null
                    || _jsonAccess.SelectToken("DataSource").ToString() == ""
                    || _jsonAccess.SelectToken("UserID").ToString() == "";
        }
    }
}

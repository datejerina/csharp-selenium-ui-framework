using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data.SqlClient;

namespace AutomationFW.Common.DataAccess
{
    public class SqlConnectionHandler : DbConnectionHandlerBase
    {
        public override string DbType { get; set; } = "SQL";

        public SqlConnectionHandler(JObject jsonAccess) : base(jsonAccess)
        {
        }

        public override DbCommand GetNewDbCommand(string commandText, DbConnection connection)
        {
            return new SqlCommand(commandText, (SqlConnection)connection);
        }

        public override DbConnection GetNewDbConnection()
        {
            var conString = new SqlConnectionStringBuilder()
            {
                DataSource = _jsonAccess.SelectToken("DataSource").ToString(),
                IntegratedSecurity = true
            };

            return new SqlConnection(conString.ConnectionString);
        }

        public override bool IsConnectionStringMissingInfo()
        {
            return _jsonAccess == null
                   || _jsonAccess.SelectToken("DataSource") == null
                   || _jsonAccess.SelectToken("DataSource").ToString() == "";
        }
    }
}
using AutomationFW.Common.DataAccess.Interfaces;
using AutomationFW.Common.Helpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Serilog;
using System;
using System.Data;
using System.Data.Common;

namespace AutomationFW.Common.DataAccess
{
    public abstract class DbConnectionHandlerBase : IDbConnectionHandler
    {
        public abstract string DbType { get; set; }
        public DbConnection Connection;
        protected JObject _jsonAccess;
        public bool isConnectionActiveFlag = false;

        public DbConnectionHandlerBase(JObject jsonAccess)
        {
            _jsonAccess = jsonAccess;
            InitializeDbConnection();
        }

        public void InitializeDbConnection()
        {
            if (IsConnectionStringMissingInfo())
            {
                Log.Information($"Insufficient connection string credentials given for {DbType} database.  " +
                    $"{DbType} connection will not be opened.  Connection will remain flagged as inactive.",
                    LogTypes.DATAACCESS);

                return;
            }

            Connection = GetNewDbConnection();

            try
            {
                Connection.Open();

                Log.Information($"Connection Established ({Connection.ServerVersion}) to {DbType} Database.  " +
                    $"Connection flagged as active.", LogTypes.DATAACCESS);

                Connection.Close();
                isConnectionActiveFlag = true;
            }
            catch (Exception e)
            {
                Log.Fatal($"Exception thrown while attempting to connect to {DbType} Database: {e.Message}",
                    LogTypes.DATAACCESS);
            }
        }

        public DataTable PerformQuery(string commandText, int timeout = 60)
        {
            return PerformQuery(GetNewDbCommand(commandText, Connection));
        }

        public DataTable PerformQuery(DbCommand dbCommand, int timeout = 60)
        {
            if (!isConnectionActiveFlag)
            {
                Assert.Inconclusive("Cannot perform given query because DbConneciton is flagged as inactive.  " +
                    "Ensure credentials are correct in Access.json, and see log file for more details.");
            }

            dbCommand.Connection = Connection;
            dbCommand.CommandTimeout = timeout;
            var dt = new DataTable();

            try
            {
                Connection.Open();
                var dr = dbCommand.ExecuteReader();
                dt.Load(dr);
                Connection.Close();
            }
            catch (Exception e)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                Log.Fatal($"Exception thrown while attempting to read from {DbType} Database: " + e.Message,
                    LogTypes.DATAACCESS);

                Assert.Inconclusive($"Exception thrown while attempting to read from {DbType} Database: " + e.Message);
            }
            return dt;
        }

        public int PerformNonQuery(string commandText, int timeout = 60)
        {
            return PerformNonQuery(GetNewDbCommand(commandText, Connection));
        }

        public int PerformNonQuery(DbCommand dbCommand, int timeout = 60)
        {
            if (!isConnectionActiveFlag)
            {
                Assert.Inconclusive("Cannot perform given non-query because DbConneciton is flagged as inactive.  " +
                    "Ensure credentials are correct in Access.json, and see log file for more details.");
            }

            dbCommand.Connection = Connection;
            dbCommand.CommandTimeout = timeout;
            int result = 0;

            try
            {
                Connection.Open();
                result = dbCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception e)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                Log.Fatal($"Exception thrown while attempting to read from {DbType} Database: " + e.Message,
                    LogTypes.DATAACCESS);

                Assert.Inconclusive($"Exception thrown while attempting to read from {DbType} Database: " + e.Message);
            }

            return result;
        }

        public abstract DbConnection GetNewDbConnection();

        public abstract DbCommand GetNewDbCommand(string commandText, DbConnection connection);

        public abstract bool IsConnectionStringMissingInfo();
    }
}
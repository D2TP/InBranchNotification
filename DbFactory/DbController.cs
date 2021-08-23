using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DbFactory
{
    public class DbController : IDbController
    {
        private string _ConnectString;
        private string _DBProvider;
        private string _DatabaseType;
        private DbProviderFactory iFactory;
        private readonly SystemSettings _systemSettings;
        public DbConnectionParam ConnectionProperty { get; set; }

         
        public DbController(IMemoryCache memoryCache)
        { 
            _systemSettings = new SystemSettings(memoryCache);

            DbConnectionParam cParam =   _systemSettings.GetConnectionDetatils();

            _ConnectString = string.Format(cParam.DatabaseType.ToUpper() == CDatabaseType.Oracle ? CConnectString.Oracle
                                         : cParam.DatabaseType.ToUpper() == CDatabaseType.PostgreSQL ? CConnectString.PostgreSQL
                                         : CConnectString.SQLServer, cParam.Server, cParam.Port, cParam.DatabaseName, cParam.UserId, cParam.Password, cParam.MaxPoolSize, cParam.ConnectionTimeout);
            _DBProvider = cParam.ProviderName;
            _DatabaseType = cParam.DatabaseType;
            ConnectionProperty = cParam;

            try
            {
                switch (_DatabaseType)
                {
                    case CDatabaseType.Oracle:
                        DbProviderFactories.RegisterFactory(_DBProvider, OracleClientFactory.Instance);
                        DbProviderFactories.TryGetFactory(_DBProvider, out iFactory);
                        break;

                    case CDatabaseType.PostgreSQL:
                        DbProviderFactories.RegisterFactory(_DBProvider, NpgsqlFactory.Instance);
                        DbProviderFactories.TryGetFactory(_DBProvider, out iFactory);
                        break;

                    case CDatabaseType.SQLServer:
                        DbProviderFactories.RegisterFactory(_DBProvider, SqlClientFactory.Instance);
                        DbProviderFactories.TryGetFactory(_DBProvider, out iFactory);
                        break;


                    default:
                        throw new Exception($"Unsupported Database Specified [{_DatabaseType }]");
                }

            }
            catch (Exception) { throw; }
        }




        // DB Section
        public DbConnection DbOpenConnection()
        {
            var SQLConn = iFactory.CreateConnection();
            try
            {
                SQLConn.ConnectionString = _ConnectString;
                SQLConn.Open();
            }
            catch (Exception) { throw; }
            return SQLConn;
        }
        public DbCommand DbCreateCommand(DbConnection SQLConn)
        {
            var SQLCmd = SQLConn.CreateCommand();
            return SQLCmd;
        }
        public DbParameter DbAddParameter(string pName, object strParam)
        {
            DbParameter p = iFactory.CreateParameter();
            p.DbType = DbCommon.GetDbTypeByName((strParam) == null ? "DBNULL" : strParam.GetType().Name);
            p.Value = strParam;
            p.ParameterName = pName;
            return p;
        }
        public void DbCreateParameters(DbCommand sCmd, object[] strParam)
        {
            int pknt = strParam.Length;
            for (int j = 0; j <= pknt - 1; j++)
            {
                sCmd.Parameters.Add(DbAddParameter((j + 1).ToString(), strParam[j]));
            }
        }
        public bool IsDbConnected(out string errMsg)
        {
            errMsg = "";
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                return true;
            }
            catch (Exception ex)
            {
                errMsg = $"DBCHK-001: {ex.Message}";
                return false;
            }
        }


        // SQL Section
        public int SQLExecute(string sSQL)
        {
            int bRet = 0;
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                bRet = SQLCmd.ExecuteNonQuery();
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public int SQLExecute(string sSQL, object[] strParam)
        {
            int bRet = 0;
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                DbCreateParameters(SQLCmd, strParam);
                bRet = SQLCmd.ExecuteNonQuery();
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public int SQLExecuteTransactions(string sSQL, object[] strParam)
        {
            int bRet = 0;
            try
            {
                using TransactionScope txnScope = new TransactionScope();
                for (int i = 0; i < strParam.Length; i++)
                {
                    object[] nParam = strParam[i] as object[];
                    SQLExecute(sSQL, nParam);
                    bRet += 1;
                }
                txnScope.Complete();
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public int SQLExecuteTransactions(string[] sSQL, object[] strParam)
        {
            int bRet = 0;
            try
            {
                using TransactionScope txnScope = new TransactionScope();
                for (int i = 0; i < strParam.Length; i++)
                {
                    object[] nParam = strParam[i] as object[];
                    SQLExecute(sSQL[i], nParam);
                    bRet += 1;
                }
                txnScope.Complete();
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public string SQLSelect(string sSQL, object[] strParam)
        {
            string strResult = "";
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                DbCreateParameters(SQLCmd, strParam);
                using var RDR = SQLCmd.ExecuteReader();
                if (RDR.HasRows)
                {
                    RDR.Read();
                    strResult = RDR[0].ToString();
                }
            }
            catch (Exception) { throw; }
            return strResult;
        }
        public int SQLSelect(string sSQL, object[] strParam, out string[] strResult)
        {
            int bRet = 0;
            strResult = null;
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                DbCreateParameters(SQLCmd, strParam);
                using var RDR = SQLCmd.ExecuteReader();
                if (RDR.HasRows)
                {
                    while (RDR.Read())
                    {
                        strResult[bRet] = RDR[0].ToString();
                        bRet += 1;
                    }
                }
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public int SQLSelect(string sSQL, object[] strParam, out List<string> strResult)
        {
            int bRet = 0;
            strResult = null;
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                DbCreateParameters(SQLCmd, strParam);
                using var RDR = SQLCmd.ExecuteReader();
                if (RDR.HasRows)
                {
                    while (RDR.Read())
                    {
                        strResult.Add(RDR[0].ToString());
                        bRet += 1;
                    }
                }
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public int SQLSelect(string sSQL, object[] strParam, out Dictionary<string, string> strResult)
        {
            int bRet = 0;
            strResult = null;
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                DbCreateParameters(SQLCmd, strParam);
                using var RDR = SQLCmd.ExecuteReader();
                if (RDR.HasRows)
                {
                    while (RDR.Read())
                    {
                        strResult.Add(RDR[0].ToString(), RDR[1].ToString());
                        bRet += 1;
                    }
                }
            }
            catch (Exception) { throw; }
            return bRet;
        }
        public DataTable SQLFetch(string sSQL)
        {
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                DbDataAdapter SQLAdapter = iFactory.CreateDataAdapter();
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                SQLAdapter.SelectCommand = SQLCmd;
                using DataSet dataSet = new DataSet();
                SQLAdapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
            catch (Exception) { throw; }
        }
        public DataTable SQLFetch(string sSQL, object[] strParam)
        {
            try
            {
                using var SQLConn = DbOpenConnection();
                using var SQLCmd = DbCreateCommand(SQLConn);
                DbDataAdapter SQLAdapter = iFactory.CreateDataAdapter();
                SQLCmd.CommandText = DbCommon.TransformSQL(sSQL, _DatabaseType);
                DbCreateParameters(SQLCmd, strParam);
                SQLAdapter.SelectCommand = SQLCmd;
                using DataSet dataSet = new DataSet();
                SQLAdapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
            catch (Exception) { throw; }
        }


        public Task<int> SQLExecuteAsync(string sSQL)
        {
            // METHOD: #1 - Not sure if this TRY...CATCH is needed though but it was recommended by many
            try
            {
                int result = SQLExecute(sSQL);
                return Task.FromResult<int>(result);
            }
            catch (Exception ex)
            {
                return Task.FromException<int>(ex);
            }
        }
        public Task<int> SQLExecuteAsync(string sSQL, object[] strParam)
        {
            // METHOD: #2 - Not sure if this TRY...CATCH is needed though but it was recommended by many
            var tcs = new TaskCompletionSource<int>();
            try
            {
                int result = SQLExecute(sSQL, strParam);
                tcs.SetResult(result);
                return tcs.Task;
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
                return tcs.Task;
            }
        }
        public Task<int> SQLExecuteTransactionsAsync(string sSQL, object[] strParam)
        {
            int result = SQLExecuteTransactions(sSQL, strParam);
            return Task.FromResult<int>(result);
        }
        public Task<int> SQLExecuteTransactionsAsync(string[] sSQL, object[] strParam)
        {
            int result = SQLExecuteTransactions(sSQL, strParam);
            return Task.FromResult<int>(result);
        }
        public Task<string> SQLSelectAsync(string sSQL, object[] strParam)
        {
            string result = SQLSelect(sSQL, strParam);
            return Task.FromResult<string>(result);
        }
        public Task<int> SQLSelectAsync(string sSQL, object[] strParam, out string[] strResult)
        {
            int result = SQLSelect(sSQL, strParam, out strResult);
            return Task.FromResult<int>(result);
        }
        public Task<int> SQLSelectAsync(string sSQL, object[] strParam, out List<string> strResult)
        {
            int result = SQLSelect(sSQL, strParam, out strResult);
            return Task.FromResult<int>(result);
        }
        public Task<int> SQLSelectAsync(string sSQL, object[] strParam, out Dictionary<string, string> strResult)
        {
            int result = SQLSelect(sSQL, strParam, out strResult);
            return Task.FromResult<int>(result);
        }
        public Task<DataTable> SQLFetchAsync(string sSQL)
        {
            // METHOD: #1 - Not sure if this TRY...CATCH is needed though but it was recommended by many
            try
            {
                DataTable result = SQLFetch(sSQL);
                return Task.FromResult<DataTable>(result);
            }
            catch (Exception ex)
            {
                return Task.FromException<DataTable>(ex);
            }
        }
        public Task<DataTable> SQLFetchAsync(string sSQL, object[] strParam)
        {
            // METHOD: #1 - Not sure if this TRY...CATCH is needed though but it was recommended by many
            try
            {
                DataTable result = SQLFetch(sSQL, strParam);
                return Task.FromResult<DataTable>(result);
            }
            catch (Exception ex)
            {
                return Task.FromException<DataTable>(ex);
            }
        }

    }
}

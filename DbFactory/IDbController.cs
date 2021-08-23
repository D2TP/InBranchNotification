using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DbFactory
{
    public interface IDbController
    {
        DbConnectionParam ConnectionProperty { get; set; }

        DbParameter DbAddParameter(string pName, object strParam);
        DbCommand DbCreateCommand(DbConnection SQLConn);
        void DbCreateParameters(DbCommand sCmd, object[] strParam);
        DbConnection DbOpenConnection();
        bool IsDbConnected(out string errMsg);
        int SQLExecute(string sSQL);
        int SQLExecute(string sSQL, object[] strParam);
        Task<int> SQLExecuteAsync(string sSQL);
        Task<int> SQLExecuteAsync(string sSQL, object[] strParam);
        int SQLExecuteTransactions(string sSQL, object[] strParam);
        int SQLExecuteTransactions(string[] sSQL, object[] strParam);
        Task<int> SQLExecuteTransactionsAsync(string sSQL, object[] strParam);
        Task<int> SQLExecuteTransactionsAsync(string[] sSQL, object[] strParam);
        DataTable SQLFetch(string sSQL);
        DataTable SQLFetch(string sSQL, object[] strParam);
        Task<DataTable> SQLFetchAsync(string sSQL);
        Task<DataTable> SQLFetchAsync(string sSQL, object[] strParam);
        string SQLSelect(string sSQL, object[] strParam);
        int SQLSelect(string sSQL, object[] strParam, out Dictionary<string, string> strResult);
        int SQLSelect(string sSQL, object[] strParam, out List<string> strResult);
        int SQLSelect(string sSQL, object[] strParam, out string[] strResult);
        Task<string> SQLSelectAsync(string sSQL, object[] strParam);
        Task<int> SQLSelectAsync(string sSQL, object[] strParam, out Dictionary<string, string> strResult);
        Task<int> SQLSelectAsync(string sSQL, object[] strParam, out List<string> strResult);
        Task<int> SQLSelectAsync(string sSQL, object[] strParam, out string[] strResult);
    }
}
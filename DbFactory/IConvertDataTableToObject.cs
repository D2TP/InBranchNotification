using System.Collections.Generic;
using System.Data;

namespace InBranchNotification.DbFactory
{
    public interface IConvertDataTableToObject
    {
        List<T> ConvertDataTable<T>(DataTable dt);
        T GetItem<T>(DataRow dr);
        List<T> ConvertDataRowList<T>(List<DataRow> dr);
    }
}
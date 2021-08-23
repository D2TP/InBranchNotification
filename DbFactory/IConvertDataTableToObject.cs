using System.Collections.Generic;
using System.Data;

namespace InBranchDashboard.DbFactory
{
    public interface IConvertDataTableToObject
    {
        List<T> ConvertDataTable<T>(DataTable dt);
        T GetItem<T>(DataRow dr);
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbFactory
{
    public static class DbCommon
    {
        private struct CDatabaseBindMarker
        {
            public const string Oracle = ":";
            public const string SQLServer = "@";
            public const string MySQL = "?";
            public const string PostgreSQL = "@";
        }

        public static string GetProviderFactoryClasses()
        {
            // Retrieve the installed providers and factories.
            var table = DbProviderFactories.GetFactoryClasses();
            StringBuilder ret = new StringBuilder();
            int i, j;
            try
            {
                j = 1;
                foreach (DataRow row in table.Rows)
                {
                    i = 0;
                    foreach (DataColumn column in table.Columns)
                    {
                        i = i + 1;
                        ret.Append(i == 1 ? string.Format("[{0}] {1} ", j, row[column]) :
                            i == 3 ? string.Format("  |  {0}", row[column]) : "");
                    }

                    ret.Append(Environment.NewLine);
                    j = j + 1;
                }
            }
            catch
            {
                throw;
            }

            return ret.ToString();
        }

        public static DbType GetDbTypeByName(string typeName)
        {
            var ColDbType = default(DbType);
            switch (typeName.ToUpper() ?? "")
            {
                case "STRING":
                    ColDbType = DbType.String;
                    break;

                case "DECIMAL":
                    ColDbType = DbType.Decimal;
                    break;

                case "DOUBLE":
                    ColDbType = DbType.Double;
                    break;

                case "DATETIME":
                    ColDbType = DbType.DateTime;
                    break;

                case "DBNULL":
                    ColDbType = DbType.String;
                    break;

                case "INT32":
                    ColDbType = DbType.Int32;
                    break;

                case "INT64":
                    ColDbType = DbType.Int64;
                    break;

                case "BYTE[]":
                    ColDbType = DbType.Binary;
                    break;
            }
            return ColDbType;
        }

        public static string TransformSQL(string sSQL, string sDBType, string sPlaceHolder = "#")
        {
            var sb = new StringBuilder(sSQL);
            int iknt = 0;
            string bindXter;
            try
            {
                int Location = sb.ToString().IndexOf(sPlaceHolder);
                while (Location > -1)
                {
                    iknt += 1;
                    bindXter = (sDBType ?? "") == CDatabaseType.MySQL ? CDatabaseBindMarker.MySQL : (sDBType ?? "") == CDatabaseType.Oracle ? CDatabaseBindMarker.Oracle + iknt.ToString() : CDatabaseBindMarker.SQLServer + iknt.ToString();
                    sb.Replace(sPlaceHolder, bindXter, Location, sPlaceHolder.Length);
                    Location = sb.ToString().IndexOf(sPlaceHolder);
                }
            }
            catch (Exception)
            {
                throw;
            }


            return sb.ToString();
        }
    }
}

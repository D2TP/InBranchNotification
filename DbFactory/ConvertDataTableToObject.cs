﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InBranchNotification.DbFactory
{


    public class ConvertDataTableToObject : IConvertDataTableToObject
    {
        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        object value = dr[column.ColumnName];
                        if (value == DBNull.Value) value = null;
                       
                        pro.SetValue(obj, value, null);
                    }
                    //       pro.SetValue(obj, dr[column.ColumnName], null);


                    else
                    {
                         continue;
                    }
                       
                }
            }
            return obj;
        }


        public List<T> ConvertDataRowList<T>(List<DataRow> dr)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dr)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

    }
}

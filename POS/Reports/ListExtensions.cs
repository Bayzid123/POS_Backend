using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Reports;
public static class ListExtensions
{
    public static DataTable ToDataTable<T>(this IList<T> data)
    {
        DataTable table = new DataTable();

        // Add columns to the table based on the properties of T
        foreach (var prop in typeof(T).GetProperties())
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        // Add rows to the table based on the data in the list
        foreach (T item in data)
        {
            DataRow row = table.NewRow();
            foreach (var prop in typeof(T).GetProperties())
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }
}

using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace LiteDB.Forms
{
    public static class LiteDataGridView
    {
        public static bool BindQuery(this DataGridView grid, string query, LiteDatabase db = null)
        {
            string exception = string.Empty;
            return grid.BindQuery(query, out exception, db);
        }

        public static bool BindQuery(this DataGridView grid, string query, out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            if (db == null) db = LiteDatabase.singleton;
            if (db == null) return false;

            try
            {
                var dataTable = new DataTable();
                var sqlAdatper = new SQLiteDataAdapter(query, db.Connection);
                sqlAdatper.Fill(dataTable);
                grid.DataSource = dataTable;

                sqlAdatper.Dispose();

            }
            catch (Exception e)
            {
                exception = e.ToString();
                return false;
            }

            return true;
        }
    }
}

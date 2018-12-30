using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
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
                var sqlAdatper = new SQLiteDataAdapter(SanitizeQuery(query), db.Connection);
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

        private static string SanitizeQuery(string query)
        {
            if(query.Contains("%"))
            {
                foreach(string match in ExtractFromString(query, "'%", "%'"))
                {
                    query = query.Replace(match, match.Replace("'", "''"));
                }
            }
            return query;
        }

        private static List<string> ExtractFromString(string source, string start, string end)
        {
            var results = new List<string>();

            string pattern = string.Format(
                "{0}({1}){2}",
                Regex.Escape(start),
                ".+?",
                 Regex.Escape(end));

            foreach (Match m in Regex.Matches(source, pattern))
            {
                results.Add(m.Groups[1].Value);
            }

            return results;
        }
    }
}

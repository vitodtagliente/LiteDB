using System;
using System.Data.SQLite;

namespace LiteDB
{
    public class LiteQuery
    {
        LiteDatabase Database;

        public LiteQuery(LiteDatabase db)
        {
            Database = db;
        }

        public bool Insert(string query)
        {
            string exception = string.Empty;
            return InsertWithException(query, out exception);
        }

        public bool InsertWithException(string query, out string exception)
        {
            exception = string.Empty;
            SQLiteCommand cmdInsert = new SQLiteCommand(query, Database.Connection);
            try
            {
                cmdInsert.ExecuteNonQuery();
                cmdInsert.Dispose();

                return true;
            }
            catch (SQLiteException e)
            {
                exception = e.ToString();
                cmdInsert.Dispose();
                return false;
            }
            catch (Exception e)
            {
                exception = e.ToString();
                cmdInsert.Dispose();
                return false;
            }
        }

        public int Count(string query)
        {
            var reader = Select(query);
            if (reader == null) return 0;
            try
            {
                reader.Read();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    Type type = reader.GetFieldType(i);
                    var value = reader.GetValue(i);

                    int count = 0;
                    int.TryParse(value.ToString(), out count);
                    return count;
                }
            }
            catch (Exception)
            {

            }

            if (reader != null) reader.Close();

            return 0;
        }

        public SQLiteDataReader Select(string query)
        {
            string exception = string.Empty;
            return SelectWithException(query, out exception);
        }

        public SQLiteDataReader SelectWithException(string query, out string exception)
        {
            exception = string.Empty;
            SQLiteCommand command = new SQLiteCommand(query, Database.Connection);
            try
            {
                SQLiteDataReader reader = command.ExecuteReader();
                command.Dispose();

                return reader;
            }
            catch (SQLiteException e)
            {
                exception = e.ToString();
                command.Dispose();
                return null;
            }
            catch (Exception e)
            {
                exception = e.ToString();
                command.Dispose();
                return null;
            }
        }
                
    }
}

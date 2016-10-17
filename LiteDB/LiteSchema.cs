using System;
using System.Text;

namespace LiteDB
{
    public static class LiteSchema<T> where T : LiteModel
    {
        public static string Name()
        {
            var instance = Activator.CreateInstance<T>();
            return instance.Tablename;
        }

        public static bool Drop(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            var instance = Activator.CreateInstance<T>();

            StringBuilder query = new StringBuilder();
            query.Append("DROP TABLE ");
            query.Append(instance.Tablename);

            if (db == null) db = LiteDatabase.singleton;
            if (db == null)
            {
                exception = "Database is NULL";
                return false;
            }

            return db.Query.InsertWithException(query.ToString(), out exception);
        }

        public static bool Drop(LiteDatabase db = null)
        {
            string exception = string.Empty;
            return Drop(out exception, db);
        }

        public static bool Exists(LiteDatabase db = null)
        {
            string exception = string.Empty;
            return Exists(out exception, db);
        }

        public static bool Exists(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            var instance = Activator.CreateInstance<T>();
            if (db == null) db = LiteDatabase.singleton;
            if (db == null)
            {
                return false;
            }

            var reader = db.Query.SelectWithException("SELECT 1 FROM " + instance.Tablename + " LIMIT 1", out exception);
            if (reader != null && reader.Read())
            {
                reader.Close();
                return true;
            }

            return false;
        }

        public static bool Create(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            var instance = Activator.CreateInstance<T>();

            if (db == null) db = LiteDatabase.singleton;
            if (db == null)
            {
                exception = "Database is NULL";
                return false;
            }

            return db.Query.InsertWithException(instance.ToSchemaQuery(), out exception);
        }

        public static bool Create(LiteDatabase db = null)
        {
            string exception = string.Empty;
            return Create(out exception, db);
        }

    }
}

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

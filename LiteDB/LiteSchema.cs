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

        public static void Clear(LiteDatabase db = null)
        {
            Clear(string.Empty);
        }

        public static void Clear(string condition, LiteDatabase db = null)
        {
            if (db == null) db = LiteDatabase.singleton;
            if (string.IsNullOrEmpty(condition) == false)
                condition = " WHERE " + condition;

            db.Query.Insert("DELETE FROM " + Name() + condition);
        }

        public static int Count(LiteDatabase db = null)
        {
            return Count(string.Empty);
        }

        public static int Count(string condition, LiteDatabase db = null)
        {
            if (db == null) db = LiteDatabase.singleton;
            if (string.IsNullOrEmpty(condition) == false)
                condition = " WHERE " + condition;

            return db.Query.Count("SELECT COUNT(*) FROM " + Name() + condition);
        }

        public static bool ResetAutoincrement(LiteDatabase db = null)
        {
            if (db == null) db = LiteDatabase.singleton;
            try
            {
                return db.Query.Insert(" UPDATE SQLITE_SEQUENCE SET SEQ = 0 WHERE NAME = '" + Name() + "'");
            }
            catch (Exception) { return false; }
        }

        public static int GetAutoIncrement(LiteDatabase db = null)
        {
            if (db == null) db = LiteDatabase.singleton;

            var reader = db.Query.Select(" SELECT SEQ FROM SQLITE_SEQUENCE WHERE NAME = '" + Name() + "'");
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

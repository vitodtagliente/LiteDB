using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDB
{
    public static class LiteSearch
    {
        public static T Find<T>(this LiteDatabase db, int id) where T : LiteModel
        {
            StringBuilder query = new StringBuilder();
            query.Append("Id = '");
            query.Append(id);
            query.Append("'");

            return Find<T>(db, query.ToString());
        }

        public static T Find<T>(this LiteDatabase db, string condition) where T : LiteModel
        {
            var instance = Activator.CreateInstance<T>();

            if (string.IsNullOrEmpty(condition) || db == null)
                return instance;

            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM ");
            query.Append(instance.Tablename);
            query.Append(" WHERE ");
            query.Append(condition);

            var reader = db.Query.Select(query.ToString());
            if (reader == null)
            {
                return instance;
            }

            try
            {
                reader.Read();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    Type type = reader.GetFieldType(i);
                    var value = reader.GetValue(i);

                    var fieldType = instance.GetType().GetField(name).FieldType;
                    Type converTo = Nullable.GetUnderlyingType(fieldType) ?? fieldType;

                    // Ci sono problemi nella coversione nel momento in cui 
                    // il campo è un DateTime e la stringa è in formato 00.00.00 anzicchè 00:00:00

                    // DateTime Fix
                    if (converTo == typeof(DateTime) && value.ToString().Contains("."))
                    {
                        value = value.ToString().Replace(".", ":");
                    }
                    // Float fix
                    if(converTo == typeof(float) && value.ToString().Contains("."))
                    {
                        value = value.ToString().Replace(".", ",");
                    }

                    var newValue = Convert.ChangeType(value, converTo);

                    instance[name] = newValue;
                }
            }
            catch (Exception)
            {
                if(reader != null) reader.Close();
                return Activator.CreateInstance<T>();
            }

            if(reader != null) reader.Close();

            return instance;
        }

        public static List<T> All<T>(this LiteDatabase db) where T : LiteModel
        {
            return All<T>(db, string.Empty);
        }

        public static List<T> All<T>(this LiteDatabase db, string expression) where T : LiteModel
        {
            var instance = Activator.CreateInstance<T>();

            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM ");
            query.Append(instance.Tablename);
            query.Append(" ");
            query.Append(expression);

            var list = new List<T>();

            if (db == null || db.Alive == false)
                return list;

            var reader = db.Query.Select(query.ToString());

            while (reader != null && reader.Read())
            {
                var element = Activator.CreateInstance<T>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    Type type = reader.GetFieldType(i);
                    var value = reader.GetValue(i);

                    var fieldType = instance.GetType().GetField(name).FieldType;
                    Type converTo = Nullable.GetUnderlyingType(fieldType) ?? fieldType;

                    // DateTime Fix
                    if (converTo == typeof(DateTime) && value.ToString().Contains("."))
                    {
                        value = value.ToString().Replace(".", ":");
                    }
                    // Float fix
                    if (converTo == typeof(float) && value.ToString().Contains("."))
                    {
                        value = value.ToString().Replace(".", ",");
                    }

                    element[name] = Convert.ChangeType(value, converTo);
                }

                list.Add(element);
            }

            if (reader != null) reader.Close();

            return list;
        }

        public static List<T> Select<T>(this LiteDatabase db, string condition) where T : LiteModel
        {
            var instance = Activator.CreateInstance<T>();

            var list = new List<T>();

            if (string.IsNullOrEmpty(condition))
                return list;

            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM ");
            query.Append(instance.Tablename);
            query.Append(" WHERE ");
            query.Append(condition);

            if (db == null || db.Alive == false)
                return list;
            
            var reader = db.Query.Select(query.ToString());

            while (reader != null && reader.Read())
            {
                var element = Activator.CreateInstance<T>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    Type type = reader.GetFieldType(i);
                    var value = reader.GetValue(i);

                    var fieldType = instance.GetType().GetField(name).FieldType;
                    Type converTo = Nullable.GetUnderlyingType(fieldType) ?? fieldType;

                    // DateTime Fix
                    if (converTo == typeof(DateTime) && value.ToString().Contains("."))
                    {
                        value = value.ToString().Replace(".", ":");
                    }

                    element[name] = Convert.ChangeType(value, converTo);
                }

                list.Add(element);
            }

            if(reader != null) reader.Close();

            return list;
        }
    }
}

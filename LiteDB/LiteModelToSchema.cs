using System;
using System.Text;
using System.Reflection;

namespace LiteDB
{
    static class LiteModelToSchema
    {
        //  Converte un tipo di sistema in un tipo per SQLite

        static string ToSQLiteType(object obj)
        {
            if (obj == null || obj.GetType() == typeof(string))
                return "TEXT";
            else if (obj.GetType() == typeof(int))
                return "INTEGER";
            else if (obj.GetType() == typeof(float))
                return "REAL";
            else if (obj.GetType() == typeof(double))
                return "REAL";
            else if (obj.GetType() == typeof(bool))
                return "TEXT";
            return string.Empty;
        }

        static string ToSQLiteAttributes(this LiteModel model, string name) 
        {
            StringBuilder str = new StringBuilder();

            Type type = model.GetType();
            var field = type.GetField(name);
            if (field != null)
            {
                var attributes = field.GetCustomAttribute<LiteField>();
                if (attributes != null)
                {
                    if (attributes.NotNULL) str.Append(" NOT NULL ");
                    else if (attributes.PrimaryKey) str.Append(" PRIMARY KEY ");
                    else if (attributes.Unique) str.Append(" UNIQUE ");

                    if (attributes.PrimaryKey && attributes.Autoincrement) str.Append(" AUTOINCREMENT ");
                }
            }

            return str.ToString();
        }

        //  Produci la query per la definizione dello schema della tabella di questo modello

        public static string ToSchemaQuery(this LiteModel model) 
        {
            StringBuilder str = new StringBuilder();

            str.Append("CREATE TABLE IF NOT EXISTS ");
            str.Append(model.Tablename);
            str.Append(" ( ");

            string comma = "";

            var fields = model.ToDictionary();

            foreach (var key in fields.Keys)
            {
                str.Append(comma);
                str.Append(key);
                str.Append(" ");
                str.Append(ToSQLiteType(fields[key]));
                str.Append(" ");
                str.Append(model.ToSQLiteAttributes(key));

                comma = ", ";
            }

            str.Append(" ) ");

            return str.ToString();
        }

    }
}

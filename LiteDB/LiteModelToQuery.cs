using System.Linq;
using System.Text;

namespace LiteDB
{
    static class LiteModelToQuery
    {
        public static string ToInsertQuery(this LiteModel model)
        {
            StringBuilder str = new StringBuilder();

            var dictionary = model.ToDictionary();

            str.Append("INSERT INTO ");
            str.Append(model.Tablename);
            str.Append(" ( ");

            string comma = string.Empty;

            foreach (var key in dictionary.Keys)
            {
                if (key == "Id" && dictionary[key].ToString() == "0") continue;

                str.Append(comma);
                str.Append(key);


                comma = ", ";
            }

            comma = string.Empty;

            str.Append(" ) VALUES ( ");

            foreach (var key in dictionary.Keys)
            {
                if (key == "Id" && dictionary[key].ToString() == "0") continue;

                str.Append(comma);
                str.Append("'");
                str.Append(dictionary[key]);
                str.Append("'");

                comma = ", ";
            }

            str.Append(" ) ");

            return str.ToString();
        }

        public static string ToUpdateQuery(this LiteModel model, string[] condition = null, string[] exception = null)
        {
            StringBuilder str = new StringBuilder();

            var dictionary = model.ToDictionary();

            if (exception == null)
                exception = new string[] { };

            if (condition == null)
                condition = new string[] { "Id" };

            str.Append("UPDATE ");
            str.Append(model.Tablename);
            str.Append(" SET ");

            string comma = string.Empty;

            foreach (var key in dictionary.Keys)
            {
                if (condition.Contains(key) || exception.Contains(key))
                    continue;

                str.Append(comma);
                str.Append(key);
                str.Append(" = '");
                str.Append(dictionary[key]);
                str.Append("'");

                comma = ", ";
            }

            str.Append(" WHERE ");

            comma = string.Empty;

            foreach (var key in condition)
            {
                if (dictionary.ContainsKey(key) == false)
                    continue;

                str.Append(comma);
                str.Append(key);
                str.Append(" = '");
                str.Append(dictionary[key]);
                str.Append("'");

                comma = " AND ";
            }

            return str.ToString();
        }
        
    }
}

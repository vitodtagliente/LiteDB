using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LiteDB
{
    public abstract class LiteModel
    {

        public string Tablename { get; private set; }

        //  Ogni modello è caratterizzato almeno dall'id

        [LiteField(PrimaryKey = true, Autoincrement = true)]
        public int Id;

        public LiteModel()
        {
            // Definisci il nome della tabella

            var type = this.GetType();

            var attributes = type.GetCustomAttribute<LiteTable>();
            if (attributes != null)
            {
                Tablename = attributes.Tablename;
            }
            else Tablename = type.Name;

            // Inizializza i datetime con la data corrente

            var fields = type.GetFields();
            foreach (var field in fields)
            {               
                object value = field.GetValue(this);
                if (value != null) {
                    if (value.GetType() == typeof(DateTime))
                        field.SetValue(this, DateTime.Now);
                } 
            }
        }
        
        //  Accesso con Reflection agli attributi del modello

        public object this[string fieldName]
        {
            get
            {
                var field = this.GetType().GetField(fieldName);
                if (field != null)
                    return field.GetValue(this);
                return null;
            }
            set
            {
                var field = this.GetType().GetField(fieldName);
                if (field != null)
                    field.SetValue(this, value);
            }
        }

        public bool Exists()
        {
            return Id != 0;
        }
                
        bool Insert(LiteDatabase db = null)
        {
            if (db == null) db = LiteDatabase.singleton;
            return db.Query.Insert(this.ToInsertQuery());
        }

        bool Insert(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            if (db == null) db = LiteDatabase.singleton;
            return db.Query.InsertWithException(this.ToInsertQuery(), out exception);
        }

        bool Update(LiteDatabase db = null)
        {
            if (db == null) db = LiteDatabase.singleton;
            return db.Query.Insert(this.ToUpdateQuery());
        }

        bool Update(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            if (db == null) db = LiteDatabase.singleton;
            return db.Query.InsertWithException(this.ToUpdateQuery(), out exception);
        }

        public bool Store(LiteDatabase db = null)
        {
            if (Exists())
                return Update(db);
            else return Insert(db);
        }

        public bool Store(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            if (Exists())
                return Update(out exception, db);
            else return Insert(out exception, db);
        }

        public bool New(LiteDatabase db = null)
        {
            return Insert(db);
        }

        public bool New(out string exception, LiteDatabase db = null)
        {
            exception = string.Empty;
            return Insert(out exception, db);
        }

        public bool Delete(LiteDatabase db = null)
        {
            string exception = string.Empty;
            return Delete(out exception, string.Empty, db);
        }

        public bool Delete(string condition, LiteDatabase db = null)
        {
            string exception = string.Empty;
            return Delete(out exception, string.Empty, db);
        }

        public bool Delete(out string exception, string condition = null, LiteDatabase db = null)
        {
            exception = string.Empty;
            if (Exists() == false) return false;

            if (db == null) db = LiteDatabase.singleton;

            string query = this.ToDeleteQuery();
            if (string.IsNullOrEmpty(condition) == false)
                query = this.ToDeleteQuery(condition);

            var result = db.Query.InsertWithException(query, out exception);
            //  Rendi il modello inutilizzabile per gli aggiornamenti
            if (result)
                this.Id = 0;
            return result;
        }

        //  Converte un modello in un dizionario del tipo attributo:valore

        public Dictionary<string, object> ToDictionary()
        {
            Type type = GetType();

            var fields = type.GetFields();

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                //  Assicurati di non inserire il nome della tabella tra queste informazioni
                if (field.Equals("Tablename")) continue;

                object value = field.GetValue(this);
                if (value != null) {
                    if (value.GetType() == typeof(string))
                        value = ((string)field.GetValue(this)).Replace("'", "''").Trim();
                    else if (value.GetType() == typeof(DateTime))
                        value = ((DateTime)field.GetValue(this)).ToString("yyyy-MM-dd HH:mm:ss");
                    else if (value.GetType() == typeof(float))
                        value = value.ToString().Replace(",", ".");
                } 
                dictionary.Add(field.Name, value);

            }

            return dictionary;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            var dictionary = ToDictionary();
            string comma = string.Empty;
            foreach(var elem in dictionary)
            {
                str.Append(comma);
                str.Append(elem.Key);
                str.Append(":");
                str.Append(elem.Value);
                comma = ", ";
            }
            return str.ToString();
        }

    }
}

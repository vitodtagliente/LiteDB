﻿using System;
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

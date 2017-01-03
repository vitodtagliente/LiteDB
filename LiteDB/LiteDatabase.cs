using System;
using System.Data.SQLite;
using System.IO;

namespace LiteDB
{
    public class LiteDatabase
    {
        public static LiteDatabase singleton { get; private set; }

        public string Filename { get; private set; }
        public SQLiteConnection Connection { get; private set; }

        public LiteQuery Query { get; private set; }

        public LiteDatabase(string filename)
        {
            if (singleton == null)
                singleton = this;

            Filename = filename;
            if (File.Exists(Filename) == false)
            {
                SQLiteConnection.CreateFile(Filename);
            }

            Query = new LiteQuery(this);
            Alive = false;
        }

        public void Open()
        {
            Connection = new SQLiteConnection("Data Source=" + Filename + ";Version=3;");
            Connection.Open();
            Alive = true;
        }

        public bool Alive { get; private set; }

        public void Close(bool clearAll = true)
        {
            try
            {
                Connection.Close();
                Connection.Shutdown();
                Connection.Dispose();
                if (clearAll)
                    SQLiteConnection.ClearAllPools();
                Alive = false;
            }
            catch (Exception) { }

            if (singleton == this)
                singleton = null;
        }
    }
}

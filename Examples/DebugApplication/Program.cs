using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace DebugApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            var db = new LiteDatabase("prova.db");
            db.Open();

            Console.WriteLine("Status Open:" + db.Alive);

            //  Creazione di tabelle

            //if (LiteSchema<Prova>.Drop())
            //    Console.WriteLine("Tabella eliminata");

            if (LiteSchema<Prova>.Create())
                Console.WriteLine("Tabella create");

            Console.WriteLine("Tablename: " + LiteSchema<Prova>.Name());

            //  Operazioni dui modelli
            
            var model = new Prova();
            model.Nome = "aaa";
            if (model.Store())
                Console.WriteLine("Inserito");


            var records = db.All<Prova>();
            foreach(var record in records)
            {
                Console.WriteLine(record.Data.AddDays(50));
                Console.WriteLine(record.ToString());                
            }

            Console.WriteLine("Elements Count: ");
            Console.WriteLine(LiteSchema<Prova>.Count());

            var adapterForm = new AdapterForm();
            adapterForm.ShowDialog();

            var gridForm = new GridForm();
            gridForm.ShowDialog();

            Console.ReadKey();
        }
    }
}

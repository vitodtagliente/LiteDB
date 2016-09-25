
namespace LiteDB
{
    //  Definisce il set di attributi associabili ai campi di un modello
    //  Operazione utile per la definizione dello schema della tabella SQLite

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class LiteField : System.Attribute
    {
        public bool NotNULL = false;
        public bool Unique = false;
        public bool PrimaryKey = false;
        public bool Autoincrement = false;
    }

}

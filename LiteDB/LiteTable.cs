
namespace LiteDB
{

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class LiteTable : System.Attribute
    {
        public string Tablename = string.Empty;
    }

}

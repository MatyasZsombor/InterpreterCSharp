using System;
namespace Interpreter
{
    public enum ObjectType
    {
        INTEGER,
        BOOLEAN,
        STRING,
        NULL,
        RETURNVALUE,
        ERROR,
        FUNCTION,
        BUILTIN,
        ARRAY
    }
    public interface EvObject
    {
        ObjectType Type();
        string Inspect();
    }

    public class Array : EvObject
    {
        public List<EvObject> elements { get; set; }

        public ObjectType Type()
        {
            return ObjectType.ARRAY;
        }

        public string Inspect()
        {
            List<string> el = new List<string>();

            foreach (var e in elements)
            {
                el.Add(e.Inspect());
            }

            return "[" + string.Join(", ", el) + "]";
        }
    }

    public class Error : EvObject
    {
        public string message { get; set; }

        public ObjectType Type()
        {
            return ObjectType.ERROR;
        }

        public string Inspect()
        {
            return "Error " + message;
        }
    }

    public class Integer : EvObject
    {
        public double value { get; set; }

        public ObjectType Type()
        {
            return ObjectType.INTEGER;
        }

        public string Inspect()
        {
            return value.ToString("");
        }
    }

    public class String : EvObject
    {
        public string value { get; set; }

        public ObjectType Type()
        {
            return ObjectType.STRING;
        }

        public string Inspect()
        {
            return value;
        }
    }

    public class Boolean : EvObject
    {
        public bool value { get; set; }

        public ObjectType Type()
        {
            return ObjectType.BOOLEAN;
        }

        public string Inspect()
        {
            return value.ToString().ToLower();
        }
    }

    public class Null : EvObject
    {
        public ObjectType Type()
        {
            return ObjectType.NULL;
        }

        public string Inspect()
        {
            return "null";
        }
    }

    public class ReturnValue : EvObject
    {
        public EvObject value { get; set; }

        public ObjectType Type()
        {
            return ObjectType.RETURNVALUE;
        }

        public string Inspect()
        {
            return value.Inspect();
        }
    }

    public class Function : EvObject
    {
        public List<Identifier> parameters { get; set; }
        public BlockStatement body { get; set; }
        public Environment environment { get; set; }

        public ObjectType Type()
        {
            return ObjectType.FUNCTION;
        }

        public string Inspect()
        {
            string str = "";
            str += "fn(";
            str += "fn(";
            str += string.Join(",", from parameter in parameters select parameter.TokenLiteral());
            str += ") {\n";
            str += body.ToString();
            str += "\n}";

            return str;
        }
    }
}
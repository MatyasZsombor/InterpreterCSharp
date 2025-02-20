namespace Interpreter;

public enum ObjectType
{
    Integer,
    Float,
    Boolean,
    String,
    Null,
    ReturnValue,
    Error,
    Function,
    Builtin,
    Array
}

public interface IEvObject
{
    ObjectType Type();
    string Inspect();
}

public class Array : IEvObject
{
    public required List<IEvObject> Elements { get; init; }
    
    public ObjectType Type() => ObjectType.Array;
    
    public string Inspect()
    => $"[{string.Join(", ",  Elements.Select(x => x.Inspect()))}]";
}

public class Error : IEvObject
{
    public required string Message { get; init; }
    
    public ObjectType Type() => ObjectType.Error;
    
    public string Inspect() => "Error: " + Message;
}

public class Integer : IEvObject
{
    public int Value { get; init; }
    
    public ObjectType Type() => ObjectType.Integer;
    
    public string Inspect() => Value.ToString("");
}

public class Float : IEvObject
{
    public double Value { get; init; }
    
    public ObjectType Type() => ObjectType.Float;
    
    public string Inspect() => Value.ToString("");
}

public class String : IEvObject
{
    public required string Value { get; init; }
    
    public ObjectType Type() => ObjectType.String;
    
    public string Inspect() => Value;
}

public class Boolean : IEvObject
{
    public bool Value { get; init; }
    
    public ObjectType Type() => ObjectType.Boolean;
    
    public string Inspect() => Value.ToString().ToLower();
}

public class Null : IEvObject
{
    public ObjectType Type() => ObjectType.Null;
    
    public string Inspect() => "null";
}

public class ReturnValue : IEvObject
{
    public required IEvObject Value { get; init; }
    
    public ObjectType Type() => ObjectType.ReturnValue;
    
    public string Inspect() => Value.Inspect();
}

public class Function : IEvObject
{
    public required List<Identifier> Parameters { get; init; }
    public required BlockStatement Body { get; init; }
    public required Environment Environment { get; init; }
    
    public ObjectType Type() => ObjectType.Function;
    
    public string Inspect() => $"fn({string.Join(",", Parameters.Select(x => x.TokenLiteral()))}){{\n{Body}\n}}";
}

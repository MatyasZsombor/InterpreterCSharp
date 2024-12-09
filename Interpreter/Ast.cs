namespace Interpreter;

public interface INode
{
    string TokenLiteral();
    string ToString();
}

public interface IStatement : INode;

public interface IExpression : INode;

public class Code : INode
{
    public List<IStatement> Statements { get; } = [];
    
    public string TokenLiteral() => Statements.Count > 0 ? Statements[0].TokenLiteral() : "";
    
    public override string ToString()
    {
        return Statements.Aggregate("", (current, item) => current + item.ToString());
    }
}

public class IndexExpression : IExpression
{
    public Token Token { get; init; }
    public required IExpression Left { get; init; }
    public required IExpression Index { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => $"({Left.ToString()}[{Index.ToString()}])";
}

public class ArrayLiteral : IExpression
{
    public Token Token { get; init; }
    public required List<IExpression> Elements { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => $"[{string.Join(",", Elements)}]";
}

public class ExpressionStatement : IStatement
{
    public Token Token;
    public required IExpression Expression;
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Expression.ToString();
}

public class LetStatement : IStatement
{
    public Token Token { get; init; }
    public required Identifier Name { get; init; }
    public required IExpression Value { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => $"{TokenLiteral()} {Name} = {Value.ToString()}";
}

public class ReturnStatement : IStatement
{
    public Token Token { get; init; }
    public required IExpression Value { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => $"{Token.Literal} {Value}";
}

public class BlockStatement : IStatement
{
    public Token Token { get; init; }
    public List<IStatement> Statements { get; init; } = [];
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
    {
        return Statements.Aggregate("{\n", (current, item) => current + item.ToString() + "\n") + "}";
    }
}

public class Identifier : IExpression
{
    public Token Token { get; init; }
    public required string Value { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Value;
}

public class IntegerLiteral : IExpression
{
    public Token Token { get; init; }
    public required int Value { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Token.Literal;
}

public class BooleanLiteral : IExpression
{
    public Token Token { get; init; }
    
    public required bool Value { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Token.Literal;
}

public class FunctionLiteral : IExpression
{
    public Token Token { get; init; }
    public List<Identifier> Parameters { get; init; } = [];
    public required BlockStatement Body { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
    {
        string str = "";
        str += Token.Literal;
        
        IEnumerable<string> parameters = from p in Parameters select p.Value;
        
        str += "(";
        str += string.Join(",", parameters) + ")";
        
        str += Body.ToString();
        
        return str;
    }
}

public class CallExpression : IExpression
{
    public Token Token { get; init; }
    public required IExpression Function { get; init; }
    public List<IExpression> Arguments { get; init; } = [];
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() =>
        $"{Function.ToString()}({string.Join(",", Arguments.Select(x => x.ToString()))})";
}

public class PrefixExpression : IExpression
{
    public Token Token { get; init; }
    public required string Op { get; init; }
    public required IExpression Right { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => "(" + Op + Right.ToString() + ")";
}

public class InfixExpression : IExpression
{
    public Token Token { get; init; }
    public required string Op { get; init; }
    public required IExpression Left { get; init; }
    public required IExpression Right { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => $"({Left.ToString()} {Op} {Right.ToString()})";
}

public class IfExpression : IExpression
{
    public Token Token { get; init; }
    public required IExpression Condition { get; init; }
    public required BlockStatement Consequence { get; init; }
    public BlockStatement? Alternative { get; set; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
    {
        string value = "if" + Condition.ToString() + " " + Consequence;
        
        if (Alternative != null)
        {
            value += "else" + Alternative;
        }
        
        return value;
    }
}

public class StringLiteral : IExpression
{
    public Token Token { get; init; }
    public required string Value { get; init; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Token.Literal;
}

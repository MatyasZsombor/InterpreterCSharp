using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Interpreter
{
	public interface Node
	{
		string TokenLiteral();
		string ToString();
	}

	public interface Statement : Node
	{

	}

	public interface Expression : Node
	{

	}

	public class Code : Node
	{
        public List<Statement> statements { get; set; } = new List<Statement>();

		public string TokenLiteral()
		{
			if(statements.Count > 0)
			{
				return statements[0].TokenLiteral();
			}
			return "";
		}
        public override string ToString()
        {
			var str = "";
			foreach (var item in statements)
			{
				str += item.ToString();
			}
			return str;
        }
    }

	public class IndexExpression : Expression
	{
		public Token token { get; set; }
		public Expression left { get; set; }
		public Expression index { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return $"({left.ToString()}[{index.ToString()}])";
        }
    }

	public class ArrayLiteral : Expression
	{
		public Token token { get; set; }
		public  List<Expression> elements { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return $"[{string.Join(",", elements)}]";
        }
    }

	public class ExpressionStatement : Statement
	{
		public Token token;
		public Expression expression;

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            if (expression != null)
                return expression.ToString();

            return "";
        }
    }

	public class LetStatement : Statement
	{
		public Token token { get; set; }
		public Identifier name { get; set; }
		public Expression value { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
			var sb = new StringBuilder();

			sb.Append(TokenLiteral() + " " + name + " = " + value?.ToString());

			return sb.ToString();
        }
    }

	public class ReturnStatement : Statement
	{
		public Token token { get; set; }
		public Expression value { get; set; }

		public string TokenLiteral()
		{
			return token.Literal; 
		}

        public override string ToString()
        {
            return $"{token.Literal} {value}";
        }
    }

	public class BlockStatement : Statement
	{
		public Token token { get; set; }
		public List<Statement> statements { get; set; } = new List<Statement>();

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
			string str = "{\n";
			foreach (var item in statements)
			{
				str += item.ToString() + "\n";
			}
			return str + "}";
        }
    }

	public class Identifier : Expression
	{
		public Token token { get; set; }
		public string value { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return value;
        }
    }

	public class IntegerLiteral : Expression
	{
		public Token token { get; set; }
		public int value { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return token.Literal;
        }
    }

	public class BooleanLiteral : Expression
	{
		public Token token { get; set; }
		public bool value { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return token.Literal;
        }
    }

	public class FunctionLiteral : Expression
	{
		public Token token { get; set; }
		public List<Identifier> parameters { get; set; } = new List<Identifier>();
		public BlockStatement body { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            string str = "";
            str += token.Literal;

            var _parameters = from p in parameters select p.value;

            str += "(";
            str += string.Join(",", _parameters) + ")";

            str += body.ToString();

            return str;
        }
    }

	public class CallExpression : Expression
	{
		public Token token { get; set; }
		public Expression function { get; set; }
		public List<Expression> arguments { get; set; } = new List<Expression>();

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            string str = "";

            str += function.ToString();
            str += "(";

            var _argumnets = from argument in arguments select argument.ToString();

            str += string.Join(",", _argumnets) + ")";

            return str;
        }

    }

	public class PrefixExpression : Expression
	{
		public Token token { get; set; }
        public string op { get; set; }
		public Expression right { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return "(" + op + right.ToString() + ")";
        }
    }

	public class InfixExpression : Expression
	{
		public Token token { get; set; }
		public string op { get; set; }
		public Expression left { get; set; }
		public Expression right { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return "(" + left?.ToString() + " " + op + " " + right?.ToString() + ")";
        }
    }

	public class IfExpression : Expression
	{
		public Token token { get; set; }
		public Expression condition { get; set; }
		public BlockStatement consequence { get; set; }
		public BlockStatement alternative { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}
        public override string ToString()
        {
            string value = "if" + condition.ToString() + " " + consequence;

			if(alternative != null)
			{
				value += "else" + alternative;
			}

			return value;
        }
    }

	public class StringLiteral : Expression
	{
		public Token token { get; set; }
		public string value { get; set; }

		public string TokenLiteral()
		{
			return token.Literal;
		}

        public override string ToString()
        {
            return token.Literal;
        }
    }
}
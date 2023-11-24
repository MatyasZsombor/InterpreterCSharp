using System;
using System.Linq.Expressions;

namespace Interpreter
{
    public enum BindingPower
    {
        LOWEST = 0,
        AND = 1,
        OR = 1,
        EQUALS = 2,
        COMPARISON = 3,
        SUM = 4,
        PRODUCT = 5,
        PREFIX = 6,
        CALL = 7,
        INDEX = 8
    }

    public class Parser
    {
        private Lexer lexer;

        private Dictionary<TokenType, PrefixParslet> prefixParslets = new Dictionary<TokenType, PrefixParslet>();
        private Dictionary<TokenType, InfixParslet> infixParslets = new Dictionary<TokenType, InfixParslet>();

        private Token curToken;
        private Token peekToken;
        public List<string> errors { get; } = new List<string>();

        public Parser(Lexer _lexer)
        {
            lexer = _lexer;

            RegisterPrefix(TokenType.IDENT, new IdentifierParslet());
            RegisterPrefix(TokenType.INT, new IntegerParslet());
            RegisterPrefix(TokenType.TRUE, new BooleanParslet());
            RegisterPrefix(TokenType.FALSE, new BooleanParslet());
            RegisterPrefix(TokenType.MINUS, new PrefixOperatorParslet());
            RegisterPrefix(TokenType.BANG, new PrefixOperatorParslet());
            RegisterPrefix(TokenType.LPAREN, new GroupedExpressionParslet());
            RegisterPrefix(TokenType.IF, new IfExpressionParslet());
            RegisterPrefix(TokenType.FUNCTION, new FunctionLiteralParslet());
            RegisterPrefix(TokenType.STRING, new StringLiteralParslet());
            RegisterPrefix(TokenType.LBRACKET, new ArrayLiteralParslet());

            RegisterInfix(TokenType.LBRACKET, new IndexExpressionParslet(BindingPower.INDEX, false));
            RegisterInfix(TokenType.AND, new InfixOperatorParslet(BindingPower.AND, false));
            RegisterInfix(TokenType.OR, new InfixOperatorParslet(BindingPower.OR, false));
            RegisterInfix(TokenType.EQ, new InfixOperatorParslet(BindingPower.EQUALS, false));
            RegisterInfix(TokenType.NOT_EQ, new InfixOperatorParslet(BindingPower.EQUALS, false));
            RegisterInfix(TokenType.LT, new InfixOperatorParslet(BindingPower.COMPARISON, false));
            RegisterInfix(TokenType.GT, new InfixOperatorParslet(BindingPower.COMPARISON, false));
            RegisterInfix(TokenType.PLUS, new InfixOperatorParslet(BindingPower.SUM, false));
            RegisterInfix(TokenType.MINUS, new InfixOperatorParslet(BindingPower.SUM, false));
            RegisterInfix(TokenType.ASTERISK, new InfixOperatorParslet(BindingPower.PRODUCT, false));
            RegisterInfix(TokenType.SLASH, new InfixOperatorParslet(BindingPower.PRODUCT, false));
            RegisterInfix(TokenType.LPAREN, new CallExpressionParslet(BindingPower.CALL, false));


            NextToken();
            NextToken();
        }

        public Code ParseCode()
        {
            Code program = new Code();

            while (!CurTokenIs(TokenType.EOF))
            {
                Statement statement = ParseStatement();
                if (statement != null)
                {
                    program.statements.Add(statement);
                }
                NextToken();
            }
            return program;
        }

        private void RegisterPrefix(TokenType tokenType, PrefixParslet parslet)
        {
            prefixParslets.Add(tokenType, parslet);
        }

        private void RegisterInfix(TokenType tokenType, InfixParslet parslet)
        {
            infixParslets.Add(tokenType, parslet);
        }

        public void NextToken()
        {
            curToken = peekToken;
            peekToken = lexer.NextToken();
        }

        public Statement ParseStatement()
        {
            switch (curToken.Type)
            {
                case TokenType.LET:
                    return ParseLetStatement();
                case TokenType.RETURN:
                    return ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            var stmt = new ExpressionStatement();
            stmt.token = curToken;
            stmt.expression = ParseExpression(BindingPower.LOWEST);

            if (PeekTokenIs(TokenType.SEMICOLON))
            {
                NextToken();
            }

            return stmt;
        }


        public Expression ParseExpression(BindingPower bindingPower)
        {
            var prefixParslet = GetPrefixParslet(curToken.Type);

            if (prefixParslet == null)
            {
                NoPrefixParseError(curToken.Type);
                return null;
            }
            var leftExp = prefixParslet.Parse(this, curToken);

            while (!PeekTokenIs(TokenType.SEMICOLON) && bindingPower < PeekBindingPower())
            {
                var infixParslet = GetInfixParslet(peekToken.Type);
                if (infixParslet == null)
                {
                    return leftExp;
                }

                NextToken();

                leftExp = infixParslet.Parse(this, leftExp, curToken);
            }

            return leftExp;
        }

        public Statement ParseLetStatement()
        {
            var stmt = new LetStatement() { token = curToken };

            if (!ExpectPeek(TokenType.IDENT))
            {
                return null;
            }

            stmt.name = new Identifier { token = curToken, value = curToken.Literal };

            if (!ExpectPeek(TokenType.ASSIGN))
            {
                return null;
            }

            NextToken();

            stmt.value = ParseExpression(BindingPower.LOWEST);

            if (PeekTokenIs(TokenType.SEMICOLON))
            {
                NextToken();
            }

            return stmt;
        }

        public ReturnStatement ParseReturnStatement()
        {
            var stmt = new ReturnStatement() { token = curToken };
            NextToken();

            stmt.value = ParseExpression(BindingPower.LOWEST);

            if (PeekTokenIs(TokenType.SEMICOLON))
            {
                NextToken();
            }
            
            return stmt;
        }

        public bool CurTokenIs(TokenType t)
        {
            return curToken.Type == t;
        }

        public bool PeekTokenIs(TokenType t)
        {
            return peekToken.Type == t;
        }

        public bool ExpectPeek(TokenType t)
        {
            if (PeekTokenIs(t))
            {
                NextToken();
                return true;
            }
            else
            {
                PeekError(t);
            }
            return false;
        }

        public void PeekError(TokenType t)
        {
            string msg = $"expected next token to be {t}, got instead {peekToken.Type}.";
            errors.Add(msg);
        }

        public void NoPrefixParseError(TokenType t)
        {
            string msg = $"no prefix parse function for {t} found.";
            errors.Add(msg);
        }

        private BindingPower PeekBindingPower()
        {
            if (!infixParslets.ContainsKey(peekToken.Type)) return BindingPower.LOWEST;

            InfixParslet parslet = infixParslets[peekToken.Type];
            return parslet.GetBindingPower();
        }

        interface PrefixParslet
        {
            Expression Parse(Parser parser, Token token);
        }

        interface InfixParslet
        {
            Expression Parse(Parser parser, Expression function, Token token);
            BindingPower GetBindingPower();
        }

        private PrefixParslet GetPrefixParslet(TokenType type)
        {
            if (prefixParslets.ContainsKey(type))
            {
                return prefixParslets[type];
            }
            return null;
        }

        private InfixParslet GetInfixParslet(TokenType type)
        {
            if (infixParslets.ContainsKey(type))
            {
                return infixParslets[type];
            }
            return null;
        }

        private List<Expression> ParseExpressionList(Parser parser)
        {
            List<Expression> list = new List<Expression>();

            if (parser.PeekTokenIs(TokenType.RBRACKET))
            {
                parser.NextToken();
                return list;
            }

            parser.NextToken();

            list.Add(parser.ParseExpression(BindingPower.LOWEST));

            while (parser.PeekTokenIs(TokenType.COMMA))
            {
                parser.NextToken();
                parser.NextToken();
                list.Add(parser.ParseExpression(BindingPower.LOWEST));
            }
            if (!parser.ExpectPeek(TokenType.RBRACKET))
            {
                return null;
            }
            return list;
        }

        public class IndexExpressionParslet : InfixParslet
        {
            private BindingPower bindingPower;
            public BindingPower GetBindingPower() => bindingPower;
            private bool isRight;

            public IndexExpressionParslet(BindingPower _bindingPower, bool _isRight)
            {
                bindingPower = _bindingPower;
                isRight = _isRight;
            }

            public Expression Parse(Parser parser, Expression left, Token token)
            {
                var exp = new IndexExpression { token = parser.curToken, left = left};

                parser.NextToken();

                exp.index = parser.ParseExpression(BindingPower.LOWEST);

                if (!parser.ExpectPeek(TokenType.RBRACKET))
                {
                    return null;
                }
                return exp;
            }
        }

        public class ArrayLiteralParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                return new ArrayLiteral
                {
                    token = token,
                    elements = parser.ParseExpressionList(parser)
                };
            }
        }

        public class StringLiteralParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                return new StringLiteral { token = token, value = token.Literal };
            }
        }

        public class IdentifierParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                return new Identifier() { token = token, value = token.Literal };
            }
        }

        public class IntegerParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                return new IntegerLiteral() { token = token, value = int.Parse(token.Literal) };
            }
        }

        public class BooleanParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                return new BooleanLiteral() { token = token, value = parser.CurTokenIs(TokenType.TRUE) };
            }
        }

        public class GroupedExpressionParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                parser.NextToken();

                var expression = parser.ParseExpression(BindingPower.LOWEST);

                if (!parser.ExpectPeek(TokenType.RPAREN))
                {
                    return null;
                }

                return expression;
            }
        }

        public class IfExpressionParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                var ifExpression = new IfExpression { token = parser.curToken };

                if (!parser.ExpectPeek(TokenType.LPAREN))
                {
                    return null;
                }

                parser.NextToken();

                ifExpression.condition = parser.ParseExpression(BindingPower.LOWEST);

                if (!parser.ExpectPeek(TokenType.RPAREN))
                {
                    return null;
                }

                if (!parser.ExpectPeek(TokenType.LBRACE))
                {
                    return null;
                }

                ifExpression.consequence = new BlockStatementParslet().Parse(parser);

                if (parser.PeekTokenIs(TokenType.ELSE))
                {
                    parser.NextToken();

                    if (!parser.ExpectPeek(TokenType.LBRACE))
                    {
                        return null;
                    }

                    ifExpression.alternative = new BlockStatementParslet().Parse(parser);
                }

                return ifExpression;

            }
        }

        public class FunctionLiteralParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                var functionLiteral = new FunctionLiteral { token = token };

                if (!parser.ExpectPeek(TokenType.LPAREN))
                {
                    return null;
                }

                functionLiteral.parameters = ParseFunctionParameters(parser);

                if (!parser.ExpectPeek(TokenType.LBRACE))
                {
                    return null;
                }

                functionLiteral.body = new BlockStatementParslet().Parse(parser);

                return functionLiteral;
            }

            public List<Identifier> ParseFunctionParameters(Parser parser)
            {


                var identifiers = new List<Identifier>();

                if (parser.PeekTokenIs(TokenType.RPAREN))
                {
                    parser.NextToken();
                    return identifiers;
                }

                parser.NextToken();

                var identifier = new Identifier { token = parser.curToken, value = parser.curToken.Literal };
                identifiers.Add(identifier);

                while (parser.PeekTokenIs(TokenType.COMMA))
                {
                    parser.NextToken();
                    parser.NextToken();
                    identifier = new Identifier { token = parser.curToken, value = parser.curToken.Literal };
                    identifiers.Add(identifier);
                }

                if (!parser.ExpectPeek(TokenType.RPAREN))
                {
                    return null;
                }

                return identifiers;

            }
        }

        public class BlockStatementParslet
        {
            public BlockStatement Parse(Parser parser)
            {
                var blockStatement = new BlockStatement { token = parser.curToken, statements = new List<Statement>() };

                parser.NextToken();

                while (!parser.CurTokenIs(TokenType.RBRACE) && !parser.PeekTokenIs(TokenType.EOF))
                {
                    var stmt = parser.ParseStatement();

                    if (stmt != null)
                    {
                        blockStatement.statements.Add(stmt);
                    }
                    parser.NextToken();
                }
                return blockStatement;
            }
        }

        public class PrefixOperatorParslet : PrefixParslet
        {
            public Expression Parse(Parser parser, Token token)
            {
                parser.NextToken();

                Expression operand = parser.ParseExpression(BindingPower.PREFIX);

                return new PrefixExpression() { token = token, op = token.Literal, right = operand };
            }
        }

        public class InfixOperatorParslet : InfixParslet
        {
            private BindingPower bindingPower;
            public BindingPower GetBindingPower() => bindingPower;
            private bool isRight;

            public InfixOperatorParslet(BindingPower _bindingPower, bool _isRight)
            {
                bindingPower = _bindingPower;
                isRight = _isRight;
            }

            public Expression Parse(Parser parser, Expression left, Token token)
            {
                var expression = new InfixExpression()
                {
                    token = token,
                    op = token.Literal,
                    left = left
                };

                parser.NextToken();
                expression.right = parser.ParseExpression(bindingPower);

                return expression;
            }
        }

        public class CallExpressionParslet : InfixParslet
        {
            private BindingPower bindingPower;
            public BindingPower GetBindingPower() => bindingPower;
            private bool isRight;

            public CallExpressionParslet(BindingPower _bindingPower, bool _isRight)
            {
                bindingPower = _bindingPower;
                isRight = _isRight;
            }

            public Expression Parse(Parser parser, Expression function, Token token)
            {
                var expression = new CallExpression { token = token, function = function };
                expression.arguments = ParseCallArguments(parser);
                return expression;
            }

            private List<Expression> ParseCallArguments(Parser parser)
            {
                var arguments = new List<Expression>();

                if (parser.PeekTokenIs(TokenType.RPAREN))
                {
                    parser.NextToken();
                    return arguments;
                }

                parser.NextToken();
                arguments.Add(parser.ParseExpression(BindingPower.LOWEST));

                while (parser.PeekTokenIs(TokenType.COMMA))
                {
                    parser.NextToken();
                    parser.NextToken();

                    arguments.Add(parser.ParseExpression(BindingPower.LOWEST));
                }

                if (!parser.ExpectPeek(TokenType.RPAREN))
                {
                    return null;
                }

                return arguments;
            }
        }
    }
}
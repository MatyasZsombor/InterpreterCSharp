namespace Interpreter;

public enum BindingPower
{
    Lowest = 0,
    And = 1,
    Or = 1,
    Equal = 2,
    Comparison = 3,
    Sum = 4,
    Product = 5,
    Prefix = 6,
    Call = 7,
    Index = 8
}

internal class ParserException : Exception;

public class Parser
{
    private readonly Lexer _lexer;
    
    private readonly Dictionary<TokenType, IPrefixParslet> _prefixParslets = new();
    private readonly Dictionary<TokenType, INfixParslet> _infixParslets = new();
    
    private Token _curToken;
    private Token _peekToken;
    public List<string> Errors { get; } = [];
    
    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        
        RegisterPrefix(TokenType.Ident, new IdentifierParslet());
        RegisterPrefix(TokenType.Int, new IntegerParslet());
        RegisterPrefix(TokenType.True, new BooleanParslet());
        RegisterPrefix(TokenType.False, new BooleanParslet());
        RegisterPrefix(TokenType.Minus, new PrefixOperatorParslet());
        RegisterPrefix(TokenType.Bang, new PrefixOperatorParslet());
        RegisterPrefix(TokenType.Lparen, new GroupedExpressionParslet());
        RegisterPrefix(TokenType.If, new IfExpressionParslet());
        RegisterPrefix(TokenType.Function, new FunctionLiteralParslet());
        RegisterPrefix(TokenType.String, new StringLiteralParslet());
        RegisterPrefix(TokenType.Lbracket, new ArrayLiteralParslet());
        
        RegisterInfix(TokenType.Lbracket, new IndexExpressionParslet(BindingPower.Index));
        RegisterInfix(TokenType.And, new InfixOperatorParslet(BindingPower.And));
        RegisterInfix(TokenType.Or, new InfixOperatorParslet(BindingPower.Or));
        RegisterInfix(TokenType.Eq, new InfixOperatorParslet(BindingPower.Equal));
        RegisterInfix(TokenType.NotEq, new InfixOperatorParslet(BindingPower.Equal));
        RegisterInfix(TokenType.Lt, new InfixOperatorParslet(BindingPower.Comparison));
        RegisterInfix(TokenType.Gt, new InfixOperatorParslet(BindingPower.Comparison));
        RegisterInfix(TokenType.Plus, new InfixOperatorParslet(BindingPower.Sum));
        RegisterInfix(TokenType.Minus, new InfixOperatorParslet(BindingPower.Sum));
        RegisterInfix(TokenType.Asterisk, new InfixOperatorParslet(BindingPower.Product));
        RegisterInfix(TokenType.Slash, new InfixOperatorParslet(BindingPower.Product));
        RegisterInfix(TokenType.Lparen, new CallExpressionParslet(BindingPower.Call));
        
        NextToken();
        NextToken();
    }
    
    public Code ParseCode()
    {
        Code program = new Code();
        List<TokenType> synchronizationPoints =
        [
            TokenType.Semicolon,
            TokenType.Eof
        ];
        
        while (!CurTokenIs(TokenType.Eof))
        {
            try
            {
                IStatement statement = ParseStatement();
                program.Statements.Add(statement);
            }
            catch
            {
                Resynchronize(synchronizationPoints);
            }
            
            NextToken();
        }
        
        return program;
    }
    
    private void Resynchronize(List<TokenType> synchronizationTokens)
    {
        while (!synchronizationTokens.Contains(_peekToken.Type))
        {
            NextToken();   
        }
    }
    
    private void RegisterPrefix(TokenType tokenType, IPrefixParslet parslet)
    {
        _prefixParslets.Add(tokenType, parslet);
    }
    
    private void RegisterInfix(TokenType tokenType, INfixParslet parslet)
    {
        _infixParslets.Add(tokenType, parslet);
    }
    
    private void NextToken()
    {
        _curToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }
    
    private IStatement ParseStatement() =>
        _curToken.Type switch
        {
            TokenType.Let    => ParseLetStatement(),
            TokenType.Return => ParseReturnStatement(),
            _                => ParseExpressionStatement()
        };
    
    private ExpressionStatement ParseExpressionStatement()
    {
        var stmt = new ExpressionStatement
        {
            Token = _curToken,
            Expression = ParseExpression(BindingPower.Lowest)
        };
        
        if (PeekTokenIs(TokenType.Semicolon))
        {
            NextToken();
        }
        
        return stmt;
    }
    
    private IExpression ParseExpression(BindingPower bindingPower)
    {
        var prefixParslet = GetPrefixParslet(_curToken.Type);
        
        var leftExp = prefixParslet.Parse(this, _curToken);
        
        while (!PeekTokenIs(TokenType.Semicolon) && bindingPower < PeekBindingPower())
        {
            var infixParslet = GetInfixParslet(_peekToken.Type);
            
            NextToken();
            
            leftExp = infixParslet.Parse(this, leftExp, _curToken);
        }
        
        return leftExp;
    }
    
    private IStatement ParseLetStatement()
    {
        Token curToken = _curToken;
        
        ExpectPeek(TokenType.Ident);
        
        Identifier name = new Identifier { Token = _curToken, Value = _curToken.Literal };
        
        ExpectPeek(TokenType.Assign);
        
        NextToken();
        
        IExpression value = ParseExpression(BindingPower.Lowest);
        
        if (PeekTokenIs(TokenType.Semicolon))
        {
            NextToken();
        }
        
        return new LetStatement { Token = curToken, Name = name, Value = value };
    }
    
    private ReturnStatement ParseReturnStatement()
    {
        Token curToken = _curToken;
        NextToken();
        
        IExpression value = ParseExpression(BindingPower.Lowest);
        
        if (PeekTokenIs(TokenType.Semicolon))
        {
            NextToken();
        }
        
        return new ReturnStatement { Token = curToken, Value = value };
    }
    
    private bool CurTokenIs(TokenType t) => _curToken.Type == t;
    
    private bool PeekTokenIs(TokenType t) => _peekToken.Type == t;
    
    private void ExpectPeek(TokenType t)
    {
        if (PeekTokenIs(t))
        {
            NextToken();
            return;
        }
        
        PeekError(t);
        
        throw new ParserException();
    }
    
    private void PeekError(TokenType t)
    {
        Errors.Add($"expected next token to be {t}, got instead {_peekToken.Type}.");
    }
    
    private void NoPrefixParseError(TokenType t)
    {
        Errors.Add($"no prefix parse function for {t} found.");
        
        throw new ParserException();
    }
    
    private void NoInfixParseError(TokenType t)
    {
        Errors.Add($"no infix parse function for {t} found.");
        
        throw new ParserException();
    }
    
    private BindingPower PeekBindingPower() =>
        !_infixParslets.TryGetValue(_peekToken.Type, out var parslet) ? BindingPower.Lowest : parslet.GetBindingPower();
    
    private interface IPrefixParslet
    {
        IExpression Parse(Parser parser, Token token);
    }
    
    private interface INfixParslet
    {
        IExpression Parse(Parser parser, IExpression function, Token token);
        BindingPower GetBindingPower();
    }
    
    private IPrefixParslet GetPrefixParslet(TokenType type)
    {
        if (_prefixParslets.TryGetValue(type, out var parslet))
        {
            return parslet;
        }
        
        NoPrefixParseError(type);
        
        throw new ParserException();
    }
    
    private INfixParslet GetInfixParslet(TokenType type)
    {
        if (_infixParslets.TryGetValue(type, out var parslet))
        {
            return parslet;
        }
        
        NoInfixParseError(type);
        
        throw new ParserException();
    }
    
    private static List<IExpression> ParseExpressionList(Parser parser)
    {
        List<IExpression> list = [];
        
        if (parser.PeekTokenIs(TokenType.Rbracket))
        {
            parser.NextToken();
            
            return list;
        }
        
        parser.NextToken();
        
        list.Add(parser.ParseExpression(BindingPower.Lowest));
        
        while (parser.PeekTokenIs(TokenType.Comma))
        {
            parser.NextToken();
            parser.NextToken();
            list.Add(parser.ParseExpression(BindingPower.Lowest));
        }
        
        parser.ExpectPeek(TokenType.Rbracket);
        
        return list;
    }
    
    private class IndexExpressionParslet(BindingPower bindingPower) : INfixParslet
    {
        public BindingPower GetBindingPower() => bindingPower;
        
        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            Token curToken = parser._curToken;
            
            parser.NextToken();
            
            IExpression index = parser.ParseExpression(BindingPower.Lowest);
            
            parser.ExpectPeek(TokenType.Rbracket);
            
            return new IndexExpression { Token = curToken, Index = index, Left = left };
        }
    }
    
    private class ArrayLiteralParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token) =>
            new ArrayLiteral
            {
                Token = token,
                Elements = ParseExpressionList(parser)
            };
    }
    
    private class StringLiteralParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token) =>
            new StringLiteral { Token = token, Value = token.Literal};
    }
    
    private class IdentifierParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token) => new Identifier { Token = token, Value = token.Literal };
    }
    
    private class IntegerParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token) =>
            new IntegerLiteral { Token = token, Value = int.Parse(token.Literal)};
    }
    
    private class BooleanParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token) =>
            new BooleanLiteral { Token = token, Value = token.Literal == "true"};
    }
    
    private class GroupedExpressionParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            parser.NextToken();
            
            var expression = parser.ParseExpression(BindingPower.Lowest);
            
            parser.ExpectPeek(TokenType.Rparen);
            
            return expression;
        }
    }
    
    private class IfExpressionParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            parser.ExpectPeek(TokenType.Lparen);
            
            parser.NextToken();
            
            IExpression condition = parser.ParseExpression(BindingPower.Lowest);
            
            parser.ExpectPeek(TokenType.Rparen);
            
            parser.ExpectPeek(TokenType.Lbrace);
            
            BlockStatement consequence = BlockStatementParslet.Parse(parser);
            IfExpression ifExpression = new IfExpression
                { Token = token, Condition = condition, Consequence = consequence };
            
            if (!parser.PeekTokenIs(TokenType.Else))
            {
                return ifExpression;
            }
            
            parser.NextToken();
            parser.ExpectPeek(TokenType.Lbrace);
            
            ifExpression.Alternative = BlockStatementParslet.Parse(parser);
            
            return ifExpression;
        }
    }
    
    private class FunctionLiteralParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            parser.ExpectPeek(TokenType.Lparen);
            
            List<Identifier> parameters = ParseFunctionParameters(parser);
            
            parser.ExpectPeek(TokenType.Lbrace);
            
            BlockStatement body = BlockStatementParslet.Parse(parser);
            
            return new FunctionLiteral { Token = token, Body = body, Parameters = parameters };
        }
        
        private static List<Identifier> ParseFunctionParameters(Parser parser)
        {
            var identifiers = new List<Identifier>();
            
            if (parser.PeekTokenIs(TokenType.Rparen))
            {
                parser.NextToken();
                
                return identifiers;
            }
            
            parser.NextToken();
            
            var identifier = new Identifier { Token = parser._curToken, Value = parser._curToken.Literal };
            identifiers.Add(identifier);
            
            while (parser.PeekTokenIs(TokenType.Comma))
            {
                parser.NextToken();
                parser.NextToken();
                identifier = new Identifier { Token = parser._curToken, Value = parser._curToken.Literal };
                identifiers.Add(identifier);
            }
            
            parser.ExpectPeek(TokenType.Rparen);
            
            return identifiers;
        }
    }
    
    private static class BlockStatementParslet
    {
        public static BlockStatement Parse(Parser parser)
        {
            var blockStatement = new BlockStatement { Token = parser._curToken, Statements = [] };
            
            parser.NextToken();
            
            while (!parser.CurTokenIs(TokenType.Rbrace) && !parser.PeekTokenIs(TokenType.Eof))
            {
                var stmt = parser.ParseStatement();
                
                try
                {
                    blockStatement.Statements.Add(stmt);
                }
                catch
                {
                    parser.Resynchronize([TokenType.Eof, TokenType.Semicolon]);
                }
                
                parser.NextToken();
            }
            
            return blockStatement;
        }
    }
    
    private class PrefixOperatorParslet : IPrefixParslet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            parser.NextToken();
            
            IExpression operand = parser.ParseExpression(BindingPower.Prefix);
            
            return new PrefixExpression { Token = token, Op = token.Literal, Right = operand };
        }
    }
    
    private class InfixOperatorParslet(BindingPower bindingPower) : INfixParslet
    {
        public BindingPower GetBindingPower() => bindingPower;
        
        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            parser.NextToken();
            IExpression right = parser.ParseExpression(bindingPower);
            
            return new InfixExpression { Token = token, Op = token.Literal, Left = left, Right = right };
        }
    }
    
    private class CallExpressionParslet(BindingPower bindingPower) : INfixParslet
    {
        public BindingPower GetBindingPower() => bindingPower;
        
        public IExpression Parse(Parser parser, IExpression function, Token token) =>
            new CallExpression
            {
                Token = token, Function = function,
                Arguments = ParseCallArguments(parser)
            };
        
        private static List<IExpression> ParseCallArguments(Parser parser)
        {
            var arguments = new List<IExpression>();
            
            if (parser.PeekTokenIs(TokenType.Rparen))
            {
                parser.NextToken();
                
                return arguments;
            }
            
            parser.NextToken();
            arguments.Add(parser.ParseExpression(BindingPower.Lowest));
            
            while (parser.PeekTokenIs(TokenType.Comma))
            {
                parser.NextToken();
                parser.NextToken();
                
                arguments.Add(parser.ParseExpression(BindingPower.Lowest));
            }
            
            parser.ExpectPeek(TokenType.Rparen);
            
            return arguments;
        }
    }
}

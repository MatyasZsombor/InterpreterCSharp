namespace Interpreter;

public class Lexer
{
    private readonly string _input;
    private int _position;
    private int _readPosition;
    private char _ch;
    
    public Lexer(string input)
    {
        _input = input;
        ReadChar();
    }
    
    private void ReadChar()
    {
        if(_readPosition > _input.Length - 1)
        {
            _ch = (char)0;
        }
        else
        {
            _ch = _input[_readPosition];
        }
        _position = _readPosition;
        _readPosition++;
    }
    
    public Token NextToken()
    {
        Token token = new Token();
        SkipWhiteSpace();
        
        switch (_ch)
        {
            case '[':
                token = NewToken(TokenType.Lbracket, "[");
                break;
            case ']':
                token = NewToken(TokenType.Rbracket, "]");
                break;
            case '=':
                if (PeekChar() == '=')
                {
                    ReadChar();
                    token = NewToken(TokenType.Eq, "==");
                }
                else
                {
                    token = NewToken(TokenType.Assign, "=");
                }
                break;
            case '&':
                if(PeekChar() == '&')
                {
                    ReadChar();
                    token = NewToken(TokenType.And, "&&");
                }
                break;
            case '|':
                if(PeekChar() == '|')
                {
                    ReadChar();
                    token = NewToken(TokenType.Or, "||");
                }
                break;
            case '+':
                token = NewToken(TokenType.Plus, "+");
                break;
            case '-':
                token = NewToken(TokenType.Minus, "-");
                break;
            case '!':
                if(PeekChar() == '=')
                {
                    char currentChar = _ch;
                    ReadChar();
                    token = new Token { Type = TokenType.NotEq, Literal = Convert.ToString(currentChar) + Convert.ToString(_ch) };
                }
                else
                {
                    token = NewToken(TokenType.Bang, "!");
                }
                break;
            case '*':
                token = NewToken(TokenType.Asterisk, "*");
                break;
            case '/':
                token = NewToken(TokenType.Slash, "/");
                break;
            case '<':
                token = NewToken(TokenType.Lt, "<");
                break;
            case '>':
                token = NewToken(TokenType.Gt, ">");
                break;
            case ',':
                token = NewToken(TokenType.Comma, ",");
                break;
            case ';':
                token = NewToken(TokenType.Semicolon, ";");
                break;
            case '.':
                token = NewToken(TokenType.Period, ".");
                break;
            case '(':
                token = NewToken(TokenType.Lparen, "(");
                break;
            case ')':
                token = NewToken(TokenType.Rparen, ")");
                break;
            case '{':
                token = NewToken(TokenType.Lbrace, "{");
                break;
            case '}':
                token = NewToken(TokenType.Rbrace, "}");
                break;
            case '"':
                ReadChar();
                token = NewStringToken();
                break;
            case (char)0:
                token = NewToken(TokenType.Eof, "");
                break;
            default:
                if (char.IsLetter(_ch))
                {
                    token.Literal = ReadIdentifier();
                    token.Type = Token.LookUpIdent(token.Literal);
                    return token;
                }
                
                if (char.IsDigit(_ch))
                {
                    token.Literal = ReadNumber();
                    token.Type = TokenType.Int;
                    return token;
                }
                token = NewToken(TokenType.Illegal, Convert.ToString(_ch));
                
                break;
        }
        ReadChar();
        return token;
    }
    
    private Token NewStringToken()
    {
        var token = new Token { Type = TokenType.String };
        
        int position = _position;
        
        while (_ch != '"')
        {
            ReadChar();
        }
        
        token.Literal = _input.Substring(position, _position - position);
        
        return token;
    }
    
    private static Token NewToken(TokenType type, string literal) =>
        new()
        {
            Type = type,
            Literal = literal
        };
    
    private string ReadIdentifier()
    {
        int position = _position;
        while (char.IsLetter(_ch))
        {
            ReadChar();
        }
        return _input.Substring(position, _position - position);
    }
    
    private string ReadNumber()
    {
        int position = _position;
        
        while (char.IsDigit(_ch))
        {
            ReadChar();
        }
        return _input.Substring(position, _position - position);
        
    }
    
    private void SkipWhiteSpace()
    {
        while(_ch is ' ' or '\t' or '\n' or '\r')
        {
            ReadChar();
        }
    }
    
    private char PeekChar()
    {
        if (_readPosition > _input.Length - 1)
        {
            return _ch = (char)0;
        }
        
        return _input[_readPosition];
    }
}
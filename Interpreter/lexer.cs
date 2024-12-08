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
                token = NewToken(TokenType.LBRACKET, "[");
                break;
            case ']':
                token = NewToken(TokenType.RBRACKET, "]");
                break;
            case '=':
                if (PeekChar() == '=')
                {
                    ReadChar();
                    token = NewToken(TokenType.EQ, "==");
                }
                else
                {
                    token = NewToken(TokenType.ASSIGN, "=");
                }
                break;
            case '&':
                if(PeekChar() == '&')
                {
                    ReadChar();
                    token = NewToken(TokenType.AND, "&&");
                }
                break;
            case '|':
                if(PeekChar() == '|')
                {
                    ReadChar();
                    token = NewToken(TokenType.OR, "||");
                }
                break;
            case '+':
                token = NewToken(TokenType.PLUS, "+");
                break;
            case '-':
                token = NewToken(TokenType.MINUS, "-");
                break;
            case '!':
                if(PeekChar() == '=')
                {
                    char currentChar = _ch;
                    ReadChar();
                    token = new Token { Type = TokenType.NOT_EQ, Literal = Convert.ToString(currentChar) + Convert.ToString(_ch) };
                }
                else
                {
                    token = NewToken(TokenType.BANG, "!");
                }
                break;
            case '*':
                token = NewToken(TokenType.ASTERISK, "*");
                break;
            case '/':
                token = NewToken(TokenType.SLASH, "/");
                break;
            case '<':
                token = NewToken(TokenType.LT, "<");
                break;
            case '>':
                token = NewToken(TokenType.GT, ">");
                break;
            case ',':
                token = NewToken(TokenType.COMMA, ",");
                break;
            case ';':
                token = NewToken(TokenType.SEMICOLON, ";");
                break;
            case '(':
                token = NewToken(TokenType.LPAREN, "(");
                break;
            case ')':
                token = NewToken(TokenType.RPAREN, ")");
                break;
            case '{':
                token = NewToken(TokenType.LBRACE, "{");
                break;
            case '}':
                token = NewToken(TokenType.RBRACE, "}");
                break;
            case '"':
                ReadChar();
                token = NewStringToken();
                break;
            case (char)0:
                token = NewToken(TokenType.EOF, "");
                break;
            default:
                if (char.IsLetter(_ch))
                {
                    token.Literal = ReadIdentifier();
                    token.Type = token.LookUpIdent(token.Literal);
                    return token;
                }
                
                if (char.IsDigit(_ch))
                {
                    token.Literal = ReadNumber();
                    token.Type = TokenType.INT;
                    return token;
                }
                token = NewToken(TokenType.ILLEGAL, Convert.ToString(_ch));
                
                break;
        }
        ReadChar();
        return token;
    }
    
    private Token NewStringToken()
    {
        var token = new Token { Type = TokenType.STRING };
        
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
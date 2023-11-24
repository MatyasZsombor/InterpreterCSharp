using System;

public class Lexer
{
    private readonly string _input;
    private int _position = 0;
    private int readPosition = 0;
    private char ch;

    public Lexer(string input)
    {
        _input = input;
        ReadChar();
    }

    private void ReadChar()
    {
        if(readPosition > _input.Length - 1)
        {
            ch = (char)0;
        }
        else
        {
            ch = _input[readPosition];
        }
        _position = readPosition;
        readPosition++;
    }

    public Token NextToken()
    {
        Token token = new Token();
        SkipWhiteSpace();

        switch (ch)
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
                    char currentChar = ch;
                    ReadChar();
                    token = new Token { Type = TokenType.NOT_EQ, Literal = Convert.ToString(currentChar) + Convert.ToString(ch) };
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
                if (char.IsLetter(ch))
                {
                    token.Literal = ReadIdentifier();
                    token.Type = token.LookUpIdent(token.Literal);
                    return token;
                }
                else if (char.IsDigit(ch))
                {
                    token.Literal = ReadNumber();
                    token.Type = TokenType.INT;
                    return token;
                }
                else
                {
                    token = NewToken(TokenType.ILLEGAL, Convert.ToString(ch));
                }
                break;
        }
        ReadChar();
        return token;
    }

    private Token NewStringToken()
    {
        var token = new Token() { Type = TokenType.STRING };

        var position = _position;

        while (ch != '"')
            ReadChar();

        token.Literal = _input.Substring(position, _position - position);

        return token;
    }

    private Token NewToken(TokenType type, string literal)
    {
        return new Token
        {
            Type = type,
            Literal = literal
        };
    }

    private string ReadIdentifier()
    {
        int position = _position;
        while (char.IsLetter(ch))
        {
            ReadChar();
        }
        return _input.Substring(position, _position - position);
    }

    private string ReadNumber()
    {
        int position = _position;

        while (char.IsDigit(ch))
        {
            ReadChar();
        }
        return _input.Substring(position, _position - position);

    }

    private void SkipWhiteSpace()
    {
        while(ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r')
        {
            ReadChar();
        }
    }

    private char PeekChar()
    {
        if (readPosition > _input.Length - 1)
        {
            return ch = (char)0;
        }
        else
        {
            return _input[readPosition];
        }
    }
}
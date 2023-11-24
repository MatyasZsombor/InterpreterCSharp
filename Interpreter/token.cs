public enum TokenType
{
    ILLEGAL,
    EOF,

    IDENT,
    INT,
    STRING,

    ASSIGN,
    PLUS,
    MINUS,
    BANG,
    ASTERISK,
    SLASH,
    EQ,
    NOT_EQ,
    AND,
    OR,

    LT,
    GT,

    COMMA,
    SEMICOLON,

    LPAREN,
    RPAREN,
    LBRACE,
    RBRACE,
    LBRACKET,
    RBRACKET,

    FUNCTION,
    LET,
    TRUE,
    FALSE,
    IF,
    ELSE,
    RETURN
}

public struct Token
{
    public TokenType Type { get; set; }
    public string Literal { get; set; }

    public TokenType LookUpIdent(string ident)
    {
        if (ident == "let")
        {
            return TokenType.LET;
        }
        else if (ident == "fn")
        {
            return TokenType.FUNCTION;
        }
        else if (ident == "true")
        {
            return TokenType.TRUE;
        }
        else if (ident == "false")
        {
            return TokenType.FALSE;
        }
        else if (ident == "if")
        {
            return TokenType.IF;
        }
        else if (ident == "else")
        {
            return TokenType.ELSE;
        }
        else if (ident == "return")
        {
            return TokenType.RETURN;
        }
        return TokenType.IDENT;
    }
}
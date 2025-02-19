namespace Interpreter;

public enum TokenType
{
    Illegal,
    Eof,
    
    Ident,
    Int,
    String,
    
    Assign,
    Plus,
    Minus,
    Bang,
    Asterisk,
    Slash,
    Eq,
    NotEq,
    And,
    Or,
    
    Lt,
    Gt,
    
    Comma,
    Semicolon,
    
    Lparen,
    Rparen,
    Lbrace,
    Rbrace,
    Lbracket,
    Rbracket,
    
    Function,
    Let,
    True,
    False,
    If,
    Else,
    While,
    Return
}

public struct Token
{
    public TokenType Type { get; set; }
    public string Literal { get; set; }
    
    public static TokenType LookUpIdent(string ident)
    {
        return ident switch
               {
                   "let"    => TokenType.Let,
                   "fn"     => TokenType.Function,
                   "true"   => TokenType.True,
                   "false"  => TokenType.False,
                   "if"     => TokenType.If,
                   "else"   => TokenType.Else,
                   "while"  => TokenType.While,
                   "return" => TokenType.Return,
                   _        => TokenType.Ident
               };
    }
}
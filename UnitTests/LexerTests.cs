namespace UnitTests;

public class LexerTests
{
    [Fact]
    public void Test1()
    {
        const string input = "4 + 3 - 2 * 5 10 + (2 + 3) 3 1 2";
        
        List<Token> expected =
        [
            new Token
            {
                Type = TokenType.Int,
                Literal = "4"
            },
            
            new Token
            {
                Type = TokenType.Plus,
                Literal = "+"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new Token
            {
                Type = TokenType.Minus,
                Literal = "-"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new Token
            {
                Type = TokenType.Asterisk,
                Literal = "*"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "5"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "10"
            },
            
            new Token
            {
                Type = TokenType.Plus,
                Literal = "+"
            },
            
            new Token
            {
                Type = TokenType.Lparen,
                Literal = "("
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new Token
            {
                Type = TokenType.Plus,
                Literal = "+"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new Token
            {
                Type = TokenType.Rparen,
                Literal = ")"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "1"
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new Token
            {
                Type = TokenType.Eof,
                Literal = ""
            }
        ];
        
        Lexer lexer = new Lexer(input);
        
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
    
    [Fact]
    public void Test2()
    {
        const string input = "! != ==";
        List<Token> expected =
        [
            new Token
            {
                Type = TokenType.Bang,
                Literal = "!"
            },
            
            new Token
            {
                Type = TokenType.NotEq,
                Literal = "!="
            },
            
            new Token
            {
                Type = TokenType.Eq,
                Literal = "=="
            },
            
            new Token
            {
                Type = TokenType.Eof,
                Literal = ""
            }
        ];
        
        Lexer lexer = new Lexer(input);
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
    
    [Fact]
    public void Test3()
    {
        const string input = "\"foobar\" \"foo bar\" ";
        List<Token> expected =
        [
            new Token
            {
                Type = TokenType.String,
                Literal = "foobar"
            },
            
            new Token
            {
                Type = TokenType.String,
                Literal = "foo bar"
            },
            
            new Token
            {
                Type = TokenType.Eof,
                Literal = ""
            }
        ];
        
        Lexer lexer = new Lexer(input);
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
    
    [Fact]
    public void Test4()
    {
        const string input = "[1, 2, 3]";
        List<Token> expected =
        [
            new Token
            {
                Type = TokenType.Lbracket,
                Literal = "["
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "1"
            },
            
            new Token
            {
                Type = TokenType.Comma,
                Literal = ","
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new Token
            {
                Type = TokenType.Comma,
                Literal = ","
            },
            
            new Token
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new Token
            {
                Type = TokenType.Rbracket,
                Literal = "]"
            },
            
            new Token
            {
                Type = TokenType.Eof,
                Literal = ""
            }
        ];
        
        Lexer lexer = new Lexer(input);
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
}
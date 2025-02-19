namespace UnitTests;

public class LexerTests
{
    [Fact]
    public void Test1()
    {
        const string input = "4 + 3 - 2 * 5 10 + (2 + 3) 3 1 2";
        
        List<Token> expected =
        [
            new()
            {
                Type = TokenType.Int,
                Literal = "4"
            },
            
            new()
            {
                Type = TokenType.Plus,
                Literal = "+"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new()
            {
                Type = TokenType.Minus,
                Literal = "-"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new()
            {
                Type = TokenType.Asterisk,
                Literal = "*"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "5"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "10"
            },
            
            new()
            {
                Type = TokenType.Plus,
                Literal = "+"
            },
            
            new()
            {
                Type = TokenType.Lparen,
                Literal = "("
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new()
            {
                Type = TokenType.Plus,
                Literal = "+"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new()
            {
                Type = TokenType.Rparen,
                Literal = ")"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "1"
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new()
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
            new()
            {
                Type = TokenType.Bang,
                Literal = "!"
            },
            
            new()
            {
                Type = TokenType.NotEq,
                Literal = "!="
            },
            
            new()
            {
                Type = TokenType.Eq,
                Literal = "=="
            },
            
            new()
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
            new()
            {
                Type = TokenType.String,
                Literal = "foobar"
            },
            
            new()
            {
                Type = TokenType.String,
                Literal = "foo bar"
            },
            
            new()
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
            new()
            {
                Type = TokenType.Lbracket,
                Literal = "["
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "1"
            },
            
            new()
            {
                Type = TokenType.Comma,
                Literal = ","
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "2"
            },
            
            new()
            {
                Type = TokenType.Comma,
                Literal = ","
            },
            
            new()
            {
                Type = TokenType.Int,
                Literal = "3"
            },
            
            new()
            {
                Type = TokenType.Rbracket,
                Literal = "]"
            },
            
            new()
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
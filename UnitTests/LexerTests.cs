namespace UnitTests;

public class LexerTests
{
    [Fact]
    public void Test1()
    {
        const string Input = "4 + 3 - 2 * 5 10 + (2 + 3) 3 1 2";
        
        List<Token> expected = new List<Token>
        {
            new()
            {
                Type = TokenType.INT,
                Literal = "4"
            },
            new()
            {
                Type = TokenType.PLUS,
                Literal = "+"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "3"
            },
            new()
            {
                Type = TokenType.MINUS,
                Literal = "-"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "2"
            },
            new()
            {
                Type = TokenType.ASTERISK,
                Literal = "*"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "5"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "10"
            },
            new()
            {
                Type = TokenType.PLUS,
                Literal = "+"
            },
            new()
            {
                Type = TokenType.LPAREN,
                Literal = "("
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "2"
            },
            new()
            {
                Type = TokenType.PLUS,
                Literal = "+"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "3"
            },
            new()
            {
                Type = TokenType.RPAREN,
                Literal = ")"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "3"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "1"
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "2"
            },
            new()
            {
                Type = TokenType.EOF,
                Literal = ""
            }
        };
        
        Lexer lexer = new Lexer(Input);
        
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
    
    [Fact]
    public void Test2()
    {
        const string Input = "! != ==";
        List<Token> expected = new List<Token>
        {
            new()
            {
                Type = TokenType.BANG,
                Literal = "!"
            },
            new()
            {
                Type = TokenType.NOT_EQ,
                Literal = "!="
            },
            new()
            {
                Type = TokenType.EQ,
                Literal = "=="
            },
            new()
            {
                Type = TokenType.EOF,
                Literal = ""
            }
        };
        
        Lexer lexer = new Lexer(Input);
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
    
    [Fact]
    public void Test3()
    {
        const string Input = "\"foobar\" \"foo bar\" ";
        List<Token> expected = new List<Token>
        {
            new()
            {
                Type = TokenType.STRING,
                Literal = "foobar"
            },
            new()
            {
                Type = TokenType.STRING,
                Literal = "foo bar"
            },
            new()
            {
                Type = TokenType.EOF,
                Literal = ""
            }
        };
        
        Lexer lexer = new Lexer(Input);
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
    
    [Fact]
    public void Test4()
    {
        const string Input = "[1, 2, 3]";
        List<Token> expected = new List<Token>
        {
            new()
            {
                Type = TokenType.LBRACKET,
                Literal = "["
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "1"
            },
            new()
            {
                Type = TokenType.COMMA,
                Literal = ","
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "2"
            },
            new()
            {
                Type = TokenType.COMMA,
                Literal = ","
            },
            new()
            {
                Type = TokenType.INT,
                Literal = "3"
            },
            new()
            {
                Type = TokenType.RBRACKET,
                Literal = "]"
            },
            new()
            {
                Type = TokenType.EOF,
                Literal = ""
            }
        };
        
        Lexer lexer = new Lexer(Input);
        foreach (var t in expected)
        {
            var curToken = lexer.NextToken();
            Assert.Equal(t, curToken);
        }
    }
}
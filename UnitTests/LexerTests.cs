using Xunit.Abstractions;

namespace UnitTests
{
    public class LexerTests
    {
        private readonly ITestOutputHelper output;

        public LexerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            const string input = "4 + 3 - 2 * 5 10 + (2 + 3) 3 1 2";

            List<Token> expected = new List<Token>
            {
              new Token()
              {
                Type = TokenType.INT,
                Literal = "4"
              },
              new Token()
              {
                Type = TokenType.PLUS,
                Literal = "+"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "3"
              },
              new Token()
              {
                Type = TokenType.MINUS,
                Literal = "-"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "2"
              },
              new Token()
              {
                Type = TokenType.ASTERISK,
                Literal = "*"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "5"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "10"
              },
              new Token()
              {
                Type = TokenType.PLUS,
                Literal = "+"
              },
              new Token()
              {
                Type = TokenType.LPAREN,
                Literal = "("
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "2"
              },
              new Token()
              {
                Type = TokenType.PLUS,
                Literal = "+"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "3"
              },
              new Token()
              {
                Type = TokenType.RPAREN,
                Literal = ")"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "3"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "1"
              },
              new Token()
              {
                Type = TokenType.INT,
                Literal = "2"
              },
              new Token()
              {
                Type = TokenType.EOF,
                Literal = ""
              }
            };

            Lexer lexer = new Lexer(input);

            foreach (var t in expected)
            {
                var curToken = lexer.NextToken();

                Console.WriteLine(curToken.ToString() + " " + t.ToString());
                Assert.Equal(t, curToken);
            }
        }

        [Fact]
        public void Test2()
        {
            const string input = "! != ==";
            List<Token> expected = new List<Token>()
            {
                new Token
                {
                    Type = TokenType.BANG,
                    Literal = "!"
                },
                new Token
                {
                    Type = TokenType.NOT_EQ,
                    Literal = "!="
                },
                new Token
                {
                    Type = TokenType.EQ,
                    Literal = "=="
                },
                new Token
                {
                    Type = TokenType.EOF,
                    Literal = ""
                }
            };

            Lexer lexer = new Lexer(input);
            foreach (var t in expected)
            {
                var curToken = lexer.NextToken();

                Console.WriteLine(curToken.ToString() + " " + t.ToString());
                Assert.Equal(t, curToken);
            }
        }

        [Fact]
        public void Test3()
        {
            const string input = "\"foobar\" \"foo bar\" ";
            List<Token> expected = new List<Token>()
            {
                new Token
                {
                    Type = TokenType.STRING,
                    Literal = "foobar"
                },
                new Token
                {
                    Type = TokenType.STRING,
                    Literal = "foo bar"
                },
                new Token
                {
                    Type = TokenType.EOF,
                    Literal = ""
                }
            };

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
            List<Token> expected = new List<Token>()
            {
                new Token
                {
                    Type = TokenType.LBRACKET,
                    Literal = "["
                },
                new Token
                {
                    Type = TokenType.INT,
                    Literal = "1"
                },
                new Token
                {
                    Type = TokenType.COMMA,
                    Literal = ","
                },
                new Token
                {
                    Type = TokenType.INT,
                    Literal = "2"
                },
                new Token
                {
                    Type = TokenType.COMMA,
                    Literal = ","
                },
                new Token
                {
                    Type = TokenType.INT,
                    Literal = "3"
                },
                new Token
                {
                    Type = TokenType.RBRACKET,
                    Literal = "]"
                },
                new Token
                {
                    Type = TokenType.EOF,
                    Literal = ""
                }
            };

            Lexer lexer = new Lexer(input);
            foreach (var t in expected)
            {
                var curToken = lexer.NextToken();
                Assert.Equal(t, curToken);
            }
        }
    }
}
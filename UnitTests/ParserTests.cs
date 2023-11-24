using Xunit.Abstractions;

namespace UnitTests;

public class ParserTests
{
    private readonly ITestOutputHelper output;

    public ParserTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void Test1()
    {
        Lexer lexer = new Lexer("42");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        Assert.Equal(new IntegerLiteral() { token = new Token() { Literal = "42", Type = TokenType.INT }, value = 42 }.ToString(), code.statements[0].ToString());
    }

    [Fact]
    public void Test2()
    {
        Lexer lexer = new Lexer("---42");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        Assert.Equal("(-(-(-42)))", code.statements[0].ToString());
    }

    [Fact]
    public void Test3()
    {
        Lexer lexer = new Lexer("-!!int");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        Assert.Equal("(-(!(!int)))", code.statements[0].ToString());
    }

    [Fact]
    public void Test4()
    {
        List<string> inputs = new List<string>()
            {
                "5 + 5;",
                "5 - 5;",
                "5 * 5;",
                "5 / 5;",
                "5 > 5;",
                "5 < 5;",
                "5 == 5;",
                "5 != 5;"
            };

        List<string> results = new List<string>()
            {
                "(5 + 5)",
                "(5 - 5)",
                "(5 * 5)",
                "(5 / 5)",
                "(5 > 5)",
                "(5 < 5)",
                "(5 == 5)",
                "(5 != 5)"
            };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(results[i], code.statements[0].ToString());
        }
    }

    [Fact]
    public void Test5()
    {
        Lexer lexer = new Lexer("false");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        CheckParserErrors(parser);

        Assert.Equal(new BooleanLiteral() { token = new Token() { Literal = "false", Type = TokenType.FALSE }, value = false }.ToString(), code.statements[0].ToString());

    }

    [Fact]
    public void Tets6()
    {
        List<string> inputs = new List<string>
            {
                "true",
                "false",
                "3 > 5 == false",
                "3 < 5 == true",
            };

        List<string> expected = new List<string>
            {
                "true",
                "false",
                "((3 > 5) == false)",
                "((3 < 5) == true)",
            };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }
    }

    [Fact]
    public void Test7()
    {
        List<string> inputs = new List<string>
            {
                "1 + (2 + 3) + 4",
                "(5 + 5) * 2",
                "2 / (5 + 5)",
                "-(5 + 5)",
                "!(true == true)"
            };

        List<string> expected = new List<string>
            {
                "((1 + (2 + 3)) + 4)",
                "((5 + 5) * 2)",
                "(2 / (5 + 5))",
                "(-(5 + 5))",
                "(!(true == true))"
            };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }
    }

    [Fact]
    public void Test8()
    {
        string input = "if (x < y) { x }";
        string expected = "if(x < y) {\nx\n}";

        Lexer lexer = new Lexer(input);

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        CheckParserErrors(parser);

        Assert.Equal(expected, code.statements[0].ToString());
    }

    [Fact]
    public void Test9()
    {
        string input = "if (x < y) { x } else { y }";
        string expected = "if(x < y) {\nx\n}else{\ny\n}";

        Lexer lexer = new Lexer(input);

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        CheckParserErrors(parser);

        Assert.Equal(expected, code.statements[0].ToString());
    }

    [Fact]
    public void Test10()
    {
        List<string> inputs = new List<string>()
        {
            "fn() {};",
            "fn(x) {};",
            "fn(x, y, z) {};",
            "fn(x, y) {x + y}"
        };

        List<string> expected = new List<string>()
        {
            "fn(){\n}",
            "fn(x){\n}",
            "fn(x,y,z){\n}",
            "fn(x,y){\n(x + y)\n}",
        };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }
    }

    [Fact]
    public void Test11()
    {
        List<string> inputs = new List<string>()
        {
            "a + add(b * c) + d",
            "add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))",
            "add(a + b + c * d / f + g)"
        };

        List<string> expected = new List<string>()
        {
            "((a + add((b * c))) + d)",
            "add(a,b,1,(2 * 3),(4 + 5),add(6,(7 * 8)))",
            "add((((a + b) + ((c * d) / f)) + g))"
        };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }

    }

    [Fact]
    public void Test12()
    {
        List<string> inputs = new List<string>()
        {
            "\"foobar\"",
            "\"hello world\";"
        };

        List<string> expected = new List<string>()
        {
            "foobar",
            "hello world"
        };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }

    }

    [Fact]
    public void Test13()
    {
        List<string> inputs = new List<string>()
        {
            "[1, 2 * 2, 3 + 3];",
            "[];",
            "a * b[2]",
            "b[1]",
            "2 * [1, 2][1]"
        };

        List<string> expected = new List<string>()
        {
            "[1,(2 * 2),(3 + 3)]",
            "[]",
            "(a * (b[2]))",
            "(b[1])",
            "(2 * ([1,2][1]))"
        };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }
    }

    [Fact]
    public void Test14()
    {
        List<string> inputs = new List<string>()
        {
            "a * [1, 2, 3, 4][b * c] * d;",
            "add(a * b[2], b[1], 2 * [1, 2][1]);"
        };

        List<string> expected = new List<string>()
        {
            "((a * ([1,2,3,4][(b * c)])) * d)",
            "add((a * (b[2])),(b[1]),(2 * ([1,2][1])))"
        };

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);

            Assert.Equal(expected[i], code.statements[0].ToString());
        }
    }

    bool CheckParserErrors(Parser parser)
    {
        List<string> errors = parser.errors;
        if (errors.Count == 0)
        {
            return false;
        }
        output.WriteLine($"Parser has {errors.Count} errors.");
        foreach (var item in errors)
        {
            output.WriteLine($"Parser error: {item}");
        }
        return true;
    }
}
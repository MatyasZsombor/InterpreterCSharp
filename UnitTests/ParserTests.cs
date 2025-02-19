using Xunit.Abstractions;

namespace UnitTests;

public class ParserTests(ITestOutputHelper output)
{
    [Fact]
    public void Test1()
    {
        Lexer lexer = new Lexer("42;");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();
        
        CheckParserErrors(parser);
        Assert.NotEmpty(code.Statements);
        
        Assert.Equal(new IntegerLiteral { Token = new Token { Literal = "42" , Type = TokenType.Int }, Value = 42}.ToString(), code.Statements[0].ToString());
    }

    [Fact]
    public void Test2()
    {
        Lexer lexer = new Lexer("---42;");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();
        
        Assert.NotEmpty(code.Statements);
        Assert.Equal("(-(-(-42)))", code.Statements[0].ToString());
    }

    [Fact]
    public void Test3()
    {
        Lexer lexer = new Lexer("-!!int;");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();
        
        Assert.NotEmpty(code.Statements);
        Assert.Equal("(-(!(!int)))", code.Statements[0].ToString());
    }

    [Fact]
    public void Test4()
    {
        List<string> inputs =
        [
            "5 + 5;",
            "5 - 5;",
            "5 * 5;",
            "5 / 5;",
            "5 > 5;",
            "5 < 5;",
            "5 == 5;",
            "5 != 5;",
            "4.5 + 5.5;",
            "4.5 - 5.5;",
            "4.5 * 5.5;",
            "4.5 / 5.5;",
        ];

        List<string> results =
        [
            "(5 + 5)",
            "(5 - 5)",
            "(5 * 5)",
            "(5 / 5)",
            "(5 > 5)",
            "(5 < 5)",
            "(5 == 5)",
            "(5 != 5)",
            "(4.5 + 5.5)",
            "(4.5 - 5.5)",
            "(4.5 * 5.5)",
            "(4.5 / 5.5)"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);

            Assert.Equal(results[i], code.Statements[0].ToString());
        }
    }

    [Fact]
    public void Test5()
    {
        Lexer lexer = new Lexer("false;");

        Parser parser = new Parser(lexer);

        var code = parser.ParseCode();

        CheckParserErrors(parser);
        Assert.NotEmpty(code.Statements);
        Assert.Equal(new BooleanLiteral { Token = new Token { Literal = "false", Type = TokenType.False }, Value = false}.ToString(), code.Statements[0].ToString());

    }

    [Fact]
    public void Test6()
    {
        List<string> inputs =
        [
            "true;",
            "false;",
            "3 > 5 == false;",
            "3 < 5 == true;"
        ];

        List<string> expected =
        [
            "true",
            "false",
            "((3 > 5) == false)",
            "((3 < 5) == true)"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }
    }

    [Fact]
    public void Test7()
    {
        List<string> inputs =
        [
            "1 + (2 + 3) + 4;",
            "(5 + 5) * 2;",
            "2 / (5 + 5);",
            "-(5 + 5);",
            "!(true == true);"
        ];

        List<string> expected =
        [
            "((1 + (2 + 3)) + 4)",
            "((5 + 5) * 2)",
            "(2 / (5 + 5))",
            "(-(5 + 5))",
            "(!(true == true))"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
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
        Assert.NotEmpty(code.Statements);
        Assert.Equal(expected, code.Statements[0].ToString());
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
        Assert.NotEmpty(code.Statements);
        Assert.Equal(expected, code.Statements[0].ToString());
    }

    [Fact]
    public void Test10()
    {
        List<string> inputs =
        [
            "fn() {};",
            "fn(x) {};",
            "fn(x, y, z) {};",
            "fn(x, y) {x + y};"
        ];

        List<string> expected =
        [
            "fn(){\n}",
            "fn(x){\n}",
            "fn(x,y,z){\n}",
            "fn(x,y){\n(x + y)\n}"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }
    }

    [Fact]
    public void Test11()
    {
        List<string> inputs =
        [
            "a + add(b * c) + d",
            "add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))",
            "add(a + b + c * d / f + g)"
        ];

        List<string> expected =
        [
            "((a + add((b * c))) + d)",
            "add(a,b,1,(2 * 3),(4 + 5),add(6,(7 * 8)))",
            "add((((a + b) + ((c * d) / f)) + g))"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }

    }

    [Fact]
    public void Test12()
    {
        List<string> inputs =
        [
            "\"foobar\"",
            "\"hello world\";"
        ];

        List<string> expected =
        [
            "foobar",
            "hello world"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }

    }

    [Fact]
    public void Test13()
    {
        List<string> inputs =
        [
            "[1, 2 * 2, 3 + 3];",
            "[];",
            "a * b[2]",
            "b[1]",
            "2 * [1, 2][1]"
        ];

        List<string> expected =
        [
            "[1,(2 * 2),(3 + 3)]",
            "[]",
            "(a * (b[2]))",
            "(b[1])",
            "(2 * ([1,2][1]))"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }
    }

    [Fact]
    public void Test14()
    {
        List<string> inputs =
        [
            "a * [1, 2, 3, 4][b * c] * d;",
            "add(a * b[2], b[1], 2 * [1, 2][1]);"
        ];

        List<string> expected =
        [
            "((a * ([1,2,3,4][(b * c)])) * d)",
            "add((a * (b[2])),(b[1]),(2 * ([1,2][1])))"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }
    }
    
    [Fact]
    public void Test15()
    {
        List<string> inputs =
        [
            "while(x < y){}",
            "while(x < y){x}",
            "while(x < y){x + y}"
        ];

        List<string> expected =
        [
            "while (x < y) {\n}",
            "while (x < y) {\nx\n}",
            "while (x < y) {\n(x + y)\n}"
        ];

        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);

            Parser parser = new Parser(lexer);

            var code = parser.ParseCode();

            CheckParserErrors(parser);
            Assert.NotEmpty(code.Statements);
            Assert.Equal(expected[i], code.Statements[0].ToString());
        }
    }
    
    private void CheckParserErrors(Parser parser)
    {
        List<string> errors = parser.Errors;
        
        output.WriteLine($"Parser has {errors.Count} errors.");
        foreach (var item in errors)
        {
            output.WriteLine($"Parser error: {item}");
        }
    }
}
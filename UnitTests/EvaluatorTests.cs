using Array = Interpreter.Array;
using Boolean = Interpreter.Boolean;
using Environment = Interpreter.Environment;
using String = Interpreter.String;

namespace UnitTests;

public class EvaluatorTests
{
    [Fact]
    public void Test1()
    {
        List<string> inputs =
        [
            "5",
            "10",
            "-5",
            "-10",
            "5 + 5 + 5 + 5 - 10",
            "2 * 2 * 2 * 2 * 2",
            "-50 + 100 + -50",
            "5 * 2 + 10",
            "5 + 2 * 10",
            "20 + 2 * -10",
            "50 / 2 * 2 + 10",
            "2 * (5 + 10)",
            "3 * 3 * 3 + 10",
            "3 * (3 * 3) + 10",
            "(5 + 10 * 2 + 15 / 3) * 2 + -10"
        ];
        
        List<double> expectedOutputs =
        [
            5,
            10,
            -5,
            -10,
            10,
            32,
            0,
            20,
            25,
            0,
            60,
            30,
            37,
            37,
            50
        ];
        
        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);
            Parser parser = new Parser(lexer);
            
            var program = parser.ParseCode();
            Evaluator evaluator = new Evaluator();
            
            Environment environment = new Environment();
            
            var result = evaluator.Eval(program, environment);
            
            Assert.NotNull(result);
            Assert.Equal(expectedOutputs[i], ((Integer)result).Value);
        }
    }
    
    [Fact]
    public void Test2()
    {
        List<string> inputs =
        [
            "true",
            "false",
            "!true",
            "!false",
            "1 < 2",
            "1 > 2",
            "1 < 1",
            "1 > 1",
            "1 == 1",
            "1 != 1",
            "1 == 2",
            "1 != 2",
            "true == true",
            "false == false",
            "true == false",
            "true != false",
            "false != true",
            "(1 < 2) == true",
            "(1 < 2) == false",
            "(1 > 2) == true",
            "(1 > 2) == false"
        ];
        
        List<bool> expected =
        [
            true,
            false,
            false,
            true,
            true,
            false,
            false,
            false,
            true,
            false,
            false,
            true,
            true,
            true,
            false,
            true,
            true,
            true,
            false,
            false,
            true
        ];
        
        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);
            Parser parser = new Parser(lexer);
            
            var program = parser.ParseCode();
            Evaluator evaluator = new Evaluator();
            
            Environment environment = new Environment();
            
            var result = evaluator.Eval(program, environment);
            
            Assert.NotNull(result);
            Assert.Equal(expected[i], ((Boolean)result).Value);
        }
    }
    
    [Fact]
    public void Test3()
    {
        List<string> inputs =
        [
            "if (true) { 10 }",
            "if (false) { 10 }",
            "if (1) { 10 }",
            "if (1 < 2) { 10 }",
            "if (1 > 2) { 10 }",
            "if (1 > 2) { 10 } else { 20 }",
            "if (1 < 2) { 10 } else { 20 }"
        ];
        
        List<int> expected =
        [
            10,
            0,
            10,
            10,
            0,
            20,
            10
        ];
        
        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);
            Parser parser = new Parser(lexer);
            
            var program = parser.ParseCode();
            Evaluator evaluator = new Evaluator();
            
            Environment environment = new Environment();
            
            var result = evaluator.Eval(program, environment);
            
            Assert.NotNull(result);
            if (expected[i] == 0)
            {
                Assert.Equal("null", result.Inspect());
                continue;
            }
            Assert.Equal(expected[i], ((Integer)result).Value);
        }
    }
    
    [Fact]
    public void Test4()
    {
        List<string> inputs =
        [
            "return 10;",
            "return 10; 9;",
            "return 2 * 5; 9;",
            "9; return 2 * 5; 9;",
            "if (10 > 1) {\n     if (10 > 1) {\n       return 10;\n}\n     return 1\n}"
        ];
        
        List<double> expected =
        [
            10,
            10,
            10,
            10,
            10
        ];
        
        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);
            Parser parser = new Parser(lexer);
            
            var program = parser.ParseCode();
            Evaluator evaluator = new Evaluator();
            
            Environment environment = new Environment();
            
            var result = evaluator.Eval(program, environment);
            
            Assert.NotNull(result);
            if (expected[i] == 0)
            {
                Assert.Equal("null", result.Inspect());
                continue;
            }
            Assert.Equal(expected[i], ((Integer)result).Value);
        }
    }
    
    [Fact]
    public void Test5()
    {
        List<string> inputs =
        [
            "\"foobar\"",
            "\"Hello World!\"",
            "\"Hello\" + \" \" + \"World!\""
        ];
        
        List<string> expected =
        [
            "foobar",
            "Hello World!",
            "Hello World!"
        ];
        
        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);
            Parser parser = new Parser(lexer);
            
            var program = parser.ParseCode();
            Evaluator evaluator = new Evaluator();
            
            Environment environment = new Environment();
            
            var result = evaluator.Eval(program, environment);
            
            Assert.NotNull(result);
            Assert.Equal(expected[i], ((String)result).Value);
        }
    }
    
    [Fact]
    public void Test6()
    {
        List<string> inputs =
        [
            "[1, 2 * 2, 3 + 3];",
            "[];"
        ];
        
        List<string> expected =
        [
            "[1, 4, 6]",
            "[]"
        ];
        
        for (int i = 0; i < inputs.Count; i++)
        {
            Lexer lexer = new Lexer(inputs[i]);
            Parser parser = new Parser(lexer);
            
            var program = parser.ParseCode();
            Evaluator evaluator = new Evaluator();
            
            Environment environment = new Environment();
            
            var result = evaluator.Eval(program, environment);
            
            Assert.NotNull(result);
            Assert.Equal(expected[i], ((Array)result).Inspect());
        }
    }
}
using Xunit.Abstractions;

namespace UnitTests
{
	public class EvaluatorTests
	{
		private readonly ITestOutputHelper output;

		public EvaluatorTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void Test1()
		{
			List<string> inputs = new List<string>
			{
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
			};

			List<double> expectedOutputs = new List<double>
			{
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
			};

			for (int i = 0; i < inputs.Count; i++)
			{
				Lexer lexer = new Lexer(inputs[i]);
				Parser parser = new Parser(lexer);

				var programm = parser.ParseCode();
				Evaluator evaluator = new Evaluator();

				Interpreter.Environment environment = new Interpreter.Environment();

				var result = evaluator.Eval(programm, environment);

				Assert.NotNull(result);
				Assert.Equal(expectedOutputs[i], (result as Integer).value);
			}
		}

		[Fact]
		public void Test2()
		{
			List<string> inputs = new List<string>
			{
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
			};

			List<bool> expected = new List<bool>
			{
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
			};

			for (int i = 0; i < inputs.Count; i++)
			{
				Lexer lexer = new Lexer(inputs[i]);
				Parser parser = new Parser(lexer);

				var programm = parser.ParseCode();
				Evaluator evaluator = new Evaluator();

				Interpreter.Environment environment = new Interpreter.Environment();

				var result = evaluator.Eval(programm, environment);

				Assert.NotNull(result);
				Assert.Equal(expected[i], (result as Interpreter.Boolean).value);
			}
		}

		[Fact]
		public void Test3()
		{
			List<string> inputs = new List<string>
			{
				"if (true) { 10 }",
				"if (false) { 10 }",
				"if (1) { 10 }",
				"if (1 < 2) { 10 }",
				"if (1 > 2) { 10 }",
				"if (1 > 2) { 10 } else { 20 }",
				"if (1 < 2) { 10 } else { 20 }",
			};

			List<int> expected = new List<int>
			{
				10,
				0,
				10,
				10,
				0,
				20,
				10
			};

			for (int i = 0; i < inputs.Count; i++)
			{
				Lexer lexer = new Lexer(inputs[i]);
				Parser parser = new Parser(lexer);

				var programm = parser.ParseCode();
				Evaluator evaluator = new Evaluator();

				Interpreter.Environment enviromment = new Interpreter.Environment();

				var result = evaluator.Eval(programm, enviromment);

				Assert.NotNull(result);
				if (expected[i] == 0)
				{
					Assert.Equal("null", result.Inspect());
					continue;
				}
				Assert.Equal(expected[i], (result as Interpreter.Integer).value);
			}
		}

		[Fact]
		public void Test4()
		{
			List<string> inputs = new List<string>
			{
				"return 10;",
				"return 10; 9;",
				"return 2 * 5; 9;",
				"9; return 2 * 5; 9;",
				"if (10 > 1) {\n     if (10 > 1) {\n       return 10;\n}\n     return 1\n}"
			};

			List<double> expected = new List<double>
			{
				10,
				10,
				10,
				10,
				10
			};

			for (int i = 0; i < inputs.Count; i++)
			{
				Lexer lexer = new Lexer(inputs[i]);
				Parser parser = new Parser(lexer);

				var programm = parser.ParseCode();
				Evaluator evaluator = new Evaluator();

				Interpreter.Environment environment = new Interpreter.Environment();

				var result = evaluator.Eval(programm, environment);

				Assert.NotNull(result);
				if (expected[i] == 0)
				{
					Assert.Equal("null", result.Inspect());
					continue;
				}
				Assert.Equal(expected[i], (result as Interpreter.Integer).value);
			}
		}
		
		[Fact]
		public void Test5()
		{
			List<string> inputs = new List<string>
			{
				"\"foobar\"",
				"\"Hello World!\"",
				"\"Hello\" + \" \" + \"World!\""
            };

			List<string> expected = new List<string>
			{
				"foobar",
				"Hello World!",
                "Hello World!"
            };

            for (int i = 0; i < inputs.Count; i++)
            {
                Lexer lexer = new Lexer(inputs[i]);
                Parser parser = new Parser(lexer);

                var programm = parser.ParseCode();
                Evaluator evaluator = new Evaluator();

                Interpreter.Environment environment = new Interpreter.Environment();

                var result = evaluator.Eval(programm, environment);

                Assert.NotNull(result);
                Assert.Equal(expected[i], (result as Interpreter.String).value);
            }
        }

		[Fact]
		public void Test6()
		{
            List<string> inputs = new List<string>
            {
                "[1, 2 * 2, 3 + 3];",
                "[];",
            };

            List<string> expected = new List<string>
            {
                "[1, 4, 6]",
                "[]",
            };

            for (int i = 0; i < inputs.Count; i++)
            {
                Lexer lexer = new Lexer(inputs[i]);
                Parser parser = new Parser(lexer);

                var programm = parser.ParseCode();
                Evaluator evaluator = new Evaluator();

                Interpreter.Environment environment = new Interpreter.Environment();

                var result = evaluator.Eval(programm, environment);

                Assert.NotNull(result);
                Assert.Equal(expected[i], (result as Interpreter.Array).Inspect());
            }
        }
	}
}
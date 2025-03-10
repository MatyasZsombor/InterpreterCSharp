﻿using System.Globalization;
using System.Text;

namespace Interpreter;

public static class Repl
{
    private static void Main()
    {
        int code = int.Parse("1F435", NumberStyles.HexNumber);
        string unicodeString = char.ConvertFromUtf32(code);
        
        Console.WriteLine("Monkey 1.0 " + unicodeString);
        
        Console.Write("Do you want to execute a file, or use the REPL? (1 for file execution, 2 for REPL): ");
        string input = Console.ReadLine()!;
        
        switch (input)
        {
            case "exit":
            case "e":
                return;
            case "1":
                ExecuteFile();
                break;
            default:
                DoRepl();
                break;
        }
    }
    
    public static void PrintParserErrors(Parser parser)
    {
        List<string> errors = parser.Errors;
        Console.WriteLine("Parser errors:");
        foreach (var item in errors)
        {
            Console.WriteLine($"\t{item}");
        }
    }
    
    private static void ExecuteFile()
    {
        Console.Clear();
        Console.Write("Enter a path: ");
        string path = Console.ReadLine()!;
        
        if (!File.Exists(path))
        {
            Console.WriteLine($"Error the file({path}) wasn't found");
            
            return;
        }
        
        string file = File.ReadAllText(path);
        
        Environment environment = new Environment();
        
        Lexer lexer = new Lexer(file);
        Parser parser = new Parser(lexer);
        
        Code code = parser.ParseCode();
        
        if (parser.Errors.Count > 0)
        {
            PrintParserErrors(parser);
        }
        
        Evaluator eval = new Evaluator();
        
        var result = eval.Eval(code, environment);
        
        if (result.Type() == ObjectType.Error)
        {
            Console.WriteLine(result.Inspect());
        }
    }
    
    private static void DoRepl()
    {
        Console.Clear();
        Console.WriteLine("Feel free to type in commands");
        Environment environment = new Environment();
        Console.OutputEncoding = Encoding.UTF8;

        ReadLine.HistoryEnabled = true;
        
        while (true)
        {
            string input = ReadLine.Read(">> ");

            if (input is "exit" or "e")
            {
                break;
            }

            Lexer lexer = new Lexer(input);
            Parser parser = new Parser(lexer);

            var program = parser.ParseCode();

            if (parser.Errors.Count > 0)
            {
                PrintParserErrors(parser);
                continue;
            }

            Evaluator evaluator = new Evaluator();
            var evaluated = evaluator.Eval(program, environment);

            Console.WriteLine(evaluated.Inspect());
        }
    }
}
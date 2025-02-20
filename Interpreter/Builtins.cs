namespace Interpreter;

public class BuiltIns
{
    public readonly Dictionary<string, BuiltIn> BuiltInFunctions = new();
    
    //TODO REIMPLEMENT BUILTINS IN THE LANGUAGE ITSELF
    
    public BuiltIns(Environment environment)
    {
        BuiltInFunctions["len"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() == ObjectType.Array)
                {
                    return new Integer { Value = ((Array)objects[0]).Elements.Count };
                }
                
                if (objects[0].Type() != ObjectType.String)
                {
                    return new Error { Message = $"argument to len not supported, got {objects[0].Type()}" };
                }
                return new Integer { Value = ((String)objects[0]).Value.Length };
            }
        };
        
        /*
        BuiltInFunctions["first"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                return objects[0].Type() != ObjectType.Array ? new Error { Message = $"argument to first not supported, got {objects[0].Type()}" } : ((Array)objects[0]).Elements[0];
            }
        };*/
        
        /*
        BuiltInFunctions["last"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                return objects[0].Type() != ObjectType.Array ? new Error { Message = $"argument to last not supported, got {objects[0].Type()}" } : ((Array)objects[0]).Elements[^1];
            }
        };*/
        
        /*
        BuiltInFunctions["rest"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to last not supported, got {objects[0].Type()}" };
                }
                
                Array array = (Array)objects[0];
                List<IEvObject> elements = [];
                
                for (int i = 1; i < array.Elements.Count; i++)
                {
                    elements.Add(array.Elements[i]);
                }
                return new Array { Elements = elements };
            }
        };*/
        
        /*
        BuiltInFunctions["push"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 2)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to last not supported, got {objects[0].Type()}" };
                }
                
                var newItems = ((Array) objects[0]).Elements.ToList();
                newItems.Add(objects[1]);
                
                return new Array { Elements = newItems };
            }
        };*/
        
        /*
        BuiltInFunctions["contains"] = new BuiltIn
        {
            Fn = objects =>
            {
                
                if (objects.Count != 2)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to last not supported, got {objects[0].Type()}" };
                }
                
                return ((Array)objects[0]).Elements.Contains(objects[1]) ? new Boolean { Value = true } : new Boolean { Value = false };
            }
        };*/
        
        BuiltInFunctions["string"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                return new String { Value = objects[0].Inspect() };
            }
        };
        
        /*
        BuiltInFunctions["add"] = new BuiltIn
        {
            
            Fn = objects =>
            {
                if (objects.Count != 2)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to last not supported, got {objects[0].Type()}" };
                }
                
                ((Array)objects[0]).Elements.Add(objects[1]);
                return new Null();
            }
        };
        
        /*
        BuiltInFunctions["delete"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 2)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to last not supported, got {objects[0].Type()}" };
                }
                
                if (objects[1].Type() != ObjectType.Integer)
                {
                    return new Error { Message = $"argument to last not supported, got {objects[1].Type()}" };
                }
                
                var newItems = new List<IEvObject>();
                for (int i = 0; i < Convert.ToInt32(((Array)objects[0]).Elements.Count); i++)
                {
                    if (i != ((Integer)objects[1]).Value)
                    {
                        newItems.Add(((Array)objects[0]).Elements[i]);
                    }
                }
                
                return new Array { Elements = newItems };
            }
        };
        
        /*
        BuiltInFunctions["sort"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to sort not supported, got {objects[0].Type()}" };
                }
                
                List<int> newItems = [];
                Array array = (Array)objects[0];
                
                foreach (var item in array.Elements)
                {
                    if (item.Type() != ObjectType.Integer)
                    {
                        return new Error { Message = $"argument to sort not supported, got {item.Type()}" };
                    }
                    newItems.Add(((Integer)item).Value);
                }
                
                newItems.Sort();
                
                List<IEvObject> finalElements = newItems.Select(item => new Integer { Value = item }).Cast<IEvObject>().ToList();
                
                return new Array { Elements = finalElements };
            }
        };
        
        
        BuiltInFunctions["reverse"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.Array)
                {
                    return new Error { Message = $"argument to reverse not supported, got {objects[0].Type()}" };
                }
                
                List<IEvObject> newElements = [..((Array)objects[0]).Elements];
                newElements.Reverse();
                
                return new Array { Elements = newElements };
            }
        };*/
        
        BuiltInFunctions["put"] = new BuiltIn
        {
            Fn = objects =>
            {
                foreach (IEvObject item in objects)
                {
                    Console.WriteLine(item.Inspect());
                }
                return new Null();
            }
        };
        
        BuiltInFunctions["get"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 0)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                string text = Console.ReadLine()!;
                
                Lexer lexer = new Lexer(text);
                
                Parser parser = new Parser(lexer);
                
                Evaluator evaluator = new Evaluator();
                
                if (parser.Errors.Count <= 0)
                {
                    return evaluator.Eval(parser.ParseCode(), environment);
                }
                
                Repl.PrintParserErrors(parser);
                return evaluator.Eval(parser.ParseCode(), environment);
            }
        };
        
        BuiltInFunctions["write"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 2)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.String)
                {
                    return new Error { Message = $"argument to write not supported, got {objects[0].Type()}" };
                }
                
                string path = objects[0].Inspect();
                
                File.WriteAllText(path, objects[1].Inspect());
                
                return new Null();
            }
        };
        
        BuiltInFunctions["read"] = new BuiltIn
        {
            Fn = objects =>
            {
                if (objects.Count != 1)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                
                if (objects[0].Type() != ObjectType.String)
                {
                    return new Error { Message = $"argument to write not supported, got {objects[0].Type()}" };
                }
                
                string path = objects[0].Inspect();
                
                
                if (!File.Exists(path))
                {
                    
                    return new Error { Message = $"the file({path}) wasn't found" };
                }
                
                string text = File.ReadAllText(path);
                
                return new String { Value = text };
            }
        };
        
        BuiltInFunctions["clear"] = new BuiltIn
        {
            Fn = objects =>
            {
                if(objects.Count != 0)
                {
                    return new Error { Message = $"wrong number of arguments, got {objects.Count}" };
                }
                Console.Clear();
                return new Null();
            }
        };
    }
}

public class BuiltIn : IEvObject
{
    public required Func<List<IEvObject>, IEvObject> Fn;
    
    public ObjectType Type() => ObjectType.Builtin;
    
    public string Inspect() => "Builtin Function";
}
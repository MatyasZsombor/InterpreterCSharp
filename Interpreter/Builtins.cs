using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Interpreter
{
	public class BuiltIns
	{
		public Dictionary<string, BuiltIn> BuiltInFunctions = new Dictionary<string, BuiltIn>();
		public Environment environment { get; set; }

		public BuiltIns()
		{
			BuiltInFunctions["len"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() == ObjectType.ARRAY)
					{
						return new Integer { value = (objects[0] as Array).elements.Count };
					}

					if (objects[0].Type() != ObjectType.STRING)
					{
						return new Error { message = $"argument to len not supported, got {objects[0].Type()}" };
					}
					return new Integer { value = (objects[0] as String).value.Length };
				}
			};

			BuiltInFunctions["first"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to first not supported, got {objects[0].Type()}" };
					}
					return (objects[0] as Array).elements[0];
				}
			};

			BuiltInFunctions["last"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to last not supported, got {objects[0].Type()}" };
					}
					return (objects[0] as Array).elements[(objects[0] as Array).elements.Count - 1];
				}
			};

			BuiltInFunctions["rest"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to last not supported, got {objects[0].Type()}" };
					}

					Array array = objects[0] as Array;
					List<EvObject> elements = new List<EvObject>();

					for (int i = 1; i < array.elements.Count; i++)
					{
						elements.Add(array.elements[i]);
					}
					return new Array { elements = elements };
				}
			};

			BuiltInFunctions["push"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 2)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to last not supported, got {objects[0].Type()}" };
					}

					var newItems = new List<EvObject>();
					foreach (var item in (objects[0] as Array).elements)
					{
						newItems.Add(item);
					}
					newItems.Add(objects[1]);

					return new Array { elements = newItems };
				}
            };

			BuiltInFunctions["contains"] = new BuiltIn()
			{
				Fn = (objects) =>
				{

					if (objects.Count != 2)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to last not supported, got {objects[0].Type()}" };
					}

					if ((objects[0] as Array).elements.Contains(objects[1]))
					{
						return new Boolean { value = true };
					}
					return new Boolean { value = false };
				}
			};

			BuiltInFunctions["string"] = new BuiltIn
			{
				Fn = (objects) =>
				{
                    if (objects.Count != 1)
                    {
                        return new Error { message = $"wrong number of arguments, got {objects.Count}" };
                    }

					return new String { value = objects[0].Inspect() };
                }
			};

			BuiltInFunctions["add"] = new BuiltIn()
			{

				Fn = (objects) =>
				{
                    if (objects.Count != 2)
                    {
                        return new Error { message = $"wrong number of arguments, got {objects.Count}" };
                    }

                    if (objects[0].Type() != ObjectType.ARRAY)
                    {
                        return new Error { message = $"argument to last not supported, got {objects[0].Type()}" };
                    }

					(objects[0] as Array).elements.Add(objects[1]);
					return new Null();
                }
			};

			BuiltInFunctions["delete"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 2)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to last not supported, got {objects[0].Type()}" };
					}

					if (objects[1].Type() != ObjectType.INTEGER)
					{
						return new Error { message = $"argument to last not supported, got {objects[1].Type()}" };
					}

					var newItems = new List<EvObject>();
					for (int i = 0; i < Convert.ToInt32((objects[0] as Array).elements.Count); i++)
					{
						if (i != (objects[1] as Integer).value)
						{
							newItems.Add((objects[0] as Array).elements[i]);
						}
					}

					return new Array { elements = newItems };
				}
			};

			BuiltInFunctions["sort"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to sort not supported, got {objects[0].Type()}" };
					}

					List<double> newItems = new List<double>();
					Array array = objects[0] as Array;

					foreach (var item in array.elements)
					{
						if (item.Type() != ObjectType.INTEGER)
						{
							return new Error { message = $"argument to sort not supported, got {item.Type()}" };
						}
						newItems.Add((item as Integer).value);
					}

					newItems.Sort();

					List<EvObject> finalElements = new List<EvObject>();

					foreach (var item in newItems)
					{
						finalElements.Add(new Integer { value = item });
					}

					return new Array { elements = finalElements };
				}
			};

			BuiltInFunctions["reverse"] = new BuiltIn
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.ARRAY)
					{
						return new Error { message = $"argument to reverse not supported, got {objects[0].Type()}" };
					}

					List<EvObject> newElements = new List<EvObject>((objects[0] as Array).elements);
					newElements.Reverse();

					return new Array { elements = newElements };
				}
			};

			BuiltInFunctions["put"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					foreach (EvObject item in objects)
					{
						Console.WriteLine(item.Inspect());
					}
					return new Null();
				}
			};

			BuiltInFunctions["get"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 0)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					string text = Console.ReadLine()!;

					Lexer lexer = new Lexer(text);

					Parser parser = new Parser(lexer);

					Evaluator evaluator = new Evaluator();

					if (parser.errors.Count > 0)
					{
						foreach (var err in parser.errors)
						{
							REPL.PrintParserErrors(parser);
						}
					}

					return evaluator.Eval(parser.ParseCode(), environment);
				}
			};

			BuiltInFunctions["write"] = new BuiltIn()
			{
				Fn = (objects) =>
				{
					if (objects.Count != 2)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.STRING)
					{
						return new Error { message = $"argument to write not supported, got {objects[0].Type()}" };
					}

					string path = @objects[0].Inspect();

					File.WriteAllText(path, objects[1].Inspect());

					return new Null();
				}
			};

			BuiltInFunctions["read"] = new BuiltIn
			{
				Fn = (objects) =>
				{
					if (objects.Count != 1)
					{
						return new Error { message = $"wrong number of arguments, got {objects.Count}" };
					}

					if (objects[0].Type() != ObjectType.STRING)
					{
						return new Error { message = $"argument to write not supported, got {objects[0].Type()}" };
					}

					string path = @objects[0].Inspect();


					if (!File.Exists(path))
					{

						return new Error { message = $"the file({path}) wasn't found" };
					}

					string text = File.ReadAllText(path);

					return new String { value = text };
				}
			};

			BuiltInFunctions["clear"] = new BuiltIn
			{
				Fn = (objects) =>
				{
					if(objects.Count != 0)
					{
                        return new Error { message = $"wrong number of arguments, got {objects.Count}" };
                    }
					Console.Clear();
					return new Null();
				}
			};
        }
	}

	public class BuiltIn : EvObject
	{
		public Func<List<EvObject>, EvObject> Fn;

		public ObjectType Type()
		{
			return ObjectType.BUILTIN;
		}

		public string Inspect()
		{
			return "Builtin Function";
		}
	}
}
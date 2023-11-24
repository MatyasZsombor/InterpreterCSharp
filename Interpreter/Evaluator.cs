using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Interpreter
{
	public class Evaluator
	{
        private static Boolean TRUE = new Boolean() { value = true };
        private static Boolean FALSE = new Boolean() { value = false };
		private static Null NULL = new Null(); 

		public EvObject Eval(Node node, Environment environment)
		{
			BuiltIns builtIns = new BuiltIns() { environment = environment};
			switch (node)
			{
				case Code code:
					return EvalProgram(code, environment);

				case ArrayLiteral arrayLiteral:
					Array array = new Array {elements = EvalExpressions(arrayLiteral.elements, environment) };

					if (array.elements.Count == 1 && IsError(array.elements[0]))
					{
						return array.elements[0];
					}
					return array;

				case IndexExpression indexExpression:
					var left = Eval(indexExpression.left, environment);

					if (IsError(left))
					{
						return left;
					}

					var index = Eval(indexExpression.index, environment);

					if (IsError(index))
					{
						return index;
					}
					return EvalIndexExpression(left, index);

				case IntegerLiteral integerLiteral:
					return new Integer() { value = integerLiteral.value };

				case StringLiteral stringLiteral:
					return new String { value = stringLiteral.value };

				case BooleanLiteral booleanLiteral:
					return booleanLiteral.value == true ? TRUE : FALSE;

				case ExpressionStatement expressionStatement:
					return Eval(expressionStatement.expression, environment);

				case PrefixExpression prefixExpression:
					var right = Eval(prefixExpression.right, environment);

                    if (IsError(right))
                    {
                        return right;
                    }

                    return EvalPrefixExpression(prefixExpression.op, right);

				case InfixExpression infixExpression:
					right = Eval(infixExpression.right, environment);
					left = Eval(infixExpression.left, environment);

                    if (IsError(right))
                    {
                        return right;
                    }

                    if (IsError(left))
                    {
                        return left;
                    }

                    return EvalInfixExpression(infixExpression.op, right, left);

				case BlockStatement blockStatement:
					return EvalBlockStatement(blockStatement, environment);

				case IfExpression ifExpression:
					return EvalIfExpression(ifExpression, environment);

				case ReturnStatement returnStatement:
					var value = Eval(returnStatement.value, environment);

                    if (IsError(value))
                    {
                        return value;
                    }

                    return new ReturnValue { value = value };

				case LetStatement letStatement:
					value = Eval(letStatement.value, environment);

					if (IsError(value))
					{
						return value;
					}
					environment.Set(letStatement.name.value, value);
                    break;

				case Identifier identifier:
					return EvalIdentifier(identifier, environment, builtIns);

				case FunctionLiteral functionLiteral:
                    var parameters = functionLiteral.parameters;
                    var body = functionLiteral.body;
                    return new Function() { parameters = parameters, body = body, environment = environment};

				case CallExpression callExpression:
					var function = Eval(callExpression.function, environment);

					if (IsError(function))
					{
						return function;
					}

					var args = EvalExpressions(callExpression.arguments, environment);

					if(args.Count == 1 && IsError(args[0]))
					{
						return args[0];
					}
					return ApplyFunction(function, args);
            }

			return NULL;
		}

		private EvObject EvalIndexExpression(EvObject left, EvObject index)
		{
			if(left.Type() == ObjectType.ARRAY && index.Type() == ObjectType.INTEGER)
			{
				return EvalArrayIndexExpression(left as Array, index as Integer);
			}
			return new Error {  message = $"index operator not supported: {left.Type()}" };
		}

		private EvObject EvalArrayIndexExpression(Array array, Integer index)
		{
			if (index.value < 0 || index.value > array.elements.Count - 1)
			{
				return new Error { message = "index was outside the bounds of the array" };
			}
			return array.elements[Convert.ToInt32(index.value)];
		}

		private EvObject ApplyFunction(EvObject fn, List<EvObject> args)
		{
            if (fn is BuiltIn)
            {
                return (fn as BuiltIn).Fn(args);
            }

            if (!(fn is Function))
            {
                return new Error() { message = $"not a function {fn.Type()}" };
            }

			var func = fn as Function;

            if (args.Count != func.parameters.Count)
                return new Error() { message = "Invalid argument count." };

            var extendedEnv = ExtendFunctionEnv(func, args);
            var evaluated = Eval(func.body, extendedEnv);

            return UnwrapReturnValue(evaluated);
        }

		public Environment ExtendFunctionEnv(Function function, List<EvObject> args)
		{
            var env = Environment.NewEnclosedEnvironment(function.environment);
            for (int i = 0; i < function.parameters.Count; i++)
            {
                env.Set(function.parameters[i].value, args[i]);
            }
            return env;
        }

		private EvObject UnwrapReturnValue(EvObject evaluated)
		{
			if (evaluated is ReturnValue)
			{
                return (evaluated as ReturnValue).value;
            }
			return evaluated;
		}

		private List<EvObject> EvalExpressions(List<Expression> exps, Environment environment)
		{
			var result = new List<EvObject>();

			foreach (var item in exps)
			{
				var evaluated = Eval(item, environment);

				if (IsError(evaluated))
				{
                    return new List<EvObject>() { evaluated };
                }
				result.Add(evaluated);
			}
			return result;
		}

		private Error CreateError(string msg)
		{
			return new Error { message = msg };
		}

		private bool IsError(EvObject evObject)
		{
			if(evObject != null)
			{
				return evObject.Type() == ObjectType.ERROR;
			}
			return false;
		}

		private EvObject EvalIdentifier(Identifier identifier, Environment environment, BuiltIns builtIns)
		{
			if (builtIns.BuiltInFunctions.ContainsKey(identifier.value))
			{
				return builtIns.BuiltInFunctions[identifier.value];
			}

			var (value, ok) = environment.Get(identifier.value);

			if (!ok)
			{
				return CreateError("identifier not found " + identifier.value);
			}

			return value;
		}

		private EvObject EvalBlockStatement(BlockStatement blockStatement, Environment environment)
		{
			EvObject result = NULL;

			foreach (var stmt in blockStatement.statements)
			{
				result = Eval(stmt, environment);

				if(result != null && result.Type() == ObjectType.RETURNVALUE || result.Type() == ObjectType.ERROR)
				{
					return result;
				}
			}
			return result;
		}

		private EvObject EvalIfExpression(IfExpression ifExpression, Environment environment)
		{
			var condition = Eval(ifExpression.condition, environment);

            if (IsError(condition))
            {
                return condition;
            }

            if (IsTruthy(condition))
			{
				return Eval(ifExpression.consequence, environment);
			}
			else if(ifExpression.alternative != null)
			{
				return Eval(ifExpression.alternative, environment);
			}
			else
			{
				return NULL;
			}
		}

		private bool IsTruthy(EvObject evObject)
		{
			switch (evObject)
			{
                case Boolean b:
                    return b == TRUE ? true : false;

                case Integer i:
                    return i.value == 0 ? false : true;

                default:
                    return true;
            }
		}

        private EvObject EvalProgram(Code code, Environment environment)
		{
			EvObject result = NULL;

			foreach (var stmt in code.statements)
			{
				result = Eval(stmt, environment);
				switch (result)
				{
					case Integer integer:
						result = integer;
						break;

					case Boolean boolean:
						result = boolean;
						break;

					case ReturnValue returnValue:
						return returnValue.value;

					case Error error:
						return error;
				}

			}
			return result;
		}

		private EvObject EvalPrefixExpression(string op, EvObject right)
		{
			switch (op)
			{
				case "!":
					return EvalBangExpression(right);

				case "-":
					return EvalMinusExpression(right);

				default:
					return CreateError($"unknow operator: {op}{right.Type}");
			}
		}

		private EvObject EvalInfixExpression(string op, EvObject right, EvObject left)
		{
			if (right.Type() == ObjectType.INTEGER && left.Type() == ObjectType.INTEGER)
			{
				return EvalIntegerInfixExpression(op, right, left);
			}
			else if(right.Type() == ObjectType.BOOLEAN && left.Type() == ObjectType.BOOLEAN)
			{
				if(op == "!=")
				{
					bool temp = (left as Boolean).value != (right as Boolean).value;
					return temp == true ? TRUE : FALSE;
				}
				else if (op == "==")
				{
					bool temp = (left as Boolean).value == (right as Boolean).value;
					return temp == true ? TRUE : FALSE;
				}
				else if(op == "||")
				{
					bool temp = (left as Boolean).value || (right as Boolean).value;
					return temp == true ? TRUE : FALSE; 
                }
				else if(op == "&&")
				{
					bool temp = (left as Boolean).value && (right as Boolean).value;
					return temp == true ? TRUE : FALSE;
				}
			}
			else if (left.Type() == ObjectType.STRING && right.Type() == ObjectType.STRING)
			{
				return EvalStringInfixExpression(op, left, right);
			}
			else if (left.Type() != right.Type())
			{
				return CreateError($"type mismatch: {left.Type()} {op} {right.Type()}");
			}
			return CreateError($"unknown operator: {left.Type()} {op} {right.Type()}");
		}

		private EvObject EvalStringInfixExpression(string op, EvObject left, EvObject right)
		{
			if(op != "+")
			{
				return CreateError($"unknown operator: {left.Type()} {op} {right.Type()}");
			}

			return new String { value = (left as String).value + (right as String).value };
		}

		private EvObject EvalIntegerInfixExpression(string op, EvObject right, EvObject left)
		{
			var leftVal = (left as Integer).value;
			var rightVal = (right as Integer).value;

			switch (op)
			{
				case "+":
					return new Integer { value = leftVal + rightVal };

				case "-":
					return new Integer { value = leftVal - rightVal };

				case "*":
					return new Integer { value = leftVal * rightVal };

				case "/":
					return new Integer { value = leftVal / rightVal };

				case "<":
					return (leftVal < rightVal) == true ? TRUE : FALSE;

				case ">":
					return (leftVal > rightVal) == true ? TRUE : FALSE;

				case "==":
					return (leftVal == rightVal) == true ? TRUE : FALSE;

				case "!=":
					return (leftVal != rightVal) == true ? TRUE : FALSE;

				default:
					return CreateError($"unknown operator: {left.Type()} {op} {right.Type()}");
			}
		}

		private EvObject EvalBangExpression(EvObject right)
		{
			if(right.Type() == ObjectType.BOOLEAN)
			{
                return !((right as Boolean).value) == true ? TRUE : FALSE;
            }
			return CreateError($"unknown operator: !{right.Type()}"); ;
		}

		private EvObject EvalMinusExpression(EvObject right)
		{
			if(right.Type() != ObjectType.INTEGER)
			{
                return CreateError($"unknown operator: -{right.Type()}");
            }
			return new Integer() { value = -(right as Integer).value };
		}
	}
}
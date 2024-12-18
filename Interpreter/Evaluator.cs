﻿namespace Interpreter;

public class Evaluator
{
    private static readonly Boolean @true = new() { Value = true };
    private static readonly Boolean @false = new() { Value = false };
    private static readonly Null @null = new();
    
    public IEvObject Eval(INode? node, Environment environment)
    {
        BuiltIns builtIns = new BuiltIns(environment);
        switch (node)
        {
            case Code code:
                return EvalProgram(code, environment);
            
            case ArrayLiteral arrayLiteral:
                Array array = new Array { Elements = EvalExpressions(arrayLiteral.Elements, environment) };
                
                if (array.Elements.Count == 1 && IsError(array.Elements[0]))
                {
                    return array.Elements[0];
                }
                
                return array;
            
            case IndexExpression indexExpression:
                var left = Eval(indexExpression.Left, environment);
                
                if (IsError(left))
                {
                    return left;
                }
                
                var index = Eval(indexExpression.Index, environment);
                
                return IsError(index) ? index : EvalIndexExpression(left, index);
            
            case IntegerLiteral integerLiteral:
                return new Integer { Value = integerLiteral.Value };
            
            case StringLiteral stringLiteral:
                return new String { Value = stringLiteral.Value };
            
            case BooleanLiteral booleanLiteral:
                return booleanLiteral.Value ? @true : @false;
            
            case ExpressionStatement expressionStatement:
                return Eval(expressionStatement.Expression, environment);
            
            case PrefixExpression prefixExpression:
                var right = Eval(prefixExpression.Right, environment);
                
                return IsError(right) ? right : EvalPrefixExpression(prefixExpression.Op, right);
            
            case InfixExpression infixExpression:
                right = Eval(infixExpression.Right, environment);
                left = Eval(infixExpression.Left, environment);
                
                if (IsError(right))
                {
                    return right;
                }
                
                return IsError(left) ? left : EvalInfixExpression(infixExpression.Op, right, left);
            
            case BlockStatement blockStatement:
                return EvalBlockStatement(blockStatement, environment);
            
            case IfExpression ifExpression:
                return EvalIfExpression(ifExpression, environment);
            
            case ReturnStatement returnStatement:
                var value = Eval(returnStatement.Value, environment);
                
                return IsError(value) ? value : new ReturnValue { Value = value };
            
            case LetStatement letStatement:
                value = Eval(letStatement.Value, environment);
                
                if (IsError(value))
                {
                    return value;
                }
                
                environment.Set(letStatement.Name.Value, value);
                
                break;
            
            case Identifier identifier:
                return EvalIdentifier(identifier, environment, builtIns);
            
            case FunctionLiteral functionLiteral:
                List<Identifier> parameters = functionLiteral.Parameters;
                var body = functionLiteral.Body;
                
                return new Function { Parameters = parameters, Body = body, Environment = environment };
            
            case CallExpression callExpression:
                var function = Eval(callExpression.Function, environment);
                
                if (IsError(function))
                {
                    return function;
                }
                
                List<IEvObject> args = EvalExpressions(callExpression.Arguments, environment);
                
                if (args.Count == 1 && IsError(args[0]))
                {
                    return args[0];
                }
                
                return ApplyFunction(function, args);
        }
        
        return @null;
    }
    
    private static IEvObject EvalIndexExpression(IEvObject left, IEvObject index)
    {
        if (left.Type() == ObjectType.Array && index.Type() == ObjectType.Integer)
        {
            return EvalArrayIndexExpression((Array)left, (Integer)index);
        }
        
        return new Error { Message = $"index operator not supported: {left.Type()}" };
    }
    
    private static IEvObject EvalArrayIndexExpression(Array array, Integer index)
    {
        if (index.Value < 0 || index.Value > array.Elements.Count - 1)
        {
            return new Error { Message = "index was outside the bounds of the array" };
        }
        
        return array.Elements[Convert.ToInt32(index.Value)];
    }
    
    private IEvObject ApplyFunction(IEvObject fn, List<IEvObject> args)
    {
        if (fn is BuiltIn builtIn)
        {
            return builtIn.Fn(args);
        }
        
        if (fn is not Function func)
        {
            return new Error { Message = $"not a function {fn.Type()}" };
        }
        
        if (args.Count != func.Parameters.Count)
        {
            return new Error { Message = "Invalid argument count." };
        }
        
        var extendedEnv = ExtendFunctionEnv(func, args);
        var evaluated = Eval(func.Body, extendedEnv);
        
        return UnwrapReturnValue(evaluated);
    }
    
    private static Environment ExtendFunctionEnv(Function function, List<IEvObject> args)
    {
        var env = Environment.NewEnclosedEnvironment(function.Environment);
        for (int i = 0; i < function.Parameters.Count; i++)
        {
            env.Set(function.Parameters[i].Value, args[i]);
        }
        
        return env;
    }
    
    private static IEvObject UnwrapReturnValue(IEvObject evaluated)
    {
        if (evaluated is ReturnValue @return)
        {
            return @return.Value;
        }
        
        return evaluated;
    }
    
    private List<IEvObject> EvalExpressions(List<IExpression> exps, Environment environment)
    {
        var result = new List<IEvObject>();
        
        foreach (var evaluated in exps.Select(item => Eval(item, environment)))
        {
            if (IsError(evaluated))
            {
                return [evaluated];
            }
            
            result.Add(evaluated);
        }
        
        return result;
    }
    
    private static Error CreateError(string msg) => new() { Message = msg };
    
    private static bool IsError(IEvObject evObject) => evObject.Type() == ObjectType.Error;
    
    private static IEvObject EvalIdentifier(Identifier identifier, Environment environment, BuiltIns builtIns)
    {
        if (builtIns.BuiltInFunctions.TryGetValue(identifier.Value, out var evalIdentifier))
        {
            return evalIdentifier;
        }
        
        (IEvObject? value, bool ok) = environment.Get(identifier.Value);
        
        return !ok ? CreateError("identifier not found " + identifier.Value) : value!;
    }
    
    private IEvObject EvalBlockStatement(BlockStatement blockStatement, Environment environment)
    {
        IEvObject result = @null;
        
        foreach (var stmt in blockStatement.Statements)
        {
            result = Eval(stmt, environment);
            
            if (result.Type() == ObjectType.ReturnValue || result.Type() == ObjectType.Error)
            {
                return result;
            }
        }
        
        return result;
    }
    
    private IEvObject EvalIfExpression(IfExpression ifExpression, Environment environment)
    {
        var condition = Eval(ifExpression.Condition, environment);
        
        if (IsError(condition))
        {
            return condition;
        }
        
        if (IsTruthy(condition))
        {
            return Eval(ifExpression.Consequence, environment);
        }
        
        return ifExpression.Alternative != null ? Eval(ifExpression.Alternative, environment) : @null;
    }
    
    private static bool IsTruthy(IEvObject evObject) =>
        evObject switch
        {
            Boolean b => b == @true,
            Integer i => i.Value != 0,
            _         => true
        };
    
    private IEvObject EvalProgram(Code code, Environment environment)
    {
        IEvObject result = @null;
        
        foreach (var stmt in code.Statements)
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
                    return returnValue.Value;
                
                case Error error:
                    return error;
            }
        }
        
        return result;
    }
    
    private IEvObject EvalPrefixExpression(string op, IEvObject right)
    {
        switch (op)
        {
            case "!":
                return EvalBangExpression(right);
            
            case "-":
                return EvalMinusExpression(right);
            
            default:
                return CreateError($"unknown operator: {op}{right.Type}");
        }
    }
    
    private IEvObject EvalInfixExpression(string op, IEvObject right, IEvObject left)
    {
        if (right.Type() == ObjectType.Integer && left.Type() == ObjectType.Integer)
        {
            return EvalIntegerInfixExpression(op, right, left);
        }
        
        if (right.Type() == ObjectType.Boolean && left.Type() == ObjectType.Boolean)
        {
            switch (op)
            {
                case "!=":
                {
                    bool temp = ((Boolean)left).Value != ((Boolean)right).Value;
                    
                    return temp ? @true : @false;
                }
                case "==":
                {
                    bool temp = ((Boolean)left).Value == ((Boolean)right).Value;
                    
                    return temp ? @true : @false;
                }
                case "||":
                {
                    bool temp = ((Boolean)left).Value || ((Boolean)right).Value;
                    
                    return temp ? @true : @false;
                }
                case "&&":
                {
                    bool temp = ((Boolean)left).Value && ((Boolean)right).Value;
                    
                    return temp ? @true : @false;
                }
            }
        }
        else if (left.Type() == ObjectType.String && right.Type() == ObjectType.String)
        {
            return EvalStringInfixExpression(op, left, right);
        }
        else if (left.Type() != right.Type())
        {
            return CreateError($"type mismatch: {left.Type()} {op} {right.Type()}");
        }
        
        return CreateError($"unknown operator: {left.Type()} {op} {right.Type()}");
    }
    
    private IEvObject EvalStringInfixExpression(string op, IEvObject left, IEvObject right)
    {
        if (op != "+")
        {
            return CreateError($"unknown operator: {left.Type()} {op} {right.Type()}");
        }
        
        return new String { Value = ((String)left).Value + ((String)right).Value };
    }
    
    private IEvObject EvalIntegerInfixExpression(string op, IEvObject right, IEvObject left)
    {
        var leftVal = ((Integer)left).Value;
        var rightVal = ((Integer)right).Value;
        
        switch (op)
        {
            case "+":
                return new Integer { Value = leftVal + rightVal };
            
            case "-":
                return new Integer { Value = leftVal - rightVal };
            
            case "*":
                return new Integer { Value = leftVal * rightVal };
            
            case "/":
                return new Integer { Value = leftVal / rightVal };
            
            case "<":
                return leftVal < rightVal ? @true : @false;
            
            case ">":
                return leftVal > rightVal ? @true : @false;
            
            case "==":
                return leftVal == rightVal ? @true : @false;
            
            case "!=":
                return leftVal != rightVal ? @true : @false;
            
            default:
                return CreateError($"unknown operator: {left.Type()} {op} {right.Type()}");
        }
    }
    
    private static IEvObject EvalBangExpression(IEvObject right)
    {
        if (right.Type() == ObjectType.Boolean)
        {
            return !((Boolean)right).Value ? @true : @false;
        }
        
        return CreateError($"unknown operator: !{right.Type()}");
    }
    
    private static IEvObject EvalMinusExpression(IEvObject right)
    {
        if (right.Type() != ObjectType.Integer)
        {
            return CreateError($"unknown operator: -{right.Type()}");
        }
        
        return new Integer { Value = -((Integer)right).Value };
    }
}

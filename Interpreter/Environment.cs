using System;
using System.Diagnostics.Metrics;

namespace Interpreter
{
    public class Environment
    {
        private Environment outer = null;

        private readonly Dictionary<string, EvObject> store = new Dictionary<string, EvObject>();

        public (EvObject, bool) Get(string name)
        {
            if (store.ContainsKey(name))
                return (store[name], true);

            if (!store.ContainsKey(name) && outer != null)
            {
                return outer.Get(name);
            }

            return (null, false);
        }

        public EvObject Set(string name, EvObject obj)
        {
            store[name] = obj;
            return obj;
        }

        public static Environment NewEnclosedEnvironment(Environment outer)
        {
            return new Environment { outer = outer };
        }

    }
}
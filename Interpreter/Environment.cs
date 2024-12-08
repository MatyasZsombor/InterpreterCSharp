namespace Interpreter
{
    public class Environment
    {
        private Environment? _outer;

        private readonly Dictionary<string, IEvObject> _store = new();

        public (IEvObject?, bool) Get(string name)
        {
            if (_store.TryGetValue(name, out var value))
            {
                return (value, true);
            }
            
            if (!_store.ContainsKey(name) && _outer != null)
            {
                return _outer.Get(name);
            }

            return (null, false);
        }

        public void Set(string name, IEvObject obj)
        {
            _store[name] = obj;
        }

        public static Environment NewEnclosedEnvironment(Environment outer) => new() { _outer = outer };
    }
}
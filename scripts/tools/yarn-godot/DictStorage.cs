
using Yarn;
using System.Collections.Generic;
    public class DictStorage : VariableStorage
    {
        Dictionary<string, Value> Data = new Dictionary<string, Value>();

        public void Clear() => Data.Clear();

        public Value GetValue(string variableName)
        {
            if (Data.TryGetValue(variableName, out Value val)) {
                return val;
            } else {
            return new Value(false);
                return new Value();
            }
        }

        public void SetValue(string variableName, Value value) => Data.Add(variableName, value);

        public void SetValue(string variableName, string stringValue) => Data.Add(variableName, new Value(stringValue));

        public void SetValue(string variableName, float floatValue)=> Data.Add(variableName, new Value(floatValue));

        public void SetValue(string variableName, bool boolValue)=> Data.Add(variableName, new Value(boolValue));
    }

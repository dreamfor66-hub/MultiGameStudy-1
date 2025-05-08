namespace FMLib.ExpressionParser
{
    public class VariableMap
    {
        public string Key;
        public float Value;

        public VariableMap(string key, float value)
        {
            Key = key;
            Value = value;
        }
    }
}
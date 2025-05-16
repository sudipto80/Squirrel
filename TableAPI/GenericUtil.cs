namespace TableAPI;

public class GenericUtil
{
    public static Dictionary<TValue, TKey> ReverseDictionary<TKey, TValue>(Dictionary<TKey, TValue> original)
    {
        Dictionary<TValue, TKey> reversed = new Dictionary<TValue, TKey>();
    
        foreach (KeyValuePair<TKey, TValue> pair in original)
        {
            reversed.Add(pair.Value, pair.Key);
        }
    
        return reversed;
    }
}
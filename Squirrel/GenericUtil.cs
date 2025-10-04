using System.Text;

namespace Squirrel;

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
    // Method 2: Using StringBuilder for better performance
    public static string NumberToColumnNameOptimized(int columnNumber)
    {
        StringBuilder columnName = new StringBuilder();
        
        while (columnNumber > 0)
        {
            columnNumber--;
            columnName.Insert(0, (char)('A' + columnNumber % 26));
            columnNumber /= 26;
        }
        
        return columnName.ToString();
    }
}
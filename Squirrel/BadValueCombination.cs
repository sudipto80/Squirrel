namespace Squirrel.Cleansing;

public class BadValueCombination
{
    public string ColumnName { get; set; }
    public HashSet<string> BadValues { get; set; }
}
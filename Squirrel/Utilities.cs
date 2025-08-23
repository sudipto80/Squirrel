namespace Squirrel;

public class Utilities
{
    public static bool IsColumnNameMatch(string columnName, string[] keywords)
    {
        return keywords.Any(keyword => columnName.Contains(keyword));
    }
}
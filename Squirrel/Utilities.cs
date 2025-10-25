namespace Squirrel;

/// <summary>
/// Provides utility functions for common operations.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Checks if the specified column name contains any of the provided keywords.
    /// </summary>
    /// <param name="columnName">The name of the column to check.</param>
    /// <param name="keywords">A collection of keywords to look for in the column name.</param>
    /// <returns>True if the column name contains any of the keywords; otherwise, false.</returns>
    public static bool IsColumnNameMatch(string columnName, params IEnumerable<string> keywords)
        => keywords.Any(columnName.Contains);
}
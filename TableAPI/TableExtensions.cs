using Squirrel;

namespace TableAPI;

public static class TableExtensions
{
    public static Table BetweenRows(this Table table, int start=0, int end=10)
    {
        if (start < 0 || end < 0 || start > end)
            throw new ArgumentOutOfRangeException("The start and end values should be non-negative and start should be less than end");
        if (end > table.RowCount)
            throw new ArgumentOutOfRangeException("The end value should be less than the number of rows in the table");
        var newTable = new Table();
        for (int i = start; i <= end; i++)
        {
            newTable.AddRow(table[i]);
        }
        return newTable;
    }
}
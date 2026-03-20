using System.Text;

namespace Squirrel;

public static class MarkdownGenerator
{
    public static string ToMarkdown(this Table table)
    {
        var header = string.Join(" | ", table.ColumnHeaders);
        var separator = string.Join(" | ", table.ColumnHeaders.Select(_ => "---"));
        var rows = table.Rows.Select(row => string.Join(" | ", row));
        return $"{header}\n{separator}\n{string.Join("\n", rows)}";
    }

    public static string ToMarkdown(this IEnumerable<Table> tables)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var table in tables)
        {
            sb.Append($"### {table.Name}");
            sb.AppendLine(table.ToMarkdown());
            sb.AppendLine();
        }
        return sb.ToString();
    }
    
}
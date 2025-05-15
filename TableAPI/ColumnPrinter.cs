using System.Text;
using ParquetSharp;

namespace Squirrel;

sealed class ColumnPrinter : ILogicalColumnReaderVisitor<string>
{
    public string OnLogicalColumnReader<TElement>(LogicalColumnReader<TElement> columnReader)
    {
        
        var stringBuilder = new StringBuilder();
        foreach (var value in columnReader) {
            stringBuilder.Append(value?.ToString() ?? string.Empty);
            stringBuilder.Append("[PARQURT_SEPARATOR]");
        }

        return stringBuilder.ToString();
    }
}
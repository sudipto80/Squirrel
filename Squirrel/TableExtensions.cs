using Squirrel;

namespace Squirrel;

public static class TableExtensions
{
    public static Table BetweenRows(this Table table, int start = 0, int end = 10)
    {
        if (start < 0 || end < 0 || start > end)
            throw new ArgumentOutOfRangeException(
                "The start and end values should be non-negative and start should be less than end");
        if (end > table.RowCount)
            throw new ArgumentOutOfRangeException("The end value should be less than the number of rows in the table");
        var newTable = new Table();
        for (int i = start; i <= end; i++)
        {
            newTable.AddRow(table[i]);
        }

        return newTable;
    }

    #region Boolean Indexing & Complex Conditions

    /// <summary>
    /// Filters rows based on a lambda expression condition
    /// </summary>
    public static Table Where(this Table table, Func<Dictionary<string, string>, bool> condition)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        var result = new Table();
        result.Rows.AddRange(table.Rows.Where(condition));
        return result;
    }

    /// <summary>
    /// Select rows where column value is between min and max (inclusive)
    /// </summary>
    public static Table Between(this Table table, string column, string min, string max)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (string.IsNullOrEmpty(min))
            throw new ArgumentNullException(nameof(min));
        if (string.IsNullOrEmpty(max))
            throw new ArgumentNullException(nameof(max));

        // Try numeric comparison first
        if (decimal.TryParse(min, out decimal minNum) && decimal.TryParse(max, out decimal maxNum))
        {
            return table.Where(row =>
            {
                if (decimal.TryParse(row[column], out decimal value))
                    return value >= minNum && value <= maxNum;
                return false;
            });
        }

        // Fall back to string comparison
        return table.Where(row =>
            string.Compare(row[column], min, StringComparison.Ordinal) >= 0 &&
            string.Compare(row[column], max, StringComparison.Ordinal) <= 0);
    }

    /// <summary>
    /// Select rows where column value is in the provided list
    /// </summary>
    public static Table In(this Table table, string column, params string[] values)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        var valueSet = new HashSet<string>(values);
        return table.Where(row => valueSet.Contains(row[column]));
    }

    /// <summary>
    /// Select rows where column value is NOT in the provided list
    /// </summary>
    public static Table NotIn(this Table table, string column, params string[] values)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        var valueSet = new HashSet<string>(values);
        return table.Where(row => !valueSet.Contains(row[column]));
    }

    /// <summary>
    /// Select rows where column has null or empty values
    /// </summary>
    public static Table IsNull(this Table table, string column)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));

        return table.Where(row => string.IsNullOrEmpty(row.GetValueOrDefault(column, "")));
    }

    /// <summary>
    /// Select rows where column has non-null, non-empty values
    /// </summary>
    public static Table IsNotNull(this Table table, string column)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));

        return table.Where(row => !string.IsNullOrEmpty(row.GetValueOrDefault(column, "")));
    }

    #endregion

    #region Positional & Pattern-Based Selection

    /// <summary>
    /// Integer-based position selection (like Python's iloc)
    /// </summary>
    public static Table Iloc(this Table table, int startIndex, int endIndex)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (endIndex < startIndex)
            throw new ArgumentOutOfRangeException(nameof(endIndex));
        if (startIndex >= table.RowCount)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        var result = new Table();
        int actualEndIndex = Math.Min(endIndex, table.RowCount - 1);

        for (int i = startIndex; i <= actualEndIndex; i++)
        {
            result.AddRow(table[i]);
        }

        return result;
    }

    /// <summary>
    /// Select every nth row, optionally with an offset
    /// </summary>
    public static Table Every(this Table table, int n, int offset = 0)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (n <= 0)
            throw new ArgumentOutOfRangeException(nameof(n), "n must be positive");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        var result = new Table();
        for (int i = offset; i < table.RowCount; i += n)
        {
            result.AddRow(table[i]);
        }

        return result;
    }

    /// <summary>
    /// Skip first n rows
    /// </summary>
    public static Table Skip(this Table table, int n)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n));

        var result = new Table();
        for (int i = n; i < table.RowCount; i++)
        {
            result.AddRow(table[i]);
        }

        return result;
    }

    /// <summary>
    /// Skip rows while condition is true
    /// </summary>
    public static Table SkipWhile(this Table table, Func<Dictionary<string, string>, bool> condition)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        var result = new Table();
        bool skipPhase = true;

        foreach (var row in table.Rows)
        {
            if (skipPhase && condition(row))
                continue;

            skipPhase = false;
            result.AddRow(row);
        }

        return result;
    }

    /// <summary>
    /// Take rows while condition is true
    /// </summary>
    public static Table TakeWhile(this Table table, Func<Dictionary<string, string>, bool> condition)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        var result = new Table();
        foreach (var row in table.Rows)
        {
            if (!condition(row))
                break;
            result.AddRow(row);
        }

        return result;
    }

    /// <summary>
    /// Select specific row indices
    /// </summary>
    public static Table SelectIndices(this Table table, params int[] indices)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (indices == null)
            throw new ArgumentNullException(nameof(indices));

        var result = new Table();
        foreach (int index in indices)
        {
            if (index >= 0 && index < table.RowCount)
            {
                result.AddRow(table[index]);
            }
        }

        return result;
    }

    #endregion

    #region Type-Aware Operations

    /// <summary>
    /// Numeric comparison with proper type handling
    /// </summary>
    public static Table WhereNumeric(this Table table, string column, string operatorStr, decimal value)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (string.IsNullOrEmpty(operatorStr))
            throw new ArgumentNullException(nameof(operatorStr));

        return table.Where(row =>
        {
            if (!decimal.TryParse(row.GetValueOrDefault(column, ""), out decimal rowValue))
                return false;

            return operatorStr.ToLower() switch
            {
                ">" or "gt" => rowValue > value,
                ">=" or "gte" => rowValue >= value,
                "<" or "lt" => rowValue < value,
                "<=" or "lte" => rowValue <= value,
                "==" or "eq" => rowValue == value,
                "!=" or "ne" => rowValue != value,
                _ => throw new ArgumentException($"Unknown operator: {operatorStr}")
            };
        });
    }

    /// <summary>
    /// Date-based filtering
    /// </summary>
    public static Table WhereDate(this Table table, string column, string operatorStr, DateTime date)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));

        return table.Where(row =>
        {
            if (!DateTime.TryParse(row.GetValueOrDefault(column, ""), out DateTime rowDate))
                return false;

            return operatorStr.ToLower() switch
            {
                ">" or "gt" => rowDate > date,
                ">=" or "gte" => rowDate >= date,
                "<" or "lt" => rowDate < date,
                "<=" or "lte" => rowDate <= date,
                "==" or "eq" => rowDate.Date == date.Date,
                "!=" or "ne" => rowDate.Date != date.Date,
                _ => throw new ArgumentException($"Unknown operator: {operatorStr}")
            };
        });
    }

    /// <summary>
    /// Date range filtering
    /// </summary>
    public static Table WhereDateRange(this Table table, string column, DateTime startDate, DateTime endDate)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));

        return table.Where(row =>
        {
            if (!DateTime.TryParse(row.GetValueOrDefault(column, ""), out DateTime rowDate))
                return false;
            return rowDate >= startDate && rowDate <= endDate;
        });
    }

    /// <summary>
    /// String contains operation
    /// </summary>
    public static Table WhereContains(this Table table, string column, string substring)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (substring == null)
            throw new ArgumentNullException(nameof(substring));

        return table.Where(row =>
            row.GetValueOrDefault(column, "").Contains(substring, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// String starts with operation
    /// </summary>
    public static Table WhereStartsWith(this Table table, string column, string prefix)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (prefix == null)
            throw new ArgumentNullException(nameof(prefix));

        return table.Where(row =>
            row.GetValueOrDefault(column, "").StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// String ends with operation
    /// </summary>
    public static Table WhereEndsWith(this Table table, string column, string suffix)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (suffix == null)
            throw new ArgumentNullException(nameof(suffix));

        return table.Where(row =>
            row.GetValueOrDefault(column, "").EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Sampling & Statistical Selection

    /// <summary>
    /// Stratified sampling maintaining group proportions
    /// </summary>
    public static Table StratifiedSample(this Table table, string column, int totalSampleSize)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(column))
            throw new ArgumentNullException(nameof(column));
        if (totalSampleSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalSampleSize));

        var groups = table.SplitOn(column);
        var result = new Table();
        var random = new Random();

        foreach (var group in groups.Values)
        {
            // Calculate proportional sample size for this group
            int groupSampleSize =
                Math.Max(1, (int)Math.Round((double)group.RowCount / table.RowCount * totalSampleSize));

            // Take random sample from this group
            var shuffled = group.Rows.OrderBy(x => random.Next()).Take(groupSampleSize);
            foreach (var row in shuffled)
            {
                result.AddRow(row);
            }
        }

        return result;
    }

    /// <summary>
    /// Systematic sampling with interval
    /// </summary>
    public static Table SystematicSample(this Table table, int interval)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (interval <= 0)
            throw new ArgumentOutOfRangeException(nameof(interval));

        var random = new Random();
        int start = random.Next(interval); // Random starting point

        return table.Every(interval, start);
    }

    // /// <summary>
    // /// Remove statistical outliers using IQR method
    // /// </summary>
    // public static Table RemoveOutliers(this Table table, string column)
    // {
    //     if (table == null)
    //         throw new ArgumentNullException(nameof(table));
    //     if (string.IsNullOrEmpty(column))
    //         throw new ArgumentNullException(nameof(column));
    //
    //     var values = table.ValuesOf(column)
    //         .Where(v => decimal.TryParse(v, out _))
    //         .Select(decimal.Parse)
    //         .OrderBy(x => x)
    //         .ToList();
    //
    //     if (values.Count < 4) // Need at least 4 values for quartiles
    //         return table;
    //
    //     var q1 = values[(int)(values.Count * 0.25)];
    //     var q3 = values[(int)(values.Count * 0.75)];
    //     var iqr = q3 - q1;
    //     var lowerBound = q1 - 1.5m * iqr;
    //     var upperBound = q3 + 1.5m * iqr;
    //
    //     return table.Where(row =>
    //     {
    //         if (!decimal.TryParse(row.GetValueOrDefault(column, ""), out decimal value))
    //             return true; // Keep non-numeric values
    //         return value >= lowerBound && value <= upperBound;
    //     });
    // }

    #endregion

    #region Pattern Matching & Search

    /// <summary>
    /// Full-text search across all columns
    /// </summary>
    public static Table Search(this Table table, string searchTerm)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (string.IsNullOrEmpty(searchTerm))
            throw new ArgumentNullException(nameof(searchTerm));

        return table.Where(row => row.Values.Any(value =>
            value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Search specific columns
    /// </summary>
    public static Table SearchColumns(this Table table, string[] columns, string searchTerm)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (columns == null)
            throw new ArgumentNullException(nameof(columns));
        if (string.IsNullOrEmpty(searchTerm))
            throw new ArgumentNullException(nameof(searchTerm));

        return table.Where(row => columns.Any(col =>
            row.GetValueOrDefault(col, "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
    }

    #endregion

    #region Window Operations

    /// <summary>
    /// Split table into equal-sized chunks
    /// </summary>
    public static IEnumerable<Table> Partition(this Table table, int size)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size));

        for (int i = 0; i < table.RowCount; i += size)
        {
            var chunk = new Table();
            for (int j = i; j < Math.Min(i + size, table.RowCount); j++)
            {
                chunk.AddRow(table[j]);
            }

            yield return chunk;
        }
    }

    /// <summary>
    /// Create sliding windows of specified size
    /// </summary>
    public static IEnumerable<Table> SlidingWindow(this Table table, int windowSize, int step = 1)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (windowSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(windowSize));
        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step));

        for (int i = 0; i <= table.RowCount - windowSize; i += step)
        {
            var window = new Table();
            for (int j = i; j < i + windowSize; j++)
            {
                window.AddRow(table[j]);
            }

            yield return window;
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Get first n rows matching condition
    /// </summary>
    public static Table Head(this Table table, int n, Func<Dictionary<string, string>, bool> condition)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));
        if (n <= 0)
            throw new ArgumentOutOfRangeException(nameof(n));

        var result = new Table();
        int count = 0;

        foreach (var row in table.Rows)
        {
            if (condition(row))
            {
                result.AddRow(row);
                count++;
                if (count >= n)
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Get last n rows matching condition
    /// </summary>
    public static Table Tail(this Table table, int n, Func<Dictionary<string, string>, bool> condition)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));
        if (n <= 0)
            throw new ArgumentOutOfRangeException(nameof(n));

        var matchingRows = table.Rows.Where(condition).Reverse().Take(n).Reverse();
        var result = new Table();
        foreach (var row in matchingRows)
        {
            result.AddRow(row);
        }

        return result;
    }

    /// <summary>
    /// Python-style slicing with step
    /// </summary>
    public static Table Slice(this Table table, int start, int end, int step = 1)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));
        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step));
        if (start < 0)
            start = Math.Max(0, table.RowCount + start);
        if (end < 0)
            end = Math.Max(0, table.RowCount + end);

        var result = new Table();
        for (int i = start; i < Math.Min(end, table.RowCount); i += step)
        {
            result.AddRow(table[i]);
        }

        return result;
    }






    #endregion

}
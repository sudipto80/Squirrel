using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

namespace Squirrel;

    
using System;
using System.Collections.Generic;
using System.Linq;



public static class TableResampleExtensions
{
    /// <summary>
    /// Resamples a time series table by grouping rows into frequency buckets and aggregating values
    /// </summary>
    /// <param name="tab">The table to resample</param>
    /// <param name="columnName">The datetime column to use for resampling</param>
    /// <param name="unalteredColumnNames">Columns to drop from this resampling attempt</param>
    /// <param name="frequency">The frequency to resample to (Daily, Weekly, Monthly, etc.)</param>
    /// <param name="method">The aggregation method to apply (Sum, Average, Max, etc.)</param>
    /// <returns>A resampled table with one row per frequency bucket</returns>
    public static Table Resample(this Table tab, string columnName, string[] unalteredColumnNames,
        DateTimeFrequency frequency, AggregationMethod method)
    {
        tab.ThrowIfTableIsNull();
        tab.ThrowIfColumnsAreNotPresentInTable(columnName);

        if (tab.RowCount == 0)
            return new Table();

        tab = tab.Drop(unalteredColumnNames);
        // Step 1: Extract timestamps from the datetime column
        var timestamps = new List<DateTime>();
        for (var i = 0; i < tab.RowCount; i++)
        {
            timestamps.Add(Convert.ToDateTime(tab[i][columnName]));
        }

        // Step 2: Group row indices by their frequency bucket
        var buckets = GroupByFrequency(timestamps, frequency);

        // Step 3: Build result table
        var result = new Table();

        // Step 4: Get all numeric columns (excluding the datetime column)
        var numericColumns = tab.ColumnHeaders
            .Where(col => col != columnName && IsNumericColumn(tab, col))
            .ToList();

        // Step 5: Aggregate data for each bucket
        foreach (var bucket in buckets.OrderBy(b => b.Key))
        {
            var newRow = new Dictionary<string, string> {
                // Add the bucket timestamp
                { columnName, bucket.Key.ToString("yyyy-MM-dd HH:mm:ss") } };

            // Aggregate each numeric column
            foreach (var col in numericColumns)
            {
                var values = bucket.Value
                    .Select(idx => Convert.ToDouble(tab[idx][col]))
                    .ToList();

                var aggregatedValue = ApplyAggregation(values, method);
                newRow.Add(col, aggregatedValue.ToString(CultureInfo.InvariantCulture));
            }

            result.AddRow(newRow);
        }

        return result;
    }

    /// <summary>
    /// Extract values at the specified frequency without aggregation (e.g., for business day frequency, it would extract values for each business day, skipping weekends)
    /// </summary>
    /// <param name="table"></param>
    /// <param name="columnName"></param>
    /// <param name="unalteredColumnNames"></param>
    /// <param name="frequency"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Table ExtractValuesAt(this Table table, string columnName, string[] unalteredColumnNames,
        DateTimeFrequency frequency)
    {
        throw new NotImplementedException("Not implemented");
    }

    /// <summary>
    /// Extract values that fall on a specific day of the week (e.g., all rows where the datetime column falls on a Monday)
    /// </summary>
    /// <param name="table"></param>
    /// <param name="datetimeColumn"></param>
    /// <param name="dow"></param>
    /// <returns></returns>
    /// <example>tab.ValuesOn(DayOfWeek.Monday)</example>
    public static Table ValuesOn(this Table table, string datetimeColumn, DayOfWeek dow)
    {
        table.ThrowIfTableIsNull();
        table.ThrowIfColumnsAreNotPresentInTable(datetimeColumn);

        var result = new Table();

        for (var i = 0; i < table.RowCount; i++)
        {
            var dt =DateTime.Parse(table[i][datetimeColumn]);
            if (dt.DayOfWeek == dow)
            {
                result.AddRow(table[i]);
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <param name="datetimeColumn"></param>
    /// <param name="columns"></param>
    /// <param name="dow"></param>
    /// <returns></returns>
    /// <remarks>tab.ValuesOfOn("close",DayOfWeek.Friday)</remarks>
    public static Table ValuesOfOn(this Table table, string datetimeColumn, string[] columns, DayOfWeek dow)
    {
        return table.ValuesOn(datetimeColumn, dow).Pick(columns);
    }
    public static Table ValuesOn(this Table table, string datetimeColumn, DateTime[] specificDates)
    {
        table.ThrowIfTableIsNull();
        table.ThrowIfColumnsAreNotPresentInTable(datetimeColumn);

        var dateSet = new HashSet<DateTime>(specificDates.Select(d => d.Date));
        var result = new Table();

        for (var i = 0; i < table.RowCount; i++)
        {
            var dt = DateTime.Parse(table[i][datetimeColumn]).Date;
            if (dateSet.Contains(dt))
            {
                result.AddRow(table[i]);
            }
        }

        return result;
    }
    /// <summary>
    /// Groups row indices by their frequency bucket
    /// </summary>
    private static Dictionary<DateTime, List<int>> GroupByFrequency(
        List<DateTime> timestamps, DateTimeFrequency frequency)
    {
        var grouped = new Dictionary<DateTime, List<int>>();

        for (int i = 0; i < timestamps.Count; i++)
        {
            var bucketDate = GetBucketDate(timestamps[i], frequency);

            if (!grouped.ContainsKey(bucketDate))
                grouped[bucketDate] = new List<int>();

            grouped[bucketDate].Add(i);
        }

        return grouped;
    }

    /// <summary>
    /// Determines which bucket a timestamp belongs to based on frequency
    /// </summary>
    private static DateTime GetBucketDate(DateTime dt, DateTimeFrequency frequency)
    {
        return frequency switch
        {
            // Business day - return same date if weekday, else skip to next weekday
            DateTimeFrequency.BusinessDay => dt.DayOfWeek switch
            {
                DayOfWeek.Saturday => dt.AddDays(2).Date,
                DayOfWeek.Sunday => dt.AddDays(1).Date,
                _ => dt.Date
            },

            // Calendar day
            DateTimeFrequency.CalendarDay => dt.Date,

            // Weekly - start of week (Monday)
            DateTimeFrequency.Weekly => GetWeekStart(dt),

            // Month end - last day of month
            DateTimeFrequency.MonthEnd => new DateTime(dt.Year, dt.Month,
                DateTime.DaysInMonth(dt.Year, dt.Month)),

            // Month start - first day of month
            DateTimeFrequency.MonthStart => new DateTime(dt.Year, dt.Month, 1),

            // Business month end - last business day of month
            DateTimeFrequency.BusinessMonthEnd => GetLastBusinessDay(dt.Year, dt.Month),

            // Business month start - first business day of month
            DateTimeFrequency.BusinessMonthStart => GetFirstBusinessDay(dt.Year, dt.Month),

            // Quarter end
            DateTimeFrequency.QuarterEnd => GetQuarterEnd(dt),

            // Quarter start
            DateTimeFrequency.QuarterStart => GetQuarterStart(dt),

            // Year end
            DateTimeFrequency.YearEnd => new DateTime(dt.Year, 12, 31),

            // Year start
            DateTimeFrequency.YearStart => new DateTime(dt.Year, 1, 1),

            // Hourly
            DateTimeFrequency.Hourly => new DateTime(dt.Year, dt.Month, dt.Day,
                dt.Hour, 0, 0),

            // Minutely
            DateTimeFrequency.Minutely => new DateTime(dt.Year, dt.Month, dt.Day,
                dt.Hour, dt.Minute, 0),

            // Secondly
            DateTimeFrequency.Secondly => new DateTime(dt.Year, dt.Month, dt.Day,
                dt.Hour, dt.Minute, dt.Second),

            _ => dt.Date
        };
    }

    private static DateTime GetWeekStart(DateTime dt)
    {
        var daysFromMonday = ((int)dt.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return dt.Date.AddDays(-daysFromMonday);
    }

    private static DateTime GetQuarterEnd(DateTime dt)
    {
        var quarter = (dt.Month - 1) / 3 + 1;
        var quarterEndMonth = quarter * 3;
        return new DateTime(dt.Year, quarterEndMonth,
            DateTime.DaysInMonth(dt.Year, quarterEndMonth));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static DateTime GetQuarterStart(DateTime dt)
    {
        var quarter = (dt.Month - 1) / 3 + 1;
        var quarterStartMonth = (quarter - 1) * 3 + 1;
        return new DateTime(dt.Year, quarterStartMonth, 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    private static DateTime GetLastBusinessDay(int year, int month)
    {
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        while (lastDay.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            lastDay = lastDay.AddDays(-1);
        }

        return lastDay;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    private static DateTime GetFirstBusinessDay(int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);
        while (firstDay.DayOfWeek == DayOfWeek.Saturday ||
               firstDay.DayOfWeek == DayOfWeek.Sunday)
        {
            firstDay = firstDay.AddDays(1);
        }

        return firstDay;
    }

    /// <summary>
    /// Checks if a column contains all numeric values
    /// </summary>
    private static bool IsNumericColumn(Table tab, string columnName)
    {
        var values = tab.ValuesOf(columnName);
        return values.All(v => double.TryParse(v, out _));
    }

    /// <summary>
    /// Applies the aggregation method to a list of values
    /// </summary>
    private static double ApplyAggregation(List<double> values, AggregationMethod method)
    {
        if (values.Count == 0)
            return 0;

        return method switch
        {
            AggregationMethod.Sum => values.Sum(),
            AggregationMethod.Average => values.Average(),
            AggregationMethod.Max => values.Max(),
            AggregationMethod.Min => values.Min(),
            AggregationMethod.Count => values.Count,
            AggregationMethod.StandardDeviation => CalculateStdDev(values),
            AggregationMethod.Variance => CalculateVariance(values),
            AggregationMethod.Range => values.Max() - values.Min(),
            AggregationMethod.AboveAverageCount => values.Count(v => v > values.Average()),
            AggregationMethod.BelowAverageCount => values.Count(v => v < values.Average()),
            AggregationMethod.AverageCount => values.Count(v => Math.Abs(v - values.Average()) < 0.0001),
            // Default to average if method is unrecognized
            _ => values.Average()
        };
    }

    private static double CalculateStdDev(List<double> values)
    {
        if (values.Count == 0) return 0;
        var avg = values.Average();
        var sumOfSquares = values.Sum(v => Math.Pow(v - avg, 2));
        return Math.Sqrt(sumOfSquares / values.Count);
    }

    private static double CalculateVariance(List<double> values)
    {
        if (values.Count == 0) return 0;
        var avg = values.Average();
        return values.Sum(v => Math.Pow(v - avg, 2)) / values.Count;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <param name="datetimeColumn"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Table GranularizeDateColumn(this Table table, string datetimeColumn, 
                                              MonthGranularizationFormat format = MonthGranularizationFormat.MonthNumbers)
    {
        table.ThrowIfTableIsNull();
        table.ThrowIfColumnsAreNotPresentInTable(datetimeColumn);

        var result = table;
        var years = new List<string>();
        var months = new List<string>();
        var days = new List<string>();
        var hours = new List<string>();
        var minutes = new List<string>();
        var seconds = new List<string>();
        var dows = new List<string>();

        for (var i = 0; i < table.RowCount; i++)
        {
            var row = table[i];
            var dt = DateTime.Parse(row[datetimeColumn]);
            years.Add(dt.Year.ToString());
            switch (format)
            {
                case MonthGranularizationFormat.ShortMonthNames:
                    months.Add(dt.ToString("MMM"));
                    break;
                case MonthGranularizationFormat.FullMonthNames:
                    months.Add(dt.ToString("MMMM"));
                    break;
                case MonthGranularizationFormat.MonthNumbers:
                    months.Add(dt.Month.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            months.Add(dt.Month.ToString());
            days.Add(dt.Day.ToString());
            hours.Add(dt.Hour.ToString());
            minutes.Add(dt.Minute.ToString());
            seconds.Add(dt.Second.ToString());
            dows.Add(dt.DayOfWeek.ToString());
        }

        result.AddColumn("Year", years);
        result.AddColumn("Month", months);
        result.AddColumn("Day", days);
        result.AddColumn("Hour", hours);
        result.AddColumn("Minute", minutes);
        result.AddColumn("Second", seconds);
        result.AddColumn("Dow", dows);
        return result;
    }
    
   
 
}


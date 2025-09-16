namespace Squirrel;

public static class StringStatisticsExtensions
{
    /// <summary>
    /// Calculates the average of string values by converting them to decimals
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>The average value as decimal</returns>
    public static decimal Average(this IEnumerable<string> values)
    {
        return values.Where(s => decimal.TryParse(s, out _))
                     .Select(decimal.Parse)
                     .Average();
    }

    /// <summary>
    /// Calculates the median of string values by converting them to decimals
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>The median value as decimal</returns>
    public static decimal Median(this IEnumerable<string> values)
    {
        var decimals = values.Where(s => decimal.TryParse(s, out _))
                             .Select(decimal.Parse)
                             .ToList();
        return BasicStatistics.Median(decimals);
    }

    /// <summary>
    /// Calculates the range of string values by converting them to decimals
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>The range (max - min) as decimal</returns>
    public static decimal Range(this IEnumerable<string> values)
    {
        return values.Where(s => decimal.TryParse(s, out _))
                     .Select(decimal.Parse)
                     .Range();
    }

    /// <summary>
    /// Calculates standard deviation of string values by converting them to decimals
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>The standard deviation as double</returns>
    public static double StandardDeviation(this IEnumerable<string> values)
    {
        var decimals = values.Where(s => decimal.TryParse(s, out _))
                             .Select(decimal.Parse)
                             .ToList();
        return StandardDeviationImpl(decimals);
    }

    /// <summary>
    /// Calculates variance of string values by converting them to decimals
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>The variance as double</returns>
    public static double Variance(this IEnumerable<string> values)
    {
        var decimals = values.Where(s => decimal.TryParse(s, out _))
                             .Select(decimal.Parse)
                             .ToList();
        return VarianceImpl(decimals);
    }

    /// <summary>
    /// Calculates kurtosis of string values by converting them to decimals
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>The kurtosis as double</returns>
    public static double Kurtosis(this IEnumerable<string> values)
    {
        var doubles = values.Where(s => double.TryParse(s, out _))
                           .Select(double.Parse)
                           .ToList();
        return doubles.Kurtosis();
    }

    /// <summary>
    /// Counts values above average from string values
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>Count of values above average</returns>
    public static int AboveAverageCount(this IEnumerable<string> values)
    {
        return values.Where(s => decimal.TryParse(s, out _))
                     .Select(decimal.Parse)
                     .AboveAverageCount();
    }

    /// <summary>
    /// Counts values below average from string values
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>Count of values below average</returns>
    public static int BelowAverageCount(this IEnumerable<string> values)
    {
        return values.Where(s => decimal.TryParse(s, out _))
                     .Select(decimal.Parse)
                     .BelowAverageCount();
    }

    /// <summary>
    /// Gets the IQR outlier range for string values
    /// </summary>
    /// <param name="values">String values that can be parsed as decimals</param>
    /// <returns>Tuple containing lower and upper bounds for outlier detection</returns>
    public static Tuple<decimal, decimal> IqrRange(this IEnumerable<string> values)
    {
        var decimals = values.Where(s => decimal.TryParse(s, out _))
                             .Select(decimal.Parse)
                             .ToList();
        return BasicStatistics.IqrRange(decimals);
    }

    // Helper methods for missing implementations
    private static double StandardDeviationImpl(IEnumerable<decimal> values)
    {
        var doubles = values.Select(d => (double)d).ToList();
        return Math.Sqrt(VarianceImpl(values));
    }

    private static double VarianceImpl(IEnumerable<decimal> values)
    {
        var doubles = values.Select(d => (double)d).ToList();
        if (!doubles.Any()) return 0.0;
        
        double mean = doubles.Average();
        return doubles.Select(d => Math.Pow(d - mean, 2)).Average();
    }
    /// <summary>
    /// Calculates the Median Absolute Deviation of string values by converting them to decimals
    /// </summary>
    public static decimal MedianAbsoluteDeviation(this IEnumerable<string> values)
    {
        var decimals = values.Where(s => decimal.TryParse(s, out _))
            .Select(decimal.Parse)
            .ToList();
        return decimals.MedianAbsoluteDeviation();
    }

    /// <summary>
    /// Calculates the percentile of string values by converting them to decimals
    /// </summary>
    public static decimal Percentile(this IEnumerable<string> values, decimal percentile)
    {
        var decimals = values.Where(s => decimal.TryParse(s, out _))
            .Select(decimal.Parse)
            .ToList();
        return decimals.Percentile(percentile);
    }
}
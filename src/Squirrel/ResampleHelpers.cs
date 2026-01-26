namespace Squirrel;

public static class ResampleHelpers
{
    /// <summary>
    /// Finds which bucket (from the generated frequency buckets) a given date belongs to
    /// </summary>
    /// <param name="date">The date to bucket</param>
    /// <param name="buckets">List of frequency bucket dates (pre-generated)</param>
    /// <param name="frequency">The frequency type</param>
    /// <returns>The bucket date this date belongs to</returns>
    private static DateTime FindBucket(DateTime date, List<DateTime> buckets, 
        DateTimeFrequency frequency)
    {
        if (buckets == null || buckets.Count == 0)
            throw new ArgumentException("Buckets list cannot be null or empty");
        
        // Sort buckets to ensure proper ordering
        var sortedBuckets = buckets.OrderBy(b => b).ToList();
        
        switch (frequency)
        {
            case DateTimeFrequency.BusinessDay:
            case DateTimeFrequency.CalendarDay:
            case DateTimeFrequency.Hourly:
            case DateTimeFrequency.Minutely:
            case DateTimeFrequency.Secondly:
                // For these, find exact or closest bucket
                return FindClosestBucket(date, sortedBuckets);
            
            case DateTimeFrequency.Weekly:
                // Find the week start bucket this date falls into
                return FindWeeklyBucket(date, sortedBuckets);
            
            case DateTimeFrequency.MonthEnd:
            case DateTimeFrequency.BusinessMonthEnd:
                // Find the month end bucket this date falls into
                return FindMonthEndBucket(date, sortedBuckets);
            
            case DateTimeFrequency.MonthStart:
            case DateTimeFrequency.BusinessMonthStart:
                // Find the month start bucket this date falls into
                return FindMonthStartBucket(date, sortedBuckets);
            
            case DateTimeFrequency.QuarterEnd:
            case DateTimeFrequency.BusinessQuarterEnd:
                // Find the quarter end bucket this date falls into
                return FindQuarterEndBucket(date, sortedBuckets);
            
            case DateTimeFrequency.QuarterStart:
            case DateTimeFrequency.BusinessQuarterStart:
                // Find the quarter start bucket this date falls into
                return FindQuarterStartBucket(date, sortedBuckets);
            
            case DateTimeFrequency.YearEnd:
            case DateTimeFrequency.BusinessYearEnd:
                // Find the year end bucket this date falls into
                return FindYearEndBucket(date, sortedBuckets);
            
            case DateTimeFrequency.YearStart:
            case DateTimeFrequency.BusinessYearStart:
                // Find the year start bucket this date falls into
                return FindYearStartBucket(date, sortedBuckets);
            
            default:
                return FindClosestBucket(date, sortedBuckets);
        }
    }
    
    /// <summary>
    /// Finds the closest bucket that is less than or equal to the date
    /// </summary>
    private static DateTime FindClosestBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        // Find the largest bucket that is <= date
        DateTime? closestBucket = null;
        
        foreach (var bucket in sortedBuckets)
        {
            if (bucket <= date)
                closestBucket = bucket;
            else
                break; // Since sorted, no need to continue
        }
        
        return closestBucket ?? sortedBuckets.First();
    }
    
    /// <summary>
    /// Finds which weekly bucket a date belongs to
    /// </summary>
    private static DateTime FindWeeklyBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        // Get the start of the week for this date (Monday)
        var daysFromMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = date.Date.AddDays(-daysFromMonday);
        
        // Find the bucket that matches or is closest before this week start
        return sortedBuckets
            .Where(b => b <= weekStart)
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
    
    /// <summary>
    /// Finds which month-end bucket a date belongs to
    /// </summary>
    private static DateTime FindMonthEndBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        // Get the end of the month for this date
        var monthEnd = new DateTime(date.Year, date.Month, 
            DateTime.DaysInMonth(date.Year, date.Month));
        
        // Find the bucket that matches this month end or the previous one
        return sortedBuckets
            .Where(b => b.Year < date.Year || 
                       (b.Year == date.Year && b.Month <= date.Month))
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
    
    /// <summary>
    /// Finds which month-start bucket a date belongs to
    /// </summary>
    private static DateTime FindMonthStartBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        // Get the start of the month for this date
        var monthStart = new DateTime(date.Year, date.Month, 1);
        
        // Find the bucket that matches this month start or the previous one
        return sortedBuckets
            .Where(b => b <= monthStart)
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
    
    /// <summary>
    /// Finds which quarter-end bucket a date belongs to
    /// </summary>
    private static DateTime FindQuarterEndBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        // Calculate which quarter this date is in (1-4)
        var quarter = (date.Month - 1) / 3 + 1;
        var quarterEndMonth = quarter * 3;
        var quarterEnd = new DateTime(date.Year, quarterEndMonth, 
            DateTime.DaysInMonth(date.Year, quarterEndMonth));
        
        // Find the bucket for this quarter end or previous
        return sortedBuckets
            .Where(b => b <= quarterEnd)
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
    
    /// <summary>
    /// Finds which quarter-start bucket a date belongs to
    /// </summary>
    private static DateTime FindQuarterStartBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        // Calculate which quarter this date is in
        var quarter = (date.Month - 1) / 3 + 1;
        var quarterStartMonth = (quarter - 1) * 3 + 1;
        var quarterStart = new DateTime(date.Year, quarterStartMonth, 1);
        
        // Find the bucket for this quarter start or previous
        return sortedBuckets
            .Where(b => b <= quarterStart)
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
    
    /// <summary>
    /// Finds which year-end bucket a date belongs to
    /// </summary>
    private static DateTime FindYearEndBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        var yearEnd = new DateTime(date.Year, 12, 31);
        
        return sortedBuckets
            .Where(b => b.Year <= date.Year)
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
    
    /// <summary>
    /// Finds which year-start bucket a date belongs to
    /// </summary>
    private static DateTime FindYearStartBucket(DateTime date, List<DateTime> sortedBuckets)
    {
        var yearStart = new DateTime(date.Year, 1, 1);
        
        return sortedBuckets
            .Where(b => b <= yearStart)
            .OrderByDescending(b => b)
            .FirstOrDefault(sortedBuckets.First());
    }
}  

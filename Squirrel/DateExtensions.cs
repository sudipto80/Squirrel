namespace Squirrel;

public static class DateExtensions
{
    public static  List<DateTime> GenerateDateRange(DateTime startDate, DateTime endDate)
    {
        return new List<DateTime>();
    }

    public static List<DateTime> GenerateDateRange(DateTime startDate, int days)
    {
        return new List<DateTime>();
    }

    public static  List<DateTime> GenerateDateRangeFromToday()
    {
        return new List<DateTime>();
    }

    /// <summary>
    /// Calculates a list of business days (weekdays excluding weekends) within a specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A list of <see cref="DateTime"/> objects representing business days between the start and end dates.</returns>
    /// <exception cref="ArgumentException">Thrown if the start date is later than the end date.</exception>
    private static List<DateTime> GetBusinessDays(DateTime startDate, DateTime endDate)
    {  
        var dateRange = new List<DateTime>();
        for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
        {
            if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
            {
                dateRange.Add(dt);   
            }
        }
        return dateRange;
    }

    /// <summary>
    /// Retrieves a list of dates representing the last day of each month between a given start and end date range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A list of <see cref="DateTime"/> objects, where each date corresponds to the last day of a month within the specified range.</returns>
    /// <exception cref="ArgumentException">Thrown if the start date is later than the end date.</exception>
    private static List<DateTime> GetMonthEndDays(DateTime startDate, DateTime endDate)
    {  
        var dateRange = new List<DateTime>();
        for (var dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
        {
            var lastDayOfMonth = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            dateRange.Add(lastDayOfMonth);
        }
        return dateRange;
    }

    /// <summary>
    /// Generates a list of dates between a specified start and end date based on a given frequency.
    /// </summary>
    /// <param name="startDate">The starting date of the range.</param>
    /// <param name="endDate">The ending date of the range.</param>
    /// <param name="frequency">The frequency type that determines the intervals used to generate dates within the range.</param>
    /// <returns>A list of <see cref="DateTime"/> objects representing the generated date range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported frequency type is provided.</exception>
    public static List<DateTime> GenerateDateRange(DateTime startDate, DateTime endDate, DateTimeFrequency frequency)
    {
        var dateRange = new List<DateTime>();
        switch (frequency)
        {
            case DateTimeFrequency.BusinessDay:
                dateRange = GetBusinessDays(startDate, endDate);
                break;
            case DateTimeFrequency.CustomBusinessDay:
                break;
            case DateTimeFrequency.CalendarDay:
                break;
            case DateTimeFrequency.Weekly:
                dateRange = GetWeeklyStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.MonthEnd:
                dateRange = GetMonthEndDays(startDate, endDate);
                break;
            case DateTimeFrequency.SemiMonthEnd:
                dateRange = GetSemiMonthEndDays(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessMonthEnd:
                dateRange = GetBusinessMonthEndDays(startDate, endDate);
                break;
            case DateTimeFrequency.CustomBusinessMonthEnd:
                break;
            case DateTimeFrequency.MonthStart:
                dateRange = GetMonthStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.SemiMonthStart:
                dateRange = GetSemiMonthStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessMonthStart:
                dateRange = GetBusinessMonthStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.CustomBusinessMonthStart:
                break;
            case DateTimeFrequency.QuarterEnd:
                dateRange = GetQuarterEndDays(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessQuarterEnd:
                break;
            case DateTimeFrequency.QuarterStart:
                dateRange = GetQuarterStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessQuarterStart:
                break;
            case DateTimeFrequency.YearEnd:
                dateRange = GetYearEndDays(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessYearEnd:
                dateRange = GetBusinessYearEndDays(startDate, endDate);
                break;
            case DateTimeFrequency.YearStart:
                dateRange = GetYearStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessYearStart:
                dateRange = GetBusinessYearStartDays(startDate, endDate);
                break;
            case DateTimeFrequency.Hourly:
                dateRange = GetHours(startDate, endDate);
                break;
            case DateTimeFrequency.BusinessHour:
                dateRange = GetBusinessHours(startDate, endDate);
                break;
            case DateTimeFrequency.CustomBusinessHour:
                break;
            case DateTimeFrequency.Minutely:
                dateRange = Minutely(startDate, endDate);
                break;
            case DateTimeFrequency.Secondly:
                break;
            case DateTimeFrequency.Milliseconds:
                break;
            case DateTimeFrequency.Microseconds:
                break;
            case DateTimeFrequency.Nanoseconds:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null);
        }

        return dateRange;
    }

    private static List<DateTime> Minutely(DateTime startDate, DateTime endDate)
    {
       return new List<DateTime>();
    }

    private static List<DateTime> GetBusinessHours(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));
    
        var businessHours = new List<DateTime>();
    
        // Start from the beginning of the hour containing startDate
        DateTime current = new DateTime(startDate.Year, startDate.Month, startDate.Day, 
            startDate.Hour, 0, 0);
    
        while (current <= endDate)
        {
            // Check if it's a business day (Monday-Friday) and business hours (9 AM - 5 PM)
            if (IsBusinessHour(current))
            {
                businessHours.Add(current);
            }
        
            current = current.AddHours(1);
        }
    
        return businessHours;
    }

    private static bool IsBusinessHour(DateTime dateTime)
    {
        // Check if it's a weekday
        if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            return false;
    
        // Check if it's within business hours (9 AM to 5 PM)
        // Note: 17 (5 PM) is typically the end, so we might include 9-16 (9 AM - 4 PM) or 9-17 (9 AM - 5 PM)
        return dateTime.Hour >= 9 && dateTime.Hour < 17;
    }

    private static List<DateTime> GetHours(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));
    
        var hours = new List<DateTime>();
    
        // Start from the beginning of the hour containing startDate
        DateTime current = new DateTime(startDate.Year, startDate.Month, startDate.Day, 
            startDate.Hour, 0, 0);
    
        while (current <= endDate)
        {
            hours.Add(current);
            current = current.AddHours(1);
        }
    
        return hours;
    }

    private static List<DateTime> GetBusinessYearStartDays(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));
    
        var dateRange = new List<DateTime>();
    
        for (int year = startDate.Year; year <= endDate.Year; year++)
        {
            DateTime yearStart = new DateTime(year, 1, 1);
            DateTime firstBusinessDay = GetFirstBusinessDay(yearStart);
            dateRange.Add(firstBusinessDay);
        }
    
        return dateRange;
    }

    private static List<DateTime> GetYearStartDays(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date.");
        return Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1)
            .Select(year => new DateTime(year, 1, 1))
            .ToList();
    }

    private static List<DateTime> GetBusinessYearEndDays(DateTime startDate, DateTime endDate)
    {
        List<DateTime> businessYearEnds = new List<DateTime>();
    
        // Start from the year containing startDate
        int currentYear = startDate.Year;
    
        while (true)
        {
            DateTime yearEnd = new DateTime(currentYear, 12, 31);
        
            // Stop if this year end is beyond endDate
            if (yearEnd > endDate)
            {
                break;
            }
        
            // Find the last business day of the year
            DateTime lastBusinessDay = GetLastBusinessDayOfYear(currentYear);
        
            // Add it if it's within the date range
            if (lastBusinessDay >= startDate && lastBusinessDay <= endDate)
            {
                businessYearEnds.Add(lastBusinessDay);
            }
        
            currentYear++;
        }
    
        return businessYearEnds;
    }

    private static DateTime GetLastBusinessDayOfYear(int year)
    {
        DateTime current = new DateTime(year, 12, 31);
    
        // Keep going backwards until we find a weekday (Monday-Friday)
        while (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
        {
            current = current.AddDays(-1);
        }
    
        return current;
    }

    private static List<DateTime> GetYearEndDays(DateTime startDate, DateTime endDate)
    {
        List<DateTime> yearEnds = new List<DateTime>();
    
        // Start from the year containing startDate
        int currentYear = startDate.Year;
    
        while (true)
        {
            DateTime yearEnd = new DateTime(currentYear, 12, 31);
        
            // Stop if this year end is beyond endDate
            if (yearEnd > endDate)
            {
                break;
            }
        
            // Add the year end if it's at or after startDate
            if (yearEnd >= startDate)
            {
                yearEnds.Add(yearEnd);
            }
        
            currentYear++;
        }
    
        return yearEnds;
    }

    private static List<DateTime> GetQuarterStartDays(DateTime startDate, DateTime endDate)
    {
        List<DateTime> quarterStarts = new List<DateTime>();
    
        // Start from the first day of the quarter containing startDate
        DateTime current = GetQuarterStart(startDate);
    
        // Add quarter starts until we pass endDate
        while (current <= endDate)
        {
            quarterStarts.Add(current);
            current = current.AddMonths(3);
        }
    
        return quarterStarts;
    }

    private static DateTime GetQuarterStart(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;

        return month switch
        {
            // Determine which quarter we're in and get its start date
            <= 3 => new DateTime(year, 1, 1),
            <= 6 => new DateTime(year, 4, 1),
            <= 9 => new DateTime(year, 7, 1),
            _ => new DateTime(year, 10, 1)
        };
    }

    private static List<DateTime> GetQuarterEndDays(DateTime startDate, DateTime endDate)
    {
        List<DateTime> quarterEnds = new List<DateTime>();
    
        // Start from the first quarter end at or after startDate
        DateTime current = GetNextQuarterEnd(startDate);
    
        // Add quarter ends until we pass endDate
        while (current <= endDate)
        {
            quarterEnds.Add(current);
            current = current.AddMonths(3);
        }
    
        return quarterEnds;
    }

    private static DateTime GetNextQuarterEnd(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        return month switch
        {
            // Determine which quarter we're in and get its end date
            <= 3 => new DateTime(year, 3, 31),
            <= 6 => new DateTime(year, 6, 30),
            <= 9 => new DateTime(year, 9, 30),
            _ => new DateTime(year, 12, 31)
        };
    }

    private static List<DateTime> GetBusinessMonthStartDays(DateTime startDate, DateTime endDate)
    {
        List<DateTime> businessMonthStarts = new List<DateTime>();
    
        // Start from the first day of the month containing startDate
        DateTime current = new DateTime(startDate.Year, startDate.Month, 1);
    
        while (current <= endDate)
        {
            // Find the first business day of the month
            DateTime firstBusinessDay = GetFirstBusinessDay(current);
        
            // Add it if it's within the date range
            if (firstBusinessDay >= startDate && firstBusinessDay <= endDate)
            {
                businessMonthStarts.Add(firstBusinessDay);
            }
        
            // Move to next month
            current = current.AddMonths(1);
        }
    
        return businessMonthStarts;
    }

    private static DateTime GetFirstBusinessDay(DateTime monthStart)
    {
        DateTime current = monthStart;
    
        // Keep advancing until we find a weekday (Monday-Friday)
        while (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
        {
            current = current.AddDays(1);
        }
    
        return current;
    }

    private static List<DateTime> GetSemiMonthStartDays(DateTime startDate, DateTime endDate)
    {
        List<DateTime> semiMonthStarts = new List<DateTime>();

        // Start from the first day of the month containing startDate
        DateTime current = new DateTime(startDate.Year, startDate.Month, 1);

        while (current <= endDate)
        {
            // Add the 1st of the month if it's within range
            if (current >= startDate && current <= endDate)
            {
                semiMonthStarts.Add(current);
            }

            // Add the 16th of the month if it's within range
            DateTime midMonth = new DateTime(current.Year, current.Month, 16);
            if (midMonth >= startDate && midMonth <= endDate)
            {
                semiMonthStarts.Add(midMonth);
            }

            // Move to next month
            current = current.AddMonths(1);
        }

        return semiMonthStarts;
    }

    private static List<DateTime> GetMonthStartDays(DateTime startDate, DateTime endDate)
    {
        var monthStarts = new List<DateTime>();
        // Start from the first day of the month containing startDate
        DateTime current = new DateTime(startDate.Year, startDate.Month, 1);
        // Add month starts until we pass endDate
        while (current <= endDate)
        {
            monthStarts.Add(current);
            current = current.AddMonths(1);
        }
        return monthStarts;
    }

    private static List<DateTime> GetSemiMonthEndDays(DateTime startDate, DateTime endDate)
    {
        var dateRange = new List<DateTime>();
        for (var dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
        {
            // Add 15th of the month if it's within range
            var midMonth = new DateTime(dt.Year, dt.Month, 15);
            if (midMonth >= startDate && midMonth <= endDate)
            {
                dateRange.Add(midMonth);
            }
            // Add last day of the month if it's within range
            var lastDayOfMonth = new DateTime(dt.Year, dt.Month,
                DateTime.DaysInMonth(dt.Year, dt.Month));
            if (lastDayOfMonth >= startDate && lastDayOfMonth <= endDate)
            {
                dateRange.Add(lastDayOfMonth);
            }
        }
        return dateRange;
    }

    private static List<DateTime> GetWeeklyStartDays(DateTime startDate, DateTime endDate)
    {
        var dateRange = new List<DateTime>();
    
        // Find the Monday of the week containing 'startDate'
        var current = startDate;
        int daysSinceMonday = ((int)current.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        current = current.AddDays(-daysSinceMonday);
    
        // Add all Mondays until endDate
        while (current <= endDate)
        {
            dateRange.Add(current);
            current = current.AddDays(7);
        }
    
        return dateRange;
    }

    private static List<DateTime> GetBusinessMonthEndDays(DateTime startDate, DateTime endDate)
    {
        var dateRange = new List<DateTime>();
        for (var dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
        {
            var lastDayOfMonth = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        
            while (lastDayOfMonth.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);
            }
        
            dateRange.Add(lastDayOfMonth);
        }
        return dateRange;
    }
}
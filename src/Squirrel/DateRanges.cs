namespace Squirrel;

public static class DateRanges
{
    private static List<DateTime> DaysFrom(this int days, DayOfWeek dayOfWeek, DateTime? from)
    {
        var resultDays = new List<DateTime>();
        var startDate = from ?? DateTime.Today;
        while (resultDays.Count < days)
        {
            if (startDate.DayOfWeek == dayOfWeek)
            {
                resultDays.Add(startDate);
            }
            startDate = startDate.AddDays(1);
        }
        return resultDays;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="days"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public static List<DateTime> MondaysFrom(this int days, DateTime? from) 
        => days.DaysFrom(DayOfWeek.Monday, from);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="days"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public static List<DateTime> TuesdaysFrom(this int days, DateTime? from) 
        => days.DaysFrom(DayOfWeek.Tuesday, from);

    public static List<DateTime> WednesdaysFrom(this int days, DateTime? from) 
        => days.DaysFrom(DayOfWeek.Wednesday, from);

    public static List<DateTime> ThursdaysFrom(this int days, DateTime? from) 
        => days.DaysFrom(DayOfWeek.Thursday, from);

    public static List<DateTime> FridaysFrom(this int days, DateTime? from) 
        => days.DaysFrom(DayOfWeek.Friday, from);
    
    public static List<DateTime> SaturdaysFrom(this int days, DateTime? from) 
        => days.DaysFrom(DayOfWeek.Saturday, from);

    public static List<DateTime> BusinessDaysFrom(this int days, DateTime? from)
    {
        var resultDays = new List<DateTime>();
        var startDate = from ?? DateTime.Today;
        while (resultDays.Count < days)
        {
            if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                resultDays.Add(startDate);
            }
            startDate = startDate.AddDays(1);
        }
        return resultDays;
    }

    public static List<DateTime> WeekdaysFrom(this int days, DateTime? from) => BusinessDaysFrom(days, from);

    public static List<DateTime> AlternateMondaysFrom(this int days, DateTime? from)
    {
        var resultDays = new List<DateTime>();
        var startDate = from ?? DateTime.Today;
        while (resultDays.Count < days)
        {
            if (startDate.DayOfWeek == DayOfWeek.Monday && (resultDays.Count == 0 || (startDate - resultDays.Last()).TotalDays >= 14))
            {
                resultDays.Add(startDate);
            }
            startDate = startDate.AddDays(1);
        }
        return resultDays;
    }
   // pulic static List<DateTime> Generaate
    //
    // public static List<int> MatchingIndices(this )
}
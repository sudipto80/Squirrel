namespace Squirrel;

public enum DateTimeFrequency
{
    /// <summary>Business day frequency</summary>
    BusinessDay,
    
    /// <summary>Custom business day frequency</summary>
    CustomBusinessDay,
    
    /// <summary>Calendar day frequency</summary>
    CalendarDay,
    
    /// <summary>Weekly frequency</summary>
    Weekly,
    
    /// <summary>Month end frequency</summary>
    MonthEnd,
    
    /// <summary>Semi-month end frequency (15th and end of month)</summary>
    SemiMonthEnd,
    
    /// <summary>Business month end frequency</summary>
    BusinessMonthEnd,
    
    /// <summary>Custom business month end frequency</summary>
    CustomBusinessMonthEnd,
    
    /// <summary>Month start frequency</summary>
    MonthStart,
    
    /// <summary>Semi-month start frequency (1st and 15th)</summary>
    SemiMonthStart,
    
    /// <summary>Business month start frequency</summary>
    BusinessMonthStart,
    
    /// <summary>Custom business month start frequency</summary>
    CustomBusinessMonthStart,
    
    /// <summary>Quarter end frequency</summary>
    QuarterEnd,
    
    /// <summary>Business quarter end frequency</summary>
    BusinessQuarterEnd,
    
    /// <summary>Quarter start frequency</summary>
    QuarterStart,
    
    /// <summary>Business quarter start frequency</summary>
    BusinessQuarterStart,
    
    /// <summary>Year end frequency</summary>
    YearEnd,
    
    /// <summary>Business year end frequency</summary>
    BusinessYearEnd,
    
    /// <summary>Year start frequency</summary>
    YearStart,
    
    /// <summary>Business year start frequency</summary>
    BusinessYearStart,
    
    /// <summary>Hourly frequency</summary>
    Hourly,
    
    /// <summary>Business hour frequency</summary>
    BusinessHour,
    
    /// <summary>Custom business hour frequency</summary>
    CustomBusinessHour,
    
    /// <summary>Minutely frequency</summary>
    Minutely,
    
    /// <summary>Secondly frequency</summary>
    Secondly,
    
    /// <summary>Milliseconds</summary>
    Milliseconds,
    
    /// <summary>Microseconds</summary>
    Microseconds,
    
    /// <summary>Nanoseconds</summary>
    Nanoseconds
}
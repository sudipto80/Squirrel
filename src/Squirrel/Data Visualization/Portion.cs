namespace Squirrel.Data_Visualization;

/// <summary>
/// How the value of a portion of a pie/doughnut chart
/// is determined. 
/// </summary>
public enum Portion
{
    /// <summary>
    /// The raw value of that category.
    /// </summary>
    RawValue,
    /// <summary>
    /// The percentage value of the category.
    /// </summary>
    Percentage,
    /// <summary>
    /// The min-max normalized percentage of the category.
    /// </summary>
    MinMaxPercentage
}
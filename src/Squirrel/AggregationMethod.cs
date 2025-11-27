using System.ComponentModel;

namespace Squirrel;


/// <summary>
/// Method to be used for aggregation/consolidation
/// </summary>
public enum AggregationMethod 
{
    /// <summary>
    /// Summation of the sequence
    /// </summary>
    [Description("by summing")]
    Sum, 
    /// <summary>
    /// Average of the sequence
    /// </summary>
    [Description("by average,by mean")]        
    Average, 
    /// <summary>
    /// Maximum value of the sequence
    /// </summary>
    [Description("by max,by maximum value")]
    Max,
    /// <summary>
    /// Minimum value of the sequence
    /// </summary>
    [Description("by min,by minimum value")]
    Min, 
    /// <summary>
    /// Total count of the sequence
    /// </summary>
    [Description("by number of entries,by count")]
    Count,
    /// <summary>
    /// Standard deviation of the sequence      
    /// </summary>
    [Description("by standard deviation")]        
    StandardDeviation, 
    /// <summary>
    /// Calculates the variance of the sequence 
    /// </summary>
    [Description("by variance")]
    Variance, 
    /// <summary>
    /// Number of instances above average
    /// </summary>
    [Description("more than average")]
    AboveAverageCount,  
    /// <summary>
    /// Number of instances which are below the average value
    /// </summary>
    [Description("less than average")]
    BelowAverageCount,
    /// <summary>
    /// Number of instances that has the value equal to average
    /// </summary>
    [Description("just average,average")]
    AverageCount,
    /// <summary>
    /// Measures the skewness of the given sequence
    /// </summary>
    [Description("skewed")]
    Skew, 
    /// <summary>
    /// Measures the Kurtosis of the given sequence
    /// </summary>
    [Description("kurtosis")]       
    Kurtosis, 
    /// <summary>
    /// Measures the range of values of the given sequence.
    /// </summary>
    [Description("by range")]
    Range 
}

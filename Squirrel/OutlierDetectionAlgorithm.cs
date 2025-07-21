namespace Squirrel;


/// <summary>
/// The Algorithm to use for outlier detection
/// </summary>
public enum OutlierDetectionAlgorithm 
{
    /// <summary>
    /// Inter Quantile Range
    /// </summary>
    IqrInterval, 
    /// <summary>
    /// Z Score
    /// </summary>
    ZScore 
}
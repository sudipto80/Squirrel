namespace Squirrel.DataVisualization;

/// <summary>
/// Represents a dataset for chart visualization with styling properties
/// </summary>
public class ChartJSBarChartDataset
{
    /// <summary>
    /// The label for the dataset which appears in the legend and tooltips
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The data points for this dataset
    /// </summary>
    public List<decimal> Data { get; set; }

    /// <summary>
    /// The fill color/background color of the dataset elements
    /// </summary>
    public RgbaColor BackgroundColor { get; set; }

    /// <summary>
    /// The border color of the dataset elements
    /// </summary>
    public RgbaColor BorderColor { get; set; }

    /// <summary>
    /// The width of the border in pixels
    /// </summary>
    public int BorderWidth { get; set; }

    public ChartJSBarChartDataset()
    {
        Data = new List<decimal>();
    }

    /// <summary>
    /// Returns a JavaScript object notation string representation of the dataset
    /// </summary>
    public override string ToString()
    {
        var dataString = string.Join(", ", Data);
        return $@"{{
                label: '{Label}',
                data: [{dataString}],
                backgroundColor: '{BackgroundColor}',
                borderColor: '{BorderColor}',
                borderWidth: {BorderWidth}
            }}";
    }
}
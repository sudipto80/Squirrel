namespace Squirrel.DataVisualization;

public static class UniversalChartGenerator
{
    /// <summary>
    /// Generates a bar chart representation using the specified columns from the given table.
    /// </summary>
    /// <param name="tab">The table containing the data to be visualized as a bar chart.</param>
    /// <param name="columns">The collection of column names whose data will be used to generate the bar chart.</param>
    /// <param name="provider">The chart provider to use for generating the bar chart. Default is ChartJS.</param>
    /// <param name="colorScheme">The color scheme to apply to the bar chart. Default is Rainbow.</param>
    /// <returns>A string representing the generated bar chart in a format compatible with the chosen provider.</returns>
    public static string ToBarChart(this Table tab,
        IEnumerable<string> columns,
        ChartProvider provider = ChartProvider.ChartJS,
        ColorScheme colorScheme = ColorScheme.Rainbow)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a pie chart representation of the specified column from the given table.
    /// </summary>
    /// <param name="tab">The table containing the data to be visualized as a pie chart.</param>
    /// <param name="columnName">The name of the column whose data will be used to generate the pie chart.</param>
    /// <param name="provider">The chart provider to use for generating the visual representation. Default is ChartJS.</param>
    /// <param name="colorScheme">The color scheme to apply to the pie chart. Default is Rainbow.</param>
    /// <returns>A string representing the generated pie chart in the format suitable for the chosen provider.</returns>
    public static string ToPieChart(this Table tab, string columnName, ChartProvider provider = ChartProvider.ChartJS,
        ColorScheme colorScheme = ColorScheme.Rainbow)
    {
        throw new NotImplementedException();
    }
}
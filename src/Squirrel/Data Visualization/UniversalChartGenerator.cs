using Squirrel.ChartJSTemplates;
using Squirrel.Data_Visualization;
using Squirrel.Data_Visualization.ChartJS;

namespace Squirrel.DataVisualization;

public enum PieChartType
{
    Pie,
    Doughnut    
}
public static class UniversalChartGenerator
{
    /// <summary>
    /// Generates a bar chart representation using the specified columns from the given table.
    /// </summary>
    /// <param name="tab">The table containing the data to be visualized as a bar chart.</param>
    /// <param name="groupByColumn">The name of the column to group the data by for the bar chart.</param>
    /// <param name="numericColumns">The collection of column names whose data will be used to generate the bar chart.</param>
    /// <param name="chartTitle">The title to be displayed on the bar chart.</param>
    /// <param name="provider">The chart provider to use for generating the bar chart. Default is ChartJS.</param>
    /// <param name="colorScheme">The color scheme to apply to the bar chart. Default is Rainbow.</param>
    /// <returns>A string representing the generated bar chart in a format compatible with the chosen provider.</returns>
    public static string ToBarChart(this Table tab,
        string groupByColumn,
        IEnumerable<string> numericColumns,
        string chartTitle,
        ChartProvider provider = ChartProvider.ChartJs,
        ColorScheme colorScheme = ColorScheme.Rainbow)
    {
        string chartCode = string.Empty;
        switch (provider)
        {
            case ChartProvider.ChartJs:
                chartCode = tab.ToBarChartByChartJs(chartTitle, groupByColumn, numericColumns, colorScheme);
                break;
            case ChartProvider.GoogleCharts:
                //   string groupByColumn,
                //          string legendText,
                //          string title,
                //          BarChartType type = BarChartType.Bar
                chartCode = tab.ToBarChartByGoogleDataVisualization(groupByColumn, "Sales", chartTitle);
                break;
            default:
                throw new NotImplementedException($"Unknown provider: {provider}");
        }
        return chartCode;
    }

    /// <summary>
    /// Generates a pie chart representation of the specified column from the given table.
    /// </summary>
    /// <param name="tab">The table containing the data to be visualized as a pie chart.</param>
    /// <param name="label">The label for the pie chart.</param>
    /// <param name="columnName">The name of the column whose data will be used to generate the pie chart.</param>
    /// <param name="portion">The portion of the data to be displayed in the pie chart. Default is Percentage.</param>
    /// <param name="provider">The chart provider to use for generating the visual representation. Default is ChartJS.</param>
    /// <param name="colorScheme">The color scheme to apply to the pie chart. Default is Rainbow.</param>
    /// <param name="chartType">The type of the chart either a pie or a doughnut</param>
    /// <returns>A string representing the generated pie chart in the format suitable for the chosen provider.</returns>
    public static string ToPieChart(this Table tab, string label, string columnName, Portion portion = Portion.Percentage, ChartProvider provider = ChartProvider.ChartJs,
        ColorScheme colorScheme = ColorScheme.Rainbow,  PieChartType chartType = PieChartType.Pie)
    {
        string chartCode = string.Empty;
        switch (provider)
        {
            case ChartProvider.ChartJs:
                chartCode = chartType switch
                {
                    PieChartType.Pie => tab
                       
                        .ToPieChartByChartJs(label, columnName, portion, colorScheme),
                    PieChartType.Doughnut => tab.ToDoughnutChartByChartJs(label, columnName, portion, colorScheme),
                    _ => chartCode
                };

                break;
            case ChartProvider.GoogleCharts:
                chartCode = chartType switch
                {
                    PieChartType.Pie => tab.ToPieByGoogleDataVisualization(columnName, label),
                    PieChartType.Doughnut => tab.ToPieByGoogleDataVisualization(columnName, label,
                        GoogleDataVisualization.PieChartType.Doughnut),
                    _ => chartCode
                };

                break;
            case ChartProvider.D3Js:
                break;
            case ChartProvider.Plotly:
                break;
            case ChartProvider.Highcharts:
                break;
            case ChartProvider.ApexCharts:
                break;
            case ChartProvider.ECharts:
                break;
            case ChartProvider.C3Js:
                break;
            case ChartProvider.FusionCharts:
                break;
            case ChartProvider.CanvasJs:
                break;
            case ChartProvider.ChartistJs:
                break;
            case ChartProvider.Nvd3:
                break;
            case ChartProvider.ZingChart:
                break;
            case ChartProvider.AnyChart:
                break;
            case ChartProvider.AmCharts:
                break;
            case ChartProvider.Recharts:
                break;
            case ChartProvider.VictoryCharts:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }
        return chartCode;
    }


    public static string ToLineChart(this Table tab, string label, string xColumnName, string yColumnName,
        Portion portion = Portion.Percentage, ChartProvider provider = ChartProvider.ChartJs)
    {
        //TODO: 
        throw new NotImplementedException();
    }

    public static string ToAreaChart(this Table tab, string[] columns, string label,
        Portion portion = Portion.RawValue, ChartProvider provider = ChartProvider.ChartJs)
    {
        //TODO: Implement Area Chart 
        throw new NotImplementedException();
    }
}


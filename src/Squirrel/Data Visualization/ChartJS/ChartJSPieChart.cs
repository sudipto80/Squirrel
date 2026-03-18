using Squirrel.DataVisualization;

namespace Squirrel.Data_Visualization.ChartJS;

public static class ChartJSPieChart
{
    private static string htmlTemplate = @"
        <!DOCTYPE html>
<html>
<head>
    <title>Pie Chart</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js""></script>
    <style>
        body {
            margin: 20px;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }
        .chart-container {
            width: 400px;
            height: 400px;
            max-width: 90%;
            max-height: 90vh;
        }
    </style>
</head>
<body>
    <div class=""chart-container"">
        <canvas id=""pieChart""></canvas>
    </div>
    <script>
        const ctx = document.getElementById('pieChart').getContext('2d');
        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: [_LABELS_],
                datasets: [{
                    label: '_Label_',
                    data: [_datasets_],
                    backgroundColor: [_rgbColors_],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true
            }
        });
    </script>
</body>
</html>";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="label"></param>
    /// <param name="colors"></param>
    /// <param name="strategy"></param>
    /// <returns></returns>

    public static string ToPieChartByChartJs(this Dictionary<string, int> data, string label, IEnumerable<RgbaColor> colors, NormalizationStrategy strategy = NormalizationStrategy.NameCase)
    {
        var dataValues = string.Join(", ", data.Values);
        var labels = string.Join(", ", data.Keys.Select(k => $"'{k.NormalizeAsPerStrategy(strategy)}'"));
        var rgbColors = string.Join(", ", colors.Select(c => $"'{c}'"));    
        return htmlTemplate.Replace("_LABELS_", labels)
            .Replace("_datasets_", dataValues)
            .Replace("_rgbColors_", rgbColors)
            .Replace("_Label_", label);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="label"></param>
    /// <param name="scheme"></param>
    /// <returns></returns>
    public static string ToPieChartByChartJs(this Dictionary<string, int> data, string label, ColorScheme scheme)
    {
        var colors = ColorPicker.GetColorsForScheme(scheme, data.Count);
        return ToPieChartByChartJs(data, label, colors);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tab"></param>
    /// <param name="label"></param>
    /// <param name="column"></param>
    /// <param name="portion"></param>
    /// <param name="scheme"></param>
    /// <returns></returns>
    public static string ToPieChartByChartJs(this Table tab, 
                   string label, string column, Portion portion = Portion.Percentage,
                   ColorScheme scheme = ColorScheme.CoolTones)
    {
        var data = tab.SplitOn(column);
        
        var dataDict = portion switch
        {
            Portion.Percentage => data.ToDictionary(t => t.Key, t => Convert.ToInt32(t.Value[column].Sum() * 100 / tab[column].Sum())),
            Portion.RawValue => data.ToDictionary(t => t.Key, t => Convert.ToInt32(t.Value[column].Sum())),
            _ => throw new ArgumentOutOfRangeException(nameof(portion), portion, null)
        };
        
        return ToPieChartByChartJs(dataDict, label, scheme);
    }
}
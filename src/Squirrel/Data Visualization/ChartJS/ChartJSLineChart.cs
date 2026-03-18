using Squirrel.DataVisualization;

namespace Squirrel.Data_Visualization.ChartJS;

public static class ChartJSLineChart
{
    private static string htmlTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <title>Line Chart</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js""></script>
</head>
<body>
    <canvas id=""lineChart"" width=""400"" height=""200""></canvas>
    <script>
        const ctx = document.getElementById('lineChart').getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: [_LABELS_],
                datasets: [_datasets_]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    </script>
</body>
</html>";

    public static string ToLineChartByChartJs(this Table tab,
        IEnumerable<string> labelColumns,
        IEnumerable<string> dataColumns, ColorScheme scheme)
    {
        var colors = ColorPicker.GetColorsForScheme(scheme, dataColumns.Count());
        var labels = labelColumns.Select(c => $"'{c}'");
        //TODO
        return htmlTemplate.Replace("_LABELS_", string.Join(", ", labels));
    }
}

using Squirrel.Data_Visualization;
using Squirrel.DataVisualization;

namespace Squirrel.ChartJSTemplates;

public static class ChartJSBarChart
{
    static string htmlTemplate = @"<!DOCTYPE html>
        <html>
        <head>
            <title>_TITLE_</title>
            <script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js""></script>
        </head>
        <body>
        <div style=""width: 600px; height: 400px; margin: 20px auto;"">
            <canvas id=""barChart""></canvas>
        </div>
        <script>
            const ctx = document.getElementById('barChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: [_LABELS_], 
                    datasets: [_datasets_]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                                    title: {
                                        display: true,
                                        text: '_chart_title_',
                                        font: {
                                            size: 18
                                        }
                                    }
                                },
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
    
    static string htmlTemplateWithBackgroundImage = 
        @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Chart.js with Background Image</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
    <style>
        .chart-container {
            position: relative;
            width: 600px;
            height: 400px;
            margin: 50px auto;
            background-image: url('_background_image_url_?w=600&h=400&crop=1');
            background-size: cover;
            background-position: center;
            padding: 20px;
            border-radius: 8px;
        }
        canvas {
            background: rgba(255, 255, 255, 0.7);
        }
    </style>
</head>
<body>
    <div class=""chart-container"">
        <canvas id=""myChart""></canvas>
    </div>
    <script>
        const ctx = document.getElementById('myChart').getContext('2d');
        const myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [_LABELS_],
                datasets: [_datasets_]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true
                    },
                    title: {
                        display: true,
                        text: '_chart_title_',
                        font: {
                            size: 18
                        }
                    }
                },
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

    public static string ToBarChartByChartJs
    (this Table tab, string chartTitle, string label, string labelColumn,
        IEnumerable<string> columns,
        ColorScheme scheme)
    {
        var count = columns.Count();
        var colors = ColorPicker.GetColorsForScheme(scheme, count);
        return tab.ToBarChartByChartJs(chartTitle, label, labelColumn, columns, colors);
    }

    public static string WithTitle(this string html, string title) =>
        html.Replace("_TITLE_", title);

    public static string SetWidth(this string html, int width) => html.Replace("width: 600px", $"width: {width}px");
    public static string SetHeight(this string html, int width) => html.Replace("height: 400px", $"height: {width}px");

    public static string ToBarChartWithBackgroundImageByChartJs
        (this Table tab, string chartTitle, string label, string labelColumn,
        IEnumerable<string> columns,
        ColorScheme scheme, string backgroundImageUrl)
    {
        var count = columns.Count();
        var colors = ColorPicker.GetColorsForScheme(scheme, count);
        var html = _toBarChartByChartJs(tab,
                                             chartTitle, 
                                             labelColumn, 
                                             columns, 
                                             colors, 
                                             BackgroundStyle.Image);
        return html.Replace("_background_image_url_", backgroundImageUrl);
    }
    public static string ToBarChartByChartJs(
        this Table tab,
        string chartTitle,
        string label,
        string labelColumn,
       
        IEnumerable<string> columns,
        IEnumerable<RgbaColor> colors)
    {
        return _toBarChartByChartJs(tab, chartTitle, labelColumn, columns,  colors);
    }

    private static string _toBarChartByChartJs(Table tab, string chartTitle, string labelColumn, IEnumerable<string> columns,
         IEnumerable<RgbaColor> colors, 
         BackgroundStyle style = BackgroundStyle.None
         )
    {
        Dictionary<string, List<int>> columnWiseData
            = new Dictionary<string, List<int>>();




        foreach (var column in columns)
        {
            if (!columnWiseData.ContainsKey(column))
            {
                columnWiseData.Add(column, tab.ValuesOf(column)
                    .Select(z => Convert.ToInt32(z)).ToList());
            }
        }

        Dictionary<string, ChartJSBarChartDataset> datasets
            = new Dictionary<string, ChartJSBarChartDataset>();

        int index = 0;
        foreach (var (columnName, columnData) in columnWiseData)
        {
            var bgColor = colors.ElementAt(index++);
            ChartJSBarChartDataset ds = new ChartJSBarChartDataset
            {
                Label = columnName,
                Data = columnData.Select(z => Convert.ToDecimal(z)).ToList(),


                BackgroundColor = bgColor,
                BorderColor = bgColor,
                BorderWidth = 1
            };
            datasets.Add(columnName, ds);
        }

        var labels = tab.ValuesOf(labelColumn)
            .Select(z => "'" + z.ToString() + "'")
            .Aggregate((a, b) => a + "," + b);

        var thisHtmlTemplate = string.Empty;
        if (style == BackgroundStyle.Image)
        {
            thisHtmlTemplate = htmlTemplateWithBackgroundImage;
        }
        else
        {
            thisHtmlTemplate = htmlTemplate;
        }
        thisHtmlTemplate = thisHtmlTemplate.Replace("_LABELS_", labels);
        var datasetStrings = datasets.Values
            .Select(ds => ds.ToString())
            .Aggregate((a, b) => a + "," + b);
        thisHtmlTemplate = thisHtmlTemplate.Replace("_chart_title_", chartTitle);
        thisHtmlTemplate = thisHtmlTemplate.Replace("_datasets_", datasetStrings);


        return thisHtmlTemplate;
    }
}
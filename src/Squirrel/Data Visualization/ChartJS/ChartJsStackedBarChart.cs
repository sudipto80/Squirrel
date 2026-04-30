using System.Text;
using Squirrel.DataVisualization;

namespace Squirrel.Data_Visualization.ChartJS;

public static class ChartJsStackedBarChart
{
   
    /// <summary>
    /// Creates a stacked bar chart using Chart.js.
    /// </summary>
    /// <param name="tab">The table containing the data</param>
    /// <param name="groupByColumn">The column used as labels (x-axis categories)</param>
    /// <param name="numericColumns">The numeric columns to plot as datasets</param>
    /// <param name="title">Title of the chart</param>
    /// <param name="yAxisTickPrefix">Optional prefix for y-axis tick labels (e.g. "$", "£")</param>
    /// <returns>A full HTML document string with the Chart.js stacked bar chart</returns>
    public static string ToStackedBarChartByChartJs(this Table tab, string groupByColumn,
        IEnumerable<string> numericColumns,
        string title,
        int widthPx,
        int heightPx,
        ColorScheme colorScheme = ColorScheme.CoolTones,
      
        string yAxisTickPrefix = "")
    {
        var colors = ColorPicker.GetColorsForScheme(colorScheme, numericColumns.Count());

        List<string> numericColList = numericColumns.ToList();

        // Build labels array: ['Jan', 'Feb', ...]
        var labels = new StringBuilder();
        for (int i = 0; i < tab.RowCount; i++)
        {
            labels.Append($"'{tab[groupByColumn, i]}'");
            if (i < tab.RowCount - 1)
                labels.Append(", ");
        }

        // Build datasets array — one per numeric column
        var datasets = new StringBuilder();
        for (int colIdx = 0; colIdx < numericColList.Count; colIdx++)
        {
            string col = numericColList[colIdx];
            var  color = colors[colIdx % colors.Length].ToHex();

            var dataValues = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                dataValues.Append(tab[col, i]);
                if (i < tab.RowCount - 1)
                    dataValues.Append(", ");
            }

            datasets.Append($@"
          {{
            label: '{col}',
            data: [{dataValues}],
            backgroundColor: '{color}'
          }}");

            if (colIdx < numericColList.Count - 1)
                datasets.Append(",");
        }

        // Build y-axis tick callback
        string tickCallback = string.IsNullOrEmpty(yAxisTickPrefix)
            ? "v => v.toLocaleString()"
            : $"v => '{yAxisTickPrefix}' + v.toLocaleString()";

        string htmlTemplate = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <title>{title}</title>
  <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
</head>
<body>
  <div style=""width: {widthPx}px; height: {heightPx}px;"">
    <canvas id=""chart""></canvas>
  </div>
  <script>
    const ctx = document.getElementById('chart').getContext('2d');
    new Chart(ctx, {{
      type: 'bar',
      data: {{
        labels: [{labels}],
        datasets: [{datasets}
        ]
      }},
      options: {{
        responsive: true,
        maintainAspectRatio: false,
        plugins: {{
          title: {{
            display: true,
            text: '{title}'
          }},
          legend: {{
            position: 'top'
          }}
        }},
        scales: {{
          x: {{ stacked: true }},
          y: {{
            stacked: true,
            ticks: {{
              callback: {tickCallback}
            }}
          }}
        }}
      }}
    }});
  </script>
</body>
</html>";

        return htmlTemplate;
    }
}
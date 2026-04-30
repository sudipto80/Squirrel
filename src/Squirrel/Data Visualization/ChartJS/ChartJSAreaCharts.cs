using System.Text;
using Squirrel.DataVisualization;

namespace Squirrel.Data_Visualization.ChartJS;

public static class ChartJSAreaCharts
{
    /// <summary>
    /// Generates an Area chart using Chart.js.
    /// </summary>
    /// <param name="tab">The table containing the data</param>
    /// <param name="groupByColumn">The column used as the x-axis labels</param>
    /// <param name="numericColumns">The numeric columns to plot as series</param>
    /// <param name="title">Title of the chart</param>
    /// <param name="yAxisTickPrefix">Optional prefix for y-axis tick labels (e.g. "$", "£")</param>
    /// <param name="widthPx">Width of the chart container in pixels (default 700)</param>
    /// <param name="heightPx">Height of the chart container in pixels (default 450)</param>
    /// <returns>A full HTML document string with the Chart.js Area chart</returns>
    public static string ToAreaChartByChartJs(this Table tab,
        string groupByColumn,
        IEnumerable<string> numericColumns,
        string title,
        string yAxisTickPrefix = "",
        ColorScheme colorScheme = ColorScheme.CoolTones,
        int widthPx = 700,
        int heightPx = 450)
    {
        string[] colors = ColorPicker.GetColorsForScheme(colorScheme, numericColumns.Count())
          .Select(t => t.ToHex()).ToArray();
       

        List<string> numericColList = numericColumns.ToList();

        // Build labels array: ['Jan', 'Feb', ...]
        var labels = new StringBuilder();
        for (int i = 0; i < tab.RowCount; i++)
        {
            labels.Append($"'{tab[groupByColumn, i]}'");
            if (i < tab.RowCount - 1)
                labels.Append(", ");
        }

        // Build datasets — fill:true makes it an area chart
        var datasets = new StringBuilder();
        for (int colIdx = 0; colIdx < numericColList.Count; colIdx++)
        {
            string col = numericColList[colIdx];
            string color = colors[colIdx % colors.Length];

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
            borderColor: '{color}',
            backgroundColor: '{color}40',
            fill: true,
            tension: 0.4
          }}");

            if (colIdx < numericColList.Count - 1)
                datasets.Append(",");
        }

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
  <h2>{title}</h2>
  <div style=""width: {widthPx}px; height: {heightPx}px;"">
    <canvas id=""chart""></canvas>
  </div>
  <script>
    const ctx = document.getElementById('chart').getContext('2d');
    new Chart(ctx, {{
      type: 'line',
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
          x: {{ stacked: false }},
          y: {{
            stacked: false,
            min: 0,
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


    /// <summary>
    /// Stacking mode for the Chart.js stacked area chart.
    /// </summary>
    public enum ChartJsAreaStackMode
    {
        /// <summary>Values stacked on top of each other.</summary>
        Absolute,

        /// <summary>Each series rescaled so the total always equals 100%.</summary>
        Percent
    }

    /// <summary>
    /// Generates a Stacked Area chart using Chart.js.
    /// </summary>
    /// <param name="tab">The table containing the data</param>
    /// <param name="groupByColumn">The column used as the x-axis labels</param>
    /// <param name="numericColumns">The numeric columns to plot as stacked series</param>
    /// <param name="title">Title of the chart</param>
    /// <param name="yAxisTickPrefix">Optional prefix for y-axis tick labels (e.g. "$", "£")</param>
    /// <param name="widthPx">Width of the chart container in pixels (default 700)</param>
    /// <param name="heightPx">Height of the chart container in pixels (default 450)</param>
    /// <param name="stackMode">Absolute (default) or Percent stacking</param>
    /// <returns>A full HTML document string with the Chart.js Stacked Area chart</returns>
    public static string ToStackedAreaChartByChartJs(this Table tab,
        string groupByColumn,
        IEnumerable<string> numericColumns,
        string title,
        string yAxisTickPrefix = "",
        ColorScheme colorScheme = ColorScheme.CoolTones,
        int widthPx = 700,
        int heightPx = 450,
        ChartJsAreaStackMode stackMode = ChartJsAreaStackMode.Absolute)
    {
      string[] colors = ColorPicker.GetColorsForScheme(colorScheme, numericColumns.Count())
        .Select(t => t.ToHex()).ToArray();

        List<string> numericColList = numericColumns.ToList();

        // Build labels array
        var labels = new StringBuilder();
        for (int i = 0; i < tab.RowCount; i++)
        {
            labels.Append($"'{tab[groupByColumn, i]}'");
            if (i < tab.RowCount - 1)
                labels.Append(", ");
        }

        // For percent mode we need row totals to rescale values
        List<double> rowTotals = new List<double>();
        if (stackMode == ChartJsAreaStackMode.Percent)
        {
            for (int i = 0; i < tab.RowCount; i++)
            {
                double total = numericColList.Sum(col => double.Parse(tab[col, i]));
                rowTotals.Add(total == 0 ? 1 : total); // guard div/0
            }
        }

        // Build datasets
        var datasets = new StringBuilder();
        for (int colIdx = 0; colIdx < numericColList.Count; colIdx++)
        {
            string col = numericColList[colIdx];
            string color = colors[colIdx % colors.Length];

            var dataValues = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                if (stackMode == ChartJsAreaStackMode.Percent)
                {
                    double val = double.Parse(tab[col, i]);
                    double pct = Math.Round((val / rowTotals[i]) * 100, 2);
                    dataValues.Append(pct);
                }
                else
                {
                    dataValues.Append(tab[col, i]);
                }

                if (i < tab.RowCount - 1)
                    dataValues.Append(", ");
            }

            datasets.Append($@"
          {{
            label: '{col}',
            data: [{dataValues}],
            borderColor: '{color}',
            backgroundColor: '{color}80',
            fill: true,
            tension: 0.4
          }}");

            if (colIdx < numericColList.Count - 1)
                datasets.Append(",");
        }

        // Tick callback — percent mode always shows % symbol
        string tickCallback = stackMode == ChartJsAreaStackMode.Percent
            ? "v => v + '%'"
            : string.IsNullOrEmpty(yAxisTickPrefix)
                ? "v => v.toLocaleString()"
                : $"v => '{yAxisTickPrefix}' + v.toLocaleString()";

        string yMax = stackMode == ChartJsAreaStackMode.Percent ? "\n            max: 100," : "";

        string htmlTemplate = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  
  <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
</head>
<body>

  <div style=""width: {widthPx}px; height: {heightPx}px;"">
    <canvas id=""chart""></canvas>
  </div>
  <script>
    const ctx = document.getElementById('chart').getContext('2d');
    new Chart(ctx, {{
      type: 'line',
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
            min: 0,{yMax}
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

    public static string ToLineChartByChartJs(this Table tab,
      string groupByColumn,
      IEnumerable<string> numericColumns,
      string title,
      string xAxisText = "",
      string yAxisText = "Visitors",
      int yAxisMinValue = 0)
    {
      List<string> numericColList = numericColumns.ToList();

      // Preset colors for up to 6 series
      List<(string border, string background)> colors = new List<(string, string)>
      {
        ("#4285F4", "rgba(66,133,244,0.1)"),
        ("#EA4335", "rgba(234,67,53,0.1)"),
        ("#34A853", "rgba(52,168,83,0.1)"),
        ("#FBBC05", "rgba(251,188,5,0.1)"),
        ("#FF6D00", "rgba(255,109,0,0.1)"),
        ("#46BDC6", "rgba(70,189,198,0.1)"),
      };

      // Build labels array: ['Jan', 'Feb', 'Mar']
      StringBuilder labelBuilder = new StringBuilder();
      for (int i = 0; i < tab.RowCount; i++)
      {
        labelBuilder.Append($"'{tab[groupByColumn, i]}'");
        if (i < tab.RowCount - 1)
          labelBuilder.Append(", ");
      }

      // Build datasets array — one per numeric column
      StringBuilder datasetBuilder = new StringBuilder();
      for (int c = 0; c < numericColList.Count; c++)
      {
        string col = numericColList[c];
        var (border, background) = colors[c % colors.Count];

        // Build data values: [12000, 15000, 13500, ...]
        StringBuilder valBuilder = new StringBuilder();
        for (int i = 0; i < tab.RowCount; i++)
        {
          valBuilder.Append(tab[col, i]);
          if (i < tab.RowCount - 1)
            valBuilder.Append(", ");
        }

        datasetBuilder.Append($@"{{
            label: '{col}',
            data: [{valBuilder}],
            borderColor: '{border}',
            backgroundColor: '{background}',
            borderWidth: 3,
            pointRadius: 6,
            pointHoverRadius: 8,
            tension: 0.4,
            fill: true,
          }}");

        if (c < numericColList.Count - 1)
          datasetBuilder.Append(",\n          ");
      }

      string htmlTemplate = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <title>{title}</title>
  <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
</head>
<body>
  <h2>{title}</h2>
  <canvas id=""chart"" width=""900"" height=""500""></canvas>
 
  <script>
    function drawChart() {{
      const ctx = document.getElementById('chart').getContext('2d');
 
      new Chart(ctx, {{
        type: 'line',
        data: {{
          labels: [{labelBuilder}],
          datasets: [
            {datasetBuilder}
          ],
        }},
        options: {{
          plugins: {{
            title: {{ display: true, text: '{title}' }},
            legend: {{ position: 'top' }},
          }},
          scales: {{
            x: {{ title: {{ display: true, text: '{xAxisText}' }} }},
            y: {{
              title: {{ display: true, text: '{yAxisText}' }},
              min: {yAxisMinValue},
              ticks: {{ callback: v => v.toLocaleString() }},
            }},
          }},
        }},
      }});
    }}
 
    drawChart();
  </script>
</body>
</html>";

      return htmlTemplate;
    }
}
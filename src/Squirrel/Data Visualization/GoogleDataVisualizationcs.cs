using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Squirrel
{
    /// <summary>
    /// Home for Google Data Visualization methods
    /// </summary>
    public static class GoogleDataVisualization
    {
        /// <summary>
        /// Type of Pie Chart supported by Google Data Visualization.
        /// </summary>
        public enum PieChartType
        {
            /// <summary>
            /// 
            /// </summary>
            Pie,

            /// <summary>
            /// 
            /// </summary>
            Pie3D,

            /// <summary>
            /// 
            /// </summary>
            Doughnut
        };

        /// <summary>
        /// Type of Bar Chart supported by Google Data Visualization
        /// Horizontal bar is called bar chart and vertical bar is called Column chart.
        /// </summary>
        public enum BarChartType
        {
            /// <summary>
            /// The horizontal bars
            /// </summary>
            Bar,

            /// <summary>
            /// The vertical bars or columns
            /// </summary>
            Column
        };

        /// <summary>
        /// Genertes the Histogram using google data visualization.
        /// </summary>
        /// <param name="hist">The histogram from which the histogram has to be drawn</param>
        /// <param name="column1">The column name which will represent the keys</param>
        /// <param name="column2">The column name which will represent the values</param>
        /// <param name="title">Title of the histogram</param>
        /// <returns>a string representation</returns>
        public static string ToHistogramByGoogleDataVisualization(this Dictionary<string, int> hist, string column1,
            string column2, string title)
        {
            string data = hist.Select(z
                    => "['" + z.Key + "'," + z.Value.ToString() + "]")
                .Aggregate((f, s) => f + "," + s);
            string template = @"<html>
              <head>
                <script type=""text/javascript"" src=""https://www.google.com/jsapi""></script>
                <script type=""text/javascript"">
                  google.load(""visualization"", ""1"", {packages:[""corechart""]});
                  google.setOnLoadCallback(drawChart);
                  function drawChart() {
                    var data = google.visualization.arrayToDataTable([
                      [!COLUMN_HEADERS!],
                      !DATA!]);

                    var options = {
                      title: '!TITLE!',
                      legend: { position: 'none' },
                    };

                    var chart = new google.visualization.Histogram(document.getElementById('chart_div'));
                    chart.draw(data, options);
                  }
                </script>
              </head>
              <body>
                <div id=""chart_div"" style=""width: 900px; height: 500px;""></div>
              </body>
            </html>".Replace("!DATA!", data)
                .Replace("!TITLE!", title)
                .Replace("!COLUMN_HEADERS!", "'" + column1 + "','" + column2 + "'");

            return template;
        }

        /// <summary>
        /// Generates a pie/3dPie/Donut chart from the given table.
        /// </summary>
        /// <param name="tab">The table</param>
        /// <param name="column">The column</param>
        /// <param name="title">The title of the chart</param>
        /// <param name="type">The type of the chart. Choose from any three options , Pie, Pie3D and Donut</param>
        /// <returns>A full html document with the chart.</returns>
        public static string ToPieByGoogleDataVisualization(this Table tab, string column, string title,
            PieChartType type = PieChartType.Pie)
        {
            if (tab == null)
            {
                throw new ArgumentNullException(nameof(tab));
            }

            if (String.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (String.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (!tab.ColumnHeaders.Contains(column))
            {
                throw new ArgumentOutOfRangeException("Column name " + nameof(column) + " doesn't exist in the table");
            }

            try
            {
                string data = "[[" + "'" + column + "','Count']," +
                              tab.SplitCount(column)
                                  .Select(z => "['" + z.Key.Replace("'", "\'") + "' , " + z.Value + "]")
                                  .Aggregate((first, second) => first + "," + Environment.NewLine + second)
                              + "]";

                string template = @" <html>
                                  <head>
                                    <script type=""text/javascript"" src=""https://www.gstatic.com/charts/loader.js""></script>
                                    <script type=""text/javascript"">
                                      google.charts.load('current',  { packages:['corechart']});
                                      google.charts.setOnLoadCallback(drawChart);
                                      function drawChart() {

                                        var data = google.visualization.arrayToDataTable(!DATA!);

                                        var options = {
                                          title: '!TITLE!'
                                          //,
                                          //is3D:true,
                                          //pieHole:0.4
                                        };

                                        var chart = new google.visualization.PieChart(document.getElementById('!CHART_TYPE!'));

                                        chart.draw(data, options);
                                      }
                                    </script>
                                  </head>
                                  <body>
                                    <div id=""!CHART_TYPE!"" style=""width: 900px; height: 500px;""></div>
                                  </body>
                                </html>".Replace("!TITLE!", title).Replace("!DATA!", data);

                if (type == PieChartType.Pie)
                    template = template.Replace("!CHART_TYPE!", "piechart");
                if (type == PieChartType.Pie3D)
                    template = template.Replace("!CHART_TYPE!", "piechart_3d").Replace("//,", ",")
                        .Replace("//is3D:true", "is3D:true");
                if (type == PieChartType.Doughnut)
                    template = template.Replace("!CHART_TYPE!", "donutchart").Replace("//,", ",")
                        .Replace("//pieHole:0.4", "pieHole:0.4");

                return template;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generates a bar/column chart from the given table for the given column
        /// </summary>
        /// <param name="tab">The table</param>
        /// <param name="column">The column</param>
        /// <param name="legendText">Legend Text</param>
        /// <param name="title">Title of the chart</param>
        /// <param name="type"></param>
        /// <returns>Gennerated HTML for the chart</returns>
        public static string ToBarChartByGoogleDataVisualization_legacy(this Table tab,
            string column,
            string legendText,
            string title,
            BarChartType type = BarChartType.Bar)
        {
            List<string> numericColumns = new List<string>();
            string numericRegex = @"^-?[0-9]\d*(\.\d+)?$"; //matches decimals with negative 
            foreach (var colName in tab.ColumnHeaders)
            {
                if (tab.ValuesOf(colName).All(m => Regex.IsMatch(m, numericRegex)))
                    numericColumns.Add(colName);
            }

            string columnHeaders = "['" + column + "'," + numericColumns.Select(x => "'" + x + "'")
                .Aggregate((m, n) => m + "," + n) + "]";
            StringBuilder dataBuilder = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                //['Kids' Menu',34],
                dataBuilder.Append("['" + tab[column, i] + "',");
                for (int j = 0; j < numericColumns.Count - 1; j++)
                {
                    dataBuilder.Append(tab[numericColumns[j], i] + ",");
                }

                dataBuilder.AppendLine(tab[numericColumns[numericColumns.Count - 1], i] + "],");
            }

            string data = dataBuilder.ToString();
            data = data.Substring(0, data.Length - 3);

            string html = @"<html>
                      <head>
                      <script type=""text/javascript"" src=""https://www.gstatic.com/charts/loader.js""></script>
                        <script type = ""text/javascript"" >
                        google.charts.load('current',  { packages:['corechart', 'bar']});
                        google.charts.setOnLoadCallback(drawChart);
                          function drawChart() {
                            var data = google.visualization.arrayToDataTable([
                              !COLUMN_HEADERS!,
                              !DATA!
                            ]);

                            var options = {
                              title: '!TITLE!',
                              vAxis: {title: '!LEGEND_TEXT!',  titleTextStyle: {color: 'red'}}
                            };

                            var chart = new google.visualization.!CHART_TYPE!(document.getElementById('chart_div'));

                            chart.draw(data, options);
                          }
                        </script>
                      </head>
                      <body>
                        <div id=""chart_div"" style=""width: 900px; height: 500px;""></div>
                      </body>
                    </html>"

                .Replace("!LEGEND_TEXT!", legendText)
                .Replace("!TITLE!", title)
                .Replace("!COLUMN_HEADERS!", columnHeaders)
                .Replace("!DATA!", data);

            if (type == BarChartType.Bar)
                html = html.Replace("!CHART_TYPE!", "BarChart");
            if (type == BarChartType.Column)
                html = html.Replace("!CHART_TYPE!", "ColumnChart");

            return html;
        }

        public static string ToBarChartByGoogleDataVisualization(this Table tab,
            string groupByColumn,
            string legendText,
            string title,
            BarChartType type = BarChartType.Bar)
        {
            List<string> numericColumns = new List<string>();
            string numericRegex = @"^-?[0-9]\d*(\.\d+)?$"; // matches decimals with negative
            foreach (var colName in tab.ColumnHeaders)
            {
                if (colName.Equals(groupByColumn)) continue;
                if (tab.ValuesOf(colName).All(m => Regex.IsMatch(m, numericRegex)))
                    numericColumns.Add(colName);
            }

            // Create column headers for data array
            string columnHeaders = "['" + groupByColumn + "'," +
                                   string.Join(",", numericColumns.Select(x => "'" + x + "'")) +
                                   "]";

            StringBuilder dataBuilder = new StringBuilder();

            for (int i = 0; i < tab.RowCount; i++)
            {
                dataBuilder.Append("['" + tab[groupByColumn, i] + "',");
                for (int j = 0; j < numericColumns.Count - 1; j++)
                {
                    dataBuilder.Append(tab[numericColumns[j], i] + ",");
                }

                // Append the last numeric value without trailing comma
                dataBuilder.Append(tab[numericColumns[numericColumns.Count - 1], i]);
                dataBuilder.AppendLine("],");
            }

            string data = dataBuilder.ToString();

            // Remove the trailing comma after the last row
            if (data.EndsWith(",\n") || data.EndsWith(",\r\n"))
            {
                data = data.Substring(0, data.Length - 2);
            }
            else if (data.EndsWith(","))
            {
                data = data.Substring(0, data.Length - 1);
            }

            string html = @"<html>
        <head>
            <script type=""text/javascript"" src=""https://www.gstatic.com/charts/loader.js""></script>
            <script type=""text/javascript"">
                google.charts.load('current', { packages:['corechart', 'bar'] });
                google.charts.setOnLoadCallback(drawChart);
                function drawChart() {
                    var data = google.visualization.arrayToDataTable([
                        !COLUMN_HEADERS!,
                        !DATA!
                    ]);
                    var options = {
                        title: '!TITLE!',
                        vAxis: { title: '!LEGEND_TEXT!', titleTextStyle: { color: 'red' } }
                    };
                    var chart = new google.visualization.!CHART_TYPE!(document.getElementById('chart_div'));
                    chart.draw(data, options);
                }
            </script>
        </head>
        <body>
            <div id=""chart_div"" style=""width: 900px; height: 500px;""></div>
        </body>
    </html>"
                .Replace("!LEGEND_TEXT!", legendText)
                .Replace("!TITLE!", title)
                .Replace("!COLUMN_HEADERS!", columnHeaders)
                .Replace("!DATA!", data);

            if (type == BarChartType.Bar)
                html = html.Replace("!CHART_TYPE!", "BarChart");
            else if (type == BarChartType.Column)
                html = html.Replace("!CHART_TYPE!", "ColumnChart");

            if (type == BarChartType.Bar)
                html = html.Replace("vAxis", "hAxis");
            return html;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToPieFromHistogramByGoogleDataVisualization(this Dictionary<string, int> histogram,
            string title,
            PieChartType type = PieChartType.Pie)
        {
            return "TO:DO";
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToBarChartFromHistogramByGoogleDataVisualization(this Dictionary<string, int> histogram,
            string title,
            BarChartType type = BarChartType.Bar)
        {
            return "TO:DO";
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="hAxisColumn"></param>
        /// <param name="vAxisColumn"></param>
        /// <returns></returns>
        public static string ToBubbleChartByGoogleVisualization(this Table tab,
            string hAxisColumn, string vAxisColumn)
        {
            return "TO:DO";
        }

        public static string ToStackedBarChartByGoogleVisualization(this Table tab, string groupByColumn,
            IEnumerable<string> numericColumns,
            string title, string xAxisText, string yAxisText, string xAxisFormat)
        {
            string htmlTemplate =
                """
                <html lang="en">
                <head>
                  <meta charset="UTF-8" />
                  <title>Stacked Bar Chart – Google Charts</title>
                  <script src="https://www.gstatic.com/charts/loader.js"></script>
                </head>
                <body>

                  <div id="chart" style="width: 900px; height: 500px;"></div>

                  <script>
                    google.charts.load('current', { packages: ['corechart'] });
                    google.charts.setOnLoadCallback(drawChart);

                    function drawChart() {
                      const data = google.visualization.arrayToDataTable([
                        !HEADERS!,
                        !TABLE_ROWS!
                      ]);

                      const options = {
                        title: '!TITLE!',
                        isStacked: true,
                        hAxis: { title: '!xAxisText!', format: '!xAxisFormat!' },
                        vAxis: { title: '!yAxisText!' },
                        colors: ['#4285F4', '#EA4335', '#FBBC05', '#34A853'],
                        legend: { position: 'top' },
                      };

                      const chart = new google.visualization.BarChart(document.getElementById('chart'));
                      chart.draw(data, options);
                    }
                  </script>
                </body>
                </html>
                """;

            List<string> numericColList = numericColumns.ToList();

            // Build headers row: ['GroupByColumn', 'Col1', 'Col2', ...]
            List<string> allCols = new List<string>();
            allCols.Add(groupByColumn);
            allCols.AddRange(numericColList);

            string headers = "[" + string.Join(", ", allCols.Select(c => $"'{c}'")) + "]";

            // Build data rows: ['GroupValue', num1, num2, ...]
            StringBuilder rowBuilder = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                rowBuilder.Append($"['{tab[groupByColumn, i]}'");
                foreach (var col in numericColList)
                {
                    rowBuilder.Append($", {tab[col, i]}");
                }

                rowBuilder.Append("]");
                if (i < tab.RowCount - 1)
                    rowBuilder.Append(",\n");
            }

            htmlTemplate = htmlTemplate
                .Replace("!TITLE!", title)
                .Replace("!HEADERS!", headers)
                .Replace("!TABLE_ROWS!", rowBuilder.ToString())
                .Replace("!xAxisText!", xAxisText)
                .Replace("!xAxisFormat!", xAxisFormat)
                .Replace("!yAxisText!", yAxisText);

            return htmlTemplate;
        }

        /// <summary>
        /// Generates an Area chart using Google Data Visualization API.
        /// </summary>
        /// <param name="tab">The table containing the data</param>
        /// <param name="groupByColumn">The column used as the x-axis (e.g. Year, Month)</param>
        /// <param name="numericColumns">The numeric columns to plot as series</param>
        /// <param name="title">Title of the chart</param>
        /// <param name="xAxisText">Label for the horizontal axis</param>
        /// <param name="yAxisMinValue">Minimum value for the vertical axis (default 0)</param>
        /// <returns>A full HTML document string with the Area chart</returns>
        public static string ToAreaChartByGoogleDataVisualization(this Table tab,
            string groupByColumn,
            IEnumerable<string> numericColumns,
            string title,
            string xAxisText = "",
            int yAxisMinValue = 0)
        {
            List<string> numericColList = numericColumns.ToList();

            // Build headers: ['Year', 'Sales', 'Expenses']
            List<string> allCols = new List<string> { groupByColumn };
            allCols.AddRange(numericColList);
            string headers = "[" + string.Join(", ", allCols.Select(c => $"'{c}'")) + "]";

            // Build data rows: ['2013', 1000, 400]
            StringBuilder rowBuilder = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                rowBuilder.Append($"['{tab[groupByColumn, i]}'");
                foreach (var col in numericColList)
                    rowBuilder.Append($", {tab[col, i]}");
                rowBuilder.Append("]");
                if (i < tab.RowCount - 1)
                    rowBuilder.Append(",\n          ");
            }

            string htmlTemplate = $@"<html>
  <head>
    <script type=""text/javascript"" src=""https://www.gstatic.com/charts/loader.js""></script>
    <script type=""text/javascript"">
      google.charts.load('current', {{'packages':['corechart']}});
      google.charts.setOnLoadCallback(drawChart);

      function drawChart() {{
        var data = google.visualization.arrayToDataTable([
          {headers},
          {rowBuilder}
        ]);

        var options = {{
          title: '{title}',
          hAxis: {{ title: '{xAxisText}', titleTextStyle: {{ color: '#333' }} }},
          vAxis: {{ minValue: {yAxisMinValue} }}
        }};

        var chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
        chart.draw(data, options);
      }}
    </script>
  </head>
  <body>
    <div id=""chart_div"" style=""width: 100%; height: 500px;""></div>
  </body>
</html>";

            return htmlTemplate;
        }


        /// <summary>
        /// Stacking mode for the stacked area chart.
        /// </summary>
        public enum AreaChartStackMode
        {
            /// <summary>Standard stacking : values summed at each x-value.</summary>
            Absolute,

            /// <summary>Each value shown as a percentage of 100%.</summary>
            Percent,

            /// <summary>Each value shown as a fraction of 1.</summary>
            Relative
        }

        /// <summary>
        /// Generates a Stacked Area chart using Google Data Visualization API.
        /// </summary>
        /// <param name="tab">The table containing the data</param>
        /// <param name="groupByColumn">The column used as the x-axis (e.g. Year, Month)</param>
        /// <param name="numericColumns">The numeric columns to plot as stacked series</param>
        /// <param name="title">Title of the chart</param>
        /// <param name="xAxisText">Label for the horizontal axis</param>
        /// <param name="yAxisMinValue">Minimum value for the vertical axis (default 0)</param>
        /// <param name="stackMode">
        ///   Stacking mode: Absolute (default, equivalent to isStacked:true),
        ///   Percent (isStacked:'percent'), or Relative (isStacked:'relative')
        /// </param>
        /// <returns>A full HTML document string with the Stacked Area chart</returns>
        public static string ToStackedAreaChartByGoogleDataVisualization(this Table tab,
            string groupByColumn,
            IEnumerable<string> numericColumns,
            string title,
            string xAxisText = "",
            int yAxisMinValue = 0,
            AreaChartStackMode stackMode = AreaChartStackMode.Absolute)
        {
            List<string> numericColList = numericColumns.ToList();

            // Resolve isStacked value
            string isStacked = stackMode switch
            {
                AreaChartStackMode.Percent => "'percent'",
                AreaChartStackMode.Relative => "'relative'",
                _ => "true" // Absolute
            };

            // Build headers: ['Year', 'Sales', 'Expenses']
            List<string> allCols = new List<string> { groupByColumn };
            allCols.AddRange(numericColList);
            string headers = "[" + string.Join(", ", allCols.Select(c => $"'{c}'")) + "]";

            // Build data rows: ['2013', 1000, 400]
            StringBuilder rowBuilder = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                rowBuilder.Append($"['{tab[groupByColumn, i]}'");
                foreach (var col in numericColList)
                    rowBuilder.Append($", {tab[col, i]}");
                rowBuilder.Append("]");
                if (i < tab.RowCount - 1)
                    rowBuilder.Append(",\n          ");
            }

            string htmlTemplate = $@"<html>
  <head>
    <script type=""text/javascript"" src=""https://www.gstatic.com/charts/loader.js""></script>
    <script type=""text/javascript"">
      google.charts.load('current', {{'packages':['corechart']}});
      google.charts.setOnLoadCallback(drawChart);

      function drawChart() {{
        var data = google.visualization.arrayToDataTable([
          {headers},
          {rowBuilder}
        ]);

        var options = {{
          title: '{title}',
          isStacked: {isStacked},
          hAxis: {{ title: '{xAxisText}', titleTextStyle: {{ color: '#333' }} }},
          vAxis: {{ minValue: {yAxisMinValue} }}
        }};

        var chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
        chart.draw(data, options);
      }}
    </script>
  </head>
  <body>
    <div id=""chart_div"" style=""width: 100%; height: 500px;""></div>
  </body>
</html>";

            return htmlTemplate;
        }

        public static string ToLineChartByGoogleDataVisualization(this Table tab,
            string groupByColumn,
            IEnumerable<string> numericColumns,
            string title,
            string xAxisText = "",
            string yAxisText = "Visitors",
            int yAxisMinValue = 0)
        {
            List<string> numericColList = numericColumns.ToList();

            // Build headers: ['Month', 'Organic', 'Direct', 'Referral']
            List<string> allCols = new List<string> { groupByColumn };
            allCols.AddRange(numericColList);
            string headers = "[" + string.Join(", ", allCols.Select(c => $"'{c}'")) + "]";

            // Build data rows: ['Jan', 12000, 8000, 3000]
            StringBuilder rowBuilder = new StringBuilder();
            for (int i = 0; i < tab.RowCount; i++)
            {
                rowBuilder.Append($"['{tab[groupByColumn, i]}'");
                foreach (var col in numericColList)
                    rowBuilder.Append($", {tab[col, i]}");
                rowBuilder.Append("]");
                if (i < tab.RowCount - 1)
                    rowBuilder.Append(",\n          ");
            }

                    string htmlTemplate = $@"<!DOCTYPE html>
        <html lang=""en"">
        <head>
          <meta charset=""UTF-8"" />
          <title>{title}</title>
          <script src=""https://www.gstatic.com/charts/loader.js""></script>
        </head>
        <body>
          <h2>{title}</h2>
          <div id=""chart"" style=""width: 900px; height: 500px;""></div>
         
          <script>
            google.charts.load('current', {{ packages: ['corechart'] }});
            google.charts.setOnLoadCallback(drawChart);
         
            function drawChart() {{
              const data = google.visualization.arrayToDataTable([
                {headers},
                {rowBuilder}
              ]);
         
              const options = {{
                title: '{title}',
                curveType: 'function',
                legend: {{ position: 'top' }},
                hAxis: {{ title: '{xAxisText}' }},
                vAxis: {{ title: '{yAxisText}', minValue: {yAxisMinValue}, format: '#,###' }},
                colors: ['#4285F4', '#EA4335', '#34A853', '#FBBC05', '#FF6D00', '#46BDC6'],
                pointSize: 6,
                lineWidth: 3,
              }};
         
              const chart = new google.visualization.LineChart(document.getElementById('chart'));
              chart.draw(data, options);
            }}
          </script>
        </body>
        </html>";

                    return htmlTemplate;
        }
    }
}

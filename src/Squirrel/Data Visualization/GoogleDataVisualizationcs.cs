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
    public static class GoogleDataVisualizationcs
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
            Donut
        };
        /// <summary>
        /// Type of Bar Chart supported by Google Data Visualization
        /// Horizontal bar is called bar chart and vertical bar is called Column chart.
        /// </summary>
        public enum BarChartType
        {
            /// <summary>
            /// 
            /// </summary>
            Bar,
            /// <summary>
            /// 
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
                    .Replace("!TITLE!",title)
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
        public static string ToPieByGoogleDataVisualization(this Table tab, string column, string title,PieChartType type = PieChartType.Pie)
        {
            if(tab == null)
            {
                throw new ArgumentNullException(nameof(tab));
            }
            if(String.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException(nameof(column));
            }
            if(String.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }
            if(!tab.ColumnHeaders.Contains(column))
            {
                throw new ArgumentOutOfRangeException("Column name " + nameof(column) + " doesn't exist in the table");
            }
            try {
                string data = "[[" + "'" + column + "','Count']," +
                                tab.Histogram(column)
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
                    template = template.Replace("!CHART_TYPE!", "piechart_3d").Replace("//,", ",").Replace("//is3D:true", "is3D:true");
                if (type == PieChartType.Donut)
                    template = template.Replace("!CHART_TYPE!", "donutchart").Replace("//,", ",").Replace("//pieHole:0.4", "pieHole:0.4");

                return template;
            }
            catch(InvalidOperationException ex)
            {
                throw ex;
            }

            catch(Exception ex)
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
            string numericRegex = @"^-?[0-9]\d*(\.\d+)?$";//matches decimals with negative 
            foreach (var colName in tab.ColumnHeaders)
            { 
                if (tab.ValuesOf(colName).All(m => Regex.IsMatch(m, numericRegex)))
                    numericColumns.Add(colName);
            }
            string columnHeaders = "['" + column +"',"  + numericColumns.Select(x => "'" + x + "'")
                                                                        .Aggregate((m, n) => m + "," + n) + "]";
            StringBuilder dataBuilder = new StringBuilder ();
            for (int i = 0; i < tab.RowCount; i++)
            {//['Kids' Menu',34],
                dataBuilder.Append("['" + tab[column, i] + "',");
                for (int j = 0; j < numericColumns.Count - 1; j++)
                {
                    dataBuilder.Append(tab[numericColumns[j], i] + ",");
                }
                dataBuilder.AppendLine(tab[numericColumns[numericColumns.Count-1],i]+ "],");
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
                            
                            .Replace("!LEGEND_TEXT!",legendText)
                            .Replace("!TITLE!",title)
                            .Replace("!COLUMN_HEADERS!",columnHeaders)
                            .Replace("!DATA!",data);

            if (type == BarChartType.Bar)
                html = html.Replace("!CHART_TYPE!", "BarChart");
            if (type == BarChartType.Column)
                html = html.Replace("!CHART_TYPE!", "ColumnChart");

            return html;
        }

        public static string ToBarChartByGoogleDataVisualization(this Table tab,
            string column,
            string legendText,
            string title,
            BarChartType type = BarChartType.Bar)
        {
            List<string> numericColumns = new List<string>();
            string numericRegex = @"^-?[0-9]\d*(\.\d+)?$"; // matches decimals with negative
            foreach (var colName in tab.ColumnHeaders)
            {
                if (colName.Equals(column)) continue;
                if (tab.ValuesOf(colName).All(m => Regex.IsMatch(m, numericRegex)))
                    numericColumns.Add(colName);
            }

            // Create column headers for data array
            string columnHeaders = "['" + column + "'," + string.Join(",", numericColumns.Select(x => "'" + x + "'")) +
                                   "]";

            StringBuilder dataBuilder = new StringBuilder();

            for (int i = 0; i < tab.RowCount; i++)
            {
                dataBuilder.Append("['" + tab[column, i] + "',");
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

      
    }
}

using Squirrel;
using Squirrel.ChartJSTemplates;
using Squirrel.Data_Visualization.ChartJS;
using Squirrel.DataVisualization;

namespace StackedBar;

class Program
{
    static void Main(string[] args)
    {
   
        var sales = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/salesData.csv");


        var bars = sales.ToBarChartWithBackgroundImageByChartJs
        (chartTitle: "Monthly Sales",
            groupByColumn: "month",
            numericColumns: sales.ColumnHeaders.Except(["month"]),
            ColorScheme.Pastel,
            backgroundImageUrl:
            "https://img.freepik.com/free-photo/sales-with-paper-bags-concept-orange-background-copy-space_23-2148305926.jpg");
        
        
        File.WriteAllText("bars.html", bars);
    }
}
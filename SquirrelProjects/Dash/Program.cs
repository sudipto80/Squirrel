using Squirrel;
using Squirrel.Data_Visualization;
using Squirrel.Data_Visualization.ChartJS;
using Squirrel.DataVisualization;

namespace Dash;

class Program
{
    static void Main(string[] args)
    {
        var salesTable = DataAcquisition.LoadCsv(
                @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/salesData7.csv")
            .Top(5);
        var box1 = new DashboardBox
        {
            Title = "Monthly Sales – Bar Chart",
            Html  = salesTable.ToStackedBarChartByChartJs("month",
                salesTable.ColumnHeaders.Except(["month"]), 
                "Monthly Sales", 200, 130)
        };

        var box3 = new DashboardBox
        {
            Title = "Summary Table",
            Html  = salesTable.ToTailwindTable(TailwindTableClass.Compact)   // whatever your table→HTML method is
        };

        string html = new Dashboard(3, 1,title: "Sales Report")
            .AddCell(0, 0, box1)

            .AddCell(1, 0, box3)
            .Render();

        File.WriteAllText("dashboard.html", html);
    }
}
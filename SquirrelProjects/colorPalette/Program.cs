using Squirrel;
using Squirrel.Data_Visualization.ChartJS;
using Squirrel.DataVisualization;

namespace colorPalette;

class Program
{
    static void Main(string[] args)
    {
        var salesTable = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/SalesData7.csv");

        var schemes = Enum.GetValues<ColorScheme>();

// Build one box per scheme
        var boxes = schemes.Select(scheme => new DashboardBox
        {
            Title = $"Monthly Sales: Stacked Bar Chart - {scheme} Theme",
            Html  = salesTable.ToStackedBarChartByChartJs(
                "month",
                salesTable.ColumnHeaders.Except(["month"]),
                "Monthly Sales",
                250, 170,
                scheme)
        }).ToArray();

// Vertical dashboard — one row per scheme
        string html = new Dashboard(DashboardLayout.Vertical, boxes.Length, title: "Sales Report (Color Palettes)")
            .AddCells(boxes)
            .Render();

        File.WriteAllText("dashboard.html", html);

    }
}
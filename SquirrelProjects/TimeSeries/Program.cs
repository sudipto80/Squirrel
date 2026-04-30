using Squirrel;
using Squirrel.ChartJSTemplates;
using Squirrel.Cleansing;
using Squirrel.DataVisualization;

namespace TimeSeries;

class Program
{
    static void Main(string[] args)
    {
        var goldData =
            DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Downloads/timeseries_data.csv");


        var goldDataTS = goldData.GranularizeDateColumn("timestamp", MonthGranularizationFormat.ShortMonthNames);
        
        //Average high price by month
        var avgHighPriceByMonth = goldDataTS.SplitOn("Month")
            .Select(t => new
            {
                Month = t.Key,
                AvgHighPrice = t.Value["high"].Average()
            })
           // .OrderByDescending(t => t.AvgHighPrice)
            .ToTableFromAnonList();

        //Exclusing Weekends (Saturdays and Sundays)
        var businessDays = goldData.Resample(columnName:"timestamp", ["volume"],
            frequency:  DateTimeFrequency.BusinessDay, 
            method: AggregationMethod.Average);
        
        //Calendar Month ends 
        var monthEnds =
            goldData.Resample(columnName:"timestamp", ["volume"],
                frequency:  DateTimeFrequency.MonthEnd, 
                method: AggregationMethod.Average);
        // Business Month ends
        var businessMonthEnds =
            goldData.Resample(columnName:"timestamp", ["volume"],
                frequency:  DateTimeFrequency.BusinessMonthEnd, 
                method: AggregationMethod.Average);

        Console.WriteLine($"Original :{goldData.RowCount} \n" +
                          $"Business Days : {businessDays.RowCount}\n" +
                          $"Calendar Month ends: {monthEnds.RowCount}\n" + 
                          $"Business Month ends: {businessMonthEnds.RowCount}");

        var allDaysChart = goldData
            
            .ToBarChartByChartJs(chartTitle: "Share Prices", groupByColumn: "timestamp", columns: ["volume"],
                ColorScheme.CoolTones);
        var businessDaysCharts = businessDays
            .ToBarChartByChartJs(chartTitle: "Share Prices", groupByColumn: "timestamp", columns: ["volume"],
                ColorScheme.CoolTones);
        //
        // var monthEndsCharts = monthEnds.Top(50)
        //     .ToBarChartByChartJs(chartTitle: "Share Prices", groupByColumn: "timestamp", columns: ["high_int"],
        //         ColorScheme.CoolTones);

        DashboardBox box1 = new DashboardBox() { Title = "All Days", Html = allDaysChart}; 
        DashboardBox box2 = new DashboardBox() { Title = "Business Days", Html = businessDaysCharts};
        // DashboardBox box3 = new DashboardBox() { Title = "Business Month Ends", Html = monthEndsCharts};
        //
        Dashboard dash = new Dashboard(1, 3);
        dash.AddCell(0,0, box1);
         dash.AddCell(0,1, box2);
        // dash.AddCell(0,2, box3);
        
        File.WriteAllText("Dash.htm", dash.Render());
        Console.WriteLine("Generated dashboard at Dash.htm");
    }
}
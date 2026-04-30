using System.Diagnostics;
using System.Net;
using System.Reflection.Emit;
using Squirrel;
using Squirrel.ChartJSTemplates;
using Squirrel.Data_Visualization;
using Squirrel.Data_Visualization.ChartJS;
using Squirrel.DataVisualization;

namespace SquirrelProjects;

class Program
{
 
    static void Main(string[] args)
    {
        
        var allStocks = DataAcquisition.LoadCsv(@"//Users/sudiptamukherjee/Downloads/AAPL_historical.csv");
        allStocks.AddColumn(columnName: "Diff", formula: "[Open] - [Close]", decimalDigits: 4);
        //Preparing to write the result in a HTML file.
        StreamWriter sw = new StreamWriter("tempApple.html");
        Func<Dictionary<string, string>, bool> greatValues = x => Math.Abs(Convert.ToDecimal(x["Diff"])) == 1M;
        Func<Dictionary<string, string>, bool> worries = x => Math.Abs(Convert.ToDecimal(x["Diff"])) == 0.99M;
        Func<Dictionary<string, string>, bool> warnings = x => Math.Abs(Convert.ToDecimal(x["Diff"])) <= 0.98M;
        string htmlTable = allStocks
            //Sort by the difference in descending order
            .SortBy("Diff", how: SortDirection.Descending)
            //Taking top 20 entries
            .Skip(1)//Skip the header
            .Top(20)
            //Pick only these columns
            .Pick("Diff", "High", "Close")
            .ToBootstrapHtmlTableWithColoredRows
            (
                infoPredicate: greatValues,
                warningPredicate: warnings,
                dangerPredicate: worries
            );

        sw.WriteLine(htmlTable);
        sw.Close();
        System.Diagnostics.Process.Start("tempApple.htm");
      
       var sampleTable  = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelProjects/SquirrelProjects/SampleTable.csv");
       
       sampleTable.PrettyDump(header:"Sample Table", headerColor:ConsoleColor.DarkRed, rowColor: ConsoleColor.Black);

    
       var tabHtml = sampleTable.ToSemanticRowsTable(row 
               => (row.Any(c => c is "A" or "A+") ? "green" : null) ?? string.Empty,
           row => (row.Any(c => c is "B+") ? "amber" : null) ?? string.Empty,
           row => (row.Any(c => c is "B") ? "red" : null) ?? string.Empty);
       File.WriteAllText("tabCode.html", tabHtml);
       //  // var md = @"| Student ID | Name | Age | Grade | Subject | Score |
       //  //            |------------|------|-----|-------|---------|-------|
       //  //            | S001 | Alice Johnson | 20 | A | Mathematics | 95 |
       //  //            | S002 | Bob Smith | 22 | B | Physics | 78 |
       //  //            | S003 | Clara Lee | 21 | A | Chemistry | 91 |
       //  //            | S004 | David Brown | 23 | C | Biology | 65 |
       //  //            | S005 | Emma Davis | 20 | B | English | 82 |
       //  //            | S006 | Frank Wilson | 22 | A | History | 88 |
       //  //            | S007 | Grace Taylor | 21 | C | Geography | 60 |
       //  //            | S008 | Henry Martin | 24 | B | Computer Science | 75 |";
       //  // var studentsMD = DataAcquisition.LoadFromMarkdown(md);
       //  //     studentsMD.PrettyDump();
       //  // var dirs = Directory.GetFiles("/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/")
       //  //     .Select(t => new FileInfo(t))
       //  //     .ToTableFromAnonList()
       //  //     .SortBy("Length", how: SortDirection.Descending)
       //  //     .Pick("Name", "Length");
       //  //
       //  // var tab2 = Enumerable.Range(0, 10)
       //  //     .Select(t => new { Name = $"File {t}", Length = t })
       //  //     .ToTableFromAnonList();
       //  //
       //  // tab2.PrettyDump(header:"Tab2");
       //  //
       //  // dirs.PrettyDump();
       //  //
       //  var births  = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/births.csv");
       //
       //
       //  // var boys = births.Filter(t => t["sex"] == "boy").RowCount;
       //  // var girls = births.Filter(t => t["sex"] == "girl").RowCount;
       //  //
       //  // var html = births
       //  //     .Filter(t => t["state"] == "CA")
       //  //     .ToPieChart("Gender Difference", "sex", provider: ChartProvider.GoogleCharts, portion: Portion.RawValue);
       //  //
       //  //
       //  // File.WriteAllText("genderPie.html", html);
       //  // var psi23 = new ProcessStartInfo
       //  // {
       //  //     FileName = "open",
       //  //     Arguments = "genderPie.html",
       //  //     RedirectStandardOutput = false,
       //  //     UseShellExecute = true,
       //  //     CreateNoWindow = true
       //  // };
       //  // Process.Start(psi23);
       //  //
       // // var pie = births.SplitOn("sex")
       // //     
       // //     .Select(t => new
       // //     {
       // //         Gender = t.Key,
       // //         TotalBirths = Convert.ToInt32(t.Value.Filter("state","CA")["births"].Sum())
       // //     })
       // //     .ToLookup(t => t.Gender)
       // //     .ToDictionary(t => t.Key, t => t.Sum(x => x.TotalBirths))
       // //     .ToDoughnutChartByChartJs("Birth", ColorScheme.CoolTones);
       // //     
       // // File.WriteAllText("birth_pie.html", pie);
       // // var psi23 = new ProcessStartInfo
       // // {
       // //     FileName = "open",
       // //     Arguments = "birth_pie.html",
       // //     RedirectStandardOutput = false,
       // //     UseShellExecute = true,
       // //     CreateNoWindow = true
       // // };
       // //    
       // // // Process.Start(psi23);
       //  var html = births
       //      
       //      .SplitOn("state")
       //     .Select(t => new
       //     {
       //         State = t.Key, Births = Convert.ToInt32(t.Value["births"].Sum() / 10000)
       //     })
       //     
       //     .ToTableFromAnonList()
       //     .SortBy("Births", how: SortDirection.Descending)
       //     .Top(10)
       //     .ToBarChartByChartJs("Label","State", "State",["Births"], colors: new RgbaColor[] { RgbaColor.FromName("pink") });
       //
       // File.WriteAllText("births.html", html);
       // //   var psi = new ProcessStartInfo
       // //   {
       // //        FileName = "open",
       // //        Arguments = "births.html",
       // //        RedirectStandardOutput = false,
       // //        UseShellExecute = true,
       // //        CreateNoWindow = true
       // //   };
       // //   
       // //    Process.Start(psi);
       // //      
       // //  //Statewise boy vs girl bar chart 
       // // var stateWise = births.SplitOn("state")
       // //      .Select(t => new
       // //      {
       // //          State = t.Key,
       // //          Boys = Convert.ToInt32(t.Value.Filter("sex", "boy")["births"].Sum() / 1000),
       // //          Girls = Convert.ToInt32(t.Value.Filter("sex", "girl")["births"].Sum() / 1000),
       // //      })
       // //      .ToTableFromAnonList()
       // //      .Top(10);
       // //
       // //  stateWise.AddColumn("Gap", "[Boys] - [Girls]", 0); 
       // //  stateWise.SortBy("Gap", how: SortDirection.Descending)
       // //      .Top(5)
       // //      .PrettyDump();
       // //  var stateWiseComp = stateWise
       // //      .SortBy("Gap", how: SortDirection.Descending)
       // //      .ToBarChartWithBackgroundImageByChartJs(
       // //          chartTitle: "Statewide Birth Comparison",
       // //          label: "Statewide gap comparison",
       // //          labelColumn: "State",
       // //          columns: ["Boys", "Girls"],
       // //          scheme: ColorScheme.CoolTones,
       // //          backgroundImageUrl: "https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8YmlydGhzJTIwY2hpbGRyZW58ZW58MHx8MHx8fDA%3D&auto=format&fit=crop&w=800&q=60");
       // //
       // // //   //[ RgbaColor.FromName("blue"),RgbaColor.FromName("pink")]);
       // // //       
       // // //           
       // // //  
       // // File.WriteAllText("statewise_births.html", stateWiseComp);
       // //     var psi2 = new ProcessStartInfo
       // //     {
       // //         FileName = "open",
       // //         Arguments = "statewise_births.html",
       // //         RedirectStandardOutput = false,
       // //         UseShellExecute = true,
       // //         CreateNoWindow = true
       // //     };
       // //  
       // //     Process.Start(psi2);
       // //  
       // //  // Girl population in CA over the years!
       // //
       // var caGirls = births
       //     .Filter("state", "CA")
       //     .Filter("sex", "girl")
       //     .Pick("year", "births")
       //     .Top(10);
       //     caGirls.PrettyDump();
       //     
       // var caGirlsHtml= caGirls.ToBarChartByChartJs("CA Girls over the years", "Girls", "year",
       //     ["births"],
       //     [RgbaColor.FromName("purple")]);
       //
       // File.WriteAllText("cagirls.html", caGirlsHtml);
       // var psi3 = new ProcessStartInfo
       // {
       //     FileName = "open",
       //     Arguments = "cagirls.html",
       //     RedirectStandardOutput = false,
       //     UseShellExecute = true,
       //     CreateNoWindow = true
       // };
       //
       // Process.Start(psi3);
       
       


    }   
}
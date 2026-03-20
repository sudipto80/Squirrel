using System.Diagnostics;
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
        var md = @"| Student ID | Name | Age | Grade | Subject | Score |
                   |------------|------|-----|-------|---------|-------|
                   | S001 | Alice Johnson | 20 | A | Mathematics | 95 |
                   | S002 | Bob Smith | 22 | B | Physics | 78 |
                   | S003 | Clara Lee | 21 | A | Chemistry | 91 |
                   | S004 | David Brown | 23 | C | Biology | 65 |
                   | S005 | Emma Davis | 20 | B | English | 82 |
                   | S006 | Frank Wilson | 22 | A | History | 88 |
                   | S007 | Grace Taylor | 21 | C | Geography | 60 |
                   | S008 | Henry Martin | 24 | B | Computer Science | 75 |";
        var studentsMD = DataAcquisition.LoadFromMarkdown(md);
            studentsMD.PrettyDump();
        var dirs = Directory.GetFiles("/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/")
            .Select(t => new FileInfo(t))
            .ToTableFromAnonList()
            .SortBy("Length", how: SortDirection.Descending)
            .Pick("Name", "Length");

        var tab2 = Enumerable.Range(0, 10)
            .Select(t => new { Name = $"File {t}", Length = t })
            .ToTableFromAnonList();
        
        tab2.PrettyDump(header:"Tab2");
        
        dirs.PrettyDump();
        
        var births  = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/births.csv");
       //
       // var pie = births.SplitOn("sex")
       //     
       //     .Select(t => new
       //     {
       //         Gender = t.Key,
       //         TotalBirths = Convert.ToInt32(t.Value.Filter("state","CA")["births"].Sum())
       //     })
       //     .ToLookup(t => t.Gender)
       //     .ToDictionary(t => t.Key, t => t.Sum(x => x.TotalBirths))
       //     .ToDoughnutChartByChartJs("Birth", ColorScheme.CoolTones);
       //     
       // File.WriteAllText("birth_pie.html", pie);
       // var psi23 = new ProcessStartInfo
       // {
       //     FileName = "open",
       //     Arguments = "birth_pie.html",
       //     RedirectStandardOutput = false,
       //     UseShellExecute = true,
       //     CreateNoWindow = true
       // };
       //    
       // // Process.Start(psi23);
       // var html = births.SplitOn("state")
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
       //   var psi = new ProcessStartInfo
       //   {
       //        FileName = "open",
       //        Arguments = "births.html",
       //        RedirectStandardOutput = false,
       //        UseShellExecute = true,
       //        CreateNoWindow = true
       //   };
       //   
       //    Process.Start(psi);
       //      
       //  //Statewise boy vs girl bar chart 
       // var stateWise = births.SplitOn("state")
       //      .Select(t => new
       //      {
       //          State = t.Key,
       //          Boys = Convert.ToInt32(t.Value.Filter("sex", "boy")["births"].Sum() / 1000),
       //          Girls = Convert.ToInt32(t.Value.Filter("sex", "girl")["births"].Sum() / 1000),
       //      })
       //      .ToTableFromAnonList()
       //      .Top(10);
       //
       //  stateWise.AddColumn("Gap", "[Boys] - [Girls]", 0); 
       //  stateWise.SortBy("Gap", how: SortDirection.Descending)
       //      .Top(5)
       //      .PrettyDump();
       //  var stateWiseComp = stateWise
       //      .SortBy("Gap", how: SortDirection.Descending)
       //      .ToBarChartWithBackgroundImageByChartJs(
       //          chartTitle: "Statewide Birth Comparison",
       //          label: "Statewide gap comparison",
       //          labelColumn: "State",
       //          columns: ["Boys", "Girls"],
       //          scheme: ColorScheme.CoolTones,
       //          backgroundImageUrl: "https://images.unsplash.com/photo-1506744038136-46273834b3fb?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8YmlydGhzJTIwY2hpbGRyZW58ZW58MHx8MHx8fDA%3D&auto=format&fit=crop&w=800&q=60");
       //
       // //   //[ RgbaColor.FromName("blue"),RgbaColor.FromName("pink")]);
       // //       
       // //           
       // //  
       // File.WriteAllText("statewise_births.html", stateWiseComp);
       //     var psi2 = new ProcessStartInfo
       //     {
       //         FileName = "open",
       //         Arguments = "statewise_births.html",
       //         RedirectStandardOutput = false,
       //         UseShellExecute = true,
       //         CreateNoWindow = true
       //     };
       //  
       //     Process.Start(psi2);
       //  
       //  // Girl population in CA over the years!
       //
       var caGirls = births
           .Filter("state", "CA")
           .Filter("sex", "girl")
           .Pick("year", "births")
           .Top(10);
           caGirls.PrettyDump();
           
       var caGirlsHtml= caGirls.ToBarChartByChartJs("CA Girls over the years", "Girls", "year",
           ["births"],
           [RgbaColor.FromName("purple")]);
       
       File.WriteAllText("cagirls.html", caGirlsHtml);
       var psi3 = new ProcessStartInfo
       {
           FileName = "open",
           Arguments = "cagirls.html",
           RedirectStandardOutput = false,
           UseShellExecute = true,
           CreateNoWindow = true
       };
       
       Process.Start(psi3);

    }   
}
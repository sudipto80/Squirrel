using System.Diagnostics;
using Squirrel;

namespace SquirrelProjects;

class Program
{
    static void Main(string[] args)
    {
        var prods = DataAcquisition.LoadCsv("/Users/sudiptamukherjee/Documents/GitHub/Squirrel/src/Squirrel/Data/products.csv");
        prods.PrettyDump();
        prods.Transpose("Month")
            .PrettyDump(rowColor: ConsoleColor.Blue);

        uint x = 30;
        //     Dictionary<string, PerfRecord> perfRecords = new Dictionary<string, PerfRecord>();
        //     
        //     var files = new[]
        //     {
        //         // Add the paths to your CSV files here
        //         "/Users/sudiptamukherjee/Downloads/house.csv",
        //         "/Users/sudiptamukherjee/Downloads/products-100000.csv",
        //         // Add more file paths as needed
        //     };  
        //     foreach (var file in files)
        //     {
        //         var record = new PerfRecord();
        //         var w = new Stopwatch();
        //         w.Start();
        //         var table = DataAcquisition.LoadCsv(file);
        //         w.Stop();
        //         record.RowCount = table.RowCount;
        //         record.ColumnCount = table.ColumnHeaders.Count;
        //         record.LoadTimeMs = w.ElapsedMilliseconds;
        //
        //         w.Restart();
        //         table.PrettyDump();
        //         w.Stop();
        //         record.DumpTimeMs = w.ElapsedMilliseconds;
        //
        //         w.Restart();
        //         var gist = table.Gist().ToTable();
        //         gist.PrettyDump();
        //         w.Stop();
        //         record.GistTimeMs = w.ElapsedMilliseconds;
        //
        //         perfRecords[file] = record;
        //     }
        //     foreach (var kvp in perfRecords)
        //     {
        //         Console.WriteLine($"File: {kvp.Key}");
        //         Console.WriteLine($"Rows: {kvp.Value.RowCount}, Columns: {kvp.Value.ColumnCount}");
        //         Console.WriteLine($"Load Time (ms): {kvp.Value.LoadTimeMs}");
        //         Console.WriteLine($"Dump Time (ms): {kvp.Value.DumpTimeMs}");
        //         Console.WriteLine($"Gist Time (ms): {kvp.Value.GistTimeMs}");
        //         Console.WriteLine();
        //     }
        //     
        // }
        //
        // public class PerfRecord
        // {
        //     public int RowCount; 
        //     public int ColumnCount;
        //     public long LoadTimeMs;
        //     public long DumpTimeMs;
        //     public long GistTimeMs;
        // }
        var tab = DataAcquisition.LoadCsv("/Users/sudiptamukherjee/Downloads/dataset.csv");
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        tab.PrettyDump(header: "All rows",
            rowColor: ConsoleColor.Black);
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        tab.BottomNPercent(20)
            .PrettyDump(header:"Bottom 20% rows",
                headerColor: ConsoleColor.Green, 
                rowColor: ConsoleColor.Blue);

    }
}
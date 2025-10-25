using System.Diagnostics;
using Amazon.S3.Model;
using Squirrel;

namespace AllDataSets;

class Program
{
    static void Main(string[] args)
    {
        var dept = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/dept.csv");
        var emp = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/emp.csv");

        // Squirrel automatically figures out the column name to join.
        // In this case "dept_id" 
        emp.MergeByColumns(dept)
            .PrettyDump(header: "Employee Details", rowColor: ConsoleColor.DarkBlue);
        
        
        var studentsA = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/students_with_gender.csv");
        var femaleStudents = studentsA.PseudoNaturalQuery("show me all F students who took Physics");
        

     
        Stopwatch w = Stopwatch.StartNew();
        var bigTranData = DataAcquisition.LoadExcel("/Users/sudiptamukherjee/Downloads/acquiring_transaction_2025-10-08T13-07-24.763624164 2.xlsx","ACQUIRING_TRANSACTIONS");
        w.Stop();

        var irrCols = bigTranData.IrrelevantColumns;

        var bigTable = bigTranData.Drop(irrCols);
            
        var bigHtmlTable    = bigTable.ToHtmlTable();
        
        StreamWriter sw1 = new StreamWriter("irrelevant.html");
        sw1.WriteLine(bigHtmlTable);
        sw1.Close();
        System.Diagnostics.Process.Start("irrelevant.html");
        
        Console.WriteLine(w.ElapsedMilliseconds);

        var merchants = bigTranData.SplitOn("merchantName");

        merchants.Select(t => new
            {
                Merchant = t.Key, Count = t.Value.RowCount,
                Range = t.Value["transactionAmount"].Select(Convert.ToDecimal).Max() -
                        t.Value["transactionAmount"].Select(Convert.ToDecimal).Min()
            })
            .ToTableFromAnonList()
            .SortBy(columnName: "Range", how: SortDirection.Descending)
            .Top(10)
            .PrettyDump(header: "Top 10 merchants by range", rowColor: ConsoleColor.Black);
        
        var html = bigTranData.Top(3)
            .ToHtmlTable();
        StreamWriter sw = new StreamWriter("top3.html");
        sw.WriteLine(html);
        sw.Close();
        System.Diagnostics.Process.Start("top3.html");
        var casinoData = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Downloads/ogwsi-snapshot-30-08-2025.csv");
        //
        // casinoData["merchant"].Distinct().ToList().ForEach(Console.WriteLine);
        //
        // //Merchant Freq
        //
        // var merchantCount = casinoData.SplitOn("merchant")
        //     .Select(t => new { Merchant = t.Key, Count = t.Value.RowCount })
        //     .ToTableFromAnonList()
        //     .SortBy(columnName: "Count", how: SortDirection.Descending);
        //
        // merchantCount.PrettyDump(header:"Merch Count", rowColor: ConsoleColor.Black);
         var students = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/students_with_gender.csv");
         var teenAgefemaleStudents =
             students.Filter(student =>
             {
                 var gender = student["Gender"];
                 var age = Convert.ToInt32(student["Age"]);
                 return gender == "F" && age is >= 13 and <= 19;
             });

         teenAgefemaleStudents.PrettyDump(header:"Female Students");
         var fieldValuesMap = new Dictionary<string, List<string>>
         {
             { "Gender", ["F"] },
             { "Major", ["Computer Science", "Physics"] }
         };

         students.Filter(fieldValuesMap)
             .PrettyDump(header: "Female students in Computer Science and Physics",
                  headerColor: ConsoleColor.DarkMagenta,
                  rowColor: ConsoleColor.Magenta);


         students.Search("Math").PrettyDump(header:"Math global search");
         
         students
             .Filter("Gender", "F")
             .FilterByRegex("Major", "Science")
             .PrettyDump(header:"Female students with science major", headerColor: ConsoleColor.DarkMagenta, rowColor: ConsoleColor.Red);

         var skills  = 
             DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/explode.csv");
         
         skills.Explode("skills")
             .PrettyDump(header: "Skills exploded", rowColor: ConsoleColor.DarkBlue);
    
         // var enrollments = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/enrollment.csv");
        //
        // var stp = Stopwatch.StartNew();
        // var bothBatches = students.InnerJoin(anotherTable: enrollments
        //                                        )
        //                                      .SortBy("Student_ID")
        //                                      .Pick("Student_ID","Enrollment_ID","Name","Major","Course_Code");
        // stp.Stop();
        // Console.WriteLine(stp.ElapsedMilliseconds);
        // bothBatches.PrettyDump(header: "Both Batches", 
        //     rowColor: ConsoleColor.Black, align: Alignment.Left);


        //MIGUEL CASINO DATA
        // Stopwatch w = Stopwatch.StartNew();
        // w.Stop();
        // Console.WriteLine(w.ElapsedMilliseconds);
        //
        //
        //
        // casinoData.AddColumn(columnName: "Percentage", formula: "[amount]-1", decimalDigits: 2)
        //     .AddColumn(columnName: "Percentage2", formula: "[Percentage]-1", decimalDigits: 2);
        //
        // var calCols = casinoData.CalculatedColumns;
        // var apple = DataAcquisition.LoadCsv(
        //     @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/AAPL.csv");
        //
        // var windows = apple.SlidingWindow(7);
        // windows.ElementAt(0)
        //     .Pick("Date","Close")
        //     .PrettyDump(header: "First Window", 
        //                                 headerColor: ConsoleColor.DarkMagenta,
        //                                 rowColor: ConsoleColor.Blue);
        // Console.ForegroundColor = ConsoleColor.Black;
        // windows.ElementAt(1)
        //        .Pick("Date","Close")
        //        .PrettyDump(header: "Second Window", 
        //                                 headerColor: ConsoleColor.DarkMagenta,
        //                                 rowColor: ConsoleColor.Blue);

        //Printing the first two windows
        // windows.ElementAt(0).PrettyDump(header: "First Window");
        // foreach (var window in windows)
        // {
        //     var avg = window["Close"].Average();
        //     Console.WriteLine(avg);
        // }
        // Console.WriteLine("Hello, World!");
        var table = new Table();
        List<string> names = table["Name"].Distinct().ToList();;
    }
}
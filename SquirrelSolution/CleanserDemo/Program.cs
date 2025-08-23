using Squirrel;
using Squirrel.Cleansing;
using Squirrel.DataCleansing;

namespace CleanserDemo;

class Program
{
    static void Main(string[] args)
    {
        var badSubject =
            DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/BadSubject.csv");

        badSubject.PrettyDump(header: "Unknown Subject Antho",
            headerColor: ConsoleColor.Red,
            rowColor: ConsoleColor.Black);

        var antho = badSubject.MarkAsMissingIfNotAnyOf("course","N/A",
            ["CS", "Math", "Physics", "Biology", "Eng", "Psychology"]);
        
        antho.PrettyDump(header: "Unknown Subject Antho Marked as Missing",
            headerColor: ConsoleColor.Green,
            rowColor: ConsoleColor.Black);


        var missingAge =
            DataAcquisition.LoadCsv(
                @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/missingAge.csv");
        
        missingAge.PrettyDump(
            header:"Missing Age Values",
            headerColor:ConsoleColor.Red,
            rowColor:ConsoleColor.Black);
        
        var filledAge = missingAge.ReplaceMissingValuesByDefault(
            DataCleansers.MissingValueHandlingStrategy.Max, 
            "", "NULL", "N/A","NA"
        );

       
        filledAge.PrettyDump(
            header:"Filled Age Values",
            headerColor:ConsoleColor.Green, 
            rowColor:ConsoleColor.Black);

        var messy = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/messy.csv");
        
        
        var emps = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/messy_dataset.csv");
       
        // Dictionary<string,MaskingStrategy> strategies = new Dictionary<string, MaskingStrategy>();
        // strategies.Add("Name", MaskingStrategy.None);
        // strategies.Add("Salary", MaskingStrategy.Hidden);
        // strategies.Add("Age", MaskingStrategy.AgeGroup);
        //
        var html =
            emps.AutoNormalize().ToBootstrapHtmlTableWithColoredRows();
        StreamWriter sw = new StreamWriter("/Users/sudiptamukherjee/temp.html");
        sw.Write(html);
        sw.Close();
        System.Diagnostics.Process.Start("/Users/sudiptamukherjee/temp.html");
        //emps.MaskColumnsWithStrategies(strategies).PrettyDump(header:"Masked");
    }
}
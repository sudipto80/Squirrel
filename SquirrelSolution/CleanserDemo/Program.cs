using Squirrel;
using Squirrel.Cleansing;

namespace CleanserDemo;

class Program
{
    static void Main(string[] args)
    {
        List<string> vs = new List<string>() { "3", "56", "-4" };
        
        var dataFile = @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/data.csv";
        var tab = DataAcquisition.LoadCsv(dataFile);
        tab.PrettyDump(header:"Before Cleansing", 
                headerColor:ConsoleColor.Red, 
                rowColor:ConsoleColor.Black);
        
        tab.RemoveIf<int>("Age", x => x is < 0 or > 100);
        tab.PrettyDump(header:"After Cleansing", 
                headerColor:ConsoleColor.Green, 
                rowColor:ConsoleColor.Black);
        
        // Dictionary mapping cities from the dataset to their primary airport codes
        var cityToAirportCode = new Dictionary<string, string>
        {
            ["New York"] = "JFK",        // John F. Kennedy International (also LGA, EWR)
            ["Chicago"] = "ORD",         // O'Hare International (also MDW)
            ["Los Angeles"] = "LAX",     // Los Angeles International
            ["Miami"] = "MIA",           // Miami International
            ["Seattle"] = "SEA",         // Seattle-Tacoma International
            ["Boston"] = "BOS",          // Logan International
            ["Denver"] = "DEN",          // Denver International
            ["Austin"] = "AUS",          // Austin-Bergstrom International
            ["Portland"] = "PDX",        // Portland International
            ["Phoenix"] = "PHX"          // Phoenix Sky Harbor International
        };
        tab.Transform("City", 
            x => cityToAirportCode[x]);

        tab.PrettyDump(header:"City as Airport Codes", 
            rowColor:ConsoleColor.DarkBlue);

        var emps = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/outliers.csv");
        
        emps.PrettyDump(header:"All emps");
        var outRemoved = emps.RemoveOutliers("Salary");
        outRemoved.PrettyDump(header:"All emps Outliers removed");
    }
}
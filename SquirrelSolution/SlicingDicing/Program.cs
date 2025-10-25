using Squirrel;

namespace SlicingDicing;

class Program
{
    static void Main(string[] args)
    {
        var data = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/SlicingDicing/employees.csv");

        var sliders = data.SlidingWindow(4).ToList();
        var next10 = data.Middle(skip: 10, take: 10);
        var stratSample = data.StratifiedSample("Department", 5);
        var depts = data.SplitOn("Department");

        var threeSplit = data.SplitByColumns(
            new string[] { "EmployeeID",  "Q1_Sales"},
            new string[] { "EmployeeID",  "Q2_Sales"},
            new string[] { "EmployeeID",  "Q3_Sales"}
        );
        foreach (var dept in depts.Keys)
        {
            Console.WriteLine(dept + " " + depts[dept].RowCount);
        }
        Console.WriteLine("Hello, World!");
    }
}
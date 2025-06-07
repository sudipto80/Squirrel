// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Squirrel;
using Squirrel.Cleansing;
using TableAPI;

// Console.WriteLine("Hello, World!");
//
// var complex = DataAcquisition.LoadCsv("C:\\Users\\Admin\\Documents\\GitHub\\Squirrel\\SquirrelSolution\\ConsoleDemo\\complex.csv");
// 
// complex.PrettyDump(rowColor:ConsoleColor.DarkMagenta);

var messyEmployees = 
     DataAcquisition.LoadCsv(@"C:\users\admin\downloads\messy_employees.csv");
Func<string, bool> isEmpty = x => x.Trim().Length == 0;
var cleanEmps =
    messyEmployees.ReplaceXWithY("IsActive", "Yes", "1")
        .ReplaceXWithY("IsActive", "No", "0")
        .ReplaceXsWithY("Salary", "$0", ["not disclosed", "$"])
        .ReplaceXWithY("JoinDate", "NA", "2021-01-01")
        .NormalizeColumn("FullName", NormalizationStrategy.NameCase)
        .NormalizeColumn("Department", NormalizationStrategy.UpperCase)
        .RemoveIf("FullName", isEmpty)
        .RemoveIf("Age", isEmpty)
        .Filter("IsActive", "1")//Pick only active employees
        .RemoveGreaterThan("Age", 70)
        .Pick("FullName","Salary","Department","Age")
        .SortBy("Salary", how:SortDirection.Descending);
cleanEmps.PrettyDump();
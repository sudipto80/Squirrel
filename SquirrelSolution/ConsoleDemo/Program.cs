// See https://aka.ms/new-console-template for more information

using System.Drawing;
using IronOcr;
using Squirrel;
using Squirrel.Cleansing;

// Console.WriteLine("Hello, World!");
//
// var complex = DataAcquisition.LoadCsv("C:\\Users\\Admin\\Documents\\GitHub\\Squirrel\\SquirrelSolution\\ConsoleDemo\\complex.csv");
// 
// complex.PrettyDump(rowColor:ConsoleColor.DarkMagenta);

// var messyEmployees = 
//      DataAcquisition.LoadCsv(@"C:\users\admin\downloads\messy_employees.csv");
// Func<string, bool> isEmpty = x => x.Trim().Length == 0;
// var cleanEmps =
//     messyEmployees.ReplaceXWithY("IsActive", "Yes", "1")
//         .ReplaceXWithY("IsActive", "No", "0")
//         .ReplaceXsWithY("Salary", "$0", ["not disclosed", "$"])
//         .ReplaceXWithY("JoinDate", "NA", "2021-01-01")
//         .NormalizeColumn("FullName", NormalizationStrategy.NameCase)
//         .NormalizeColumn("Department", NormalizationStrategy.UpperCase)
//         .RemoveIf("FullName", isEmpty)
//         .RemoveIf("Age", isEmpty)
//         .Filter("IsActive", "1")//Pick only active employees
//         .RemoveGreaterThan("Age", 70)
//         .Pick("FullName","Salary","Department","Age")
//         .SortBy("Salary", how:SortDirection.Descending);
// cleanEmps.PrettyDump();
//

// var badCombs = DataAcquisition.LoadCsv("C:\\Users\\Admin\\Documents\\GitHub\\Squirrel\\SquirrelSolution\\ConsoleDemo\\badComb.csv");
//
// Dictionary<string, HashSet<string>> combinations
//     = new Dictionary<string, HashSet<string>>();
//
//
//
// //Electronics and Textile can't have Liters as the unit 
// Func<string, string, bool> badCom1 = (x,y) => x.Equals("Electronics") && y.Equals("Liters");
// Func<string, string, bool> badCom2 = (x,y) => x.Equals("Textiles") && y.Equals("Liters");
//
// Func<string,string,bool> categoryUnitViolations = 
//     (x,y) => (x.Equals("Electronics") || x.Equals("Textiles")) && y.Equals("Liters");
//
// Func<string,string,bool> countryCurrencyViolations = 
//     (x,y) => (x.Equals("USA") || x.Equals("US")) && !y.Equals("USD");
//
// List<Func<string,string,bool>> badCombinations = [badCom1, badCom2];
//
//
//     
//
//
// badCombs
//         //.RemoveCombination("Category", "Unit", badCom1)
//         //.RemoveCombination("Category","Unit", badCom2)
//         .RemoveCombination("Category","Unit", categoryUnitViolations)
//         .RemoveCombination("Country","Currency", countryCurrencyViolations)
//         .PrettyDump(rowColor:ConsoleColor.Red);
//
// Console.WriteLine(badCombs.RowCount);

Aspose.OCR.AsposeOcr recognitionEngine = new Aspose.OCR.AsposeOcr();
Aspose.OCR.OcrInput source = new Aspose.OCR.OcrInput(Aspose.OCR.InputType.SingleImage);
source.Add("C:\\Users\\Admin\\Downloads\\receipt2.jpg");
Aspose.OCR.OcrOutput results = recognitionEngine.Recognize(source);
Console.WriteLine(results[0].RecognitionText);
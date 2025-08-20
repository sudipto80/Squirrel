using Squirrel;
using Squirrel.Cleansing;

namespace CleanserDemo;

class Program
{
    static void Main(string[] args)
    {
       
        var emps = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/outliers.csv");
       
        // Dictionary<string,MaskingStrategy> strategies = new Dictionary<string, MaskingStrategy>();
        // strategies.Add("Name", MaskingStrategy.None);
        // strategies.Add("Salary", MaskingStrategy.Hidden);
        // strategies.Add("Age", MaskingStrategy.AgeGroup);
        //
        emps.AutoMask().PrettyDump(header:"Auto Masked");
        //emps.MaskColumnsWithStrategies(strategies).PrettyDump(header:"Masked");
    }
}
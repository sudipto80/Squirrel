using Squirrel;
using Squirrel.Cleansing;

namespace CleanserDemo;

class Program
{
    static void Main(string[] args)
    {
       
        var emps = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/outliers.csv");
       
        
        emps.MaskColumn("Name",MaskingStrategy.StarExceptLastFour).PrettyDump(header:"Masked");
    }
}
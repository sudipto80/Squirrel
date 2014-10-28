using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squirrel;


namespace Tips
{
    class Program
    {
        static void Main(string[] args)
        {
            //Problem : Locate average percentage of Tip paid by men and women from tips.csv
            //Done in 3 lines of C# code using Squirrel.NET 


            //Loading the data to Squirrel.NET Table is easy
            Table tips = DataAcquisition.LoadCSV(@"..\..\tips.csv");

            //Add a new column based on the formula
            tips.AddColumn(columnName: "tip%", formula: "[tip]*100/[totbill]", decimalDigits: 3);

            tips
             
                //Pick only these columns
                .Pick("sex", "tip%")                    
                //Aggregate the Tip% values by calculating the average
                .Aggregate("sex", AggregationMethod.Average)
                //Round off the result till 2 decimal points 
                .RoundOffTo(2)
                 //Dump the result to console. 
                .PrettyDump();
        }
    }
}

using Squirrel;
using Squirrel.Cleansing;
using Squirrel.DataCleansing;

namespace CleanserDemo;

class Program
{
    static void Main(string[] args)
    {

        var patientDetails =
            DataAcquisition.LoadCsv(
                @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/maskDemo.csv");
        
        patientDetails.PrettyDump(header:"Raw", rowColor: ConsoleColor.Black);
        patientDetails.AutoNormalize()
            .PrettyDump(header:"Auto Normalized", rowColor: ConsoleColor.DarkCyan);
        
        patientDetails.AutoMask()
            .PrettyDump(header:"Auto Masked", rowColor: ConsoleColor.DarkMagenta);

        //home-sale-prices.csv");
        
       
            
        
      
        var sal = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/empSals.csv");
        // Convert currency to numeric
        sal.PrettyDump(header:"Before Format Conversion", 
            rowColor: ConsoleColor.Black);
        sal = sal.TransformCurrencyToNumeric(columns:["Salary"]);
        sal.PrettyDump(header:"After Format Conversion", 
            rowColor: ConsoleColor.Blue);
       
        //tab.PrettyDump(header:"After Format Conversion");

        // Convert 1/0 to boolean
        //sal = sal.ConvertOnesAndZerosToBoolean("IsActive");

        // Extract only numbers and decimals
        sal = sal.Transform("PhoneNumber", x => x.KeepJustNumbersAndDecimal());


        //tab.PrettyDump(header:"After Value Filtering");

        
        var badEmails =
            DataAcquisition.LoadCsv(
                "/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/badEmails.csv");
        var emailPattern = @"^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9.-]*[a-zA-Z0-9])?\.[a-zA-Z]{2,}$";
        
        var tab = badEmails.RemoveNonMatches("email", emailPattern);
        
        badEmails.Pick("customer_id", "first_name", "last_name", "email")
            .PrettyDump(header:"Before", rowColor: ConsoleColor.Black);
        
        tab.Pick("customer_id", "first_name", "last_name", "email")
            .PrettyDump(header:"After", rowColor: ConsoleColor.Black);

        var orders = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/orders.csv");
        //Getting orders betwen 1st April to 30th June 
        var aprilToJune  = 
                orders
                    
                    .RemoveIfBefore("order_date",
                                   new DateTime(2024,04,01))
                    .RemoveIfAfter("order_date",
                                 new DateTime(2024,06,30));
        
        var aprToJun  = orders
            .RemoveIfNotBetween("order_date",
                                new DateTime(2024,04,1),
                                new DateTime(2024,06,30));

        var aprToJun2 = 
            orders.RemoveIf<DateTime>("order_date", t =>
                            t >= new DateTime(2024,04,1) && 
                            t <=  new DateTime(2024,06,30));

        var booksAndElectronics = 
            orders.RemoveNonMatches(columnName:"product_category", 
                                    regexPattern:"Books|Electronics");
            
        aprilToJune.PrettyDump();
        var students = DataAcquisition.LoadCsv
            (@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/badAge.csv");
        
         students.PrettyDump(header:"Students", rowColor:ConsoleColor.Red);
         var removed = students
             
             // Removing students with negative or >100 age (impossible!)
             // Removing teenagers 
             .RemoveLessThan("age", 0)
             .RemoveGreaterThan("age", 100)
             .RemoveIfBetween("age", 13, 19);
         
         Console.ForegroundColor = ConsoleColor.Black;
         removed.PrettyDump(header:"Students Removed",rowColor:ConsoleColor.Blue);
         


        
        
        var housing =
            DataAcquisition.LoadCsv(
                @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CleanserDemo/housing.csv");
        
        housing.ExtractOutliers("price")
            .PrettyDump(header:"Housing Prices Outliers",
                        rowColor:ConsoleColor.Red);
        
        Console.ForegroundColor = ConsoleColor.Black;
        housing.RemoveOutliers("price")
            .PrettyDump(header:"Housing Prices Outliers Removed!",
                        rowColor:ConsoleColor.Blue);
            
        
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

        antho.RemoveIncompleteRows(missingMarkers:["NA","N/A"])
             .PrettyDump(header:"Incomplete Rows Removed",
                         rowColor:ConsoleColor.Blue);

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
using Squirrel;
using Squirrel.Cleansing;

namespace ConsoleApp13;

class Program
{
    static void Main(string[] args)
    {
        var people = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/ConsoleApp13/people.csv");

        var stratSample = people.StratifiedSample("City",
                                15);
        
        stratSample.PrettyDump(header: "Stratified Sample", rowColor: ConsoleColor.Black);

        var cities = people.SplitOn("City");

        cities.Select(t => new {City = t.Key, PeopleCount = t.Value.RowCount})
            .ToTableFromAnonList()
            .SortBy(columnName:"PeopleCount", how: SortDirection.Descending)
            .PrettyDump(header:"People from different cities");


        people
            .SplitOn("Occupation")
            .Select(t => 
                new {Occupation = t.Key, PeopleCount = t.Value.RowCount})
            .ToTableFromAnonList()
            .SortBy(columnName:"PeopleCount", how: SortDirection.Descending)
            .PrettyDump(header:"People by Occupation",
                        rowColor: ConsoleColor.Black);

        var peopleTabs = people.SplitByRows(4);
        var salesData = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/ConsoleApp13/salesData.csv");

        var saleFigures = salesData.SplitByColumns(
            ["Sales_Rep", "Q1_Sales"],
            ["Sales_Rep", "Q2_Sales"],
            ["Sales_Rep", "Q3_Sales"],
            ["Sales_Rep", "Q4_Sales"]
        );
        
        for(int i = 1; i <= saleFigures.Count; i++)
            saleFigures[i-1].Name = $"Q{i}_Sales";

        // Print the first four rows for each table
        saleFigures
            .ForEach(t => t.SortBy(t.Name, how: SortDirection.Descending)
                                .Top(3)
                                .PrettyDump(header: $"Details of {t.Name}", rowColor: ConsoleColor.Black));


        //Taking 5 random rows from the table 
        var randomPeople = people.RandomSample(5);
        //Taking another 5 random rows from the table 
        var randomPeople2 = people.RandomSample(5);
        randomPeople.PrettyDump(header: "5 Random People", rowColor: ConsoleColor.Black);
        randomPeople2.PrettyDump(header: "Another 5 Random People", rowColor: ConsoleColor.Black);
        
        //Who are the bottom 5 performers by total annual sales?
        var salTabSorted = 
            salesData
                .AddColumn(columnName:"AnnualSales", 
                           formula: "[Q1_Sales] + [Q2_Sales] + [Q3_Sales] + [Q4_Sales]", 
                           decimalDigits:2)
            .SortBy("AnnualSales", how: SortDirection.Descending);
            
        salTabSorted.Bottom(5)
            .Pick("Sales_Rep", "AnnualSales")
            .PrettyDump(header: "Bottom 5 Performers", rowColor: ConsoleColor.Black);;
        
        // Pick top 5 performers by total annual sales
        salTabSorted.Top(5)
            .Pick("Sales_Rep", "AnnualSales")
            .PrettyDump(header: "Top 5 Performers", rowColor: ConsoleColor.Black);;;

        var mid = people.Middle(4, 10);
        salesData.Gist().ToTable()
            .PrettyDump(header: "Gist", rowColor: ConsoleColor.Black);
        var q1S = saleFigures[0].SortBy("Q1_Sales", how: SortDirection.Descending);
        
        q1S.Top(4).PrettyDump(header: "Q1 Sales", rowColor: ConsoleColor.Black);
        
        saleFigures.Select((t, i) =>
            new
            {
                Name = t.Name,
                Performer = 
                    saleFigures[i]
                        .SortBy(t.Name, how: SortDirection.Descending)
                    [0]["Sales_Rep"]

            })
            .ToTableFromAnonList()
            
            .PrettyDump(header:"Best Performers", rowColor: ConsoleColor.Blue);;
        
        // var tab = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/ConsoleApp13/data.csv");
        // Console.ForegroundColor = ConsoleColor.Magenta;
        // tab.PrettyDump(header: "All rows", 
        //                rowColor: ConsoleColor.Black);
        // Console.ForegroundColor = ConsoleColor.Magenta;
        // tab
        //     //Top 50% rows     
        //    .BottomNPercent(20)
        //    .PrettyDump(header: "Top 50% rows",
        //                headerColor: ConsoleColor.Green, 
        //                rowColor: ConsoleColor.Blue);
    }
}
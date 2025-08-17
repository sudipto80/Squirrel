using System.Diagnostics;
using Squirrel;

namespace Universe;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch w = Stopwatch.StartNew();
        w.Start();
        var symbols =
            DataAcquisition.LoadCsv(
                @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/Universe/symbols_universe.csv");

        var allUniqueSymbols = symbols.ValuesOf("Symbol").Distinct();
        
        var dividends =
            DataAcquisition.LoadCsv("/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/Universe/dividend_yields.csv");
        var monthlyPrices =
            DataAcquisition.LoadCsv("/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/Universe/monthly_prices.csv");
        string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        
        Dictionary<string,double> changes = new  Dictionary<string, double>();
        foreach (var sym in allUniqueSymbols)
        {
            try
            {
                var thisMonth = months[Convert.ToInt16(DateTime.Now.Month.ToString())];
                var lastMonth = months[Convert.ToInt16(DateTime.Today.AddMonths(-1).Month.ToString())];
                var sortedMonthlyPrices = monthlyPrices
                    .Filter("Symbol", sym);

                var sortedMonthlyPricesLastMonth =
                    sortedMonthlyPrices.Filter("Month", lastMonth)
                        .ValuesOf("Price")
                        .Select(Convert.ToDouble)
                        .Average();

                var sortedMonthlyPricesThisMonth =
                    sortedMonthlyPrices.Filter("Month", thisMonth)
                        .ValuesOf("Price")
                        .Select(Convert.ToDouble)
                        .Average();

                var percentageChange = (sortedMonthlyPricesThisMonth - sortedMonthlyPricesLastMonth) * 100 /
                                       sortedMonthlyPricesThisMonth;

                changes.Add(sym, percentageChange);
            }
            catch (Exception e)
            {
                //Console.WriteLine("SYMBOL : " + sym);
                //Console.WriteLine(e.Message);
            }
        }

        foreach (var key in changes.Keys)
        {
            Console.WriteLine(key + " " + changes[key]);
        }
        w.Stop();
        Console.WriteLine(w.Elapsed.TotalMilliseconds);
        Console.WriteLine("Hello, World!");
    }
}
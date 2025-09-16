using Squirrel;

namespace AllDataSets;

class Program
{
    static void Main(string[] args)
    {
        var apple = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/AllDataSets/AAPL.csv");

        var windows = apple.SlidingWindow(50);

        foreach (var window in windows)
        {
            var avg = window["Close"].Average();
            Console.WriteLine(avg);
        }
        Console.WriteLine("Hello, World!");
    }
}
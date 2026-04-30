using System;
using System.Linq;
using Squirrel;

namespace HousingDataJournalism
{
    class ExtensionMethodsAnalysis
    {
        static void Main(string[] args)
        {
            var houses = DataAcquisition.LoadCsv("house.csv");
            
            Console.WriteLine("=== DATA JOURNALISM WITH SQUIRREL EXTENSION METHODS ===\n");
            
            // Demonstrate various extension methods for data journalism
            
            // 1. Complex filtering with Where extension
            Console.WriteLine("=== 1. FINDING UNDERVALUED GEMS ===");
            var undervaluedGems = houses
                .Where("price", "<", 92000m)
                .Where(row => decimal.Parse(row["net_sqm"]) > 50)
                .Where(row => decimal.Parse(row["center_distance"]) < 1000)
                .Where(row => int.Parse(row["age"]) < 20);
            
            Console.WriteLine($"Found {undervaluedGems.RowCount} undervalued properties:");
            Console.WriteLine("- Price < 92,000");
            Console.WriteLine("- Size > 50 sqm");
            Console.WriteLine("- Distance < 1000m from center");
            Console.WriteLine("- Age < 20 years\n");
            
            if (undervaluedGems.RowCount > 0)
            {
                undervaluedGems.Top(5)
                    .Pick("bedroom_count", "net_sqm", "price", "center_distance", "metro_distance", "age")
                    .PrettyDump(header: "Top 5 Undervalued Gems",
                               rowColor: ConsoleColor.DarkGreen);
                Console.ResetColor();
            }
            Console.WriteLine();
            
            // 2. Using Between for range queries
            Console.WriteLine("=== 2. MID-MARKET PROPERTIES ===");
            var midMarket = houses
                .Between("price", "93000", "96000")
                .Between("net_sqm", "40", "80");
            
            Console.WriteLine($"Mid-market properties: {midMarket.RowCount}");
            Console.WriteLine("Price: 93,000 - 96,000");
            Console.WriteLine("Size: 40-80 sqm\n");
            
            // 3. Using In for categorical filtering
            Console.WriteLine("=== 3. IDEAL FAMILY HOMES ===");
            var familyHomes = houses
                .In("bedroom_count", "3", "4", "5")
                .Where("price", "<", 100000m)
                .Where(row => decimal.Parse(row["metro_distance"]) < 150);
            
            Console.WriteLine($"Family homes (3-5 bedrooms, affordable, near metro): {familyHomes.RowCount}\n");
            
            // 4. Stratified sampling for survey design
            Console.WriteLine("=== 4. STRATIFIED SAMPLE BY BEDROOM COUNT ===");
            var sample = houses.StratifiedSample("bedroom_count", 100);
            Console.WriteLine($"Created stratified sample of {sample.RowCount} properties");
            
            var bedroomDist = sample.Histogram("bedroom_count");
            Console.WriteLine("Sample distribution by bedrooms:");
            foreach (var kvp in bedroomDist.OrderBy(x => int.Parse(x.Key)))
            {
                Console.WriteLine($"  {kvp.Key} bedroom(s): {kvp.Value} properties");
            }
            Console.WriteLine();
            
            // 5. Every nth row for systematic sampling
            Console.WriteLine("=== 5. SYSTEMATIC SAMPLE (Every 50th property) ===");
            var systematicSample = houses.Every(50);
            Console.WriteLine($"Systematic sample size: {systematicSample.RowCount}\n");
            
            // 6. Search across all columns
            Console.WriteLine("=== 6. FULL-TEXT SEARCH ===");
            // Note: This searches for numeric patterns, adjust as needed
            var expensiveProps = houses.Search("10");
            Console.WriteLine($"Properties with '10' in any field: {expensiveProps.RowCount}\n");
            
            // 7. Sliding window analysis for time series patterns
            Console.WriteLine("=== 7. SLIDING WINDOW ANALYSIS (Sorted by Age) ===");
            var sortedByAge = houses.SortBy("age");
            var windows = sortedByAge.SlidingWindow(100, 50).Take(5);
            
            int windowNum = 1;
            foreach (var window in windows)
            {
                var avgPrice1 = window["price"].Average();
                var avgAge = window["age"].Average();
                Console.WriteLine($"Window {windowNum}: Avg Age = {avgAge:F1} years, Avg Price = {avgPrice1:F2}");
                windowNum++;
            }
            Console.WriteLine();
            
            // 8. Head and Tail with conditions
            Console.WriteLine("=== 8. FIRST 5 LUXURY PROPERTIES ===");
            var luxuryProps = houses.Head(5, row => 
                decimal.Parse(row["price"]) > 98000 && 
                decimal.Parse(row["center_distance"]) < 500);
            
            Console.WriteLine($"Found {luxuryProps.RowCount} luxury properties\n");
            
            if (luxuryProps.RowCount > 0)
            {
                luxuryProps.Pick("bedroom_count", "net_sqm", "price", "center_distance", "metro_distance", "age", "floor")
                    .PrettyDump(header: "Luxury Properties (Price > 98K, Distance < 500m)",
                               rowColor: ConsoleColor.DarkMagenta);
                Console.ResetColor();
            }
            Console.WriteLine();
            
            // 9. Combining multiple filters for investigative journalism
            Console.WriteLine("=== 9. POTENTIAL PRICE ANOMALIES ===");
            
            // Find properties that are unusually expensive for their characteristics
            var avgPrice2 = houses.ValuesOf("price").Select(decimal.Parse).Average();
            
            var anomalies = houses
                .Where("price", ">", avgPrice2 * 1.05m)
                .Where(row => decimal.Parse(row["net_sqm"]) < 40)
                .Where(row => int.Parse(row["age"]) > 50);
            
            Console.WriteLine("Properties that are expensive despite being small and old:");
            Console.WriteLine($"Count: {anomalies.RowCount}\n");
            
            if (anomalies.RowCount > 0)
            {
                var top = anomalies.SortBy("price", how: SortDirection.Descending).Top(5);
                
                top.Pick("bedroom_count", "net_sqm", "price", "age", "center_distance", "metro_distance", "floor")
                    .PrettyDump(header: "Top 5 Price Anomalies",
                               rowColor: ConsoleColor.DarkRed);
                Console.ResetColor();
            }
            Console.WriteLine();
            
            // 10. Using WhereStartsWith for categorical analysis
            Console.WriteLine("=== 10. PATTERN-BASED FILTERING ===");
            // Note: This would work better with categorical string data
            // For numeric data, we'll use GetRowsWhere instead
            
            var priceRanges = new[]
            {
                ("Budget (< 92k)", houses.GetRowsWhere(row => decimal.Parse(row["price"]) < 92000)),
                ("Mid (92-95k)", houses.GetRowsWhere(row => 
                    decimal.Parse(row["price"]) >= 92000 && decimal.Parse(row["price"]) < 95000)),
                ("Premium (95-98k)", houses.GetRowsWhere(row => 
                    decimal.Parse(row["price"]) >= 95000 && decimal.Parse(row["price"]) < 98000)),
                ("Luxury (98k+)", houses.GetRowsWhere(row => decimal.Parse(row["price"]) >= 98000))
            };
            
            Console.WriteLine("Market distribution by price range:");
            foreach (var (label, segment) in priceRanges)
            {
                var pct = (decimal)segment.RowCount / houses.RowCount * 100;
                Console.WriteLine($"{label,-20}: {segment.RowCount,4} properties ({pct:F1}%)");
            }
            Console.WriteLine();
            
            // 11. Partition for parallel analysis
            Console.WriteLine("=== 11. PARTITIONED ANALYSIS ===");
            var partitions = houses.Partition(1000).ToList();
            Console.WriteLine($"Dataset split into {partitions.Count} partitions of ~1000 rows each");
            
            for (int i = 0; i < Math.Min(3, partitions.Count); i++)
            {
                var partition = partitions[i];
                var avgPrice = partition.ValuesOf("price").Select(decimal.Parse).Average();
                Console.WriteLine($"Partition {i + 1}: {partition.RowCount} rows, Avg Price = {avgPrice:F2}");
            }
            Console.WriteLine();
            
            // 12. Complex investigative query
            Console.WriteLine("=== 12. INVESTIGATIVE JOURNALISM QUERY ===");
            Console.WriteLine("Question: Are new developments concentrated in specific areas?");
            
            var newDevelopments = houses
                .Where(row => int.Parse(row["age"]) <= 5)
                .SortBy("center_distance");
            
            // Group by distance zones
            newDevelopments.AddColumn("zone", "Round([center_distance] / 200,2) * 200", 0);
            var zoneDistribution = newDevelopments.Histogram("zone");
            
            Console.WriteLine($"\nNew developments (≤5 years old): {newDevelopments.RowCount}");
            Console.WriteLine("\nDistribution by distance from center:");
            
            var sortedZones = zoneDistribution.OrderBy(x => decimal.Parse(x.Key)).Take(10);
            foreach (var kvp in sortedZones)
            {
                var bar = new string('█', kvp.Value / 2);
                Console.WriteLine($"{kvp.Key,6}m: {bar} ({kvp.Value})");
            }
            
            Console.WriteLine("\nConclusion: Check if certain zones have disproportionate new construction");
        }
    }
}
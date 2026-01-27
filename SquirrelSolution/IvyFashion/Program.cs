using Squirrel;

namespace IvyFashion;

class Program
{
    static void Main(string[] args)
    {
        var fashions = DataAcquisition.LoadCsv(
            @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/IvyFashion/fashion.csv");

        // Brand and Product Counts

        var brands = fashions.SplitOn("Brand");
        var detailedData = new Dictionary<string, Dictionary<string, int>>();
        brands.Select(t =>
                new
                {
                    Brand = t.Key,
                    Products = t.Value.SplitOn("Product Name")
                        .Select(z => new { Product = z.Key, Count = z.Value.RowCount })
                }
            ).ToList()
            .ForEach(z =>
            {
               detailedData.Add(z.Brand, new Dictionary<string, int>());
                foreach (var product in z.Products)
                {
                    detailedData[z.Brand].Add(product.Product, product.Count);
                }
            });
        
        
            
        
     
        
        // How many products are there for each brand
        brands.Select(t =>
            {
                var prices =
                    t.Value["Price"].Select(z => Convert.ToDecimal(z.Trim()))
                        .ToList();
                return new
                {
                    Brand = t.Key,
                    Count = t.Value.RowCount,
                    // Highest and lowest price item for this brand
                    HighestPrice = prices.Max(),
                    LowestPrice = prices.Min()
                };
            })
            //Create a new table with the new columns from the Select projection 
            .ToTableFromAnonList()
            .PrettyDump(header: "Brand and Product Counts", rowColor: ConsoleColor.Blue);

        //Convert to strongly typed list 
        var records = RecordTable<FashionProduct>.FromTable(fashions);

        var prodList = records.Rows;


        // Filter by product then by size 
        fashions.Filter("Product Name", "Dress")
            .SortInThisOrder("Size", ["S", "M", "L", "XL"])
            .PrettyDump(header: "Dress", rowColor: ConsoleColor.Magenta);

        // Filter on multiple columns 
        fashions.Filter("Brand", "Adidas")
            .Filter("Product Name", "Dress")
            .Filter("Size", "M")
            .PrettyDump(header: "Adidas Dress M", rowColor: ConsoleColor.Magenta);

        // You can also use Dictionaries 
        fashions.FilterByRegex("Category", "Women")
            .SortBy("Rating", how: SortDirection.Descending)
            .PrettyDump(header: "Women's fashion", rowColor: ConsoleColor.Red);
        ;

        // Add column programmatically 
        fashions.AddColumn(columnName: "ValueForMoney", formula: "[Rating]/[Price]", decimalDigits: 2);
        var valueForMoney = fashions.SortBy("ValueForMoney", how: SortDirection.Descending)
            // Pick the columns we want 
            .Pick("Product Name", "Brand", "Category", "Price", "Rating", "ValueForMoney");
        // Dump to console 
        valueForMoney
            //Pick the top 15
            .Top(15)
            .PrettyDump(header: "Top 5 fashion for money", rowColor: ConsoleColor.Green);

        valueForMoney
            //Pick the bottom 15
            .Bottom(15)
            .PrettyDump(header: "Bottom 5 fashion for money", rowColor: ConsoleColor.DarkRed);

    }
}
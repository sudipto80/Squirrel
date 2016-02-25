Stock Price Analysis 
----------------------
Yahoo finance provides historical data about stock prices for a given symbol like this. This data is for Apple Computers. Symbol is "AAPL". However Yahoo doesn't include the Symbol in the result. 

<img src="aapl.png"/>

Using the following code we can query data from yahoo finance for multiple stock symbols and run some analysis for the last 30 days data. In this case I have queried for "AAPL" (Apple Computers), "GOOG" (Google) and "MSFT" (Microsoft). The result you shall get might vary but it will be similar. 

```csharp
//Stock symbols of companies for which you want to run this 
string[] symbols = { "AAPL", "GOOG", "MSFT" };
Dictionary<string, Table> stocks = new Dictionary<string, Table>();
string template = @"http://real-chart.finance.yahoo.com/table.csv?s=[Symbol]&d=8&e=4&f=2016
                    &g=d&a=0&b=2&c=1962&ignore=.csv";
foreach (var symb in symbols)
{
    WebClient wc = new WebClient();
    wc.DownloadFile(template.Replace("[Symbol]", symb), "temp" + symb + ".csv");
    stocks.Add(symb, DataAcquisition.LoadCSV("temp" + symb + ".csv").Top(30));
}

//Creating the symbol column. 
symbols.ToList().ForEach(s => stocks[s].AddColumn("Symbol", new List<string>(Enumerable.Repeat(s, 30))));

//Merging the results using LINQ. 
Table allStocks = symbols.Select(ticker => stocks[ticker])
                         .Aggregate((first, second) => first.Merge(second));

//Adding the column "Diff" programmatically. 
allStocks.AddColumn(columnName: "Diff", formula: "[Open] - [Close]", decimalDigits: 4);

//Preparing to write the result in a HTML file.
StreamWriter sw = new StreamWriter("temp.htm");

Func<Dictionary<string, string>, bool> greatValues = x => Math.Abs(Convert.ToDecimal(x["High"])) >= 500;
Func<Dictionary<string, string>, bool> worries = x => Math.Abs(Convert.ToDecimal(x["Diff"])) >= 6;
Func<Dictionary<string, string>, bool> warnings = x => Math.Abs(Convert.ToDecimal(x["Diff"])) <= 2;
string htmlTable = allStocks
                     //Sort by the difference in descending order
                    .SortBy("Diff", how: SortDirection.Descending)
                     //Taking top 20 entries
                    .Top(20)
                    //Pick only these columns
                    .Pick("Symbol", "Diff", "High", "Close")
                    .ToBootstrapHTMLTableWithColoredRows
					(
							infoPredicate: greatValues,
							warningPredicate: warnings,
							dangerPredicate: worries
					);

sw.WriteLine(htmlTable);
sw.Close();
System.Diagnostics.Process.Start("temp.htm");
```

This produces an output similar to this one. Here Bootstraps colored row is used to highlight some of the rows that are of interest. For example stock prices above 500 is a great value. The Func<> definitions are used to determine the colors. 
The value of the Diff column doesn't match the exact diffrene between Open and Close because the differences are taken till 4 decimal digits. If you change it to 6 then it will match up exactly.  

<img src="stock_analysis.png"/>

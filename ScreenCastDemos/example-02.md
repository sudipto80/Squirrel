Example #2 (Iris dataset aggregation)
-----
<img src="https://s3.amazonaws.com/assets.datacamp.com/blog_assets/Machine+Learning+R/iris-machinelearning.png"/>

***data*** for this example is available at ***../Data/iris.csv***

<p> The Iris flower data set or Fisher's Iris data set is a multivariate data set introduced by Sir Ronald Fisher (1936) as an example of discriminant analysis.It is sometimes called Anderson's Iris data set because Edgar Anderson collected the data to quantify the morphologic variation of Iris flowers of three related species.[2] Two of the three species were collected in the Gaspé Peninsula "all from the same pasture, and picked on the same day and measured at the same time by the same person with the same apparatus". ]---- Taken from Wikipedia </p>

<p>A botanist might want to find several aggregation reports from this dataset</p>


<img src="http://gifyu.com/images/iris.gif" border="0">

Here is the code that uses Squirrel to address these requirements.
```csharp
Table iris = DataAcquisition.LoadCSV(@"iris.csv");
StringBuilder builder = new StringBuilder();
 
builder.AppendLine("<html>");
 
builder.AppendLine("<h2>Range</h2>");
builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Range).ToBasicBootstrapHTMLTable(BootstrapTableDecorators.BootstrapTableClasses.Table_Striped)););
 
builder.AppendLine("<h2>Average</h2>");
builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Average).ToBasicBootstrapHTMLTable(BootstrapTableDecorators.BootstrapTableClasses.Table_Striped)););
 
builder.AppendLine("<h2>Max</h2>");
builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Max).ToBasicBootstrapHTMLTable(BootstrapTableDecorators.BootstrapTableClasses.Table_Striped)););
 
builder.AppendLine("<h2>Min</h2>");
builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Min).ToBasicBootstrapHTMLTable(BootstrapTableDecorators.BootstrapTableClasses.Table_Striped)););
 
builder.AppendLine("</html>");
StreamWriter writer = new StreamWriter("temp.html");
writer.WriteLine(builder.ToString());
writer.Close();
 
System.Diagnostics.Process.Start("temp.html"); 
```
This produces the following output
<img src="http://gifyu.com/images/IrisAggregationResult.png" alt="IrisAggregationResult.png" border="0" />

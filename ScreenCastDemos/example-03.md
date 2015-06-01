Example #3 (Finding Gender-Ratio statistics in North America)
----
<img src="http://alivecampus.com/wp-content/uploads/2013/12/male-female-ratio1.jpg" width="200" />

***data*** for this example is available at ***../Data/births.csv***

<p>Gender Ratio is an important sociological index to gauge the growth of a nation. The following data (only a few are shown here) is a list of "boy" and "girl" birth statistics for different states across North America. The data analysis task a sociologist might be interested to do is to find gender-ratio of North America for different states</p>

<p>The following C# code achieves this using Squirrel</p>
<img src="http://gifyu.com/images/births.gif" border="0">
```csharp
Table births = DataAcquisition.LoadCSV(@"..\..\births.csv");
var splits = births.SplitOn("sex");
 
var boys = splits["boy"].Aggregate("state").Drop("year");
var girls = splits["girl"].Aggregate("state").Drop("year");
 
Table combined = new Table();
 
combined.AddColumn("State", boys["state"]);
combined.AddColumn("Boys", boys["births"]);
combined.AddColumn("Girls", girls["births"]);
combined.AddColumn("Difference", "[Boys]-[Girls]", 4);
combined.AddColumn("GenderRatio", "[Girls]/[Boys]", 4);
//Showing 5 stats with lowest gender ratio at the end of 2006.
string tab = combined.Pick("State", "GenderRatio")
.SortBy("GenderRatio")
.Top(5)
.ToBasicBootstrapHTMLTable(BootstrapTableDecorators.BootstrapTableClasses.Table_Striped);
 
StreamWriter sw = new StreamWriter("temp.htm");
sw.WriteLine("<html><h2>Gender Ratio in North America by the end of 2006</h2>" + tab + "</html>");
sw.Close();
System.Diagnostics.Process.Start("temp.htm"); 
```

This produces the following result 
<img src="http://gifyu.com/images/GenderRatioResult.png" alt="GenderRatioResult.png" border="0" />

<a href="https://gist.github.com/sudipto80/5c53f9d53c5372cdb4c8"></a>

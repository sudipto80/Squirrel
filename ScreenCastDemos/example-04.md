Example #4 (Finding top gold winning nations in Olympics)
---------------
<img src="http://www.mytchett.surrey.sch.uk/assets/images/olympics/Olympic%20Medals.jpg" width="300" /></img>
From a given data of Olympics records like as shown below, how easy do you think to generate 
a pie chart for top gold winning nations. 
<img src="http://gifyu.com/images/OlympicsInput.png" alt="OlympicsInput.png" border="0" />

<p>Here is the code using Squirrel to generate a pie chart for top gold winning nations.</p>
```csharp 
Table olympics = DataAcquisition.LoadCSV("C:\\Squirrel\\olympics.csv");
StreamWriter sw = new StreamWriter("temp.htm");

string html = olympics
             .SortBy("Gold Medals",how:SortDirection.Descending)
             .Top(20)
             .ToPieByGoogleDataVisualization("Country", "Top Gold Winning Nations in Olympics");
sw.WriteLine(html);
sw.Close();

System.Diagnostics.Process.Start("temp.htm");
```
<p> And here is the output that you shall get.</p>
<img src="http://gifyu.com/images/OlympicsOut.png" alt="OlympicsOut.png" border="0" />

Changing this Pie chart to a donut chart is a simple enum change. Just change the call 
```csharp
.ToPieByGoogleDataVisualization("Country", "Top Gold Winning Nations in Olympics");
```
to 
```csharp
.ToPieByGoogleDataVisualization("Country", "Top Gold Winning Nations in Olympics",GoogleDataVisualizationcs.PieChartType.Donut);
```

and you shall get the following chart. Changing the Enum to Pie3D will generate a 3D Pie chart.
<img src="http://gifyu.com/images/OlympicGoldDonut.png" alt="OlympicGoldDonut.png" border="0" />

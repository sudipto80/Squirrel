Example #1 (Do women pay more tip than men?)
------
<img src="http://gifyu.com/images/women_dining.jpg" border="0">

***data*** for this example is available at ***../Data/tips.csv***

The data from which this analytics has to be calculated is available in tips.csv file and first few rows of that file looks like this 

<img src="http://gifyu.com/images/tips.gif" alt="tips.gif" border="0" />

Here is how you can use Squirrel to find an answer to that question
<!--<img src="http://gifyu.com/images/tips_final.gif"/>-->
```csharp
//Problem : Locate average percentage of Tip paid by men and women from tips.csv
//Done in 3 lines of C# code using Squirrel.NET
 
 
//Loading the data to Squirrel.NET Table is easy
Table tips = DataAcquisition.LoadCSV(@"..\..\tips.csv");
 
//Add a new column based on the formula
tips.AddColumn(columnName: "tip%", formula: "[tip]*100/[totbill]", decimalDigits: 3);
 
tips
//Pick only these columns
.Pick("sex", "tip%")
//Aggregate the Tip% values by calculating the average
.Aggregate("sex", AggregationMethod.Average)
//Round off the result till 2 decimal points
.RoundOffTo(2)
//Dump the result to console.
.PrettyDump(); 
```
This produces the following result as shown here in tabular format

| sex |     tip%|
|:-----|------:|
|   F  |   16.65|    
|M     |15.77|

This proves that women actually pays more tip than men!

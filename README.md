
Squirrel
====
<a href="Squirrel"><img src="https://raw.github.com/sudipto80/Squirrel/newb/img/icon_26718.png" align="right" height="100" width="100" />
***Agile Data Analytics for .NET*** </br>


Get **MORE** done with <font color='Blue'>**less**</font> code</br>
</br>
<p>
Squirrel is a simple and easy to use interfaces for querying and reporting of data. APIs for Data acquisition, Data filtering, Data cleansing, etc. provide simple solution, often in one step, for many real world problems.
</p>
</a></br>[`Squirrel logo is designed by Pirog tetyana for The Noun Project`](https://raw.github.com/sudipto80/Squirrel/newb/img/license.txt)

Squirrel (Hello World Example)
------
Finding whethere women are more generaous than men when it comes to paying tip</br>
The data from which this analytics has to be calculated is available in tips.csv file and first few rows of that file looks like this 
<img src="http://gifyu.com/images/tips.gif" border="0">
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

<a href="https://gist.github.com/sudipto80/5c53f9d53c5372cdb4c8"></a>

Squirrel Example: Stock Analytics
---------------------

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/a4aBLN75TXc/0.jpg)](http://www.youtube.com/watch?v=a4aBLN75TXc)


API Overview
------------

1. **BasicStatistics** - Basic statistical functions like Median, Range, Standard Deviation, Kurtosis, etc.
2. **CustomComparers** - Several customized comparators for sorting data.
3. **DataAcqusition** - Data loaded/dumped from/to various formats, e.g. CSV, TSV, HTML, ARFF, etc.
4. **DatabaseConnectors** - Data can be loaded from popular DB repositories by using the connectors for SQL Server and MongoDB.
5. **DataCleansers** - Extraction/Removal of outliers or data that matches specific boolean criteria.
6. **OrderedTable** - A data structure to hold sort results temporarily.
7. **SmartDefaults** - Defaults values for various use cases and data sets.
8. **Story** - Query generator for various use cases and data sets.
9. **Table** - An ubiquitous data structure used to encapsulate the data. Several APIs are part of the *Table* -
* Filter data using regular expressions or SQL clause.
* Sort data based on columns and their values.
* Programmatic manipulation i.e. deletion, updation and insertion of data.
* Merge data columns; Find subsets and exclusive or common rows in tables.
* Other utilities to split or drop data columns; Find rows that meet a specific statistical critieria, e.g. top 10, below average, etc.
* Natural queries

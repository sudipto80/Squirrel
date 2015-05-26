
Squirrel
======== 

<img src="http://gifyu.com/images/T-Shirt.png" border="0" height="250" width="300">

<!--<a href="Squirrel"><img src="https://raw.github.com/sudipto80/Squirrel/newb/img/icon_26718.png" align="left" t="100" width="100" ></a>-->

[`Squirrel by Pirog tetyana from The Noun Project`](https://raw.github.com/sudipto80/Squirrel/newb/img/license.txt)

***Agile Data Analytics for .NET***

Get ***MORE*** done with ***less*** code

Squirrel is a simple and easy to use interface for querying and reporting of data. APIs for Data acquisition, Data filtering, Data cleansing, etc. provide simple solution, often in one step, for many real world problems. 

For a quick start take a look at the [CheatSheet] (http://www.slideshare.net/sudipto80/squirrel-do-morewithlesscodelightcheatsheet)


Here is a high level block diagram of all the components of Squirrel.
![Block Diagram](http://gifyu.com/images/blocks.png "High Level Block Diagram")

Key Design Philosophy
---------------------
Here are couple of design decisions that have been the guiding principle for the choice of internal data structure for Squirrel ```Table``` data structure to make data accessing/manipulating more intuitive and efficient at the same time.
* Each row in the table should be available by zero based integer indexing as we do in arrays and ```List<T>``` So if a table ```birthRates``` exists then we should be able to get to 10th row by the syntax ```birthRates[9]```
* A column at a given index should be available by the column name index. So if we have a table ```StockValues``` that stores average stock values in a year of different companies where the row depicts the year and the column depicts the company for which the stock price is stored, then we should be able to get the stock price for Microsoft (Symbol “MSFT”) for 5th year as ```StockValues[4][“MSFT”]```
* Value at row ```“k”``` (Expressed as an integer) and column ```“m”``` (Expressed as a string) has to be accessible by either of the syntax ```table[k][“m”]``` or ```table[“m”][k]```.

API Overview
------------

1. **BasicStatistics** - Basic statistical functions like Median, Range, Standard Deviation, Kurtosis, etc.
2. **CustomComparers** - Several customized comparators for sorting data.
3. **DataAcqusition** - Data loaded/dumped from/to various formats, e.g. CSV, TSV, HTML, ARFF, etc.
4. **DatabaseConnectors** - Data can be loaded from popular DB repositories by using the connectors for SQL Server and MongoDB.
5. **DataCleansers** - Extraction/Removal of outliers or data that matches specific boolean criteria.
6. **OrderedTable** - A data structure to hold sort results temporarily.
7. **Table** - An ubiquitous data structure used to encapsulate the data. Several APIs are part of the *Table* -
   * Filter data using regular expressions or SQL clause.
   * Sort data based on columns and their values.
   * Programmatic manipulation i.e. deletion, updation and insertion of data.
   * Merge data columns; Find subsets and exclusive or common rows in tables.
   * Other utilities to split or drop data columns; Find rows that meet a specific statistical critieria, e.g. top 10, below average, etc.
   * Natural queries

[Here](https://raw.github.com/sudipto80/Squirrel/newb/doc/TableAPI.chm) is the detailed API documentation.

Documentation
------------
BasicStatistics
=======
```
Median
```
```
IQRRange
```
```
Range
```
```
Kurtosis
```
```
StandardDeviation
```
```
AverageCount
```
```
AboveAverageCount
```
```
BelowAverageCount
``` 
BootstrapTableDecorators
=======
```
ToBootstrapHTMLTableWithColoredRows
```
```
ToBasicBootstrapHTMLTable
``` 
DataAcquisition
=======
```
LoadXLS
```
```
LoadFixedLength
```
```
LoadFixedLength
```
```
LoadARFF
```
```
LoadHTMLTable
```
```
LoadLinearEntries
```
```
LoadCSV
```
```
LoadDataTable
```
```
LoadTSV
```
```
LoadFlatFile
```
```
PrettyDump
```
```
ToTable
```
```
ToHTMLTable
```
```
ToCSV
```
```
ToTSV
```
```
ToDataTable
```
```
ToARFF
``` 
RelationalDatabaseConnectors
=======
```
LoadFromSQLServer2008
```
```
LoadFromMongoDB
``` 
GoogleDataVisualizationcs
=======
```
ToHistogramByGoogleDataVisualization
```
```
ToPieByGoogleDataVisualization
```
```
ToBarChartByGoogleDataVisualization
```
```
ToPieFromHistogramByGoogleDataVisualization
```
```
ToBarChartFromHistogramByGoogleDataVisualization
```
```
ToBubbleChartByGoogleVisualization
``` 
Table
=======
```
GetPercentage
```
```
Filter
```
```
FilterByRegex
```
```
Filter
```
```
Filter
```
```
Filter
```
```
RunSQLQuery
```
```
SortInThisOrder
```
```
SortBy
```
```
ModifyColumnName
```
```
ValuesOf
```
```
ValuesOf
```
```
AddRow
```
```
AddRowsByShortHand
```
```
AddRows
```
```
ExtractAndAddAsColumn
```
```
AddColumn
```
```
TransformCurrencyToNumeric
```
```
AddColumn
```
```
RemoveColumn
```
```
Transpose
```
```
RoundOffTo
```
```
RoundOffTo
```
```
AggregateColumns
```
```
Aggregate
```
```
Aggregate
```
```
Transform
```
```
Histogram
```
```
SplitOn
```
```
MergeByColumns
```
```
Merge
```
```
Exclusive
```
```
Common
```
```
IsSubset
```
```
MergeColumns
```
```
Drop
```
```
Pick
```
```
RandomSample
```
```
Top
```
```
Bottom
```
```
TopNPercent
```
```
BottomNPercent
```
```
Middle
```
```
SplitByRows
```
```
SplitByColumns
```
```
Shuffle
```
```
ShowMe
```
```
HowMany
```
```
GetPercentage
```
```
Filter
```
```
FilterByRegex
```
```
Filter
```
```
Filter
```
```
Filter
```
```
RunSQLQuery
```
```
SortInThisOrder
```
```
SortBy
```
```
ModifyColumnName
```
```
ValuesOf
```
```
ValuesOf
```
```
AddRow
```
```
AddRowsByShortHand
```
```
AddRows
```
```
ExtractAndAddAsColumn
```
```
AddColumn
```
```
TransformCurrencyToNumeric
```
```
AddColumn
```
```
RemoveColumn
```
```
Transpose
```
```
RoundOffTo
```
```
RoundOffTo
```
```
AggregateColumns
```
```
Aggregate
```
```
Aggregate
```
```
Transform
```
```
Histogram
```
```
SplitOn
```
```
MergeByColumns
```
```
Merge
```
```
Exclusive
```
```
Common
```
```
IsSubset
```
```
MergeColumns
```
```
Drop
```
```
Pick
```
```
RandomSample
```
```
Top
```
```
Bottom
```
```
TopNPercent
```
```
BottomNPercent
```
```
Middle
```
```
SplitByRows
```
```
SplitByColumns
```
```
Shuffle
```
```
ShowMe
```
```
HowMany
``` 
OrderedTable
=======
```
ThenBy
``` 
SmartDefaults
=======
```
DoesMatchingEntryExist
```
```
GetSmartDefaultValues
``` 
Story
=======
```
ColumnNamesInvolved
```
```
MethodInvolved
```
```
ToDescription
```
```
ToChart
```
```
HandleIt
```
```
HandleIt
```
```
GetCommand
```
```
GetCommand
```
```
QuickDashboard
```
```
AggregationTables
```
```
Gist
```
```
PseudoNaturalQuery
``` 
DataCleansers
=======
```
RemoveImpssibleCombinations
```
```
ReplaceMissingValues
```
```
KeepJustNumbersAndDecimal
```
```
Distinct
```
```
ExtractOutliers
```
```
RemoveOutliers
```
```
RemoveIfBetween
```
```
RemoveIfNotBetween
```
```
RemoveMatches
```
```
RemoveNonMatches
```
```
RemoveIfBefore
```
```
RemoveIfAfter
```
```
RemoveIfBetween
```
```
RemoveIfNotBetween
```
```
RemoveIfNotAnyOf
```
```
RemoveIfAnyOf
```
```
RemoveLessThan
```
```
RemoveLessThanOrEqualTo
```
```
RemoveGreaterThan
```
```
RemoveGreaterThanOrEqualTo
```
```
RemoveIf
```
```
RemoveIfNot
```
```
ConvertOnesAndZerosToBoolean
``` 


Dependency
----------

There is a dependency for [NCalc](https://ncalc.codeplex.com/) for the following methods 
```csharp
AddColumn() 
AddRows()
AddRowsByShortHand()
``` 
NuGet Package
-------------
<img src="http://cdn.ws.citrix.com/wp-content/uploads/2011/09/NugetIcon.png"/>
You can integrate the package using NuGet by giving the following command</br>
```
PM> Install-Package TableAPI 
```
[Here is the NuGet Package page](https://www.nuget.org/packages/TableAPI/)
Examples
--------

1. [Do women pay more tip than men?](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-01.md)
2. [Iris dataset aggregation](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-02.md)
3. [Finding Gender-Ratio statistics in North America](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-03.md)
4. [Finding top gold winning nations in Olympics](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-04.md)
5. [How much money someone will accumulate at retirement](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-05.md)
6. [Titanic Survivor Analysis per class](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-06.md)


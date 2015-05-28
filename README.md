
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

Documentation
-------------


6IEnumerable<KeyValuePair<String,List<KeyValuePair<String,String>>>> (15 items)4 





 
 





  
  
  
  
  
  
  
  
 
 
 





  
  
 
 
 
 
 
 





  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
 
 
 





  
  
 
 
 





  
  
  
  
  
  
 
 
 





  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
 
 
 





  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
 
 
 





  
  
 
 
 





  
  
  
  
  
  
  
  
  
  
 
 
 





  
 
 
 





  
 
 
 





  
 
 
 





  
 
 
 





  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
 
BasicStatistics
============
|[**```Median```**](Median.md)||

|[**```IQRRange```**](IQRRange.md)|This must go to the Math API|

|[**```Range```**](Range.md)|Returns the range of values for the given columns.|

|[**```Kurtosis```**](Kurtosis.md)|Returns the kurtosis of a data set. Kurtosis characterizes the relative peakedness or flatness of a distribution compared with the normal distribution. Positive kurtosis indicates a relatively peaked distribution. Negative kurtosis indicates a relatively flat distribution.|

|[**```StandardDeviation```**](StandardDeviation.md)||

|[**```AverageCount```**](AverageCount.md)||

|[**```AboveAverageCount```**](AboveAverageCount.md)|Returns a number of|

|[**```BelowAverageCount```**](BelowAverageCount.md)|Returns the number of instances that are below average value|
BootstrapTableDecorators
============
|[**```ToBootstrapHTMLTableWithColoredRows```**](ToBootstrapHTMLTableWithColoredRows.md)|Bootstrap offers functionalities to color rows of a given table.|

|[**```ToBasicBootstrapHTMLTable```**](ToBasicBootstrapHTMLTable.md)|Returns a basic HTML table in bootstrap format.|
CustomComparers
============
DataAcquisition
============
|[**```LoadXLS```**](LoadXLS.md)|Loads the data from an Excel workbook to a table|

|[**```LoadFixedLength```**](LoadFixedLength.md)|Loads data from fixed column length files.|

|[**```LoadARFF```**](LoadARFF.md)|Loads data from .arff format Data in Weka toolkit is from .arff source|

|[**```LoadHTMLTable```**](LoadHTMLTable.md)|Loads a HTML table to the corresponding Table container|

|[**```LoadLinearEntries```**](LoadLinearEntries.md)||

|[**```LoadCSV```**](LoadCSV.md)|Loads a CSV file to a respective Table data structure.|

|[**```LoadDataTable```**](LoadDataTable.md)|Loads data from a ADO.NET DataTable to a Table|

|[**```LoadTSV```**](LoadTSV.md)|Loads Data from Tab Separated File|

|[**```LoadFlatFile```**](LoadFlatFile.md)|Loads data from any flat file|

|[**```PrettyDump```**](PrettyDump.md)|Dumps the table in a pretty format to console.|

|[**```ToTable```**](ToTable.md)|Returns the tabular representation of a gist|

|[**```ToHTMLTable```**](ToHTMLTable.md)|Returns the html table representation of the table.|

|[**```ToCSV```**](ToCSV.md)|Generates a CSV representation of the table|

|[**```ToTSV```**](ToTSV.md)|Generates a TSV representation of the table|

|[**```ToDataTable```**](ToDataTable.md)|Generates a DataTable out of the current Table|

|[**```ToARFF```**](ToARFF.md)|Returns the string representations of the table as a ARFF file.|
RelationalDatabaseConnectors
============
|[**```LoadFromSQLServer2008```**](LoadFromSQLServer2008.md)||

|[**```LoadFromMongoDB```**](LoadFromMongoDB.md)||
GoogleDataVisualizationcs
============
|[**```ToHistogramByGoogleDataVisualization```**](ToHistogramByGoogleDataVisualization.md)|Genertes the Histogram using google data visualization.|

|[**```ToPieByGoogleDataVisualization```**](ToPieByGoogleDataVisualization.md)|Generates a pie/3dPie/Donut chart from the given table.|

|[**```ToBarChartByGoogleDataVisualization```**](ToBarChartByGoogleDataVisualization.md)|Generates a bar/column chart from the given table for the given column|

|[**```ToPieFromHistogramByGoogleDataVisualization```**](ToPieFromHistogramByGoogleDataVisualization.md)|TODO|

|[**```ToBarChartFromHistogramByGoogleDataVisualization```**](ToBarChartFromHistogramByGoogleDataVisualization.md)|TODO|

|[**```ToBubbleChartByGoogleVisualization```**](ToBubbleChartByGoogleVisualization.md)|TODO|
Table
============
|[**```GetPercentage```**](GetPercentage.md)|Returns the percentage of the value "value" from all the values of the given column|

|[**```Filter```**](Filter.md)|Finding a value by regular expression|

|[**```FilterByRegex```**](FilterByRegex.md)|Finding a value by regular expression|

|[**```RunSQLQuery```**](RunSQLQuery.md)|Runs SQL Query against the Table|

|[**```SortInThisOrder```**](SortInThisOrder.md)|Sort by a custom ordering of elements.|

|[**```SortBy```**](SortBy.md)|Sorts the current table by the given column.|

|[**```ModifyColumnName```**](ModifyColumnName.md)|Modifies column name|

|[**```ValuesOf```**](ValuesOf.md)|The generic version of the values of method.|

|[**```AddRow```**](AddRow.md)|Adds a new row to the table|

|[**```AddRowsByShortHand```**](AddRowsByShortHand.md)|Add rows by short hand of common programmatic rules like "columnName++" etc|

|[**```AddRows```**](AddRows.md)|Add rows by short hand of common programmatic rules like "columnName++" etc|

|[**```ExtractAndAddAsColumn```**](ExtractAndAddAsColumn.md)|Extracts words from values of a given column and then uses these extracted values to create a new column|

|[**```AddColumn```**](AddColumn.md)|Adds a column for which value gets calculated from a given formula.|

|[**```TransformCurrencyToNumeric```**](TransformCurrencyToNumeric.md)|Removes all currency symbol and comma from the values of the given column.|

|[**```RemoveColumn```**](RemoveColumn.md)|Remove the given column from the table|

|[**```CumulativeFold```**](CumulativeFold.md)|A generic folder method. This helps to generate a cumulative fold|

|[**```Transpose```**](Transpose.md)|Change rows as columns|

|[**```RoundOffTo```**](RoundOffTo.md)|Rounds off all the numeric columns to given number of digits|

|[**```AggregateColumns```**](AggregateColumns.md)|Aggregate values of given columns as per the given aggregation method.|

|[**```Aggregate```**](Aggregate.md)|Aggregate values of given columns as per the given aggregation method.|

|[**```Transform```**](Transform.md)|Removes all currency symbol and comma from the values of the given column.|

|[**```Histogram```**](Histogram.md)|Generates the histogram for the column from the table|

|[**```SplitOn```**](SplitOn.md)|Splits a table on the distinct values of a given column|

|[**```MergeByColumns```**](MergeByColumns.md)|Merge two tables removing duplicate rows from the resultant table|

|[**```Merge```**](Merge.md)|Merge two tables removing duplicate rows from the resultant table|

|[**```Exclusive```**](Exclusive.md)|Extracts those rows from the table which are not present in the another one.|

|[**```Common```**](Common.md)|Finding common rows from two tables. Since it returns a table, there can be cascaded calls.|

|[**```IsSubset```**](IsSubset.md)|Checks whether a given table is subset of this table or not|

|[**```MergeColumns```**](MergeColumns.md)|Returns a table with merged columns|

|[**```Drop```**](Drop.md)|Returns a table with all the columns except those mentioned in parameters|

|[**```Pick```**](Pick.md)|Returns a table with just the columns mentioned.|

|[**```RandomSample```**](RandomSample.md)|Random Sample rows from the table|

|[**```Top```**](Top.md)|Retuns top n rows|

|[**```Bottom```**](Bottom.md)|Returns last n rows|

|[**```TopNPercent```**](TopNPercent.md)|Returns top n percent entries from the table|

|[**```BottomNPercent```**](BottomNPercent.md)|Returns the bottom n % entries as a new table|

|[**```Middle```**](Middle.md)|Returns a section of rows from the middle of the table|

|[**```SplitByRows```**](SplitByRows.md)|Splits the table acording to the rows|

|[**```SplitByColumns```**](SplitByColumns.md)|Generates multiple tables with the specified columns per table.|

|[**```Shuffle```**](Shuffle.md)|Retuns top n rows|

|[**```ShowMe```**](ShowMe.md)|Some times we are interested to find rows in the table that match a given condition|

|[**```HowMany```**](HowMany.md)||
OrderedTable
============
|[**```ThenBy```**](ThenBy.md)|Sorts an already sorted table by another column|

|[**```GetPercentage```**](GetPercentage.md)||

|[**```Filter```**](Filter.md)||

|[**```FilterByRegex```**](FilterByRegex.md)||

|[**```RunSQLQuery```**](RunSQLQuery.md)||

|[**```SortInThisOrder```**](SortInThisOrder.md)||

|[**```SortBy```**](SortBy.md)||

|[**```ModifyColumnName```**](ModifyColumnName.md)||

|[**```ValuesOf```**](ValuesOf.md)||

|[**```AddRow```**](AddRow.md)||

|[**```AddRowsByShortHand```**](AddRowsByShortHand.md)||

|[**```AddRows```**](AddRows.md)||

|[**```ExtractAndAddAsColumn```**](ExtractAndAddAsColumn.md)||

|[**```AddColumn```**](AddColumn.md)||

|[**```TransformCurrencyToNumeric```**](TransformCurrencyToNumeric.md)||

|[**```RemoveColumn```**](RemoveColumn.md)||

|[**```CumulativeFold```**](CumulativeFold.md)||

|[**```Transpose```**](Transpose.md)||

|[**```RoundOffTo```**](RoundOffTo.md)||

|[**```AggregateColumns```**](AggregateColumns.md)||

|[**```Aggregate```**](Aggregate.md)||

|[**```Transform```**](Transform.md)||

|[**```Histogram```**](Histogram.md)||

|[**```SplitOn```**](SplitOn.md)||

|[**```MergeByColumns```**](MergeByColumns.md)||

|[**```Merge```**](Merge.md)||

|[**```Exclusive```**](Exclusive.md)||

|[**```Common```**](Common.md)||

|[**```IsSubset```**](IsSubset.md)||

|[**```MergeColumns```**](MergeColumns.md)||

|[**```Drop```**](Drop.md)||

|[**```Pick```**](Pick.md)||

|[**```RandomSample```**](RandomSample.md)||

|[**```Top```**](Top.md)||

|[**```Bottom```**](Bottom.md)||

|[**```TopNPercent```**](TopNPercent.md)||

|[**```BottomNPercent```**](BottomNPercent.md)||

|[**```Middle```**](Middle.md)||

|[**```SplitByRows```**](SplitByRows.md)||

|[**```SplitByColumns```**](SplitByColumns.md)||

|[**```Shuffle```**](Shuffle.md)||

|[**```ShowMe```**](ShowMe.md)||

|[**```HowMany```**](HowMany.md)||
SmartDefaults
============
|[**```DoesMatchingEntryExist```**](DoesMatchingEntryExist.md)||

|[**```GetSmartDefaultValues```**](GetSmartDefaultValues.md)||
Story
============
|[**```ColumnNamesInvolved```**](ColumnNamesInvolved.md)||

|[**```MethodInvolved```**](MethodInvolved.md)||

|[**```ToDescription```**](ToDescription.md)||

|[**```ToChart```**](ToChart.md)||

|[**```HandleIt```**](HandleIt.md)||

|[**```GetCommand```**](GetCommand.md)||

|[**```QuickDashboard```**](QuickDashboard.md)|Returns the HTML content for a quick dashboard for a given table|

|[**```AggregationTables```**](AggregationTables.md)||

|[**```Gist```**](Gist.md)|Calculates the gist of values for numeric and currency columns.|

|[**```PseudoNaturalQuery```**](PseudoNaturalQuery.md)||
Alignment
============
|[**```HasFlag```**](HasFlag.md)||
SortDirection
============
|[**```HasFlag```**](HasFlag.md)||
OutlierDetectionAlgorithm
============
|[**```HasFlag```**](HasFlag.md)||
AggregationMethod
============
|[**```HasFlag```**](HasFlag.md)||
DataCleansers
============
|[**```RemoveImpssibleCombinations```**](RemoveImpssibleCombinations.md)|Replaces missing values by the given scheme.|

|[**```ReplaceMissingValues```**](ReplaceMissingValues.md)|Replaces missing values by the given scheme.|

|[**```KeepJustNumbersAndDecimal```**](KeepJustNumbersAndDecimal.md)|Removes every other character from the given string except numbers|

|[**```Distinct```**](Distinct.md)|Removes duplicate rows from the table|

|[**```ExtractOutliers```**](ExtractOutliers.md)|Extracts the rows which have outlier values for the given column name|

|[**```RemoveOutliers```**](RemoveOutliers.md)|Remove all rows that correspond to a value for the given column which is an outlier|

|[**```RemoveIfBetween```**](RemoveIfBetween.md)|Removes those rows at which the value of the given column falls under the given range|

|[**```RemoveIfNotBetween```**](RemoveIfNotBetween.md)|Removes rows from the table that are not between the given values. This is only for the numeric columns.|

|[**```RemoveMatches```**](RemoveMatches.md)|Remove those rows where the values match the given regular expression|

|[**```RemoveNonMatches```**](RemoveNonMatches.md)|Remove those rows where the values don't match with the given regular expression|

|[**```RemoveIfBefore```**](RemoveIfBefore.md)|Removes those rows from the given table where the value of the given date occurs before the date in the column given.|

|[**```RemoveIfAfter```**](RemoveIfAfter.md)|Removes those rows from the given table for the given column|

|[**```RemoveIfNotAnyOf```**](RemoveIfNotAnyOf.md)|Removes items that are not in the list of expected values.|

|[**```RemoveIfAnyOf```**](RemoveIfAnyOf.md)|Remove all rows that has an illegal value in the given column|

|[**```RemoveLessThan```**](RemoveLessThan.md)|Removes items that are less than the given value|

|[**```RemoveLessThanOrEqualTo```**](RemoveLessThanOrEqualTo.md)|Removes items that are equal to or less than the given value|

|[**```RemoveGreaterThan```**](RemoveGreaterThan.md)|Removes rows where the values of the given column are greater than the decimal value|

|[**```RemoveGreaterThanOrEqualTo```**](RemoveGreaterThanOrEqualTo.md)|Removes values greater than or equal to the given value for the given column|

|[**```RemoveIf```**](RemoveIf.md)|Removes those rows at which the value of the given column falls under the given range|

|[**```RemoveIfNot```**](RemoveIfNot.md)|Removes rows from the table that are not between the given values. This is only for the numeric columns.|

|[**```ConvertOnesAndZerosToBoolean```**](ConvertOnesAndZerosToBoolean.md)|Converts to|

Examples
--------

1. [Do women pay more tip than men?](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-01.md)
2. [Iris dataset aggregation](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-02.md)
3. [Finding Gender-Ratio statistics in North America](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-03.md)
4. [Finding top gold winning nations in Olympics](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-04.md)
5. [How much money someone will accumulate at retirement](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-05.md)
6. [Titanic Survivor Analysis per class](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-06.md)


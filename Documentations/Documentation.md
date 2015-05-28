BasicStatistics
============
|Method Name|Summary|
|:-------------|:---------|
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
|Method Name|Summary|
|:-------------|:---------|
|[**```ToBootstrapHTMLTableWithColoredRows```**](ToBootstrapHTMLTableWithColoredRows.md)|Bootstrap offers functionalities to color rows of a given table.|
|[**```ToBasicBootstrapHTMLTable```**](ToBasicBootstrapHTMLTable.md)|Returns a basic HTML table in bootstrap format.|

DataAcquisition
============
|Method Name|Summary|
|:-------------|:---------|
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

GoogleDataVisualizationcs
============
|Method Name|Summary|
|:-------------|:---------|
|[**```ToHistogramByGoogleDataVisualization```**](ToHistogramByGoogleDataVisualization.md)|Genertes the Histogram using google data visualization.|
|[**```ToPieByGoogleDataVisualization```**](ToPieByGoogleDataVisualization.md)|Generates a pie/3dPie/Donut chart from the given table.|
|[**```ToBarChartByGoogleDataVisualization```**](ToBarChartByGoogleDataVisualization.md)|Generates a bar/column chart from the given table for the given column|
|[**```ToPieFromHistogramByGoogleDataVisualization```**](ToPieFromHistogramByGoogleDataVisualization.md)|TODO|
|[**```ToBarChartFromHistogramByGoogleDataVisualization```**](ToBarChartFromHistogramByGoogleDataVisualization.md)|TODO|
|[**```ToBubbleChartByGoogleVisualization```**](ToBubbleChartByGoogleVisualization.md)|TODO|
Table
============
|Method Name|Summary|
|:-------------|:---------|
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
|Method Name|Summary|
|:-------------|:---------|
|[**```ThenBy```**](ThenBy.md)|Sorts an already sorted table by another column|

SmartDefaults
============
|Method Name|Summary|
|:-------------|:---------|

|[**```DoesMatchingEntryExist```**](DoesMatchingEntryExist.md)||
|[**```GetSmartDefaultValues```**](GetSmartDefaultValues.md)||
Story
============
|Method Name|Summary|
|:-------------|:---------|

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

DataCleansers
============
|Method Name|Summary|
|:-------------|:---------|
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

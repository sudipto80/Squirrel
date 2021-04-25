using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TableAPI;

namespace Squirrel.Cleansing
{
	/// <summary>
	/// Home of all the different data cleansing methods. 
	/// </summary>
	public static class DataCleansers
	{
		
		/// <summary>
		/// 
		/// </summary>
		public enum MissingValueHandlingStrategy
		{
			/// <summary>
			/// Mark missing values with NA if NA is not already a representation of the missing value
			/// </summary>
			MarkWithNa,
			/// <summary>
			/// Fill missing values with average of  values of the given column
			/// </summary>
			Average,
            /// <summary>
            /// Fill missing values with the default values of the given type
            /// </summary>
            Default,
			/// <summary>
			/// Fill missing values with minimum of  values of the given column
			/// </summary>
			Min,
			/// <summary>
			/// Fill missing values with maximum of  values of the given column
			/// </summary>
			Max
		}

		private static Dictionary<string,string> _handleMissingValue(this Dictionary<string,string> row,Table tab,
									string columnName,
									   MissingValueHandlingStrategy strategy)
		{
			if (strategy == MissingValueHandlingStrategy.MarkWithNa)
			{
				row[columnName] = "NA";
			}
			if (strategy == MissingValueHandlingStrategy.Max)
			{
				row[columnName] = tab[columnName].Where(z => z.Trim().Length > 0).Select(Convert.ToDecimal).Max().ToString(CultureInfo.InvariantCulture);
			}
			if (strategy == MissingValueHandlingStrategy.Min)
			{
				row[columnName] = tab[columnName].Where(z => z.Trim().Length > 0).Select(Convert.ToDecimal).Max().ToString(CultureInfo.InvariantCulture);
			}
			if (strategy == MissingValueHandlingStrategy.Average)
			{
				row[columnName] = tab[columnName].Where(z => z.Trim().Length > 0).Select(Convert.ToDecimal).Max().ToString(CultureInfo.InvariantCulture);
			}
			return row;

		}

		/// <summary>
		/// TO:DO
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="strategy"></param>
		/// <param name="missingValueRepresentations"></param>
		/// <returns></returns>
		public static Table ReplaceMissingValuesByDefault(this Table tab,
			MissingValueHandlingStrategy strategy = MissingValueHandlingStrategy.MarkWithNa,
			params string[] missingValueRepresentations)
		{
			//If any of the missing value representative string appears in numeric column
			//replace with zero

			//else if it occurs in non-numeric column, replace with empty strings.
			Table temp = new Table();

			foreach (var row in tab.Rows)
			{
				Dictionary<string, string> currentRow = new Dictionary<string, string>();
				foreach (var colHead in tab.ColumnHeaders)
				{
					if (missingValueRepresentations.Any(z => z.Equals(row[colHead])))
					{
						temp.AddRow(_handleMissingValue(row, tab, colHead, strategy));

					}
					else
					{
						temp.AddRow(row);
					}

				}
			}
			return temp;
		}

		/// <summary>
		/// Removes every other character from the given string except numbers
		/// </summary>
		/// <param name="input">The input string</param>
		/// <returns>A cleansed string</returns>
		public static string KeepJustNumbersAndDecimal(this string input)
		{
			return input.Where(x => "0123456789.".Contains(x)).Select(z=>z.ToString())
															  .Aggregate((a, b) => a + b);
		}
	 

		/// <summary>
		/// TODO : Cleans table wherever there are duplicate values for a given column.
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static Table DistinctBy(this Table tab, string columnName)
		{
			//To do
			return tab;
		}
		/// <summary>
		/// Removes duplicate rows from the table
		/// </summary>
		/// <returns>A table with all unique rows</returns>
		/// <remarks>This method find distinct rows by calculating a hash by adding all the values of all the columns.
		/// So this can be a bottleneck for a table with many columns.</remarks>
		public static Table Distinct(this Table tab)
		{
			tab.ThrowIfTableIsNull();

			Dictionary<string, List<Dictionary<string, string>>> noDups =
				new Dictionary<string, List<Dictionary<string, string>>>();

			for (int i = 0; i < tab.RowCount; i++)
			{
				var hash = string.Empty;
				try
				{
					hash = tab.Rows[i].OrderBy(m => m.Key).Select(m => m.Value).Aggregate((x, y) => x + y);
				}
				catch (NullReferenceException ex)
				{
					//When Rows is null 

				}
				catch (InvalidOperationException ex)
				//When sequence contains no element Aggregate will throw this exception
				{

				}

				if (!noDups.ContainsKey(hash))
					noDups.Add(hash, new List<Dictionary<string, string>>() {tab.Rows[i]});
				else
					noDups[hash].Add(tab.Rows[i]);
			}

			var noDuplicates = new Table();
		  
			noDuplicates.Rows.AddRange(noDups.Select(t => t.Value.First()));
			return noDuplicates;
		}
		
		/// <summary>
		/// Extracts the rows which have outlier values for the given column name 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="algo">The algorithm to be used to extract outliers</param>
		/// <returns>A table with rows which are believed to be having values in the outlier range for the given column.</returns>
		public static Table ExtractOutliers(this Table tab, string columnName,
			OutlierDetectionAlgorithm algo = OutlierDetectionAlgorithm.IQR_Interval)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			var outliers = new Table();
			List<decimal> allValues;
			try
			{
				allValues = tab.ValuesOf(columnName).Select(Convert.ToDecimal).ToList();
			}
			catch(FormatException ex)
			{
				throw new FormatException($"some values of column {columnName} can't be converted to decimal ");
			}
			var iqrRange = BasicStatistics.IqrRange(allValues);
			for (int i = 0; i < allValues.Count; i++)
			{
				if (allValues[i] < iqrRange.Item1 || allValues[i] > iqrRange.Item2)
					outliers.Rows.Add(tab.Rows[i]);
			}
			return outliers;
		}
		/// <summary>
		/// Remove all rows that correspond to a value for the given column which is an outlier
		/// </summary>
		/// <param name="tab">The table which needs cleansing </param>
		/// <param name="columnName">The name of the column among which the outliers has to be found.</param>
		/// <param name="algo">The algorithm to be used to determine outliers in the values of the column given.</param>
		/// <returns>A cleansed table</returns>
		/// <example> 
		/// //This will remove all rows for where values of the column "Age" falls under suspecting range
		/// //of possible oulier 
		/// Table outliersRemoved = tableWithOutliers.RemoveOutliers("Age");
		/// </example>
		public static Table RemoveOutliers(this Table tab, string columnName,
											OutlierDetectionAlgorithm algo = OutlierDetectionAlgorithm.IQR_Interval)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);
			var allValues = tab.ValuesOf(columnName)
										 .Select(Convert.ToDecimal)
										 .ToList();
			//Get the IQR Range
			var iqrRange = BasicStatistics.IqrRange(allValues);
			for (int i = 0; i < allValues.Count; i++)
			{
				//Any value which is less or more than the given range is a possible outlier
				if (allValues[i] < iqrRange.Item1 || allValues[i] > iqrRange.Item2)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes those rows at which the value of the given column falls under the given range
		/// </summary>
		/// <param name="columnName">The column name</param>
		/// <param name="tab">The table which needs cleansing</param>
		/// <param name="low">The low value of the range</param>
		/// <param name="high">The high value of the range</param>
		/// <returns>A table with any matching rows deleted.</returns>
		/// <example>
		/// //Removes all rows where the value for the column "Col" is between 32 and 56
		/// Table cleansedTable = tab.RemoveIfBetween("Col",32M,56M);</example>
		public static Table RemoveIfBetween(this Table tab, string columnName, decimal low, decimal high)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);
			var vals = tab.ValuesOf(columnName).Select(Convert.ToDecimal).ToList();
			for (int i = 0; i < vals.Count(); i++)
			{
				if (low <= vals[i] && vals[i] <= high)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes rows from the table that are not between the given values. 
		/// This is only for the numeric columns.
		/// </summary>
		/// <param name="tab">The table to cleanse</param>
		/// <param name="columnName">The numeric column</param>
		/// <param name="low">The lowest value of the range</param>
		/// <param name="high">The highest value of the range</param>
		/// <returns>A cleansed table</returns>
		/// <example>
		/// //Removes all rows from the table where the value for the column "Age" is not between
		/// //the allowable range of 1 and 100.
		/// Table cleansedTable = tab.RemoveIfNotBetween("Age",1M,100M);
		/// </example>
		public static Table RemoveIfNotBetween(this Table tab, string columnName, decimal low, decimal high)
		{
			List<decimal> vals = tab.ValuesOf(columnName).Select(t => Convert.ToDecimal(t)).ToList();
			for (int i = 0; i < vals.Count(); i++)
			{
				if (!(low <= vals[i] && vals[i] <= high))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Remove those rows where the values match the given regular expression
		/// </summary>
		/// <param name="tab">The table that needs to be cleansed</param>
		/// <param name="columnName">The column name</param>
		/// <param name="regexPattern">The regular expression against which we want to validate</param>
		/// <returns>A cleansed table with matching rows removed</returns>
		/// <example>
		/// //Removes the rows where the "Name" column matches the numeric values.
		/// Table cleansedTable = tab.RemoveMatches("Name","[0-9]+");
		/// </example>
		public static Table RemoveMatches(this Table tab, string columnName, string regexPattern)
		{
			List<string> values = tab.ValuesOf(columnName).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (Regex.Match(values[i], regexPattern).Success)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Remove those rows where the values don't match with the given regular expression
		/// </summary>
		/// <param name="tab">The table that needs to be cleansed.</param>
		/// <param name="columnName">The column name</param>
		/// <param name="regexPattern">The regular expression</param>
		/// <returns>A cleansed table where the non matching rows are removed.</returns>
		/// <seealso cref="RemoveMatches"/>
		public static Table RemoveNonMatches(this Table tab,string columnName, string regexPattern)
		{
			List<string> values = tab.ValuesOf(columnName).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (!Regex.Match(values[i], regexPattern).Success)
					tab.Rows.RemoveAt(i);
			}
			return tab;

		}
		/// <summary>
		/// Removes those rows from the given table where the value of the given date 
		/// occurs before the date in the column given.
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="dateColumnName">The date column</param>
		/// <param name="date">The value of the date</param>
		/// <returns>A table with those rows where the date occurs before the given date; removed.</returns>
		public static Table RemoveIfBefore(this Table tab, string dateColumnName, DateTime date)
		{
			List<DateTime> dates = tab.ValuesOf(dateColumnName)
									   .Select(m => Convert.ToDateTime(m))
									   .ToList();

			for (int i = 0; i < dates.Count; i++)
			{
				if (dates[i].CompareTo(date) < 0)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes those rows from the given table for the given column
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="dateColumnName">The date column</param>
		/// <param name="date">The terminal date after which we need to remove values</param>
		/// <returns>A cleansed table</returns>
		/// <seealso cref="RemoveIfBefore"/>
		public static Table RemoveIfAfter(this Table tab, string dateColumnName, DateTime date)
		{
			List<DateTime> dates = tab.ValuesOf(dateColumnName).Select(m => Convert.ToDateTime(m)).ToList();

			for (int i = 0; i < dates.Count; i++)
			{
				if (dates[i].CompareTo(date) >= 1)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Remove all the rows where the date does fall in the given range.
		/// </summary>
		/// <param name="tab">The table that needs to be cleansed</param>
		/// <param name="dateColumnName">The date column on which we want to run the filter</param>
		/// <param name="startDate">The start date of the range</param>
		/// <param name="endDate">The end date of the range</param>
		/// <returns>A cleansed table</returns>
		
		public static Table RemoveIfBetween(this Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
		{
			List<DateTime> dates = tab.ValuesOf(dateColumnName).Select(Convert.ToDateTime).ToList();
			for (int i = 0; i < dates.Count; i++)
			{
				if (dates[i].CompareTo(startDate) >= -1 && dates[i].CompareTo(endDate) <= 1)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Remove all the rows where the date doesn't fall in between the given range.
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="dateColumnName">The date column</param>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <returns>A cleansed table with the violating rows removed.</returns>
		public static Table RemoveIfNotBetween(this Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
		{
			List<DateTime> dates = tab.ValuesOf(dateColumnName).Select(Convert.ToDateTime).ToList();
			for (int i = 0; i < dates.Count; i++)
			{
				if (!(dates[i].CompareTo(startDate) >= -1 && dates[i].CompareTo(endDate) <= 1))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes items that are not in the list of expected values.
		/// </summary>
		/// <param name="tab">The table to be cleansed</param>
		/// <param name="columnName">The name of the column on which we want to run this filter</param>
		/// <param name="expectedValues">Set of expected values</param>
		/// <returns>A cleansed table.</returns>
		public static Table RemoveIfNotAnyOf(this Table tab,string columnName, params string[] expectedValues)
		{
			List<string> values = tab.ValuesOf(columnName).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (!expectedValues.Contains(values[i]))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Remove all rows that has an illegal value in the given column
		/// </summary>
		/// <param name="tab">The table to be cleansed</param>
		/// <param name="columnName">The column name</param>
		/// <param name="illegalValues">Set of illegal values</param>
		/// <returns>A cleansed table with rows with illegal values removed.</returns>
		public static Table RemoveIfAnyOf(this Table tab,string columnName, params string[] illegalValues)
		{
			List<string> values = tab.ValuesOf(columnName).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (illegalValues.Contains(values[i]))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes items that are less than the given value
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName">The column for which the values have to be checked </param>
		/// <param name="value">The basis value below which all values has to be removed.</param>
		/// <returns>A Table with violating values removed.</returns>
		public static Table RemoveLessThan(this Table tab, string columnName, decimal value)
		{
			List<decimal> values = tab.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i] < value)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes items that are equal to or less than the given value 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName">The column for which values has to be checked against the given value </param>
		/// <param name="value">The given value</param>
		/// <returns>A table with violating values removed from the specified column.</returns>
		public static Table RemoveLessThanOrEqualTo(this Table tab, string columnName, decimal value)
		{
			var values = tab.ValuesOf(columnName).Select(Convert.ToDecimal).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i] <= value)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}

	  
		/// <summary>
		/// Removes rows where the values of the given column are greater than the decimal value
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName">The column name for which the values need to be removed.</param>
		/// <param name="value">The value (The threshold value)</param>
		/// <returns>A cleansed table where the values in column "columnName" are greater than the given value</returns>
		/// <example>Table cleansed = tab.RemoveGreaterThan("Age",120);</example>
		public static Table RemoveGreaterThan(this Table tab, string columnName, decimal value)
		{
			List<decimal> values = tab.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i] > value)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes values greater than or equal to the given value for the given column
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName">The column for which the given values have to be removed. </param>
		/// <param name="value">The given value; values greater than which has to be removed.</param>
		/// <returns>A table with violating values removed.</returns>
		public static Table RemoveGreaterThanOrEqualTo(this Table tab, string columnName, decimal value)
		{
			List<decimal> values = tab.ValuesOf(columnName).Select(Convert.ToDecimal).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i] >= value)
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Generic remove function. 
		/// </summary>
		/// 
		/// <typeparam name="T">The type of the column.</typeparam>
		/// <param name="tab">The cleansed </param>
		/// <param name="columnName">Name of the column</param>
		/// <param name="predicate">The predicated that dictates the delete operation</param>
		/// <returns>A table with the defaulters removed</returns>
		public static Table RemoveIf<T>(this Table tab, string columnName, Func<T, bool> predicate)
					 where T : IEquatable<T>
		{
			int i = 0;
			List<int> indicesToRemove = new List<int>();
			List<T> castedList = tab.ValuesOf(columnName).Cast<T>().ToList();
			for (; i < castedList.Count; i++)
			{
				T temp = castedList[i];
				if (predicate(temp))
				{
					indicesToRemove.Add(i);
				}
			}
			indicesToRemove.ForEach(k => tab.Rows.RemoveAt(k));
			return tab;
		}
		/// <summary>
		/// Rempves list of rows that doesn't match a given condition. 
		/// </summary>
		/// <typeparam name="T">The data type of the column</typeparam>
		/// <param name="tab"></param>
		/// <param name="columnName">The column name</param>
		/// <param name="predicate">The predicate based on which the rows will be deleted.</param>
		/// <returns>A table with rows removed.</returns>
		public static Table RemoveIfNot<T>(this Table tab, string columnName, Func<T, bool> predicate) where T : IEquatable<T>
		{
			int i = 0;
			List<int> indicesToRemove = new List<int>();
			List<T> castedList = tab.ValuesOf(columnName).Cast<T>().ToList();
			for (; i < castedList.Count; i++)
			{
				T temp = castedList[i];
				if (!predicate(temp))
				{
					indicesToRemove.Add(i);
				}
			}
			indicesToRemove.ForEach(k => tab.Rows.RemoveAt(k));
			return tab;
		}

		/// <summary>
		/// Converts to 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public static Table ConvertOnesAndZerosToBoolean(this Table tab, string column)
		{
			for (int i = 0; i < tab.RowCount; i++)
			{
				if (tab[column][i] == "1")
					tab[column][i] = "true";
				if (tab[column][i] == "0")
					tab[column][i] = "false";
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="badValues"></param>
		/// <returns></returns>
		public static Table RemoveIfContains(this Table tab, string columnName, params string[] badValues)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			for (int i = 0; i < tab.RowCount; i++)
			{
				if( badValues.Any( badValue =>  tab.Rows[i][columnName].Contains(badValue)))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="goodValues"></param>
		/// <returns></returns>
		public static Table RemoveIfDoesNotContain(this Table tab, string columnName, params string[] goodValues)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			for (int i = 0; i < tab.RowCount; i++)
			{
				if (!goodValues.All(goodValue => tab.Rows[i][columnName].Contains(goodValue)))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="badPrefixes"></param>
		/// <returns></returns>
		public static Table RemoveIfStartsWith(this Table tab, string columnName, params string[] badPrefixes)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			for (int i = 0; i < tab.RowCount; i++)
			{
				if (badPrefixes.Any(badPrefix => tab.Rows[i][columnName].StartsWith(badPrefix)))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="badSuffixes"></param>
		/// <returns></returns>
		public static Table RemoveIfEndsWith(this Table tab, string columnName, params string[] badSuffixes)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			for (int i = 0; i < tab.RowCount; i++)
			{
				if (!badSuffixes.Any(badSuffix => tab.Rows[i][columnName].EndsWith(badSuffix)))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Removes rows from teh table that don't start with any of the given list of good prefixes. 
		/// </summary>
		/// <param name="tab">The Table instance from which rows have to be removed</param>
		/// <param name="columnName">The column where to look for values</param>
		/// <param name="goodPrefixes">List of good prefixes.</param>
		/// <returns>A table with defaulter rows deleted.</returns>
		/// <example>
		/// //Returns a table where rows where the values start with "Range" and "From"
		/// //
		/// var cleaned  = tab.RemoveIfDoesNotStartWith("Range","From","To");
		/// </example>
		public static Table RemoveIfDoesNotStartWith(this Table tab, string columnName, params string[] goodPrefixes)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			for (int i = 0; i < tab.RowCount; i++)
			{
				if (!goodPrefixes.Any(goodPrefix => tab.Rows[i][columnName].StartsWith(goodPrefix)))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// Remove rows where the values of the given column don't end with the given list of suffixes
		/// </summary>
		/// <param name="tab">The Table instance from which bad defaulter rows have to be removed</param>
		/// <param name="columnName">The name of the column</param>
		/// <param name="goodSuffixes">List of good suffixes</param>
		/// <returns>A Table instance with defaulter rows deleted.</returns>
		/// <example>
		/// //The following line removes all those rows from the table "tab" where values of 
		/// //the column "Quarter" don't end with any of "i","ii","iii" or "iv"
		/// var cleaned = tab.RemoveIfDoesNotEndWith("Quarter","i","ii","iii","iv");</example>
		public static Table RemoveIfDoesNotEndWith(this Table tab, string columnName, params string[] goodSuffixes)
		{

			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);

			for (int i = 0; i < tab.RowCount; i++)
			{
				if (!goodSuffixes.Any(goodSuffix => tab.Rows[i][columnName].EndsWith(goodSuffix)))
					tab.Rows.RemoveAt(i);
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="possibleValues"></param>
		/// <returns></returns>
		public static Table MarkAsMissingIfNotAnyOf(this Table tab, string columnName, 
													 params string[] possibleValues)
		{
			return tab;
		}

		/// <summary>
		/// Removes rows with impossible value combinations
		/// For example for the Gender column 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="possibleValues"></param>
		/// <returns></returns>
		public static Table RemoveRowsWithImpossibleValues(this Table tab,
															string columnName,
															params string[] possibleValues)
		{
			for (int i = 0; i < tab.RowCount; i ++)
			{
				if (!possibleValues.Any(possibleValue => tab[columnName][i].Contains(possibleValue)))
				{
					tab.Rows.RemoveAt(i);
				}
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="possibleValueCombos"></param>
		/// <returns></returns>
		public static Table RemoveRowsWithImpossibleValues(this Table tab,
			Dictionary<string, string[]> possibleValueCombos)
		{
			foreach (var col in possibleValueCombos.Keys)
			{
				tab = tab.RemoveRowsWithImpossibleValues(col, possibleValueCombos[col]);
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public static Table RemoveIncompleteRows(this Table tab)
		{
			for (int i = 0; i < tab.RowCount; i++)
			{
				foreach (var col in tab.ColumnHeaders)
				{
					if(tab[col][i].Trim().Length == 0)
						tab.Rows.RemoveAt(i);
				}
			}
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="strategy"></param>
		/// <returns></returns>
		public static Table Normalize(this Table tab, string columnName,
			 NormalizationStrategy strategy = NormalizationStrategy.SentenceCase)
		{
            Table normalizedTable = new Table(); 
            foreach (var col in tab.ColumnHeaders)
            {
                if(col == columnName)
                {
                    normalizedTable.AddColumn(col, tab.ValuesOf(col).NormalizeAsPerStrategy(strategy));
                }
                else
                {
                    normalizedTable.AddColumn(col, tab.ValuesOf(col));
                }
            }
            return normalizedTable;
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="normalizationSchemes"></param>
		/// <returns></returns>
		public static Table Normalize(this Table tab, Dictionary<string, NormalizationStrategy> normalizationSchemes)
		{
			foreach (var key in normalizationSchemes.Keys)
            {
                //Normalize one column at a time
                tab = tab.Normalize(key, normalizationSchemes[key]);
            }
            return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static Table Truncate(this Table tab, string columnName, int length)
		{
			return tab;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="truncateLengths"></param>
		/// <returns></returns>
		public static Table Truncate(this Table tab, Dictionary<string, int> truncateLengths)
		{
			return tab;
		}

		/// <summary>
		/// Transforms values of a column as per the given transformer
		/// </summary>
		/// <param name="tab">The Table instance</param>
		/// <param name="columnName">The column on which the transformer function has to be run</param>
		/// <param name="tranformer">The transformer method</param>
		public static Table Transform(this Table tab, string columnName, Func<string, string> tranformer)
		{
			tab.ThrowIfTableIsNull();
			tab.ThrowIfColumnsAreNotPresentInTable(columnName);
			if (tranformer == null)
				throw new ArgumentNullException($"The scheme to transform the values of the given colum {columnName} is null");

			var temp = new Table();
			temp.Rows.AddRange(tab.Rows);
			for (int i = 0; i < tab.RowCount; i++)
			{
				temp.Rows[i][columnName] = tranformer(temp.Rows[i][columnName]);
			}
			return temp;
		}
		/// <summary>
		/// Removes all currency symbol and comma from the values of the given column.
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columns">The name of the column</param>
		/// <returns>A table without the currency symbol and commas</returns>
		/// 
		[Description("Clean numeric columns")]
		public static Table TransformCurrencyToNumeric(this Table tab, params string[] columns)
		{
			Table newTable = tab;

			foreach (string col in columns)
			{


				newTable = tab.Transform(col, x => x.Replace(",", string.Empty)
												.Replace("$", string.Empty)//Remove US Dollar symbol
												.Replace("£", string.Empty)//Remove GBP Pound symbol
												.Replace("€", string.Empty)//Remove Euro Symbol
												.Replace("¥", string.Empty));//Remove Chinese Yen symbol
			}
			return newTable;
		}
	}
}

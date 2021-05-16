using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using NCalc;
using Squirrel.Cleansing;
using TableAPI;

// ReSharper disable once CheckNamespace
namespace Squirrel
{
	
	/// <summary>
	/// The alignment for the pretty dump
	/// </summary>
	public enum Alignment 
	{ 
		/// <summary>
		/// Alligns the content to left
		/// </summary>
		Left,
		/// <summary>
		/// Alligns the content to right
		/// </summary>
		Right 
	}
	/// <summary>
	/// Sorting Direction. This enum is used with Sorting methods SortBy and SortInThisOrder
	/// </summary>
	public enum SortDirection
	{
		/// <summary>
		/// To be used when sorting in the ascending order
		/// </summary>
		Ascending,
		/// <summary>
		/// To be used when to be sorting in the descending order
		/// </summary>
		Descending 
	}
	/// <summary>
	/// The Algorithm to use for outlier detection
	/// </summary>
	public enum OutlierDetectionAlgorithm 
	{
		/// <summary>
		/// Inter Quantile Range
		/// </summary>
		IQR_Interval, 
		/// <summary>
		/// Z Score
		/// </summary>
		Z_Score 
	}
	/// <summary>
	/// Method to be used for aggregation/consolidation
	/// </summary>
	public enum AggregationMethod 
	{
		/// <summary>
		/// Summation of the sequence
		/// </summary>
		[Description("by summing")]
		Sum, 
		/// <summary>
		/// Average of the sequence
		/// </summary>
		[Description("by average,by mean")]        
		Average, 
		/// <summary>
		/// Maximum value of the sequence
		/// </summary>
		[Description("by max,by maximum value")]
		Max,
		/// <summary>
		/// Minimum value of the sequence
		/// </summary>
		[Description("by min,by minimum value")]
		Min, 
		/// <summary>
		/// Total count of the sequence
		/// </summary>
		[Description("by number of entries,by count")]
		Count,
		/// <summary>
		/// Standard deviation of the sequence      
		/// </summary>
		[Description("by standard deviation")]        
		StandardDeviation, 
		/// <summary>
		/// Calculates the variance of the sequence 
		/// </summary>
		[Description("by variance")]
		Variance, 
		/// <summary>
		/// Number of instance above average
		/// </summary>
		[Description("more than average")]
		AboveAverageCount,  
		/// <summary>
		/// Number of instances which are below the average value
		/// </summary>
		[Description("less than average")]
		BelowAverageCount,
		/// <summary>
		/// Number of instances that has the value equal to average
		/// </summary>
		[Description("just average,average")]
		AverageCount,
		/// <summary>
		/// Measures the skewness of the given sequence
		/// </summary>
		[Description("skewed")]
		Skew, 
		/// <summary>
		/// Measures the Kurtosis of the given sequence
		/// </summary>
		[Description("kurtosis")]       
		Kurtosis, 
		/// <summary>
		/// Measures the range of values of the given sequence.
		/// </summary>
		[Description("by range")]
		Range 
	}


	/// <summary>
	/// The Table class. This is the represenation of the ubiquitous Table structure. 
	/// </summary>
	public class Table
	{   
		private HashSet<string> _columnHeaders = new HashSet<string>();
		private List<Dictionary<string, string>> _rows = new List<Dictionary<string, string>>();
		/// <summary>
		/// Marks whether the missing values have been handled or not for a given Table instance
		/// </summary>
		public bool MissingValueHandled { get; set; }
		/// <summary>
		/// Each table can be given a name.
		/// </summary>
		public string Name { get; set; }
		#region Filtering
		/// <summary>
		/// Returns the percentage of the value "value" from all the values of the given column
		/// </summary>
		/// <param name="column">The column for which the percentage has to be obtained</param>
		/// <param name="value">The value for which the percentage has to be obtained </param>
		/// <returns>Percentage value of how many times the given value "value" appears in the column "column"</returns>
		/// <example>
		/// //Find how many times the value "0" appears in the "Survived" column in titanic dataset
		/// titanic.GetPercentage("Survived","0");
		/// </example>
		/// <remarks>See this example https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-06.md
		/// This will show you how to use this method.</remarks>
		public double GetPercentage(string column,string value)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(column);
				
		   
			if(value == null)
			{
				throw new ArgumentNullException(nameof(value), "The value provided to search is null");
			}
			try
			{
				return Convert.ToDouble(ValuesOf(column).Count(z => z == value))
					   /Convert.ToDouble(ValuesOf(column).Count);
			}
			catch (DivideByZeroException ex)
			{
				throw new DivideByZeroException(
					$"There was no value for the given colmun. Thus a divided by zero exception occured." + Environment.NewLine +
					  ex.StackTrace);
			}
		}

		/// <summary>
		/// Basic filtering based on the given predicate.
		/// </summary>
		/// <param name="predicate">The predicate takes a row and returns a bool.</param>
		/// <returns>The result table with filtered values</returns>
		/// <example>
		///Finds all iris flowers where the SepalWidth is more than 3.0
		///Table filtered = iris.Filter(x =&gt; Convert.ToDouble(x["SepalWidth"]) &gt; 3.0);
		///</example>
		public Table Filter(Func<Dictionary<string, string>, bool> predicate)
		{
			this.ThrowIfTableIsNull();
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate), "The predicate provided is null");
			}
			var result = new Table();
			result.Rows.AddRange(Rows.Where(predicate.Invoke));
			return result;
		}

		/// <summary>
		/// Finding a value by regular expression
		/// </summary>
		/// <param name="column">The column where the values has to be sought</param>
		/// <param name="regexPattern">The regex pattern</param>
		/// <returns>A table filled with rows that has a column with matching value.</returns>
		/// <example>
		/// //Returns a table which matches different spellings of my name 
		/// //Sometimes people write my name as "Sudipto". 
		/// //So the following call will return all the rows that has either "Sudipta" or "Sudipto" in the "Name" column.
		/// Table filtered = tab.FilterByRegex("Name","Sudipt[a|o]");
		/// </example>
		public Table FilterByRegex(string column, string regexPattern)
		{

			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(column);

			if (regexPattern == null)
			{
				throw new ArgumentNullException (nameof(regexPattern),
					 "The regular expression pattern provided is null");
			}
			var filteredTable = new Table();
			foreach (var row in _rows)
			{
				try
				{
					if (Regex.IsMatch(row[column], regexPattern))
						filteredTable.AddRow(row);
				}
				catch(KeyNotFoundException ex)
				{
					throw new KeyNotFoundException("The provided column name [" + column + "] doesn't exist." +
												   Environment.NewLine + ex.Message +  Environment.NewLine + ex.StackTrace);
				}
			}
			return filteredTable;
	  
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldSearchValuesMap"></param>
		/// <returns></returns>
		public Table Filter(Dictionary<string,List<string>> fieldSearchValuesMap)
		{

			this.ThrowIfTableIsNull();
			if(fieldSearchValuesMap == null)
			{
				throw new ArgumentNullException(nameof(fieldSearchValuesMap), 
					"No value is provided to filter on");
			}
			var result = this;
			foreach (var key in fieldSearchValuesMap.Keys)
			{
				try
				{
					result = result.Filter(key, fieldSearchValuesMap[key].ToArray());
				}
				catch(KeyNotFoundException ex)//If key "key" is not found in fieldSearchValueMap 
				{
					throw;
				}
			}
			return result;
		}
		/// <summary>
		/// Finds all the matching rows from the source table
		/// </summary>
		/// <param name="fieldSearchValueMap">Key value pair by which the filtering will be performed</param>
		/// <returns>A table with all the filtered rows. If no matching row is found, a table with 0 row is returned
		/// </returns>
		public Table Filter(Dictionary<string, string> fieldSearchValueMap)
		{
			this.ThrowIfTableIsNull();
			var filteredTable = new Table();
			for (int i = 0; i < _rows.Count; i++)
			{
				bool matching = false;
				foreach (string key in _rows[i].Keys)
				{
					if (fieldSearchValueMap.ContainsKey(key) && _rows[i][key] == fieldSearchValueMap[key])
						matching = true;
					else
					{
						//One column didn't match.
						matching = false;
						break;
					}
				}
				if (matching)
					filteredTable._rows.Add(_rows[i]);
			}
			return filteredTable;
		}
		/// <summary>
		/// Filters values from a given column
		/// </summary>
		/// <param name="column">The column to run the filter on</param>
		/// <param name="values">Values to match against</param>
		/// <returns></returns>
		public Table Filter(string column, params string[] values)
		{
			var filteredTable = new Table();
			for (int i = 0; i < _rows.Count; i++)
			{
				if (values.Any(t => _rows[i][column].Equals(t)))
					filteredTable.AddRow(_rows[i]);
			}
			return filteredTable;
		}
	   
		/// <summary>
		/// Runs SQL Query against the Table
		/// </summary>
		/// <param name="sql">The query in the form Select * from [Table] Where A = 'a'</param>
		public Table RunSQLQuery(string sql)
		{
			var resultTable = new Table();
			return resultTable;
			//HashSet<string> columns = ColumnHeaders;
			//StreamWriter sw = new StreamWriter(@"temp.csv");
			//sw.WriteLine(columns.Aggregate((a, b) => a + "," + b));
			//string numberRegex = "[0-9]+.*[0-9]+";
			//for (int i = 0; i < _rows.Count; i++)
			//{
			//	foreach (string col in columns)
			//	{
			//		if (Regex.IsMatch(_rows[i][col], numberRegex))
			//		{
			//			sw.Write(_rows[i][col] + ",");
			//		}
			//		else
			//		{
			//			sw.Write("\"" + _rows[i][col] + "\",");
			//		}
			//	}
			//	sw.WriteLine();
			//}
			//sw.Close();
			//string strCSVConnString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source='temp.csv';Extended Properties='text;HDR=YES;'";
			//sql = sql.Replace("[Table]", @"temp.csv");

			//OleDbDataAdapter oleda = new OleDbDataAdapter(sql, strCSVConnString);
			//DataTable dataTable = new DataTable();
			//oleda.Fill(dataTable);


			//var resultTable = new Table();
			//for (int i = 0; i < dataTable.Rows.Count; i++)
			//{
			//	Dictionary<string, string> currentRow = new Dictionary<string, string>();
			//	foreach (string column in columns)
			//	{
			//		try
			//		{
			//			currentRow.Add(column, dataTable.Rows[i][column].ToString());
			//		}
			//		catch {
			//		}
			//	}
			//	resultTable.AddRow(currentRow);
			//}
			//return resultTable;
		}

		#endregion

		#region Sorting 
		/// <summary>
		/// Sort by a custom ordering of elements. 
		/// </summary>
		/// <param name="columnName">The column to sort</param>
		/// <param name="inThisOrder">The custom ordering of elements</param>
		/// <param name="how">Ascending or Descending</param>
		/// <returns>A sorted table with the custom ordering.</returns>
		public OrderedTable SortInThisOrder(string columnName,List<string> inThisOrder, 
											SortDirection how = SortDirection.Ascending)
		{             
			CustomComparers.SortInThisOrder = inThisOrder;

			var sortedTable = new OrderedTable();

			var sorter = new CustomComparers.CustomSorter();

			if (how == SortDirection.Ascending)
				sortedTable._rows = _rows.OrderBy(m => m[columnName], sorter).ToList();
			else
				sortedTable._rows = _rows.OrderByDescending(m => m[columnName], sorter).ToList();

			return sortedTable;
		}
		
		/// <summary>
		/// Sorts the current table by the given column. 
		/// </summary>
		/// <param name="columnName">The column to sorty by</param>
		/// <param name="smartSort"></param>
		/// <param name="smartDefaultFile"></param>
		/// <param name="how">Whether the sorting has to be done in ascending or in descending order or not.</param>
		/// <returns>An ordered table where rows are ordered by the given column.</returns>
		/// <example>var sortedTable = tab.SortBy("Age");//Sorts the table by the column "Age"</example>
		[Description("sort by")]
		public OrderedTable SortBy(string columnName, bool smartSort = false, string smartDefaultFile = @"SmartDefaults.xml", SortDirection how = SortDirection.Ascending)
		{
			var sortedTable = new OrderedTable();
			
			CustomComparers.NumericComparer comp = new CustomComparers.NumericComparer();
			CustomComparers.DateComparer dateComp = new CustomComparers.DateComparer();
			CustomComparers.CurrencyCustomSorter currencyComp = new CustomComparers.CurrencyCustomSorter();

			if (smartSort)
			{
				SmartDefaults.Instance.GetSmartDefaultValues(smartDefaultFile);//Populate and keep it ready once. 

				KeyValuePair<bool, string> matchingEntriesIfAny = SmartDefaults.Instance.DoesMatchingEntryExist(ValuesOf(columnName));

				//Smart Sort the day and month names 
				if (matchingEntriesIfAny.Key)
				{
					List<string> sortingOrder = SmartDefaults.DefaultValues[matchingEntriesIfAny.Value];
					return SortInThisOrder(columnName, sortingOrder, how);
				}
			}
			

			string dateRegex = @"^[0-3]?[0-9]/[0-3]?[0-9]/(?:[0-9]{2})?[0-9]{2}$";//matches date with regular expression.
			string numericRegex = @"^-?[0-9]\d*(\.\d+)?$";//matches decimals with negative          
			string currencyRegex = @"[$£€¥][0-9]\d*(\.\d+)?$";//matches currencies
			
			bool isDateColumn = ValuesOf(columnName).All(m=>Regex.IsMatch(m,dateRegex));
			bool isNumericColumn = ValuesOf(columnName).All(m => Regex.IsMatch(m, numericRegex));
			bool isCurrencyColumn = ValuesOf(columnName).All(m => Regex.IsMatch(m, currencyRegex));

			//The column we are trying to sort has all numeric values
			if (isNumericColumn)
			{
				if (how == SortDirection.Ascending)
					sortedTable._rows = _rows.OrderBy(m => m[columnName], comp).ToList();
				else
					sortedTable._rows = _rows.OrderByDescending(m => m[columnName], comp).ToList();
			}
			if (isCurrencyColumn)
			{
				if (how == SortDirection.Ascending)
					sortedTable._rows = _rows.OrderBy(m => m[columnName], currencyComp).ToList();
				else
					sortedTable._rows = _rows.OrderByDescending(m => m[columnName], currencyComp).ToList();
			}
			//The column we are trying to has all date values
			else if (isDateColumn)
			{
				if (how == SortDirection.Ascending)
					sortedTable._rows = _rows.OrderBy(m => m[columnName], dateComp).ToList();
				else
					sortedTable._rows = _rows.OrderByDescending(m => m[columnName], dateComp).ToList();
			}

			//the column has all string values. So we are assuming that default "alphabetic" sort will do.
			else if(!isNumericColumn && !isDateColumn && !isCurrencyColumn)
			{
				if (how == SortDirection.Ascending)
					sortedTable._rows = _rows.OrderBy(m => m[columnName]).ToList();
				else
					sortedTable._rows = _rows.OrderByDescending(m => m[columnName]).ToList();
			}
			return sortedTable;
		}
		#endregion 

		#region Table Manipulation/Programmatic Column and Row Insertion
		
		/// <summary>
		/// Modifies column name
		/// </summary>
		/// <param name="oldName">Old/Current name of the column</param>
		/// <param name="newName">New/Proposed name of the column</param>
		/// <returns>A table with modified column header names</returns>
		/// <example>
		/// //Changes the name of the column "First_Name" to "FirstName"
		/// Table modCols = tab.ModifyColumnName("First_Name","FirstName");
		/// </example>
		[Description("Change column name")]
		public Table ModifyColumnName(string oldName, string newName)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(oldName);
			
			var newRows = new List<Dictionary<string, string>>();
			foreach (var row in _rows)
			{
				var temp = row;
				var val = temp[oldName];
				temp.Remove(oldName);
				temp.Add(newName, val);
				newRows.Add(temp);
			}
			var modTab = new Table {_rows = newRows};
			return modTab;
		}

		/// <summary>
		/// Returns all the values of the given column 
		/// </summary>
		/// <param name="columnName">column for which we want to extract all values</param>
		/// <returns>All the values of the given column as a list of string.</returns>
		/// <example>
		/// //List of string codes will hold all the values of
		/// List&lt;string&gt; codes  = tab.ValuesOf(&quot;Code&quot;);
        /// </example>
		public List<string> ValuesOf(string columnName)
		{

			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);

			return _rows.Select(t => t[columnName]).ToList();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="columnName"></param>
		/// <param name="transformer"></param>
		/// <returns></returns>
		public List<T> ValuesOf<T>(string columnName,Func<string,T> transformer)
        {
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);
			
			return _rows.Select(t => transformer(t[columnName]))
					   .ToList();
		}


  

		/// <summary>
		/// Returns only the numeric columns
		/// </summary>
		public List<string> NumericColumns
		{
			get
			{
				var numericColumns = new List<string> ();
				string numericRegex = @"^-?[0-9]\d*(\.\d+)?$";//matches decimals with negative 
				foreach (var header in ColumnHeaders)
				{
					if (Regex.Match(header, numericRegex).Success)
						numericColumns.Add(header);
				}
				return numericColumns;
			}
		}

		/// <summary>
		/// Returns names of the columns of the table
		/// </summary>
		/// <seealso cref="Table.NumericColumns"/>
		public HashSet<string> ColumnHeaders
		{
			get
			{
				try
				{
					return new HashSet<string>(_rows.First().Select(r => r.Key));
				}
				catch
				{
					return new HashSet<string>();
				}
			}
		}

		/// <summary>
		/// Adds a new row to the table
		/// </summary>
		/// <param name="row">The new row</param>
		public void AddRow(Dictionary<string, string> row)
		{
			this.ThrowIfTableIsNull();
			_rows.Add(row);
		}
		/// <summary>
		/// Add rows by short hand of common programmatic rules like "columnName++" etc
		/// </summary>
		/// <param name="formula">Age[1]=Age[0]+1 is same as "Age++"</param>
		/// <param name="count">How many rows are needed.</param>
		/// <example>
		/// //Increment age by 24 times
		/// tab.AddRowsByShortHand("Age++",24); 
		/// </example>
		/// <seealso cref="Table.AddRows"/>
		public void AddRowsByShortHand(string formula, int count)
		{          
			//++,--,+=,-=,*=,/=
			if(string.IsNullOrWhiteSpace(formula))
				throw new ArgumentNullException(nameof(formula) + " is null");
			if(count <= 0)
				throw new ArgumentOutOfRangeException($"Value for {count} should be non-zero");

			string columnName = Regex.Match(formula, "[a-zA-Z]+").Value;
			bool plusPlus = formula.EndsWith("++");
			bool minusMinus = formula.EndsWith("--");
			bool plusEqual = formula.Contains("+=");
			bool minusEqual = formula.Contains("-=");
			bool multiplyEqual = formula.Contains("*=");
			bool divideEqual = formula.Contains("/=");

			if (plusPlus)
				AddRows(columnName + "[1]=" + columnName + "[0]+1",count);
			if (minusMinus)
				AddRows(columnName + "[1]=" + columnName + "[0]-1", count);
			if(formula.Contains("="))
			{
				//a-=2
				decimal number = Convert.ToDecimal(formula.Substring(formula.IndexOf('=')+1));
				if(plusEqual)
				{
					AddRows(columnName + "[1]=" + columnName + "[0]+" + number, count);
				}
				if (minusEqual)
				{
					AddRows(columnName + "[1]=" + columnName + "[0]-" + number, count);
				}
				if (multiplyEqual)
				{
					AddRows(columnName + "[1]=" + columnName + "[0]*" + number, count);
				}
				if (divideEqual)
				{
					AddRows(columnName + "[1]=" + columnName + "[0]/" + number, count);
				}
			}           
		}
		/// <summary>
		/// Adds values to a column
		/// </summary>
		/// <param name="formula">The formula to create new values</param>
		/// <param name="count">How many rows to create</param>
		public void AddRows(string formula, int count)
		{
			if (String.IsNullOrWhiteSpace(formula))
				throw new ArgumentNullException(nameof(formula) + " is null");
			if (count <= 0)
				throw new ArgumentOutOfRangeException($"Value for {count} should be non-zero");
			string pattern = "[[0-9]+]";
			string[] tokens = formula.Split('=');
			string[] columnsPresent = Regex.Matches(tokens[1], "[a-zA-Z]+[[0-9]+]")
										   .Cast<Match>()
										   .Select(z => z.Value.Substring(0,z.Value.IndexOf('[')))
										   .ToArray();
			string columnNameLeft = formula.Trim().Substring(0, formula.IndexOf('['));
			int index = Convert.ToInt32(Regex.Match(tokens[0], pattern).Value.Replace("[", string.Empty).Replace("]", string.Empty));
			int start = Convert.ToInt32(Regex.Match(tokens[1], pattern).Value.Replace("[", string.Empty).Replace("]", string.Empty));

			int startCopy = start;
			int j = 0;
			for (int i = start; j < count; i++)
			{
				string statement = tokens[1];
				foreach (var columnName in columnsPresent)
				{
					statement = statement.Replace(columnName + "[" + startCopy + "]", _rows[i][columnName]);
				}
				Expression exp = new Expression(statement);
				if (RowCount <= index)
				{
					var temp = new Dictionary<string, string> {{columnNameLeft, exp.Evaluate().ToString()}};
					_rows.Add(temp);
				}
				_rows[index][columnNameLeft] = exp.Evaluate().ToString();

				
				j++;
				index++;
			}
		}
		/// <summary>
		/// Returns the number of rows in the table
		/// </summary>
		public int RowCount
		{
			
			get
			{
				this.ThrowIfTableIsNull();
				try
				{
					return _rows.Count;
				}
				catch(NullReferenceException ex)
				{
					throw new NullReferenceException("Rows of the Table instance is null.");
				}
			}

		}
		/// <summary>
		/// Read-only property for getting rows of the table
		/// </summary>
		public List<Dictionary<string, string>> Rows => _rows;

		/// <summary>
		/// Extracts words from values of a given column 
		/// and then uses these extracted values to create a new column
		/// </summary>
		/// <param name="columnName">New</param>
		/// <param name="fromColumnName">Name of the column from which values are to be extracted</param>
		/// <param name="pattern">The regular expression pattern to use to extract values from the column.</param>
		/// <example></example>
		///<exception cref="ArgumentNullException"></exception>
		
		public void ExtractAndAddAsColumn(string columnName, string fromColumnName, string pattern)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(fromColumnName);
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentNullException($"The name of the column {columnName} is either null or empty or whitespace.");
			if (string.IsNullOrWhiteSpace(pattern))
				throw new ArgumentNullException($"The regular expression pattern provided is to extract values is null or empty or whitespace");
			var values = ValuesOf(fromColumnName).Select(val => Regex.Match(val, pattern).Value).ToList();
			AddColumn(columnName, values);
		}
		/// <summary>
		/// This method let's us add column based on string column values.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="format"></param>
		public void AddColumn(string columnName, string format)
		{
			//[City]([Edition])-> 
			var columns = Regex.Matches(format, "[[a-zA-Z0-9 _-]+]").Cast<Match>()
																	//Dropping brackets [ and ]   
																	.Select(t => t.Value.Substring(1, t.Value.Length - 2))
																	.ToList();
			format = format.Replace("[", string.Empty).Replace("]", string.Empty);
			List<string> values = new List<string>();
			for (int i = 0; i < this.RowCount; i++)
			{
				Dictionary<string, string> thisRow = new Dictionary<string, string>();
				foreach (var col in columns)
				{
					thisRow.Add(col, this[i][col]);
				}
				string temp = format;
				foreach (var key in thisRow.Keys)
				{
					temp = temp.Replace(key, thisRow[key]);
				}
				values.Add(temp);
			}

			AddColumn(columnName, values);
		}
		/// <summary>
		/// Adds a column for which value gets calculated from a given formula.
		/// </summary>
		/// <param name="columnName">Name of the column</param>
		/// <param name="formula">Formula to calculate values of the new column</param>
		/// <param name="decimalDigits"></param>
		public Table AddColumn(string columnName, string formula, int decimalDigits)
		{
			this.ThrowIfTableIsNull();
			//this.ThrowIfColumnsAreNotPresentInTable(columnName);
			if (columnName == null)
				throw new ArgumentNullException($"{nameof(columnName)} is null ");
			if (formula == null)
				throw new ArgumentNullException($"nameof(formula) is null");
			if (decimalDigits < 0)
				throw new ArgumentNullException($"{nameof(decimalDigits)} is null");
			
			string copyFormula = formula;
		   
			string[] columns = formula.Split(new[] { '+', '-', '*', '/', '(', ')',' ' },StringSplitOptions.RemoveEmptyEntries);

			columns = columns.Select(t => t.Replace("[", string.Empty).Replace("]", string.Empty))
							 .Intersect(ColumnHeaders)
							 .Select(z=>"["+z+"]")
							 .ToArray();

			for (int i = 0; i < RowCount; i++)
			{
				formula = copyFormula;
				foreach (string column in columns)
				{
					try
					{
						formula = formula.Replace(column, _rows[i][column.Replace("[", string.Empty)
										 .Replace("]", string.Empty)]);
					}
					
					//catch(KeyNotFoundException ex)//Occurs when the column name is not found
					//{
				 //       throw new KeyNotFoundException(nameof())                        
				//	}
					catch (Exception ex)
					{
						return this;
					}
				}
		 

				_rows[i].Add(columnName, Math.Round(Convert.ToDecimal(new Expression(formula).Evaluate().ToString()),
								  decimalDigits).ToString(CultureInfo.InvariantCulture));         
			}
			return this;
		}
		
		
		/// <summary>
		/// Create a column from given values
		/// </summary>
		/// <param name="columnName">column name</param>
		/// <param name="values">given values</param>
		/// <example></example>
		/// <remarks>This is an in-place algorithm that modifies the current table.</remarks>
		public void AddColumn(string columnName, List<string> values)
		{
			this.ThrowIfTableIsNull();
			
			if (_rows.Count == 0)
			{
				for (int i = 0; i < values.Count; i++)
				{
					_rows.Add(new Dictionary<string, string>());
				}
			}
			for (int i = 0; i < _rows.Count; i++)
				_rows[i].Add(columnName, values[i]);
		}

	 
		/// <summary>
		/// Remove the given column from the table
		/// </summary>
		/// <param name="columnName">column to remove </param>
		public void RemoveColumn(string columnName)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);
			
			for (int i = 0; i < RowCount; i++)
				_rows[i].Remove(columnName);
		}
	   
		/// <summary>
		/// Indexer of the table.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Dictionary<string, string> this[int index]
		{
			get
			{
				if (index >= _rows.Count)
					throw new ArgumentOutOfRangeException($"{nameof(index)} is out of range");
				return _rows[index];
			}
			set
			{
				if (index >= _rows.Count)
					throw new ArgumentOutOfRangeException($"{nameof(index)} is out of range");
				_rows[index] = value;
			}
		}
	  
		/// <summary>
		/// Indexing over column.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public List<string> this[string columnName]
		{
			get
			{
				this.ThrowIfTableIsNull();
				this.ThrowIfColumnsAreNotPresentInTable(columnName);
				
				return ValuesOf(columnName);
			}
			set
			{
				AddColumn(columnName, value);
			}
		}
		/// <summary>
		/// Returns value for a given column at a given index
		/// </summary>
		/// <param name="columnName">The name of the column</param>
		/// <param name="index">The index</param>
		/// <returns>The value of the given column at the given index as a string</returns>
		/// <example>string stockValue = stocks["MSFT",4];</example>
		public string this[string columnName, int index]
		{
			get
			{
				this.ThrowIfTableIsNull();
				this.ThrowIfColumnsAreNotPresentInTable(columnName);
				if (index >= _rows.Count)
					throw new ArgumentOutOfRangeException($"{nameof(index)} is out of range");

				return this[columnName][index];
			}
		}
		/// <summary>
		/// A generic folder method. This helps to generate a cumulative fold
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="how"></param>
		/// <returns></returns>
		/// 
		[Description("Fold cumulatively")]
		public Table CumulativeFold(string columnName, AggregationMethod how = AggregationMethod.Sum)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);

			var result = new Table();
			var allNumericColumns = new List<string>();
			allNumericColumns.AddRange(ColumnHeaders.Where(m => ValuesOf(m).All(z => Char.IsNumber(z, 0))));

			for (int i = 0; i < RowCount; i++)
			{
				var currentRow = new Dictionary<string, string>();
				currentRow.Add(columnName, this[i][columnName]);
				foreach (string col in allNumericColumns)
				{
					if (col != columnName)
					{
						switch(how)
						{
							case AggregationMethod.Sum:
								currentRow.Add(col, this[col].Take(i + 1).Select(s => Convert.ToDecimal(s)).Sum().ToString());
								break;
							case AggregationMethod.Average:
								currentRow.Add(col, this[col].Take(i + 1).Select(s => Convert.ToDecimal(s)).Average().ToString());
								break;
							case AggregationMethod.Max:
								currentRow.Add(col, this[col].Take(i + 1).Select(s => Convert.ToDecimal(s)).Max().ToString());
								break;
							case AggregationMethod.Min:
								currentRow.Add(col, this[col].Take(i + 1).Select(s => Convert.ToDecimal(s)).Min().ToString());
								break;
						}
					}
				}
				result.AddRow(currentRow);
			}
			return result;
		}
		
		/// <summary>
		/// Change rows as columns
		/// </summary>
		/// <returns></returns>
		public Table Transpose()
		{
			//This table 
			//A 3 5 2 1 0  
			//B 2 3 3 1 1
			//C 4 5 2 3 4

			//Will become this if transposed.
			//A B C
			//3 2 4
			//5 3 3
			//2 3 2
			//1 1 3
			//0 1 4

			//So in the process of transpose values of the first column
			//becomes the headers and value of each subsequent columns become rows 
			//TO DO. -> This code doesn't work yet. 
			Table transposed = new Table();
			this[ColumnHeaders.ElementAt(0)].ToList().ForEach( m => transposed.ColumnHeaders.Add(m));
			for (int i = 1; i < ColumnHeaders.Count; i++)
			{
				Dictionary<string, string> row = new Dictionary<string, string>();
				this[ColumnHeaders.ElementAt(i)].ToList().ForEach(n => row.Add(ColumnHeaders.ElementAt(i), n));
				transposed.Rows.Add(row);
			}
			return transposed;

		}
		/// <summary>
		/// Rounds off all the numeric columns to given number of digits
		/// </summary>
		/// <param name="decimalDigits"></param>
		/// <returns></returns>
		/// 
		[Description("Round off to,Round the digits to")]
		public Table RoundOffTo(int decimalDigits)
		{          
			if (decimalDigits > 10 || decimalDigits < 0)
				throw new ArgumentOutOfRangeException(nameof(decimalDigits), "RoundOffTo() requires a non negative number less than 10");
			for (int r = 0; r < RowCount; r++)
			{
				foreach (string col in ColumnHeaders)
				{
					try
					{
						if (Regex.IsMatch(this[r][col], @"^-?[0-9]\d*(\.\d+)?$"))
							this[r][col] = Math.Round(Convert.ToDecimal(this[r][col]), decimalDigits).ToString();
					}
					catch (KeyNotFoundException ex)
					{
						throw new KeyNotFoundException(nameof(col) +  "The following column wasn't found");
					}
					catch (Exception ex)
					{

					}
				}
			}
			return this;
		}
		/// <summary>
		/// Rounds off decimal digits for each column as per the given count.
		/// Sometimes each column might need to show a certain values after decimal.
		/// </summary>
		/// <param name="decimalDigitsForEachColumn">string representation of the precision required for each column. 
		/// </param>
		/// <example>salesReport.RoundOffTo("Q1(4),Q2(3),AverageSales(4)").PrettyDump();//Assume that there are these columns Q1,Q2 and AverageSales
		/// //and you want to round off Q1 to 4 digits, Q2 to 3 digits and AverageSales to 4 digits after decimal.</example>
		/// <returns>A table with specified number of digits after decimal for each of the column as mentioned.</returns>
		public Table RoundOffTo(string decimalDigitsForEachColumn)
		{
			//Q1(4),Q4(5),Q2(3)
			Dictionary<string, int> roundOffMap = new Dictionary<string, int>();
			string[] tokens = decimalDigitsForEachColumn.Split(',');
			foreach (string tok in tokens)
			{
				string[] internalTokens = tok.Split('(', ')');
				roundOffMap.Add(internalTokens[0], Convert.ToInt16(internalTokens[1]));
			}
			foreach (string col in ColumnHeaders)
				for (int r = 0; r < RowCount; r++)
					if (Regex.IsMatch(this[r][col], @"^-?[0-9]\d*(\.\d+)?$"))
						this[r][col] = Math.Round(Convert.ToDecimal(this[r][col]),  roundOffMap[col]).ToString();

			
			return this;
		}
		/// <summary>
		/// Aggregate values of given columns as per the given aggregation method.
		/// </summary>
		/// <param name="newColumnName"></param>
		/// <param name="columns">The columns for which values has to be aggregated</param>
		/// <param name="how">The aggregation method</param>
		/// <returns>A table with columns values aggregated.</returns>
		public Table AggregateColumns(string newColumnName, AggregationMethod how = AggregationMethod.Average, params string[] columns)
		{
			//Zone, Jan, Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec
			//East,1,2,3,4,5,5,6,7,8,9,10,11,12
			//West,2,3,4,2,12,3,4,2,2,3,4,22

			List<decimal> newValues = new List<decimal>();
			for (int i = 0; i < RowCount; i++)
			{
				
				List<decimal> values = new List<decimal> ();
				foreach (string col in columns)
					values.Add(Convert.ToDecimal(this[col][i]));
				switch (how)
				{
					case AggregationMethod.Average:
						newValues.Add(values.Average());
						break;
					case AggregationMethod.Sum:
						newValues.Add(values.Sum());
						break;
						
				}
			}
			columns.ToList().ForEach(RemoveColumn);
			AddColumn(newColumnName, newValues
									   .Select(t => t.ToString(provider: CultureInfo.InvariantCulture)).ToList());
			return this;
		}
		
		
		/// <summary>
		/// Aggregates values of a column
		/// This can also be achieved by CumulativeFold () method 
		/// </summary>
		/// <param name="columnName">Aggregate values for each distinct value in this column</param>
		/// <param name="how">The aggregation scheme to be used</param>
		/// <returns>A flattened table</returns>
		/// <example>var salesPerMonth = allSales.Aggregate("Month");</example>
		/// <example>var avgSalesPerMonth = allSales.Aggregate("Month",AggregationMethod.Average);</example>
		public Table Aggregate(string columnName, AggregationMethod how = AggregationMethod.Sum)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);
			List<string> allDistinctValues = ValuesOf(columnName)
											 .Distinct()
											 .ToList();
			

			List<string> allNumericColumns = new List<string>();
			
			allNumericColumns.AddRange(ColumnHeaders.Where(col =>  ValuesOf(col).Where(z => z.Trim().Length >0)
																				.All(m => char.IsNumber(m,0))));           

			var aggregatedTable = new Table();
			foreach (string value in allDistinctValues)
			{
				var aggRow = new Dictionary<string, string> {{columnName, value}};

				var tempTable = Filter(columnName,value);
				foreach (string col in allNumericColumns)
				{
					if (col == columnName) continue;
					if (how == AggregationMethod.Count)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).Count().ToString());
					if (how == AggregationMethod.Max)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).Max()
							.ToString(CultureInfo.InvariantCulture));
					if (how == AggregationMethod.Min)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).Min()
							.ToString(CultureInfo.InvariantCulture));
					if (how == AggregationMethod.Sum)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).Sum()
							.ToString(CultureInfo.InvariantCulture));
					if (how == AggregationMethod.Average)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).Average()
							.ToString(CultureInfo.InvariantCulture));
					if (how == AggregationMethod.AboveAverageCount)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).AboveAverageCount()
							.ToString());
					if(how == AggregationMethod.BelowAverageCount)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).BelowAverageCount()
							.ToString());
					if (how == AggregationMethod.Range)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDecimal).Range()
							.ToString(CultureInfo.InvariantCulture));
					if (how == AggregationMethod.Kurtosis)
						aggRow.Add(col, tempTable[col].Select(Convert.ToDouble).ToList().Kurtosis()
							.ToString(CultureInfo.InvariantCulture));
				}                
				aggregatedTable.AddRow(aggRow);
			}
			return aggregatedTable;
		}
		/// <summary>
		/// Creates an aggregated view of the given table. 
		/// </summary>
		/// <param name="columnName">The column which will be the key for Aggregation</param>
		/// <param name="how">The method which will be used to perform the aggregation</param>
		/// <returns>A table with aggregated values</returns>
		/// <remarks></remarks>
		public Table Aggregate(string columnName, Func<List<string>, string> how)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);
			if(how == null)
				throw new ArgumentNullException($"The scheme to aggregate the table provided in {nameof(how)} is null");
			List<string> allDistinctValues = ValuesOf(columnName)
											 .Distinct()
											 .ToList();
			

			List<string> allNumericColumns = new List<string>();
			allNumericColumns.AddRange(ColumnHeaders.Where(col => ValuesOf(col).All(m => Char.IsNumber(m, 0))));           

			Table aggregatedTable = new Table();
			foreach (string value in allDistinctValues)
			{
				Dictionary<string, string> aggRow = new Dictionary<string, string>();
				aggRow.Add(columnName, value);
				
				Table tempTable = Filter(columnName, value);
				foreach (string col in allNumericColumns.Skip(1))
				{
					if (col != columnName)
					{
						aggRow.Add(col, how(tempTable[col]));
					}
				}
				aggregatedTable.AddRow(aggRow);

			}
			return aggregatedTable;
		}

		 
		/// <summary>
		/// Generates the histogram for the column from the table
		/// </summary>
		/// <param name="columnName">The column for which a histogram has to be created</param>
		/// <returns>A dictionary where keys represent the values and values represent the count of each such value</returns>
		/// 
		[Description("Frequency distribution, Show histogram,Histogram, Frequency Distribution")]
		public Dictionary<string, int> Histogram(string columnName)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);

			return SplitOn(columnName).ToDictionary(t => t.Key, t => t.Value.RowCount);
			
		}
		/// <summary>
		/// Splits a table on the distinct values of a given column
		/// </summary>
		/// <param name="columnName">The column; depending on values of which we want to perform the split</param>
		/// <returns>A mapping between values of the column and their corresponding table</returns>
		/// <github>
		/// SplitOn
		/// =======
		/// Splits a table on the distinct values of a given column.
		/// 
		/// </github>
		[Description("Split by,Split on,Break by,Partition by,Divide by")]
		public Dictionary<string, Table> SplitOn(string columnName)
		{
			this.ThrowIfTableIsNull();
			this.ThrowIfColumnsAreNotPresentInTable(columnName);

			var tables = new Dictionary<string, Table>();

            //Rows over

			foreach (var row in Rows)
			{
				if (!tables.ContainsKey(row[columnName]))
					tables.Add(row[columnName],
						 new Table { _rows = new List<Dictionary<string, string>> { row } });
				else
				{

					tables[row[columnName]].AddRow(row);
				}
			}
	
			return tables;
		}
		#endregion


		#region Set Operations
		/// <summary>
		/// Generation of a single table with merged columns.
		/// </summary>
		/// <param name="anotherTable">The other table</param>
		/// <param name="connectorColumn">The column based on which the index of the matching rows has to be found.
		/// The connector column has to be non numeric.</param>
		/// <returns>A Table with merged columns</returns>       
		/// <example>This is an example use case of this method.
		/// Table 1 Columns
		/// Name  | Age | Gender 
		/// Sam   | 23  | M
		/// Jane  | 19  | F
		/// Raskin| 14  | M

		/// Table 2 Columns
		/// Name | Course 
		/// Jane | C#
		/// Sam  | F#
		/// Raskin| Python


		/// Merged Columns in the resultant Table.
		/// Name | Age | Gender | Course
		/// Sam  | 23  | M      | F#
		/// Jane | 19  | F      | C#
		/// Raskin| 14 | M      | Python
		/// Table merged = t1.MergeByColumns(t2);//Uses the first column to perform the join
		/// Table mergedByName = t1.MergeByColumns(t2,"Name");//Uses the column "Name" to perform the join
		/// //This is how merge works
        /// 
		/// </example>
  

		
		
		public Table MergeByColumns(Table anotherTable, string connectorColumn = "Not Provided")
		{
			this.ThrowIfTableIsNull();
			anotherTable.ThrowIfTableIsNull();
			if (connectorColumn != "Not Provided") //A column name is provided
			{
				this.ThrowIfColumnsAreNotPresentInTable(connectorColumn);
				anotherTable.ThrowIfColumnsAreNotPresentInTable(connectorColumn);
			}
			if (connectorColumn == "Not Provided")
				connectorColumn = ColumnHeaders.First();

			Table mergedColumnsTable = new Table();

			ColumnHeaders.ToList().ForEach(m => mergedColumnsTable.AddColumn(m, ValuesOf(m)));
			
			anotherTable.ColumnHeaders.Where(t=>t!=connectorColumn)
					   .ToList().ForEach(m =>
									mergedColumnsTable.AddColumn(m, anotherTable.SortInThisOrder(connectorColumn, 
									ValuesOf(connectorColumn)).ValuesOf(m)));


			return mergedColumnsTable.SortBy(connectorColumn);
		}

		/// <summary>
		/// Merge two tables removing duplicate rows from the resultant table
		/// </summary>
		/// <param name="anotherTable">The other table</param>
		/// <param name="removeDups">Flag to set if we want to remove duplicate rows</param>
		/// <returns>A table with merged rows and duplicates removed; if indicated to do so.</returns>
		public Table Merge(Table anotherTable, bool removeDups = false)
		{
			this.ThrowIfTableIsNull();
			anotherTable.ThrowIfTableIsNull();

			var mergedTable = new Table();
			if (!ColumnHeaders.OrderBy(head => head).SequenceEqual(anotherTable.ColumnHeaders.OrderBy(head => head)))
				return this; //Merge is not possible'


			mergedTable._rows = _rows;

			mergedTable._rows.AddRange(anotherTable._rows);
			if (removeDups)
				return mergedTable.Distinct();

			return mergedTable;


		}

		/// <summary>
		/// Extracts those rows from the table which are not present in the another one.
		/// </summary>
		/// <param name="anotherTable">The other table with which we have to compare.</param>
		/// <returns>A new table with rows that are only available in the current table, not in the other one.</returns>
		/// <example>Table exclusive = t1.Exclusive(t2);//Find rows that are exclusively available in table "t1"</example>
		
		public Table Exclusive(Table anotherTable)
		{
			this.ThrowIfTableIsNull();
			anotherTable.ThrowIfTableIsNull();

			Table result = new Table ();
			for (int i = 0; i < RowCount; i++)
			{
				if(!anotherTable._rows.Any(r => IsSameRow(r,_rows[i])))
					result.AddRow(_rows[i]);
			}
			return result;
		}
		/// <summary>
		/// Finding common rows from two tables. Since it returns a table, there can be cascaded calls.
		/// </summary>
		/// <param name="anotherTable">The other table with which we want to find common rows</param>
		/// <returns>A table with only common rows</returns>
		/// <example>Table comRows = t1.Common(t2);//returns common rows that are available in "t1" and "t2"</example>
		#region Useful Utility Methods


		/// <summary>
		/// Returns a table with merged columns 
		/// </summary>
		/// <param name="newColumnName">The new column header</param>
		/// <param name="separator">The character separator to use between values of participating columns</param>
		/// <param name="columns">Columns to merge</param>
		/// <returns>Returns a table with the merged column</returns>
		/// <example>
		/// //Merges two columns "First_Name" and "Last_Name" 
		/// //and the new column is named "Full_Name"
		/// Table mergedColumns = t1.MergeColumns("Full_Name", ' ',"First_Name","Last_Name");</example>
		public Table MergeColumns(string newColumnName,
								  char separator = ' ',
								  bool removeColums = false,
								  params string[] columns)
		{
			var mergedValues = new List<string>();
			for (int i = 0; i < columns.Length - 1; i++)
				mergedValues = ValuesOf(columns[i])
							   .Zip(ValuesOf(columns[i + 1]), (a, b) => a + separator.ToString() + b)
							   .ToList();

			AddColumn(newColumnName, mergedValues);
			//merged!

			//If the user wants to remove the columns 
			//from which this column in generated then 
			if (removeColums)
				columns.ToList().ForEach(RemoveColumn);
			return this;
		}
		/// <summary>
		/// Returns a table with all the columns except those mentioned in parameters
		/// </summary>
		/// <param name="columns">List of column names</param>
		/// <returns>A table with all the columns except the mentioned ones</returns>
		/// <example>
		/// //Assuming that there is a Table instance called 
		/// //tab and it has a column "Sex" 
		/// //The following dumps the table to console except the "Sex" column.
		/// tab.Drop("Sex").PrettyDump();</example>
		[Description("Hide,Don't show")]
		public Table Drop(params string[] columns)
		{
			return Pick(ColumnHeaders.Except(columns).ToArray());
		}
		/// <summary>
		/// Returns a table with just the columns mentioned.
		/// </summary>
		/// <param name="columns">Name of the columns that we want to show.</param>
		/// <returns>A table with only the columns mentioned in the parameter </returns>
		/// <example>t.Pick("Name","Age")
		///           .PrettyDump();//dumps the table with only two columns</example>
		/// <seealso cref="Table.Drop"/>          
		[Description("Pick only,Show only,Show just,Just,Show me only,Just show,Only show")]
		public Table Pick(params string[] columns)
		{
			Table skippedColumnTable = new Table();
			foreach (var row in Rows)
			{
				var tempRow = new Dictionary<string, string>();
				foreach (string key in row.Keys)                
				{
					if (columns.Contains(key))
						tempRow.Add(key, row[key]);
				}
				var orderedRow = new Dictionary<string, string>();
				foreach (string col in columns)
				{
					orderedRow.Add(col, tempRow[col]);
				}
				skippedColumnTable.AddRow(orderedRow);
			}
			return skippedColumnTable;
		}
	
		/// <summary>
		/// Random Sample rows from the table
		/// </summary>
		/// <param name="sampleSize">Size of the sample</param>
		/// <returns>A table with number of rows specified by the sampleSize</returns>
		/// <example>Table randSamples = t.RandomSample(20);
		/// This returns 20 random rows to randSamples table from table "t"
		/// </example>
		/// <seealso cref="Table.Shuffle"/>
		[Description("Pick random sample,Random sample,Generate random sample")]
		public Table RandomSample(int sampleSize)
		{
			if (sampleSize <= 0 || sampleSize > this.RowCount)
				throw new ArgumentOutOfRangeException($"Sample size provided is negative or zero or beyond RowCount");
			return Shuffle().Top(sampleSize);
		}
		   
		
		/// <summary>
		/// Retuns top n rows 
		/// </summary>
		/// <param name="n">number of rows from top.</param>
		/// <returns>A table with top n rows</returns>
		/// <example>tab.Top(10);//Returns a table with only the first 10 rows of the current Table.</example>
		[Description("First,Top,From the beginning,From top")]
		public Table Top(int n)
		{
			if (n <= 0)
				throw new ArgumentOutOfRangeException($"Size provided for top n elements is negative or zero");
			var headTable = new Table();
			if (n >= RowCount)
				headTable._rows = _rows;
			else
				headTable._rows = _rows.Take(n).ToList();
			return headTable;
		}
		/// <summary>
		/// Returns last n rows
		/// </summary>
		/// <param name="n">number of rows from bottom</param>
		/// <returns>A table with n rows from bottom</returns>
		/// <example>t.Bottom(10);//returns a table with last 10 rows of the table instance "t"</example>
		[Description("Last,Bottom,From the end,From bottom")]
		public Table Bottom(int n)
		{
			if (n <= 0)
				throw new ArgumentOutOfRangeException($"Number of rows provided is either negative or zero.");
			var tailTable = new Table();
			if (n >= RowCount)
				tailTable._rows = _rows;
			else
				tailTable._rows = _rows.Skip(RowCount - n).ToList();
			return tailTable;
		}
		/// <summary>
		/// Returns top n percent entries from the table
		/// </summary>
		/// <param name="n">The percentage of rows to pick from top</param>
		/// <returns>A table with n% rows from top</returns>
		/// <example>Table topFewPerc = tab.TopNPercent(12);//returns top 12% rows from "tab" to "topFewPerc" Table</example>
		[Description("Top percent")]
		public Table TopNPercent(int n)
		{
			if (n <= 0)
				throw new ArgumentOutOfRangeException($"Value of percentage provided is negative");
			int rowCount = Convert.ToInt16( Math.Floor((float) RowCount * n / 100.0));
			return Top(rowCount);
		}
		

		/// <summary>
		/// Returns the bottom n % entries as a new table
		/// </summary>
		/// <param name="n">Percentage expressed as an integer</param>
		/// <returns></returns>
		/// <example>
		/// //Returns last 5% rows in bottom5Percent table
		/// var bottom5Percent = tab.BottomNPercent(5);
		/// </example>
		[Description("Bottom percent")]
		public Table BottomNPercent(int n)
		{
			if (n <= 0)
				throw new ArgumentOutOfRangeException($"Value of percentage provided is negative");
			int rowCount = Convert.ToInt16(Math.Floor((float)RowCount * n / 100.0));
			return Bottom(rowCount);
		}
		/// <summary>
		/// Returns a section of rows from the middle of the table
		/// </summary>
		/// <param name="skip">Skip these many rows</param>
		/// <param name="take">Then tale these many rows to form the new table</param>
		/// <returns></returns>
		public Table Middle(int skip,int take)
		{
			if (take > RowCount)
				take = RowCount;

			var mid = new Table();
			mid.Rows.AddRange( Rows.Skip(skip).Take(take));
			return mid;
		}
		/// <summary>
		/// Splits the table acording to the rows
		/// </summary>
		/// <param name="rowsPerSplit">The number of rows to be included in each table.</param>
		/// <returns></returns>
		public List<Table> SplitByRows(int rowsPerSplit)
		{

			return Enumerable.Range(0, RowCount - rowsPerSplit + 1)
							 .Select(m => Middle(m * rowsPerSplit, rowsPerSplit))
							 .ToList();
			
		}
		/// <summary>
		/// Generates multiple tables with the specified columns per table. 
		/// </summary>
		/// <param name="columnSplitDescription">The array with names of columns to include in each table.</param>
		/// <returns>A list of tables with specified columns for each table.</returns>
		/// 
		[Description("Split columns,Split by columns")]
		public List<Table> SplitByColumns(params string[][] columnSplitDescription)
		{
			List<Table> tables = new List<Table>();
			foreach (var columnDefinition in columnSplitDescription)
			{
				Table temp = new Table();
				foreach (var col in columnDefinition)
					temp.AddColumn(col, ValuesOf(col));
				tables.Add(temp);
			}
			return tables;
		}
		
		
		/// <summary>
		/// Random shuffle. Returns a shuffled table. 
		/// This can be very handy while generating a random sample
		/// </summary>
		/// <seealso cref="Table.RandomSample"/>
		[Description("Shuffle,Random shuffle,Randomize,Un order")]
		public Table Shuffle()
		{
			this.ThrowIfTableIsNull();

			var shuffledTable = new Table();
			shuffledTable._rows = _rows.OrderBy(r => Guid.NewGuid()).ToList();
			return shuffledTable;
		}
		/// <summary>
		/// Checks if two rows are same or not. 
		/// If all the columns of two rows have the same value, 
		/// then they are same.
		/// </summary>
		/// <param name="firstRow">The first row</param>
		/// <param name="secondRow">The second row</param>
		/// <returns>True if both rows are same, else returns false.</returns>
		private bool IsSameRow(Dictionary<string, string> firstRow, Dictionary<string, string> secondRow)
		{
			if(firstRow == null || secondRow == null)
				throw new Exception("Either of the rows provided is null"); 

			return firstRow.Keys.All(k => secondRow.Keys.Contains(k)) &&
				firstRow.Keys.All(k => secondRow[k] == firstRow[k]);
		}
		#endregion
		public Table Common(Table anotherTable)
		{
			
			this.ThrowIfTableIsNull();
			anotherTable.ThrowIfTableIsNull();

			var result = new Table();
			for (int i = 0; i < RowCount; i++)
			{
				if (anotherTable._rows.Any(r => IsSameRow(r, _rows[i])))
					result.AddRow(_rows[i]);
			}
			return result;
		}
		/// <summary>
		/// Checks whether a given table is subset of this table or not
		/// </summary>
		/// <param name="anotherTable">Other table for which the </param>
		/// <returns>true if the argument table is a subset of the table object
		/// which called the method</returns>
		/// <example>bool subet = t1.IsSubset(t2);
		/// Where t1 and t2 are table instances.</example>
		public bool IsSubset(Table anotherTable)
		{
			this.ThrowIfTableIsNull();
			anotherTable.ThrowIfTableIsNull();

			bool isSubset = false;
			foreach (var row in _rows)
			{
				if (anotherTable._rows.Any(r => IsSameRow(row, r)))
				{
					isSubset = true;
				}
				else
				{
					isSubset = false;
					break;
				}
			}
			return isSubset;
		}
		#endregion 


		#region Natural Query
		/// <summary>
		/// Some times we are interested to find rows in the table that match a given condition 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Table ShowMe(string query)
		{
			Table result = this;
			string[] words = query.Split(' ');//how many easy courses are there 
		
			foreach (string word in words)
			{
				foreach (string column in ColumnHeaders)
				{
					if (ValuesOf(column).Contains(word))
					{
						result = result.Filter(column, word);
					}
				}
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public int HowMany(string query)
		{
			return ShowMe(query).RowCount;
		}
		#endregion
	  
	}

}

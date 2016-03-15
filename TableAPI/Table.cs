using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NCalc;
using System.Data;
using System.Globalization;
using System.ComponentModel;



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
    };
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
            if(this == null)
            {
                throw new ArgumentNullException("this", "The Table instance is null");
            }
            if(column == null)
            {
                throw new ArgumentNullException("column", "The provided column is null");
            }
            if(value == null)
            {
                throw new ArgumentNullException("value", "The vlue provided to search is null");
            }
            try {
                return Convert.ToDouble(this.ValuesOf(column).Count(z => z == value))
                     / Convert.ToDouble(this.ValuesOf(column).Count);
            }
            catch(DivideByZeroException ex)
            {
                throw new DivideByZeroException("There was no value for the given colmun. Thus a divided by zero exception occured.");
            }
        }
        /// <summary>
        /// Basic filtering based on the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate takes a row and returns a bool.</param>
        /// <returns>The result table with filtered values</returns>
        /// <example>
        /////Finds all iris flowers where the SepalWidth is more than 3.0
        ///Table filtered = iris.Filter(x => Convert.ToDouble(x["SepalWidth"]) > 3.0);
        ///</example>
        public Table Filter(Func<Dictionary<string, string>, bool> predicate)
        {
            if (this == null)
            {
                throw new ArgumentNullException("this", "The Table instance is null");
            }
            if(predicate == null)
            {
                throw new ArgumentNullException("predicate", "The predicate provided is null");
            }
            Table result  = new Table ();
            result.Rows.AddRange(this.Rows.Where(t => predicate.Invoke(t)));
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

            if (this == null)
            {
                throw new ArgumentNullException("this", "The Table instance is null");
            }
            if (column == null)
            {
                throw new ArgumentNullException("column", "The provided column name is null");
            }
            if (regexPattern == null)
            {
                throw new ArgumentNullException ("regexPattern", "The regular expression pattern provided is null");
            }
            Table filteredTable = new Table();
            for (int i = 0; i < _rows.Count; i++)
            {
                try
                {
                    if (Regex.IsMatch(_rows[i][column], regexPattern))
                        filteredTable.AddRow(_rows[i]);
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

            if (this == null)
            {
                throw new ArgumentNullException("this", "The Table instance is null");
            }
            if(fieldSearchValuesMap == null)
            {
                throw new ArgumentNullException("fieldSearchValuesMap", "No value is provided to filter on");
            }
            Table result = this;
            foreach (var key in fieldSearchValuesMap.Keys)
            {
                try
                {
                    result = result.Filter(key, fieldSearchValuesMap[key].ToArray());
                }
                catch(KeyNotFoundException ex)//If key "key" is not found in fieldSearchValueMap 
                {
                    throw ex;
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
            
            Table filteredTable = new Table();
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
            Table filteredTable = new Table();
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
            HashSet<string> columns = this.ColumnHeaders;
            StreamWriter sw = new StreamWriter(@"C:\temp.csv");
            sw.WriteLine(columns.Aggregate((a, b) => a + "," + b));
            string numberRegex = "[0-9]+.*[0-9]+";
            for (int i = 0; i < _rows.Count; i++)
            {
                foreach (string col in columns)
                {
                    if (Regex.IsMatch(_rows[i][col], numberRegex))
                    {
                        sw.Write(_rows[i][col] + ",");
                    }
                    else
                    {
                        sw.Write("\"" + _rows[i][col] + "\",");
                    }
                }
                sw.WriteLine();
            }
            sw.Close();
            string strCSVConnString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source='C:\temp.csv';Extended Properties='text;HDR=YES;'";
            sql = sql.Replace("[Table]", @"C:\temp.csv");

            OleDbDataAdapter oleda = new OleDbDataAdapter(sql, strCSVConnString);
            System.Data.DataTable dataTable = new System.Data.DataTable();
            oleda.Fill(dataTable);


            Table resultTable = new Table();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Dictionary<string, string> currentRow = new Dictionary<string, string>();
                foreach (string column in columns)
                {
                    try
                    {
                        currentRow.Add(column, dataTable.Rows[i][column].ToString());
                    }
                    catch { continue; }
                }
                resultTable.AddRow(currentRow);
            }
            return resultTable;
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
        public OrderedTable SortInThisOrder(string columnName,List<string> inThisOrder, SortDirection how = SortDirection.Ascending)
        {             
            CustomComparers.sortInThisOrder = inThisOrder;

            OrderedTable sortedTable = new OrderedTable();

            CustomComparers.CustomSorter sorter = new CustomComparers.CustomSorter();

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
            OrderedTable sortedTable = new OrderedTable();
            
            Squirrel.CustomComparers.NumericComparer comp = new Squirrel.CustomComparers.NumericComparer();
            Squirrel.CustomComparers.DateComparer dateComp = new Squirrel.CustomComparers.DateComparer();
            Squirrel.CustomComparers.CurrencyCustomSorter currencyComp = new CustomComparers.CurrencyCustomSorter();

            if (smartSort)
            {
                SmartDefaults.Instance.GetSmartDefaultValues(smartDefaultFile);//Populate and keep it ready once. 

                KeyValuePair<bool, string> matchingEntriesIfAny = SmartDefaults.Instance.DoesMatchingEntryExist(this.ValuesOf(columnName));

                //Smart Sort the day and month names 
                if (matchingEntriesIfAny.Key == true)
                {
                    List<string> sortingOrder = SmartDefaults.DefaultValues[matchingEntriesIfAny.Value];
                    return this.SortInThisOrder(columnName, sortingOrder, how);
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
        /// Table modCols = tab.ModifyColumnName("First_Name","FirstName");</example>
        [Description("Change column name")]
        public Table ModifyColumnName(string oldName, string newName)
        {
            List<Dictionary<string, string>> newRows = new List<Dictionary<string, string>>();
            foreach (var row in _rows)
            {
                Dictionary<string, string> temp = row;
                string val = temp[oldName];
                temp.Remove(oldName);
                temp.Add(newName, val);
                newRows.Add(temp);
            }
            Table modTab = new Table();
            modTab._rows = newRows;
            return modTab;
        }
       
        /// <summary>
        /// Returns all the values of the given column 
        /// </summary>
        /// <param name="columnName">column for which we want to extract all values</param>
        /// <returns>All the values of the given column as a list of string.</returns>
        /// <example>
        /// //List of string codes will hold all the values of
        /// //
        /// List&lt;string&gt; codes  = tab.ValuesOf("Code");</example>
        public List<string> ValuesOf(string columnName)
        {
            try
            {

                return _rows.Select(t => t[columnName]).ToList();
            }
            catch(KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("The column " + columnName +" is not found " + ex.Message);
                
            }

        }
        /// <summary>
        /// Returns only the numeric columns
        /// </summary>
        public List<string> NumericColumns
        {
            get
            {
                List<string> numericColumns = new List<string> ();
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
                    statement = statement.Replace(columnName + "[" + startCopy.ToString() + "]", _rows[i][columnName]);
                }
                Expression exp = new Expression(statement);
                if (RowCount <= index)
                {
                    Dictionary<string, string> temp = new Dictionary<string, string>();
                    temp.Add(columnNameLeft, exp.Evaluate().ToString());
                    _rows.Add(temp);
                }
                _rows[index][columnNameLeft] = exp.Evaluate().ToString();

                start = index;
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
        public List<Dictionary<string, string>> Rows
        {
            get
            {
                return _rows;
            }
        }
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
            List<string> values = new List<string> ();
            foreach (var val in this.ValuesOf(fromColumnName))
                values.Add(Regex.Match(val, pattern).Value);
            this.AddColumn(columnName, values);
        }
        /// <summary>
        /// Adds a column for which value gets calculated from a given formula.
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="formula">Formula to calculate values of the new column</param>
        /// <param name="decimalDigits"></param>
        public void AddColumn(string columnName, string formula, int decimalDigits)
        {
            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));
            if (formula == null)
                throw new ArgumentNullException(nameof(formula));
            
            string copyFormula = formula;
           
            string[] columns = formula.Split(new char[] { '+', '-', '*', '/', '(', ')',' ' },StringSplitOptions.RemoveEmptyEntries);

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
                    
                    catch(KeyNotFoundException ex)//Occurs when the column name is not found
                    {
                 //       throw new KeyNotFoundException(nameof())                        
                    }
                    catch (Exception ex)
                    {

                    }
                }
         

                _rows[i].Add(columnName, Math.Round(Convert.ToDecimal(new Expression(formula).Evaluate().ToString()),decimalDigits).ToString());         
            }
        }
        
        /// <summary>
        /// Removes all currency symbol and comma from the values of the given column.
        /// </summary>
        /// <param name="columns">The name of the column</param>
        /// <returns>A table without the currency symbol and commas</returns>
        /// 
        [Description("Clean numeric columns")]
        public Table TransformCurrencyToNumeric(params string[] columns)
        {
            Table newTable = this;
            
            foreach (string col in columns)
            {

                
                newTable =  Transform(col, x => x.Replace(",",string.Empty)
                                                 .Replace("$",string.Empty)//Remove US Dollar symbol
                                                 .Replace("£",string.Empty)//Remove GBP Pound symbol
                                                 .Replace("€",string.Empty)//Remove Euro Symbol
                                                 .Replace("¥",string.Empty));//Remove Chinese Yen symbol
            }
            return newTable;
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
            if (_rows.Count == 0)
                for (int i = 0; i < values.Count; i++)
                    _rows.Add(new Dictionary<string, string>());
            for (int i = 0; i < _rows.Count; i++)
                _rows[i].Add(columnName, values[i]);
        }
        /// <summary>
        /// Remove the given column from the table
        /// </summary>
        /// <param name="columnName">column to remove </param>
        public void RemoveColumn(string columnName)
        {
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
                return _rows[index];
            }
            set
            {
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
            Table result = new Table();
            List<string> allNumericColumns = new List<string>();
            allNumericColumns.AddRange(ColumnHeaders.Where(m => ValuesOf(m).All(z => Char.IsNumber(z, 0))));

            for (int i = 0; i < RowCount; i++)
            {
                Dictionary<string, string> currentRow = new Dictionary<string, string>();
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
            this[this.ColumnHeaders.ElementAt(0)].ToList().ForEach( m => transposed.ColumnHeaders.Add(m));
            for (int i = 1; i < this.ColumnHeaders.Count; i++)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                this[this.ColumnHeaders.ElementAt(i)].ToList().ForEach(n => row.Add(this.ColumnHeaders.ElementAt(i), n));
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
            for (int r = 0; r < this.RowCount; r++)
            {
                foreach (string col in this.ColumnHeaders)
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
                string[] internalTokens = tok.Split(new char[] { '(', ')' });
                roundOffMap.Add(internalTokens[0], Convert.ToInt16(internalTokens[1]));
            }
            foreach (string col in this.ColumnHeaders)
                for (int r = 0; r < this.RowCount; r++)
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
            for (int i = 0; i < this.RowCount; i++)
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
            columns.ToList().ForEach(col => this.RemoveColumn(columnName: col));
            this.AddColumn(newColumnName, newValues.Select(t => t.ToString()).ToList());
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
            List<string> allDistinctValues = ValuesOf(columnName)
                                             .Distinct()
                                             .ToList();
            

            List<string> allNumericColumns = new List<string>();
            
            allNumericColumns.AddRange(ColumnHeaders.Where(col =>  ValuesOf(col).Where(z => z.Trim().Length >0).All(m => Char.IsNumber(m,0))));           

            Table aggregatedTable = new Table();
            foreach (string value in allDistinctValues)
            {
                Dictionary<string, string> aggRow = new Dictionary<string, string>();
                aggRow.Add(columnName, value);
                Dictionary<string, string> filterValue = new Dictionary<string, string>();
                filterValue.Add(columnName, value);
                Table tempTable = Filter(columnName,value);
                foreach (string col in allNumericColumns)
                {
                   if (col != columnName)
                    {
                        if (how == AggregationMethod.Count)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).Count().ToString());
                        if (how == AggregationMethod.Max)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).Max().ToString());
                        if (how == AggregationMethod.Min)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).Min().ToString());
                        if (how == AggregationMethod.Sum)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).Sum().ToString());
                        if (how == AggregationMethod.Average)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).Average().ToString());
                        if (how == AggregationMethod.AboveAverageCount)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).AboveAverageCount().ToString());
                        if(how == AggregationMethod.BelowAverageCount)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).BelowAverageCount().ToString());
                        if (how == AggregationMethod.Range)
                            aggRow.Add(col, tempTable[col].Select(t => Convert.ToDecimal(t)).Range().ToString());
                        if (how == AggregationMethod.Kurtosis)
                            aggRow.Add(col, BasicStatistics.Kurtosis(tempTable[col].Select(t => Convert.ToDouble(t)).ToList()).ToString());

                       
                    }
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
                Dictionary<string, string> filterValue = new Dictionary<string, string>();
                filterValue.Add(columnName, value);
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
        /// Transforms values of a column as per the given transformer
        /// </summary>
        /// <param name="columnName">The column on which the transformer function has to be run</param>
        /// <param name="tranformer">The transformer method</param>
        public Table Transform(string columnName, Func<string, string> tranformer)
        {
            Table temp = new Table();
            temp._rows = _rows;
            for (int i = 0; i < RowCount; i++)
            {
                temp._rows[i][columnName] = tranformer(temp._rows[i][columnName]);
            }
            return temp;
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
            Dictionary<string, int> histogram = new Dictionary<string, int>();
            HashSet<string> values = new HashSet<string>(ValuesOf(columnName));
            foreach (string value in values)
            {
                if (!histogram.ContainsKey(value))
                    histogram.Add(value, Filter(columnName, value).RowCount);
            }
            return histogram;
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
            Dictionary<string, Table> tables = new Dictionary<string, Table>();
            List<string> values = ValuesOf(columnName);
            List<string> distinctValues = values.Distinct().ToList();


            foreach (string val in distinctValues)
            {
                tables.Add(val, Filter(columnName, val));
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
        /// <example>
        /// //This is an example use case of this method.
        ////Table 1 Columns
        ////Name  | Age | Gender 
        ////Sam   | 23  | M
        ////Jane  | 19  | F
        ////Raskin| 14  | M

        ////Table 2 Columns
        ////Name | Course 
        ////Jane | C#
        ////Sam  | F#
        ////Raskin| Python


        ////Merged Columns in the resultant Table.
        ////Name | Age | Gender | Course
        ////Sam  | 23  | M      | F#
        ////Jane | 19  | F      | C#
        ////Raskin| 14 | M      | Python
        /// Table merged = t1.MergeByColumns(t2);//Uses the first column to perform the join
        /// Table mergedByName = t1.MergeByColumns(t2,"Name");//Uses the column "Name" to perform the join</example>
        /// 
        public Table MergeByColumns(Table anotherTable, string connectorColumn = "Not Provided")
        {
            if (connectorColumn == "Not Provided")
                connectorColumn = this.ColumnHeaders.First();

            Table mergedColumnsTable = new Table();

            this.ColumnHeaders.ToList().ForEach(m => mergedColumnsTable.AddColumn(m, this.ValuesOf(m)));
            
            anotherTable.ColumnHeaders.Where(t=>t!=connectorColumn)
                       .ToList().ForEach(m =>
                                    mergedColumnsTable.AddColumn(m, anotherTable.SortInThisOrder(connectorColumn, 
                                    this.ValuesOf(connectorColumn)).ValuesOf(m)));


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
            Table mergedTable = new Table();
            if (!ColumnHeaders.OrderBy(head => head).SequenceEqual(anotherTable.ColumnHeaders.OrderBy(head=>head)))
                return this;//Merge is not possible'
            else
            {
                mergedTable._rows = _rows;

                mergedTable._rows.AddRange(anotherTable._rows);
                if (removeDups)
               //     return mergedTable.Distinct();
             //   else
                    return mergedTable;
            }
            return mergedTable;
        }
        /// <summary>
        /// Extracts those rows from the table which are not present in the another one.
        /// </summary>
        /// <param name="anotherTable">The other table with which we have to compare.</param>
        /// <returns>A new table with rows that are only available in the current table, not in the other one.</returns>
        /// <example>Table exclusive = t1.Exclusive(t2);//Find rows that are exclusively available in table "t1"</example>
        /// <github>
        /// 
        /// </github>
        public Table Exclusive(Table anotherTable)
        {
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
        public Table Common(Table anotherTable)
        {
            if (anotherTable == null)
                throw new ArgumentNullException(nameof(anotherTable));

            Table result = new Table();
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
        public Table MergeColumns(string newColumnName, char separator = ' ',params string[] columns)
        {
            List<string> mergedValues = new List<string>();
            for (int i = 0; i < columns.Length - 1; i++)
                mergedValues = this.ValuesOf(columns[i]).Zip(this.ValuesOf(columns[i + 1]), (a, b) => a + separator.ToString() + b).ToList();

            this.AddColumn(newColumnName, mergedValues);
            columns.ToList().ForEach(col => this.RemoveColumn(col));
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
                Dictionary<string, string> tempRow = new Dictionary<string, string>();
                foreach (string key in row.Keys)                
                {
                    if (columns.Contains(key))
                        tempRow.Add(key, row[key]);
                }
                Dictionary<string, string> orderedRow = new Dictionary<string, string>();
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
            return this.Shuffle().Top(sampleSize);
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
            Table headTable = new Table();
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
            Table tailTable = new Table();
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
            int rowCount = Convert.ToInt16( Math.Floor((float) this.RowCount * n / 100.0));
            return this.Top(rowCount);
        }
        

        /// <summary>
        /// Returns the bottom n % entries as a new table
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// 
        [Description("Bottom percent")]
        public Table BottomNPercent(int n)
        {
            int rowCount = Convert.ToInt16(Math.Floor((float)this.RowCount * n / 100.0));
            return this.Bottom(rowCount);
        }
        /// <summary>
        /// Returns a section of rows from the middle of the table
        /// </summary>
        /// <param name="skip">Skip these many rows</param>
        /// <param name="take">Then tale these many rows to form the new table</param>
        /// <returns></returns>
        public Table Middle(int skip,int take)
        {
            Table mid = new Table();
            mid.Rows.AddRange( this.Rows.Skip(skip).Take(take));
            return mid;
        }
        /// <summary>
        /// Splits the table acording to the rows
        /// </summary>
        /// <param name="rowsPerSplit">The number of rows to be included in each table.</param>
        /// <returns></returns>
        public List<Table> SplitByRows(int rowsPerSplit)
        {
            List<Table> splits = new List<Table>();

            return Enumerable.Range(0, this.RowCount - rowsPerSplit + 1)
                             .Select(m => this.Middle(m * rowsPerSplit, rowsPerSplit))
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
                    temp.AddColumn(col, this.ValuesOf(col));
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
            if (this == null)
            {
                throw new ArgumentNullException("this", "The Table instance is null");
            }
            Table shuffledTable = new Table();
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
            return firstRow.Keys.All(k => secondRow.Keys.Contains(k)) &&
                firstRow.Keys.All(k => secondRow[k] == firstRow[k]);
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
            List<string> guessedColumns = new List<string>();
            Dictionary<string, string> filterSettings = new Dictionary<string, string>();
            foreach (string word in words)
            {
                foreach (string column in ColumnHeaders)
                {
                    if (this.ValuesOf(column).Contains(word))
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

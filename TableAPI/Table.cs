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



namespace Squirrel
{
    public enum MissingValueHandlingStrategy { MarkWithNA, Average }
    /// <summary>
    /// 
    /// </summary>
    public enum Alignment { Left, Right }
    /// <summary>
    /// Sorting Direction. This enum is used with Sorting methods SortBy and SortInThisOrder
    /// </summary>
    public enum SortDirection { Ascending, Descending }
    public enum ChartType {Pie, Donut, Bar};
    public enum ChartProvider { HighCharts, GoogleVisualization };
    public enum OutlierDetectionAlgorithm { IQR_Interval, Z_Score };
    /// <summary>
    /// Method to be used for aggregation/consolidation
    /// </summary>
    public enum AggregationMethod 
    {
        Sum, //Summation of the sequence
        Average, //Average of the sequence
        Max,//Maximum value of the sequence
        Min, //Minimum value of the sequence
        Count,//Total count of the sequence
        StandardDeviation, //Standard deviation of the sequence      
        Variance, //Calculates the variance of the sequence 
        AboveAverageCount, //Number of instance above average 
        BelowAverageCount,//Number of instances which are below average values
        AverageCount,//Number of instances that has the value equal to average
        Skew, //Measures the skewness of the given sequence
        Kurtosis, //Measures the Kurtosis of the given sequence
        Range //Measures the range of values of the given sequence.
    }
    /// <summary>
    /// The Table class. This is the represenation of the ubiquitous Table structure. 
    /// </summary>
    public class Table
    {   
        private HashSet<string> _columnHeaders = new HashSet<string>();
        private List<Dictionary<string, string>> _rows = new List<Dictionary<string, string>>();      
        #region Data I/O from several formats

        /// <summary>
        /// Deletes the tags from a HTML line
        /// </summary>
        /// <param name="codeLine">HTML code from which tags has to be removed</param>
        /// <param name="exceptTheseTags">Remove all tags except this one</param>
        /// <returns></returns>
        private static string StripTags( string codeLine,List<string> exceptTheseTags)
        {
            string tag = string.Empty;
            string html = string.Empty;
            var tags = new List<string>();
            for (int i = 0; i < codeLine.Length; i++)
            {
                tag = string.Empty;
                if (codeLine[i] == '<')
                {
                    i++;
                    do
                    {
                        tag = tag + codeLine[i];
                        i++;
                    } while (codeLine[i] != '>');
                    tags.Add("<" + tag + ">");
                }
            }
            tags.RemoveAll(t=> exceptTheseTags.Contains(t));
            foreach (string k in codeLine.Split(tags.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                html = html + k + " ";

            return html;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="workbookName"></param>
        /// <returns></returns>
        public Table LoadXLS(string fileName,string workbookName)
        {
            //Use this open source project to read data from Excel.
            //stand on the shoulder of giants.
            //http://exceldatareader.codeplex.com/
            //http://code.google.com/p/linqtoexcel/ (Love This!)
            return this;
        }
        /// <summary>
        /// Loads a file with fixed length columns where the length of each column is provided by a dictionary
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <param name="fieldLengthMap">The mapping of column name and the length</param>
        /// <returns>A table with loaded values</returns>
        public static Table LoadFixedLength(string fileName, Dictionary<string, int> fieldLengthMap)
        {
            //Sam  
            
            Table thisTable = new Table();
            //fieldLengthMap.Select(f => f.Key).ToList().ForEach(m => thisTable.ColumnHeaders.Add(m));

            Dictionary<string, List<string>> columnWiseValues = new Dictionary<string, List<string>>();

            string line = string.Empty;

            StreamReader sr = new StreamReader(fileName);
            
            while ((line = sr.ReadLine()) != null)
            {
                int start = 0;
                int fieldCount = fieldLengthMap.Count;
                foreach (string k in fieldLengthMap.Keys)
                {
                    
                    if (fieldCount == 1) //last field (handle differently)
                    {
                        if (!columnWiseValues.ContainsKey(k))

                            columnWiseValues.Add(k, new List<string>() { line.Substring(start, line.Length - start) });
                        else
                            columnWiseValues[k].Add(line.Substring(start, line.Length - start));
                    }
                    else
                    {
                        int max = fieldLengthMap[k] >= line.Length ? line.Length : fieldLengthMap[k];
                        if (!columnWiseValues.ContainsKey(k))
                        {

                            columnWiseValues.Add(k, new List<string>() { line.Substring(start, max) });
                        }
                        else
                            columnWiseValues[k].Add(line.Substring(start, max));

                        start += fieldLengthMap[k];
                    }
                    fieldCount--;
                }
            }

            foreach (string v in columnWiseValues.Keys)
                thisTable.AddColumn(v, columnWiseValues[v]);

            return thisTable;
        }
        /// <summary>
        /// Loads a file with fixed length columns.
        /// </summary>
        /// <param name="fileName">The name of the file with fixed length columns</param>
        /// <param name="headersWithLength">A comma separated string representing each column name and the width
        /// For example if the file has three columns name,age and course where name can be of length 20
        /// age is 2 and course is 5 character long then the headerWithLength parameter has to be passed like
        /// name(20),age(2),course(5)</param>
        /// <returns>A table with the loaded values</returns>
        /// <example>Table fixedLengthTable = Table.LoadFixedLength("tab.txt","name(20),age(2),course(5)");</example>
        public static Table LoadFixedLength(string fileName, string headersWithLength)// "name(20),age(2),course(5)"
        {
            string[] tokens = headersWithLength.Split(',');

            Dictionary<string, int> expectations = new Dictionary<string, int>();

            foreach (string tok in tokens)
            {
                string[] internalTokens = tok.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                expectations.Add(internalTokens[0], Convert.ToInt16(internalTokens[1]));
            }
            return LoadFixedLength(fileName, expectations);
        }
        /// <summary>
        /// Loads data from .arff format
        /// Data in Weka toolkit is from .arff source
        /// </summary>
        /// <param name="fileName">The arff filename</param>
        /// <returns>Returns a table with the loaded values</returns>
        /// <example>Table play = Table.LoadARFF(".\data\play.arff");</example>
        public static Table LoadARFF(string fileName)
        {
            Table result = new Table();
            List<string> columnHeaders = new List<string> ();
            StreamReader arffReader = new StreamReader(fileName);
            string line = string.Empty;
            while ((line = arffReader.ReadLine()) != null)
            {
                if (line.Trim().ToLower().StartsWith("@attribute"))
                    columnHeaders.Add(line.Split(' ')[1]);
                if (line.Trim().ToLower().StartsWith("@data"))
                    break;
            }
            List<string> dataLines = File.ReadAllLines(fileName).Where(rline => !rline.Trim().StartsWith("%") && !rline.Trim().StartsWith("@"))
                                          .ToList();

            foreach (string dataLine in dataLines)
            {
                Dictionary<string, string> currentRow = new Dictionary<string, string>();
                string[] tokens = dataLine.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    for (int i = 0; i < tokens.Length; i++)
                        currentRow.Add(columnHeaders[i], tokens[i]);
                    result.Rows.Add(currentRow);
                }
            }

            return result;
        }
        /// <summary>
        /// Loads a HTML table to the corresponding Table container
        /// </summary>
        /// <param name="htmlTable">The HTML code that creates the table</param>
        /// <returns>A table with all the data from the html table</returns>
        public static Table LoadHTMLTable(string htmlTable)
        {
            StreamReader htmlReader = new StreamReader(htmlTable);
            string totalTable = htmlReader.ReadToEnd();
            htmlReader.Close();
            //sometimes the tags "<td> <th> and <tr> can have extra attributes. We don't care for that. we have to get rid of that
            totalTable = totalTable.Replace("<td ", "<td><").Replace("<th ", "<th><").Replace("<tr ", "<tr><");
            totalTable = StripTags(totalTable, new List<string>() { "<td>", "</td>", "<th>", "</th>", "<tr>", "</tr>" });
            
            totalTable = totalTable.Replace("\r", string.Empty).Replace("\t",string.Empty).Replace("\n", string.Empty);
            totalTable = totalTable.Replace("<tr><th>", string.Empty)
                              .Replace("</th></tr>", "\"" +  Environment.NewLine)
                              .Replace("</th><th>", "\",\"")
                              .Replace("</td></tr>", "\"" +  Environment.NewLine )
                              .Replace("</td><td>", "\",\"")
                              .Replace("<tr><td>",   "\"" + Environment.NewLine);
            StreamWriter sw = new StreamWriter("TemporaryFile.csv");
            sw.WriteLine(totalTable);
            sw.Close();
            Table loadedTable = LoadCSV("TemporaryFile.csv",true);
            return loadedTable;
        }
        /// <summary>
        /// Loads a CSV file to a respective Table data structure.
        /// </summary>
        /// <param name="csvFileName">The file for which values has to be loaded into a table data structure.</param>
        /// <returns>A table which has all the values in the CSV file</returns>
        public static Table LoadCSV(string csvFileName, bool wrappedWihDoubleQuotes = false)
        {
            if (wrappedWihDoubleQuotes)
            {
                return LoadFlatFile(csvFileName, new string[] { "\",\"" ,"\""});
            }
            else
                return LoadFlatFile(csvFileName, new string[] { "," });
        }
        /// <summary>
        /// Loads Data from Tab Separated File
        /// </summary>
        /// <param name="tsvFileName">The file name to read from</param>
        /// <returns>A table loaded with these values</returns>
        public static Table LoadTSV(string tsvFileName)
        {
            return LoadFlatFile(tsvFileName, new string[] { "\t" });
        }
        
        /// <summary>
        /// Loads data from any flat file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="delimeters">Delimeters</param>
        /// <returns>A table loaded with all the values in the file.</returns>
        public static Table LoadFlatFile(string fileName,string[] delimeters)
        {

            Table loadedCSV = new Table();
            StreamReader csvReader = new StreamReader(fileName);
            string line = string.Empty;
            int lineNumber = 0;
            HashSet<string> columns = new HashSet<string>();
            while ((line = csvReader.ReadLine()) != null)
            {
                if (lineNumber == 0)//reading the column headers
                {
                    line.Split(delimeters, StringSplitOptions.None)

                        .Where(z => z.Trim().Length != 0)//Remove non empty column names
                           .ToList()
                        .ForEach(col => columns.Add(col.Trim(new char[] { '"', ' ' })));
                    lineNumber++;
                }
                else
                {
                    string[] values = null;
                    if (line.Trim().Length > 0)
                    {
                        values = line.Split(delimeters, StringSplitOptions.None);
                        values = values.Take(columns.Count).ToArray();
                        Dictionary<string, string> tempRow = new Dictionary<string, string>();

                        for (int i = 0; i < columns.Count; i++)
                        {
                            try
                            {
                                tempRow.Add(columns.ElementAt(i), values[i].Trim(new char[] { '"', ' ' }));
                            }
                            catch { continue; }
                        }
                        if (tempRow.Keys.Count == columns.Count)
                            loadedCSV.AddRow(tempRow);

                    }
                }
            }
            return loadedCSV;
        }


        /// <summary>
        /// Dumps the table in a pretty format to console.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="align"></param>
        public void PrettyDump(string header = "None", Alignment align = Alignment.Right)
        {
            if (header != "None")
                Console.WriteLine(header);
            Dictionary<string, int> longestLengths = new Dictionary<string, int>();

            foreach (string col in ColumnHeaders)
                longestLengths.Add(col, ValuesOf(col).OrderByDescending(t => t.Length).First().Length);
            foreach (string col in ColumnHeaders)
                if (longestLengths[col] < col.Length)
                    longestLengths[col] = col.Length;
            foreach (string col in ColumnHeaders)
            {
                if (align == Alignment.Right)
                    Console.Write(" " + col.PadLeft(longestLengths[col]) + new string(' ', 4));
                if (align == Alignment.Left)
                    Console.Write(" " + col.PadRight(longestLengths[col]) + new string(' ', 4));
            }
            Console.WriteLine();
            for (int i = 0; i < RowCount; i++)
            {
                foreach (string col in ColumnHeaders)
                {
                    if (_rows[i].ContainsKey(col))
                    {
                        if (align == Alignment.Right)
                            Console.Write(" " + _rows[i][col].PadLeft(longestLengths[col]) + new string(' ', 4));
                        if (align == Alignment.Left)
                            Console.Write(" " + _rows[i][col].PadRight(longestLengths[col]) + new string(' ', 4));
                    }
                }
                Console.WriteLine();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cssURL"></param>
        /// <returns></returns>
        public string ToHTMLTable()
        {
            //To Do
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine("<table>");
            foreach (string header in ColumnHeaders)
            {
                tableBuilder.AppendLine("<th>" + header + "</th>");
            }
            for (int i = 0; i < RowCount; i++)
            {
                tableBuilder.AppendLine("<tr>");
                foreach (string header in ColumnHeaders)
                    tableBuilder.AppendLine("<td>" + this[header][i] + "</td>");
                tableBuilder.AppendLine("</tr>");
            }
            tableBuilder.AppendLine("</table>");
            return tableBuilder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToCSV()
        {
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToTSV()
        {
            return ToValues('\t');

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delimChar"></param>
        /// <returns></returns>
        private string ToValues(char delimChar)
        {
            return
                //Write the headers
              ColumnHeaders.Select(t => "\"" + t + "\"").Aggregate((a, b) => a + delimChar.ToString() + b)
                //Push a newline 
              + Environment.NewLine
                //Add the rows
              + this.Rows.Select(t => t.Values.Select(z => "\"" + z + "\"").Aggregate((a, b) => a + delimChar.ToString() + b))
               .Aggregate((f, s) => f + Environment.NewLine + s);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToARFF()
        {
            StringBuilder arffBuilder = new StringBuilder();
            
            foreach (string header in ColumnHeaders)
            {
                arffBuilder.AppendLine("@attribute " + header + " {" + ValuesOf(header).Distinct().Aggregate((a, b) => a + "," + b) + "}");
            }
            arffBuilder.AppendLine("@data");
            for (int i = 0; i < RowCount; i++)
            {
                List<string> values  = new List<string> ();
                foreach (string header in ColumnHeaders)
                    values.Add(this[header][i]);
                arffBuilder.AppendLine(values.Aggregate((a, b) => a + "," + b));
            }
            return arffBuilder.ToString() ;
        }
        /// <summary>
        /// Generates a DataTable out of the current Table 
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable thisTable = new DataTable();
            ColumnHeaders.ToList().ForEach(m => thisTable.Columns.Add(m));

            foreach (var row in this.Rows)
            {
                DataRow dr = thisTable.NewRow();
                foreach (string column in ColumnHeaders)
                    dr[column] = row[column];
                thisTable.Rows.Add(dr);
            }
            return thisTable;
        }
     
        #endregion

        #region Filtering
        /// <summary>
        /// Finding a value by regular expression
        /// </summary>
        /// <param name="column">The column where the values has to be sought</param>
        /// <param name="regexPattern">The regex pattern</param>
        /// <returns>A table filled with rows that has a column with matching value.</returns>
        public Table FilterByRegex(string column, string regexPattern)
        {
            Table filteredTable = new Table();
            for (int i = 0; i < _rows.Count; i++)
            {
                if (Regex.IsMatch(_rows[i][column], regexPattern))
                    filteredTable.AddRow(_rows[i]);
            }
            return filteredTable;
      
        }
        /// <summary>
        /// Finds all the matching rows from the source table
        /// </summary>
        /// <param name="_fieldSearchValueMap">Key value pair by which the filtering will be performed</param>
        /// <returns>A table with all the filtered rows. If no matching row is found, a table with 0 row is returned
        /// </returns>
        public Table Filter(Dictionary<string, string> _fieldSearchValueMap)
        {
            Table filteredTable = new Table();
            for (int i = 0; i < _rows.Count; i++)
            {
                bool matching = false;
                foreach (string key in _rows[i].Keys)
                {
                    if (_fieldSearchValueMap.ContainsKey(key) && _rows[i][key] == _fieldSearchValueMap[key])
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
            StreamWriter sw = new StreamWriter(@"C:\Personal\temp.csv");
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
            string strCSVConnString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Personal;Extended Properties='text;HDR=YES;'";
            sql = sql.Replace("[Table]", @"C:\Personal\temp.csv");

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
        /// <param name="how">Whether the sorting has to be done in ascending or in descending order or not.</param>
        /// <returns></returns>
        public OrderedTable SortBy(string columnName, SortDirection how = SortDirection.Ascending)
        {
            OrderedTable sortedTable = new OrderedTable();
            
            Squirrel.CustomComparers.NumericComparer comp = new Squirrel.CustomComparers.NumericComparer();
            Squirrel.CustomComparers.DateComparer dateComp = new Squirrel.CustomComparers.DateComparer();
            Squirrel.CustomComparers.CurrencyCustomSorter currencyComp = new CustomComparers.CurrencyCustomSorter();

            SmartDefaults.Instance.GetSmartDefaultValues();//Populate and keep it ready once. 

            KeyValuePair<bool, string> matchingEntriesIfAny = SmartDefaults.Instance.DoesMatchingEntryExist(this.ValuesOf(columnName));

            //Smart Sort the day and month names 
            if(matchingEntriesIfAny.Key==true)
            {   
                List<string> sortingOrder = SmartDefaults.DefaultValues[ matchingEntriesIfAny.Value];
                return this.SortInThisOrder(columnName, sortingOrder, how);
            }
            

            string dateRegex = @"^[0-3]?[0-9]/[0-3]?[0-9]/(?:[0-9]{2})?[0-9]{2}$";
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
        /// <returns></returns>
        public List<string> ValuesOf(string columnName)
        {
            try
            {
                return _rows.Select(t => t[columnName]).ToList();
            }
            catch
            {
                return new List<string>() { string.Empty };
            }
        }
        /// <summary>
        /// Returns names of the columns of the table
        /// </summary>
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
        /// Adds values to a column
        /// </summary>
        /// <param name="formula">The formula to create new values</param>
        /// <param name="count">How many rows to create</param>
        public void AddRows(string formula, int count)
        {           
            string pattern = "[[0-9]+]";
            string[] tokens = formula.Split('=');
            string columnName = formula.Trim().Substring(0, formula.IndexOf('['));
            int index = Convert.ToInt32(Regex.Match(tokens[0], pattern).Value.Replace("[", string.Empty).Replace("]", string.Empty));
            int start = Convert.ToInt32(Regex.Match(tokens[1], pattern).Value.Replace("[", string.Empty).Replace("]", string.Empty));

            int startCopy = start;
            int j = 0;
            for (int i = start; j < count; i++)
            {               
                Expression exp = new Expression(tokens[1].Replace(columnName + "[" + startCopy.ToString() + "]", _rows[i][columnName]));
                if (RowCount <= index)
                {
                    Dictionary<string, string> temp = new Dictionary<string, string>();
                    temp.Add(columnName, exp.Evaluate().ToString());
                    _rows.Add(temp);
                }
                _rows[index][columnName] = exp.Evaluate().ToString();
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
                return _rows.Count;
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
        /// Adds a column for which value gets calculated from a given formula.
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="formula">Formula to calculate values of the new column</param>
        public void AddColumn(string columnName, string formula, int decimalDigits)
        {
            
            string copyFormula = formula;
         

            //List<string> values = new List<string>();
            string[] columns = formula.Split(new char[] { '+', '-', '*', '/', '(', ')',' ' },StringSplitOptions.RemoveEmptyEntries);

            columns = columns.Select(t => t.Replace("[", string.Empty).Replace("]", string.Empty)).Intersect(ColumnHeaders).Select(z=>"["+z+"]").ToArray();

            for (int i = 0; i < RowCount; i++)
            {
                formula = copyFormula;
                foreach (string column in columns)
                {
                    formula = formula.Replace(column, _rows[i][column.Replace("[",string.Empty).Replace("]",string.Empty)]);
                }
         

              //  values.Add(formula);
                _rows[i].Add(columnName, Math.Round(Convert.ToDecimal(new Expression(formula).Evaluate().ToString()),decimalDigits).ToString());         
            }
        }
        //public Table TransformToCurrency(params string[] columns)
        //{
            
            
        //}
        /// <summary>
        /// Removes all currency symbol and comma from the values of the given column.
        /// </summary>
        /// <param name="columns">The name of the column</param>
        /// <returns>A table without the currency symbol and commas</returns>
        public Table TransformCurrencyToNumeric(params string[] columns)
        {
            Table newTable = this;
            foreach (string col in columns)
            {
                newTable =  Transform(col, x => x.Replace(",",string.Empty)
                                                 .Replace("$",string.Empty)//US Dollar
                                                 .Replace("£",string.Empty)//GBP Pound
                                                 .Replace("€",string.Empty)//
                                                 .Replace("¥",string.Empty));//Chinese Yen
            }
            return newTable;
        }
        /// <summary>
        /// Create a column from given values
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <param name="values">given values</param>
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
        public string this[string columnName, int index]
        {
            get
            {
                return this[columnName][index];
            }
        }
        /// <summary>
        /// A generic folder method. This helps to generate 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="how"></param>
        /// <returns></returns>
        public Table CumulativeFold(string columnName, AggregationMethod how)
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
                        }
                    }
                }
                result.AddRow(currentRow);
            }
            return result;
        }
        /// <summary>
        /// Calculates cumulative summation of values in the given column
        /// </summary>
        /// <param name="columName">The given column</param>
        /// <returns>A table with cumulative sums for the given column</returns>
        public Table CumulativeSum(string columnName)
        {            
            Table result = new Table();            
            List<string> allNumericColumns = new List<string>();
            allNumericColumns.AddRange(ColumnHeaders.Where(m => ValuesOf(m).All(z => Char.IsNumber(z,0))));

            for (int i = 0; i < RowCount; i++)
            {
                Dictionary<string, string> currentRow = new Dictionary<string, string>();
                currentRow.Add(columnName, this[i][columnName]);
                foreach (string col in allNumericColumns)
                    if (col != columnName)
                        currentRow.Add(col, this[col].Take(i + 1).Select(s => Convert.ToDecimal(s)).Sum().ToString());
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
        public Table RoundOffTo(int decimalDigits)
        {
            foreach (string col in this.ColumnHeaders)
                for (int r = 0; r < this.RowCount; r++)
                    if (Regex.IsMatch(this[r][col], @"^-?[0-9]\d*(\.\d+)?$"))
                        this[r][col] = Math.Round(Convert.ToDecimal(this[r][col]), decimalDigits).ToString();

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="decimalDigitsForEachColumn"></param>
        /// <returns></returns>
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
        /// <param name="columns">The columns for which values has to be aggregated</param>
        /// <param name="how">The aggregation method</param>
        /// <returns></returns>
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
        /// </summary>
        /// <param name="columnName">Aggregate values for each distinct value in this column</param>
        /// <param name="how">The aggregation scheme to be used</param>
        /// <returns>A flattened table</returns>
        public Table Aggregate(string columnName, AggregationMethod how = AggregationMethod.Sum)
        {
            List<string> allDistinctValues = ValuesOf(columnName)
                                             .Distinct()
                                             .ToList();
            //Month | Item | Quantity  | Revenue
            //----------------------------------
            //Jan   | P C   |  1       | 10000
            //Jan   | Mobile|  3       |  1000
            //Feb   | PC    |  3       | 30000

            //Aggregate("Month",AggregateMethod.Sum);

            //This should generate the following output

            //Month | Quantity | Revenue
            //--------------------------
            //Jan   | 4        | 11000
            //Feb   | 3        | 30000

            List<string> allNumericColumns = new List<string>();
            
            allNumericColumns.AddRange(ColumnHeaders.Where(col =>  ValuesOf(col).All(m => Char.IsNumber(m,0))));           

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
        /// <remarks>
        ///  
                    //Month | Item | Quantity  | Revenue
                    //----------------------------------
                    //Jan   | P C   |  1       | 10000
                    //Jan   | Mobile|  3       |  1000
                    //Feb   | PC    |  3       | 30000

                    //Aggregate("Month",AggregateMethod.Sum);

                    //This should generate the following output

                    //Month | Quantity | Revenue
                    //--------------------------
                    //Jan   | 4        | 11000
                    //Feb   | 3        | 30000
     
        /// </remarks>
        
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
        /// Splits a table on the values of a given column
        /// </summary>
        /// <param name="columnName">The column; depending on values of which we want to perform the split</param>
        /// <returns>A mapping between values of the column and their corresponding table</returns>
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

        #region Data Cleansing
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Table DistinctBy(string columnName)
        {
            return this;
        }
        /// <summary>
        /// Removes duplicate rows from the table
        /// </summary>
        /// <returns>A table with all unique rows</returns>
        public Table Distinct()
        {   
            Dictionary<string, List<Dictionary<string, string>>> noDups =
                new Dictionary<string, List<Dictionary<string, string>>>();

            for (int i = 0; i < _rows.Count; i++)
            {
                string hash = _rows[i].OrderBy(m=>m.Key).Select(m => m.Value).Aggregate((x, y) => x + y);
                if (!noDups.ContainsKey(hash))
                    noDups.Add(hash, new List<Dictionary<string, string>>() { _rows[i] });
                else
                    noDups[hash].Add(_rows[i]);
            }

            Table noDuplicates = new Table();
            noDuplicates._rows = noDups.Select(t => t.Value.First()).ToList();

            return noDuplicates;
        }
        public decimal Median(List<decimal> numbers)
        {
            numbers = numbers.OrderBy(m=>m).ToList();
            if (numbers.Count % 2 == 0)
            {
                return (numbers[numbers.Count / 2] + numbers[numbers.Count / 2 - 1]) / 2;
            }
            else
            {
                return numbers[numbers.Count / 2 ];
            }
        }
        private Tuple<decimal,decimal> IQRRange(List<decimal> numbers)
        {
            decimal median = Median(numbers);
            List<decimal> smaller = numbers.Where(n => n < median).ToList();
            List<decimal> bigger = numbers.Where(n => n > median).ToList();
            decimal Q1 = Median(smaller);
            decimal Q3 = Median(bigger);
            decimal IQR = Q3 - Q1;
            return new Tuple<decimal, decimal>(Q1 - 1.5M * IQR, Q3 + 1.5M * IQR);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Table ExtractOutliers(string columnName, OutlierDetectionAlgorithm algo = OutlierDetectionAlgorithm.IQR_Interval)
        {
            Table outliers = new Table();
            List<decimal> allValues = ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();

            Tuple<decimal, decimal> iqrRange = IQRRange(allValues);
            for (int i = 0; i < allValues.Count; i++)
            {
                if (allValues[i] < iqrRange.Item1 || allValues[i] > iqrRange.Item2)
                    outliers.Rows.Add(this.Rows[i]);
            }
            return outliers;
        }
        /// <summary>
        /// Remove all rows that correspond to a value for the given column which is an outlier
        /// </summary>
        /// <param name="columnName">The name of the column among which the outliers has to be found.</param>
        /// <returns>A cleansed table</returns>
        public Table RemoveOutliers(string columnName, OutlierDetectionAlgorithm algo = OutlierDetectionAlgorithm.IQR_Interval)
        {
            List<decimal> allValues = ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();

            Tuple<decimal,decimal> iqrRange = IQRRange(allValues);
            for (int i = 0; i < allValues.Count; i++)
            {
                if (allValues[i] < iqrRange.Item1 || allValues[i] > iqrRange.Item2)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes those rows at which the value of the given column falls under the given range
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <param name="low">The low value of the range</param>
        /// <param name="high">The high value of the range</param>
        /// <returns>A table with any matching rows deleted.</returns>
        public Table RemoveIfBetween(string columnName, decimal low, decimal high)
        {
            List<decimal> vals = this.ValuesOf(columnName).Select(t => Convert.ToDecimal(t)).ToList();
            for (int i = 0; i < vals.Count(); i++)
            {
                if (low <= vals[i] && vals[i] <= high)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes rows from the table that 
        /// </summary>
        /// <param name="columnName">The numeric column</param>
        /// <param name="low">The lowest value of the range</param>
        /// <param name="high">The highest value of the range</param>
        /// <returns>A cleansed table</returns>
        public Table RemoveIfNotBetween(string columnName, decimal low, decimal high)
        {
            List<decimal> vals = this.ValuesOf(columnName).Select(t => Convert.ToDecimal(t)).ToList();
            for (int i = 0; i < vals.Count(); i++)
            {
                if (!(low <= vals[i] && vals[i] <= high))
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Remove those rows where the values match the given regular expression
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <param name="regexPattern">The regular expression against which we want to validate</param>
        /// <returns>A cleansed table with matching rows removed</returns>
        public Table RemoveMatches(string columnName, string regexPattern)
        {
            List<string> values = this.ValuesOf(columnName).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (Regex.Match(values[i], regexPattern).Success)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Remove those rows where the values don't match with the given regular expression
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <param name="regexPattern">The regular expression</param>
        /// <returns>A cleansed table where the non matching rows are removed.</returns>
        public Table RemoveNonMatches(string columnName, string regexPattern)
        {
            List<string> values = this.ValuesOf(columnName).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (!Regex.Match(values[i], regexPattern).Success)
                    this.Rows.RemoveAt(i);
            }
            return this;

        }
        /// <summary>
        /// Removes those rows from the given table where the value of the given date 
        /// occurs before the date in the column given.
        /// </summary>
        /// <param name="dateColumnName">The date column</param>
        /// <param name="date">The value of the date</param>
        /// <returns>A table with those rows where the date occurs before the given date; removed.</returns>
        public Table RemoveIfBefore(string dateColumnName, DateTime date)
        {
            List<DateTime> dates = this.ValuesOf(dateColumnName)
                                       .Select(m => Convert.ToDateTime(m))
                                       .ToList();

            for (int i = 0; i < dates.Count; i++)
            {
                if (dates[i].CompareTo(date) < 0)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes those rows from the given table for the given column
        /// </summary>
        /// <param name="dateColumnName">The date column</param>
        /// <param name="date">The terminal date after which we need to remove values</param>
        /// <returns>A cleansed table</returns>
        public Table RemoveIfAfter(string dateColumnName, DateTime date)
        {
            List<DateTime> dates = this.ValuesOf(dateColumnName).Select ( m => Convert.ToDateTime(m)).ToList();

            for (int i = 0; i < dates.Count; i++)
            {
                if (dates[i].CompareTo(date) >= 1)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Remove all the rows where the date does fall in the given range.
        /// </summary>
        /// <param name="dateColumnName">The date column on which we want to run the filter</param>
        /// <param name="startDate">The start date of the range</param>
        /// <param name="endDate">The end date of the range</param>
        /// <returns>A cleansed table</returns>
        public Table RemoveIfBetween(string dateColumnName, DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = this.ValuesOf(dateColumnName).Select(t => Convert.ToDateTime(t)).ToList();
            for (int i = 0; i < dates.Count; i++)
            {
                if (dates[i].CompareTo(startDate) >= -1 && dates[i].CompareTo(endDate) <= 1)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Remove all the rows where the date doesn't fall in between the given range.
        /// </summary>
        /// <param name="dateColumnName">The date column</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A cleansed table with the violating rows removed.</returns>
        public Table RemoveIfNotBetween(string dateColumnName, DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = this.ValuesOf(dateColumnName).Select(t => Convert.ToDateTime(t)).ToList();
            for (int i = 0; i < dates.Count; i++)
            {
                if (!(dates[i].CompareTo(startDate) >= -1 && dates[i].CompareTo(endDate) <= 1))
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes items that are not in the list of expected values.
        /// </summary>
        /// <param name="columnName">The name of the column on which we want to run this filter</param>
        /// <param name="expectedValues">Set of expected values</param>
        /// <returns>A cleansed table.</returns>
        public Table RemoveIfNotAnyOf(string columnName, params string[] expectedValues)
        {
            List<string> values = this.ValuesOf(columnName).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (!expectedValues.Contains(values[i]))
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Remove all rows that has an illegal value in the given column
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <param name="illegalValues">Set of illegal values</param>
        /// <returns>A cleansed table with rows with illegal values removed.</returns>
        public Table RemoveIfAnyOf(string columnName, params string[] illegalValues)
        {
            List<string> values = this.ValuesOf(columnName).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (illegalValues.Contains(values[i]))
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes items that are less than the given value
        /// </summary>
        /// <param name="columnName">The column for which the values have to be checked </param>
        /// <param name="value">The basis value below which all values has to be removed.</param>
        /// <returns>A Table with violating values removed.</returns>
        public Table RemoveLessThan(string columnName, decimal value)
        {
            List<decimal> values = this.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] < value)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes items that are equal to or less than the given value 
        /// </summary>
        /// <param name="columnName">The column for which values has to be checked against the given value </param>
        /// <param name="value">The given value</param>
        /// <returns>A table with violating values removed from the specified column.</returns>
        public Table RemoveLessThanOrEqualTo(string columnName, decimal value)
        {
            List<decimal> values = this.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] <= value)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes rows where the values of the given column are greater than the decimal value
        /// </summary>
        /// <param name="columnName">The column name for which the values need to be removed.</param>
        /// <param name="value">The value (The threshold value)</param>
        /// <returns></returns>
        public Table RemoveGreaterThan(string columnName, decimal value)
        {
            List<decimal> values = this.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > value)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Removes values greater than or equal to the given value for the given column
        /// </summary>
        /// <param name="columnName">The column for which the given values have to be removed. </param>
        /// <param name="value">The given value; values greater than which has to be removed.</param>
        /// <returns>A table with violating values removed.</returns>
        public Table RemoveGreaterThanOrEqualTo(string columnName, decimal value)
        {
            List<decimal> values = this.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] >= value)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Generic remove function. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Table RemoveIf<T>(string columnName, Func<T, bool> predicate) where T : IEquatable<T>
        {
            int i = 0;
            List<int> indicesToRemove = new List<int>();
            List<T> castedList = this.ValuesOf(columnName).Cast<T>().ToList();
            for (; i < castedList.Count; i++)
            {
                T temp = castedList[i];
                if (predicate(temp))
                {
                    indicesToRemove.Add(i);
                }
            }
            indicesToRemove.ForEach(k => this.Rows.RemoveAt(k));
            return this;
        }
        /// <summary>
        /// Rempves list of rows that doesn't match a given condition. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Table RemoveNotIf<T>(string columnName, Func<T, bool> predicate) where T : IEquatable<T>
        {
            int i = 0;
            List<int> indicesToRemove = new List<int>();
            List<T> castedList = this.ValuesOf(columnName).Cast<T>().ToList();
            for (; i < castedList.Count; i++)
            {
                T temp = castedList[i];
                if (!predicate(temp))
                {
                    indicesToRemove.Add(i);
                }
            }
            indicesToRemove.ForEach(k => this.Rows.RemoveAt(k));
            return this;
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
        public Table MergeByColumns(Table anotherTable, string connectorColumn = "Not Provided")
        {
            //This is an example use case of this method.
            //Table 1 Columns
            //Name  | Age | Gender 
            //Sam   | 23  | M
            //Jane  | 19  | F
            //Raskin| 14  | M

            //Table 2 Columns
            //Name | Course 
            //Jane | C#
            //Sam  | F#
            //Raskin| Python

            
            //Merged Columns in the resultant Table.
            //Name | Age | Gender | Course
            //Sam  | 23  | M      | F#
            //Jane | 19  | F      | C#
            //Raskin| 14 | M      | Python


            if (connectorColumn == "Not Provided")
                connectorColumn = this.ColumnHeaders.First();

            Table mergedColumnsTable = new Table();

            this.ColumnHeaders.ToList().ForEach(m => mergedColumnsTable.AddColumn(m, this.ValuesOf(m)));
            
            anotherTable.ColumnHeaders.Where(t=>t!=connectorColumn)
                       .ToList().ForEach(m =>
                                    mergedColumnsTable.AddColumn(m, anotherTable.SortInThisOrder(connectorColumn, this.ValuesOf(connectorColumn)).ValuesOf(m)));


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
                    return mergedTable.Distinct();
                else
                    return mergedTable;
            }
        }
        /// <summary>
        /// Extracts those rows from the table which are not present in the another one.
        /// </summary>
        /// <param name="anotherTable">The other table with which we have to compare.</param>
        /// <returns>A new table with rows that are only available in the current table, not in the other one.</returns>
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
        public Table Common(Table anotherTable)
        {
            if (anotherTable == null)
                throw new ArgumentNullException("Table");

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
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="how"></param>
        /// <returns></returns>
        public Table SplitColumns(string columnName, Func<string, List<KeyValuePair<string, string>>> how)
        {
            return this;
        }
        /// <summary>
        /// Returns a table with merged columns 
        /// </summary>
        /// <param name="newColumnName">The new column header</param>
        /// <param name="separator">The character separator to use between values of participating columns</param>
        /// <param name="columns">Columns to merge</param>
        /// <returns>Returns a table with the merged column</returns>
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
        public Table Drop(params string[] columns)
        {
            return Pick(ColumnHeaders.Except(columns).ToArray());
        }
        /// <summary>
        /// Returns a table with just the columns mentioned.
        /// </summary>
        /// <param name="columns">Name of the columns that we want to show.</param>
        /// <returns>A table with only the columns mentioned in the parameter </returns>
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
        /// Random Sample
        /// </summary>
        /// <param name="sampleSize">a</param>
        /// <returns></returns>
        public Table RandomSample(int sampleSize)
        {
            return this.Shuffle().Top(sampleSize);
        }
        
         
        
        /// <summary>
        /// Retuns top n rows 
        /// </summary>
        /// <param name="n">n number </param>
        /// <returns>A table with top n rows</returns>
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
        /// <param name="n">number of n</param>
        /// <returns></returns>
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
        /// <param name="n"></param>
        /// <returns></returns>
        public Table TopNPercent(int n)
        {
            int rowCount = Convert.ToInt16( Math.Floor((float) this.RowCount * n / 100.0));
            return this.Top(rowCount);
        }
        /// <summary>
        /// Returns those rows in form of a table for which the value 
        /// of the given column is above average for the column.
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>A table with values above average for the given column.</returns>
        public Table AboveAverage(string columnName)
        {
            List<decimal> values = this.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();

            decimal average = values.Average();

            for (int i = 0; i < RowCount; i++)
            {
                if (values[i] < average)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }
        /// <summary>
        /// Returns those rows in form of a table for which the value of the given column 
        /// is below average for the given column
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <returns>A table with values less than average for that particular column.</returns>
        public Table BelowAverage(string columnName)
        {
            List<decimal> values = this.ValuesOf(columnName).Select(m => Convert.ToDecimal(m)).ToList();

            decimal average = values.Average();

            for (int i = 0; i < RowCount; i++)
            {
                if (values[i] >= average)
                    this.Rows.RemoveAt(i);
            }
            return this;
        }

        /// <summary>
        /// Returns the bottom n % entries as a new table
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
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
        /// <param name="rowsPerSplit"></param>
        /// <returns></returns>
        public List<Table> SplitByRows(int rowsPerSplit)
        {
            List<Table> splits = new List<Table>();

            return Enumerable.Range(0, this.RowCount - rowsPerSplit + 1)
                             .Select(m => this.Middle(m * rowsPerSplit, rowsPerSplit))
                             .ToList();
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnSplitDescription"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="pinnedColumn"></param>
        /// <param name="columnsSplitDescription"></param>
        /// <returns></returns>
        public List<Table> SplitByColumns(string pinnedColumn, string columnSplitDescription)
        {
            string split = "{Jan,Feb,Mar},{Apr,May,Jun,July}";

            return new List<Table> { this };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinnedColumn"></param>
        /// <param name="columnsPerSplit"></param>
        /// <returns></returns>
        public List<Table> SplitByColumns(string pinnedColumn, int columnsPerSplit)
        {
            return new List<Table> { this };
        }
        
        /// <summary>
        /// Random shuffle
        /// </summary>
        public Table Shuffle()
        {            
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
        /// <returns></returns>
        private bool IsSameRow(Dictionary<string, string> firstRow, Dictionary<string, string> secondRow)
        {
            return firstRow.Keys.All(k => secondRow.Keys.Contains(k)) &&
                firstRow.Keys.All(k => secondRow[k] == firstRow[k]);
        }
        #endregion
        #region Natural Query
        /// <summary>
        /// 
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

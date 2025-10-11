using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ExcelDataReader;
using HtmlAgilityPack;
using Parquet;
using Parquet.Schema;
using ParquetSharp;
using Squirrel;

namespace Squirrel
{
    
    /// <summary>
    /// The class that holds the data I/O methods for several file formats.
    /// </summary>
    public static class DataAcquisition
    {
        /// <summary>
        /// Loads data from a CSV file with custom delimiters.
        /// </summary>
        /// <param name="csvFileName">The name of the CSV file.</param>
        /// <param name="delimiter">The delimiter used in the CSV file.</param>
        /// <returns>A table with all the values from the CSV file.</returns>
        public static Table LoadCsv(string csvFileName, char delimiter)
        {
            // Method implementation goes here
            return new Table();
        }

        /// <summary>
        /// Loads data from a Google Sheets document.
        /// </summary>
        /// <param name="spreadsheetId">The ID of the Google Sheets document.</param>
        /// <param name="range">The range of cells to load.</param>
        /// <returns>A table with all the values from the Google Sheets document.</returns>
        public static Table LoadGoogleSheets(string spreadsheetId, string range)
        {
            // Method implementation goes here
            return new Table();
        }

        /// <summary>
        /// Loads data from an API endpoint.
        /// </summary>
        /// <param name="apiUrl">The URL of the API endpoint.</param>
        /// <returns>A table with all the values from the API response.</returns>
        public static Table LoadApi(string apiUrl)
        {
            // Method implementation goes here
            return new Table();
        }

        /// <summary>
        /// Loads data from a MongoDB collection.
        /// </summary>
        /// <param name="connectionString">The connection string to the MongoDB database.</param>
        /// <param name="databaseName">The name of the MongoDB database.</param>
        /// <param name="collectionName">The name of the MongoDB collection.</param>
        /// <returns>A table with all the values from the MongoDB collection.</returns>
        public static Table LoadMongoDb(string connectionString, string databaseName, string collectionName)
        {
            // Method implementation goes here
            return new Table();
        }

        /// <summary>
        /// Loads data from an Excel file with multiple sheets.
        /// </summary>
        /// <param name="filePath">The path of the Excel file.</param>
        /// <param name="sheetName">The name of the sheet to load.</param>
        /// <returns>A table with all the values from the specified sheets.</returns>
        public static Table LoadExcel(string filePath, string sheetName = "Sheet1")
        {
            var loaded = new Table();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Make sure this is called before any ExcelDataReader operations
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var index  = result.Tables.IndexOf(sheetName);
                    var t = result.Tables[index];
                    loaded = LoadDataTable(t);
                    loaded.Name = t.TableName;
                    // The result of each spreadsheet is in result.Tables
                }
            }
            var colHeaderNames = loaded.Rows[0].Keys.ToList();
            var revColNameMap = loaded.Rows[0];
            var colValues =
                colHeaderNames.ToDictionary(col => revColNameMap[col], 
                    col => loaded[col].Skip(1).ToList());
            var modLoad = new Table();
            foreach (var col in colValues.Keys)
            {
                modLoad.AddColumn(col, colValues[col]);
            }

            modLoad.Name = loaded.Name;
            return modLoad;
        }

       
        /// <summary>
        /// Loads data from a JSON file.
        /// </summary>
        /// <param name="jsonFileName">The name of the JSON file.</param>
        /// <returns>A table with all the values from the JSON file.</returns>
        public static RecordTable<T> LoadJson<T>(string jsonFileName)
        {
            RecordTable<T> t = new RecordTable<T>() { Rows =  []};
            // Method implementation goes here
            return t;
        }

        /// <summary>
        /// Loads data from an XML file.
        /// </summary>
        /// <param name="xmlFileName">The name of the XML file.</param>
        /// <returns>A table with all the values from the XML file.</returns>
        public static Table LoadXml(string xmlFileName)
        {
            // Method implementation goes here
            return new Table();
        }

        

        /// <summary>
        /// Loads data from an Avro file.
        /// </summary>
        /// <param name="avroFileName">The name of the Avro file.</param>
        /// <returns>A table with all the values from the Avro file.</returns>
        public static Table LoadAvro(string avroFileName)
        {
            // Method implementation goes here
            return new Table();
        }

        /// <summary>
        /// Loads data from a YAML file.
        /// </summary>
        /// <param name="yamlFileName">The name of the YAML file.</param>
        /// <returns>A table with all the values from the YAML file.</returns>
        public static Table LoadYaml(string yamlFileName)
        {
            // Method implementation goes here
            return new Table();
        }



       

        /// <summary>
        /// Creates a table out of a list of anonynous type objects. 
        /// This is useful when you are creating a bunch of objects 
        /// of anonymous type and want to 
        /// </summary>
        /// <typeparam name="T">Type of the anonynous type</typeparam>
        /// <param name="list">list of anonynous objects</param>
        /// <returns>A table with rows created from a list of anonymnous objects</returns>
        /// <summary>
        /// Creates a table out of a list of anonymous type objects.
        /// This is useful when you are creating a bunch of objects
        /// of anonymous type and want to create a table using those objects  
        /// as rows of the table.
        /// </summary>
        /// <typeparam name="T">Type of the anonymous type</typeparam>
        /// <param name="list">list of anonymous objects</param>
        /// <returns>A table with rows created from a list of anonymous objects</returns>
        public static Table ToTableFromAnonList<T>(this IEnumerable<T> list) where T : class
        {
            Table result = new();
            var firstType = list.First().GetType();
            List<string> props = firstType.GetProperties().Select(l => l.Name).ToList();
            
            foreach (var l in list)
            {
                Dictionary<string, string> row = new();
                foreach (var p in props)
                    row.Add(p, firstType.GetProperty(p)!.GetValue(l)!.ToString()!);
                result.Rows.Add(row);
            }

            return result;
        }

        /// <summary>
        /// Creates a strongly typed table instance RecordTable<T>
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static RecordTable<T> ToRecordTableFromAnonList<T>(this IEnumerable<T> list)
        {
            var tab = new RecordTable<T>{Rows = []};
            foreach (var v in list)
            {
                tab.Rows.Add(v);
            }

            return tab;
        }
        /// <summary>
        /// Loads the data from an Excel workbook to a table
        /// </summary>
        /// <param name="fileName">The name of the Excel file</param>
        /// <param name="workbookName">The name of the workbook</param>
        /// <returns>A table which holds the values from the workbook.</returns>
        public static Table LoadXls(string fileName, string workbookName)
        {
            Table tab = new Table();
            //Use this open source project to read data from Excel.
            //We are standing on the shoulder of giants.
            //http://exceldatareader.codeplex.com/
            //http://code.google.com/p/linqtoexcel/ (Love This!)
            return tab;
        }

        /// <summary>
        /// Loads data from fixed column length files.
        /// </summary>
        /// <param name="fileName">The fixed column length file </param>
        /// <param name="fieldLengthMap">The dictionary that has the mapping of field names and their lengths</param>
        /// <returns>A table with all loaded values.</returns>
        public static Table LoadFixedLength(string fileName, Dictionary<string, int> fieldLengthMap)
        {
            Table thisTable = new Table();

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
        /// Loads data from file with fixed column length. 
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="headersWithLength">Headers with column widths in brackets as shown "name(20),age(2),course(5)"</param>
        /// <returns>A table with all the values loaded.</returns>
        /// <example>Table data = DataAcquisition.LoadFixedLength("data.txt","name(20),age(2),course(5)");</example>
        public static Table LoadFixedLength(string fileName, string headersWithLength) // "name(20),age(2),course(5)"
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
        /// <returns>Returns a table with the loaded values from the .arff files</returns>
        /// <example>Table play = Table.LoadARFF(".\data\play.arff");</example>
        public static Table LoadArff(string fileName)
        {
            Table result = new Table();
            List<string> columnHeaders = new List<string>();
            StreamReader arffReader = new StreamReader(fileName);
            string line = string.Empty;
            while ((line = arffReader.ReadLine()) != null)
            {
                var trimmedLine = line.Trim().ToLower();
                if (trimmedLine.StartsWith("@attribute"))
                    columnHeaders.Add(line.Split(' ')[1]);
                if (trimmedLine.StartsWith("@data"))
                    break;
            }

            List<string> dataLines = File.ReadLines(fileName)
                .Where(line => !line.TrimStart().StartsWith("%") && !line.TrimStart().StartsWith("@"))
                .ToList();

            foreach (string dataLine in dataLines)
            {
                Dictionary<string, string> currentRow = new Dictionary<string, string>();
                string[] tokens = dataLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    for (int i = 0; i < tokens.Length; i++)
                        currentRow.Add(columnHeaders[i], tokens[i]);
                    result.Rows.Add(currentRow);
                }
            }

            return result;
        }
        
       

       
        private static List<string> GetRowData(string tableCode)
        {
            string pattern = @"<td\b[^>]*>(.*?)</td>";

            return Regex.Matches(tableCode, pattern)
                .Cast<Match>()
                .Select(t =>
                    t.Value.Substring(t.Value.IndexOf('>') + 1,
                            t.Value.LastIndexOf('<') - t.Value.IndexOf('>') - 1)
                        .Trim())
							 
                .ToList();


        }   
       

        /// <summary>
        /// Loads an HTML table to the corresponding Table container
        /// </summary>
        /// <param name="htmlTable">The file that holds the HTML code that creates the table</param>
        /// <returns>A table with all the data from the html table</returns>
        public static Table LoadHtmlTable(string htmlTable)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(htmlTable));
            List<List<string>> table =
                doc.DocumentNode.SelectSingleNode("//table")
                    .Descendants("tr")  
                    .Where(tr => tr.Elements("td").Count() > 1)
                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim())
                        .ToList())
                    .ToList();

            var headers = new List<string>();
            if (doc.DocumentNode.InnerHtml.Contains("<th ") ||
                doc.DocumentNode.InnerHtml.Contains("<th>"))
            {
                headers = doc.DocumentNode.SelectNodes("//th")
                    .Select(t => t.InnerText).ToList();
            }
            else
            {
                if (headers.Count == 0)
                {
                    headers = table[0];
                    table = table.Skip(1).ToList();
                }
            }
            var tab = new Table();
            for (var index = 0; index < table.Count; index++)
            {
                var t = table[index];
                var row = new Dictionary<string, string>();
                for (int colIndex = 0; colIndex < headers.Count; colIndex++)
                {
                    row.Add(headers[colIndex], t[colIndex]);
                }

                tab.AddRow(row);
            }

            tab.Name = Path.GetFileNameWithoutExtension(htmlTable);
            return tab;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<string> GetValues(ReadOnlySpan<char> line, ReadOnlySpan<char> delims, string prefix = "")
        {
            line = prefix + line.ToString() ;
            var values = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;
            int len = line.Length;

            for (int i = 0; i < len; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (!inQuotes && delims.Contains(c))
                {
                    AddValue(line.Slice(startIndex, i - startIndex), values);
                    startIndex = i + 1;
                }
            }

            AddValue(line.Slice(startIndex), values);
            return values;




        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddValue(ReadOnlySpan<char> value, List<string> values)
        {
            value = value.Trim();
            if (value.IsEmpty)
            {
                values.Add(string.Empty);
                return;
            }

            if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            {
                value = value.Slice(1, value.Length - 2);
            }

            values.Add(value.IndexOf('"') != -1 ? UnescapeQuotes(value) : value.ToString());
        }

        static string UnescapeQuotes(ReadOnlySpan<char> value)
        {
            Span<char> result = stackalloc char[value.Length];
            int resultIndex = 0;
            bool skipNext = false;

            for (int i = 0; i < value.Length; i++)
            {
                if (skipNext)
                {
                    skipNext = false;
                    continue;
                }

                if (value[i] == '"' && i + 1 < value.Length && value[i + 1] == '"')
                {
                    skipNext = true;
                }

                result[resultIndex++] = value[i];
            }

            return new string(result.Slice(0, resultIndex));
        }

        /// <summary>
        /// Returns a list of values from each CSV row
        /// </summary>
        /// <param name="line">The string representation of a CSV row</param>
        /// <returns>Values of the line</returns>
        static List<string> GetValues2(string line, char[] delims)
        {
            string magic = Guid.NewGuid().ToString();
            //if wrapped with quotes from both sides, ignore and trim
            //else
            if (!line.Contains("\""))
                return line.Split(delims).ToList();
            string[] toks = line.Split(new char[] { ',' }, StringSplitOptions.None);

            List<int> startingWithQuote = new List<int>();
            List<int> endingWithQuote = new List<int>();
            for (int i = 0; i < toks.Length; i++)
            {
                if (toks[i].Length > 0)
                {
                    if (toks[i][0] == '"' && toks[i][toks[i].Length - 1] == '"')
                        continue;

                    if (toks[i].StartsWith("\""))
                        startingWithQuote.Add(i);
                    if (toks[i].EndsWith("\""))
                        endingWithQuote.Add(i);


                }
            }

            List<string> values = new List<string>();

            for (int i = 0; i < toks.Length; i++)
            {
                //if (toks[i] == magic)
                //    continue;
                if (!startingWithQuote.Contains(i) && !endingWithQuote.Contains(i))
                {
                    values.Add(RemoveDoubleQuoteFromEndIfPossible(toks[i].Replace("\"\"",
                        "\""))); //.Trim(new char[] { ' ', '"' }));
                }


                else
                {
                    List<string> innerToks = new List<string>();
                    if (startingWithQuote.Count > 0 && endingWithQuote.Count > 0)
                    {
                        for (int k = startingWithQuote[0]; k <= endingWithQuote[0]; k++)
                        {
                            innerToks.Add(toks[k]);


                        }

                        string concatenatedValue = innerToks.Aggregate((m, n) => m + "," + n);
                        concatenatedValue = concatenatedValue.Substring(1, concatenatedValue.Length - 2);
                        values.Add(RemoveDoubleQuoteFromEndIfPossible(concatenatedValue.Replace("\"\"", "\"")));
                        int startingRemoveIndex = startingWithQuote[0];
                        int endingRemoveIndex = endingWithQuote[0];
                        startingWithQuote.RemoveAt(0);
                        endingWithQuote.RemoveAt(0);
                        for (int m = startingRemoveIndex; m <= endingRemoveIndex; m++)
                            toks[m] = magic;
                        continue;
                    }
                    else
                        values.Add(toks[i]);
                }
            }

            return values.ToList();


        }

        /// <summary>
        /// Loads a CSV file to a respective Table data structure.
        /// </summary>
        /// <param name="csvFileName">The file for which values has to be loaded into a table data structure.</param>
        /// <param name="hasHeader">True if the CSV has headers. Default is set to true.</param>
        /// <returns>A table which has all the values in the CSV file</returns>
        public static Table LoadCsv(string csvFileName, bool hasHeader = true)
        {


            return LoadFlatFile(csvFileName, hasHeader, new char[] { ',' });

        }

        /// <summary>
        /// Loads data from a ADO.NET DataTable to a Table
        /// </summary>
        /// <param name="dt">The DataTable</param>
        /// <returns>A Table</returns>
        /// <example>Table loadedDataTable = DataAcquisition.LoadDataTable(dt);</example>
        public static Table LoadDataTable(DataTable dt)
        {
            Table temp = new Table();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> rowRep = new Dictionary<string, string>();
                foreach (var col in dt.Columns.Cast<DataColumn>().Select(z => z.ColumnName))
                {
                    rowRep.Add(col, row[col].ToString());
                }

                temp.AddRow(rowRep);
            }

            return temp;
        }

        /// <summary>
        /// Loads Data from Tab Separated File
        /// </summary>
        /// <param name="tsvFileName">The file name to read from</param>
        /// <returns>A table loaded with these values</returns>
        public static Table LoadTsv(string tsvFileName, bool hasHeader)
        {
            return LoadFlatFile(tsvFileName, hasHeader, new char[] { '\t' });
        }

        /// <summary>
        /// Loads data from any flat file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="delimeters">Delimeters</param>
        /// <returns>A table loaded with all the values in the file.</returns>
        public static Table LoadFlatFile(string fileName, bool hasHeader, char[] delimeters)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            string prefix = "";
            string suffix = "";
            string lastOne = "";
            Table loadedCsv = new Table();
            StreamReader csvReader = new StreamReader(fileName);
            string line = string.Empty;
            int lineNumber = 0;
            HashSet<string> columns = new HashSet<string>();
            while ((line = csvReader.ReadLine()) != null)
            {

            
                if (hasHeader && lineNumber == 0) //reading the column headers
                {
                    //if (line.Contains("\""))
                    //    line = "\"" + line + "\"";
                    //Because sometimes the column header can have a comma in them
                    //and that can spoil the order
                    GetValues(line, delimeters, prefix).ForEach(col => columns.Add(col));
                    //  line.Split(delimeters, StringSplitOptions.None)
                    //    .ToList()
                    //  .ForEach(col => columns.Add(col.Trim(new char[] { '"', ' ' })));
                    lineNumber++;
                }
                else
                {
                    string[] values = null;
                    if (line.Trim().Length > 0)
                    {
                        values = GetValues(line, delimeters, prefix).ToArray();
                        if (values.Length == 0)
                            values = line.Split(delimeters, StringSplitOptions.None);
                        if (values.Length == columns.Count)
                        {
                            lastOne = values.Last();
                            if (lastOne.StartsWith("\"") || lastOne.EndsWith("\""))
                            {
                                prefix += line;
                                continue;
                            }
                           
                            var tempRow = new Dictionary<string, string>();
                                for (int i = 0; i < values.Length; i++)
                                {
                                    try
                                    {
                                        tempRow.Add(columns.ElementAt(i),
                                            values[
                                                i]); // RemoveDoubleQuoteFromEndIfPossible(values[i].Trim().Replace("\"\"", "\"")));
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                
                                loadedCsv.AddRow(tempRow);
                                prefix = "";
                            

                        }
                        else
                        {
                            prefix += line;
                        }
                    }
                }
            }

            loadedCsv.Name = name;
            return loadedCsv;
        }

        private static string RemoveDoubleQuoteFromEndIfPossible(string input)
        {
            //List<int> indices = new List<int>();
            //for (int x = 0; x < input.Length; x++)
            //    if (input[x] == '"')
            //        indices.Add(x);

            //if (indices.Count == 2 && indices[0] == 0 && indices[1] == input.Length - 1)
            //    return input.Trim(new char[] { ' ', '"' });
            //else
            return input;
        }

        /// <summary>
        /// Dumps the table in a pretty format to console.
        /// </summary>
        /// <param name="tab">The table to be dumped.</param>
        /// <param name="headerColor">The header foreground color</param>
        /// <param name="rowColor">The row color</param>
        /// <param name="header">The header for the table</param>
        /// <param name="align">The alignment. Possible values are left or right</param>
        /// <example>tab.PrettyDump();//The default dump </example>
        /// <example>tab.PrettyDump(header:"Sales Report");//dumping the table with a header</example>
        /// <example>tab.PrettyDump(header:"Sales Report", align:Alignment.Left);//Right alignment is default</example>        
        public static void PrettyDump(this Table tab, ConsoleColor headerColor = ConsoleColor.Green,
            ConsoleColor rowColor = ConsoleColor.White,
            string header = "None", Alignment align = Alignment.Right)
        {
            if (header != "None")
                Console.WriteLine(header);
            Dictionary<string, int> longestLengths = new Dictionary<string, int>();

            foreach (string col in tab.ColumnHeaders)
                longestLengths.Add(col, tab.ValuesOf(col).OrderByDescending(t => t.Length).First().Length);
            foreach (string col in tab.ColumnHeaders)
                if (longestLengths[col] < col.Length)
                    longestLengths[col] = col.Length;
            Console.ForegroundColor = headerColor;
            foreach (string col in tab.ColumnHeaders)
            {
                if (align == Alignment.Right)
                    Console.Write(" " + col.PadLeft(longestLengths[col]) + new string(' ', 4));
                if (align == Alignment.Left)
                    Console.Write(" " + col.PadRight(longestLengths[col]) + new string(' ', 4));
            }

            Console.WriteLine();
            Console.ForegroundColor = rowColor;
            for (int i = 0; i < tab.RowCount; i++)
            {
                foreach (string col in tab.ColumnHeaders)
                {
                    if (!tab.Rows[i].ContainsKey(col)) continue;
                    switch (align)
                    {
                        case Alignment.Right:
                            Console.Write($" {tab.Rows[i][col].PadLeft(longestLengths[col])}{new string(' ', 4)}");
                            break;
                        case Alignment.Left:
                            Console.Write($" {tab.Rows[i][col].PadRight(longestLengths[col])}{new string(' ', 4)}");
                            break;
                    }
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Returns the tabular representation of a gist 
        /// </summary>
        /// <param name="gist">The pre-calculated gist</param>
        /// <returns>A table representing the gist</returns>
        /// <example>Tab gistTab = tab.Gist().ToHtmlTable();</example>
        public static Table ToTable(this List<Tuple<string, string, string>> gist)
        {

            var dic = gist.ToLookup(g => g.Item1).Select(g => new KeyValuePair<string, IEnumerable<string>>(g.Key,
                g.Select(x => x.Item2 + "=" + x.Item3))); //.Dump();
            Table gistTable = new Table();
            foreach (var v in dic)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("Header", v.Key);
                foreach (var z in v.Value)
                {
                    string[] toks = z.Split('=');
                    row.Add(toks[0], toks[1]);
                }

                gistTable.Rows.Add(row);
            }

            return gistTable;


        }

        /// <summary>
        /// Returns the html table representation of the table.
        /// </summary>
        /// <param name="tab"></param>
        /// <returns>A string representing the table in HTML format.</returns>
        public static string ToHtmlTable(this Table tab)
        {
            if (tab.RowCount == 0)
                return "<table></table>";
    
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine("<table>");
    
            // Build header row
            tableBuilder.AppendLine("<thead><tr>");
            foreach (string header in tab.ColumnHeaders)
            {
                tableBuilder.AppendLine("<th>" + header + "</th>");
            }
            tableBuilder.AppendLine("</tr></thead>");
    
            // Build body rows - iterate through Rows directly
            tableBuilder.AppendLine("<tbody>");
            foreach (var row in tab.Rows)
            {
                tableBuilder.AppendLine("<tr>");
                foreach (string header in tab.ColumnHeaders)
                {
                    tableBuilder.AppendLine("<td>" + row[header] + "</td>");
                }
                tableBuilder.AppendLine("</tr>");
            }
            tableBuilder.AppendLine("</tbody>");
    
            tableBuilder.AppendLine("</table>");
            return tableBuilder.ToString();
        }

        /// <summary>
        /// Generates a CSV representation of the table
        /// </summary>
        /// <returns>a string with the table as csv</returns>
        public static string ToCsv(this Table tab)
        {
            return tab.ToValues(',');
        }

        /// <summary>
        /// Generates a TSV representation of the table
        /// </summary>
        /// <returns>a string with the table as TSV value</returns>
        public static string ToTsv(this Table tab)
        {
            return tab.ToValues('\t');
        }

        private static string ToValues(this Table tab, char delim)
        {
            if(tab.ColumnHeaders.Count == 0) return string.Empty;
            Func<string, string> quote = x => "\"" + x + "\"";
            StringBuilder csvOrtsvBuilder = new StringBuilder();
            //Append column headers 
            csvOrtsvBuilder.AppendLine(tab.ColumnHeaders.Aggregate((a, b) => quote(a) + delim.ToString() + quote(b)));
            //Append rows 
            for (int i = 0; i < tab.RowCount - 1; i++)
            {
                foreach (string header in tab.ColumnHeaders)
                {
                    csvOrtsvBuilder.Append(quote(tab[header, i]));
                    csvOrtsvBuilder.Append(delim);
                }

                csvOrtsvBuilder.Append(quote(tab[tab.ColumnHeaders.ElementAt(tab.ColumnHeaders.Count - 1),
                    tab.RowCount - 1]));
                csvOrtsvBuilder.AppendLine(); //Push in a new line.
            }

            return csvOrtsvBuilder.ToString();
        }

        /// <summary>
        /// Generates a DataTable out of the current Table 
        /// </summary>
        /// <returns></returns>
        public static DataTable ToDataTable(this Table tab)
        {
            DataTable thisTable = new DataTable();
            tab.ColumnHeaders.ToList().ForEach(m => thisTable.Columns.Add(m));

            foreach (var row in tab.Rows)
            {
                DataRow dr = thisTable.NewRow();
                foreach (string column in tab.ColumnHeaders)
                    dr[column] = row[column];
                thisTable.Rows.Add(dr);
            }

            return thisTable;
        }

        /// <summary>
        /// Returns the string representations of the table as a ARFF file. 
        /// </summary>
        /// <returns></returns>
        public static string ToArff(this Table tab)
        {
            StringBuilder arffBuilder = new StringBuilder();

            foreach (string header in tab.ColumnHeaders)
            {
                arffBuilder.AppendLine("@attribute " + header + " {" + tab.ValuesOf(header)
                    .Distinct()
                    .Aggregate((a, b) => a + "," + b) + "}");
            }

            arffBuilder.AppendLine("@data");
            for (int i = 0; i < tab.RowCount; i++)
            {
                List<string> values = new List<string>();
                foreach (string header in tab.ColumnHeaders)
                    values.Add(tab[header][i]);
                arffBuilder.AppendLine(values.Aggregate((a, b) => a + "," + b));
            }

            return arffBuilder.ToString();
        }

        /// <summary>
        /// Converts a table to a RecordTable 
        /// </summary>
        /// <param name="tab">The table to be converted</param>
        /// <typeparam name="T">The type of the record</typeparam>
        /// <returns>A record table</returns>
        public static RecordTable<T> AsRecordTable<T>(this Table tab) => RecordTable<T>.FromTable(tab);

        public static void LoadParquet(string path)
        {
            using var file = new ParquetFileReader(path);

            for (int rowGroup = 0; rowGroup < file.FileMetaData.NumRowGroups; ++rowGroup)
            {
                using var rowGroupReader = file.RowGroup(rowGroup);
                var groupNumRows = checked((int)rowGroupReader.MetaData.NumRows);

                var colNames
                    = ((ParquetSharp.RowGroupMetaData)rowGroupReader.MetaData).Schema.GroupNode.Fields
                    .Select(x => x.Name).ToList();

                var colVals = rowGroupReader.Column(3).LogicalReader()
                    .Apply(new ColumnPrinter())
                    .Split(new char[]{','})
                    .ToList();
                
                var groupTimestamps = rowGroupReader.Column(0).LogicalReader<DateTime>().ReadAll(groupNumRows);
                var groupObjectIds = rowGroupReader.Column(1).LogicalReader<int>().ReadAll(groupNumRows);
                var groupValues = rowGroupReader.Column(2).LogicalReader<float>().ReadAll(groupNumRows);

                Console.WriteLine("Read Parquet file:");
                for (int i = 0; i < groupNumRows; ++i)
                {
                    Console.WriteLine($"Timestamp: {groupTimestamps[i]}, ObjectId: {groupObjectIds[i]}, Value: {groupValues[i]}");
                }
            }

            file.Close();
        }
        
        // Method 1: Read S3 object content as string (improved version of your method)
        public static async Task<string> ReadS3ObjectAsString(string accessKey, string secretKey,
            string bucketName, string objectKey)
        {
            var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);
    
            var getRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            using var response = await s3Client.GetObjectAsync(getRequest);
            using var responseStream = response.ResponseStream;
            using var reader = new StreamReader(responseStream);
    
            return await reader.ReadToEndAsync();
        }
        public static async Task<List<S3Object>> ListAllS3Objects(string accessKey, string secretKey, string bucketName)
        {
            var s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                RegionEndpoint.USEast1
            );
    
            var allObjects = new List<S3Object>();
            string continuationToken = null;
    
            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 1000, // Maximum objects per request
                    ContinuationToken = continuationToken
                };
        
                var response = await s3Client.ListObjectsV2Async(request);
                allObjects.AddRange(response.S3Objects);
                continuationToken = response.NextContinuationToken;
        
            } while (continuationToken != null);
    
            return allObjects;
        }
        public static async Task<string> LoadFromS3AsString(string accessKey, string secretKey,
            string bucketName, string resourceName)
        {
            var s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                RegionEndpoint.USEast1
            );
            
            //READ Content of S3 
            // Download a specific object
            var getRequest = new GetObjectRequest
            {
                BucketName = bucketName, 
                Key = resourceName// The file path/name in S3
            };

            StreamWriter sw = new StreamWriter("test.csv");
            using (var response = await s3Client.GetObjectAsync(getRequest))
            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                sw.WriteLine(reader.ReadToEnd());
            }
            sw.Close();
            //TODO: Depending on the extension
            // We can load any pdefined functions.
            return File.ReadAllText("test.csv");
        }
        /// <summary>
        /// Reads data from S3 bucket
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="bucketName"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        /// <remarks>Look here http://bit.ly/4mCY5PL</remarks>
        public static async Task<Table> LoadFromS3(string accessKey, string secretKey,
            string bucketName, string resourceName)
        {
            var s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                RegionEndpoint.USEast1
            );
            
            //READ Content of S3 
            // Download a specific object
            var getRequest = new GetObjectRequest
            {
                BucketName = bucketName, 
                Key = resourceName// The file path/name in S3
            };

            StreamWriter sw = new StreamWriter("test.csv");
            using (var response = await s3Client.GetObjectAsync(getRequest))
            using (var responseStream = response.ResponseStream)
            using (var reader = new StreamReader(responseStream))
            {
                sw.WriteLine(reader.ReadToEnd());
            }
            sw.Close();
            //TODO: Depending on the extension
            // We can load any pdefined functions.
            return DataAcquisition.LoadCsv("test.csv");
        }
    }
}
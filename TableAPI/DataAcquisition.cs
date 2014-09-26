using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    public static class DataAcquisition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        public static Table LoadCSVFromString(string csvString)
        {
            return new Table();
        }

        #region Data I/O from several formats

        /// <summary>
        /// Deletes the tags from a HTML line
        /// </summary>
        /// <param name="codeLine">HTML code from which tags has to be removed</param>
        /// <param name="exceptTheseTags">Remove all tags except this one</param>
        /// <returns></returns>
        private static string StripTags(string codeLine, List<string> exceptTheseTags)
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
            tags.RemoveAll(t => exceptTheseTags.Contains(t));
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
        public static Table LoadXLS(string fileName, string workbookName)
        {
            Table tab = new Table();
            //Use this open source project to read data from Excel.
            //stand on the shoulder of giants.
            //http://exceldatareader.codeplex.com/
            //http://code.google.com/p/linqtoexcel/ (Love This!)
            return tab;
        }
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
        /// Loads data from file with fixed column length. 
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="headersWithLength">Headers with column widths in brackets as shown name(20),age(2),course(5)</param>
        /// <returns>A table with all the values loaded.</returns>
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
            List<string> columnHeaders = new List<string>();
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

            totalTable = totalTable.Replace("\r", string.Empty).Replace("\t", string.Empty).Replace("\n", string.Empty);
            totalTable = totalTable.Replace("<tr><th>", string.Empty)
                              .Replace("</th></tr>", "\"" + Environment.NewLine)
                              .Replace("</th><th>", "\",\"")
                              .Replace("</td></tr>", "\"" + Environment.NewLine)
                              .Replace("</td><td>", "\",\"")
                              .Replace("<tr><td>", "\"" + Environment.NewLine);
            StreamWriter sw = new StreamWriter("TemporaryFile.csv");
            sw.WriteLine(totalTable);
            sw.Close();
            Table loadedTable = LoadCSV("TemporaryFile.csv", true);
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
                return LoadFlatFile(csvFileName, new string[] { "\",\"", "\"" });
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
        public static Table LoadFlatFile(string fileName, string[] delimeters)
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
                    line.Split(delimeters, StringSplitOptions.RemoveEmptyEntries)
                        .ToList()
                        .ForEach(col => columns.Add(col.Trim(new char[] { '"', ' ' })));
                    lineNumber++;
                }
                else
                {
                    string[] values = null;
                    if (line.Trim().Length > 0)
                    {
                        values = line.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);

                        Dictionary<string, string> tempRow = new Dictionary<string, string>();
                        for (int i = 0; i < values.Length; i++)
                        {
                            try
                            {
                                tempRow.Add(columns.ElementAt(i), values[i].Trim(new char[] { '"', ' ' }));
                            }
                            catch { continue; }
                        }
                        loadedCSV.AddRow(tempRow);
                    }
                }
            }
            return loadedCSV;
        }


        /// <summary>
        /// Dumps the table in a pretty format to console.
        /// </summary>
        public static void PrettyDump(this Table tab, string header = "None", Alignment align = Alignment.Right)
        {
            if (header != "None")
                Console.WriteLine(header);
            Dictionary<string, int> longestLengths = new Dictionary<string, int>();

            foreach (string col in tab.ColumnHeaders)
                longestLengths.Add(col, tab.ValuesOf(col).OrderByDescending(t => t.Length).First().Length);
            foreach (string col in tab.ColumnHeaders)
                if (longestLengths[col] < col.Length)
                    longestLengths[col] = col.Length;
            foreach (string col in tab.ColumnHeaders)
            {
                if (align == Alignment.Right)
                    Console.Write(" " + col.PadLeft(longestLengths[col]) + new string(' ', 4));
                if (align == Alignment.Left)
                    Console.Write(" " + col.PadRight(longestLengths[col]) + new string(' ', 4));
            }
            Console.WriteLine();
            for (int i = 0; i < tab.RowCount; i++)
            {
                foreach (string col in tab.ColumnHeaders)
                {
                    if (tab.Rows[i].ContainsKey(col))
                    {
                        if (align == Alignment.Right)
                            Console.Write(" " + tab.Rows [i][col].PadLeft(longestLengths[col]) + new string(' ', 4));
                        if (align == Alignment.Left)
                            Console.Write(" " + tab.Rows[i][col].PadRight(longestLengths[col]) + new string(' ', 4));
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
        public static string ToHTMLTable(this Table tab)
        {
            //To Do
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine("<table>");
            foreach (string header in tab.ColumnHeaders)
            {
                tableBuilder.AppendLine("<th>" + header + "</th>");
            }
            for (int i = 0; i < tab.RowCount; i++)
            {
                tableBuilder.AppendLine("<tr>");
                foreach (string header in tab.ColumnHeaders)
                    tableBuilder.AppendLine("<td>" + tab[header][i] + "</td>");
                tableBuilder.AppendLine("</tr>");
            }
            tableBuilder.AppendLine("</table>");
            return tableBuilder.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ToCSV()
        {
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ToTSV()
        {
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ToARFF(this Table tab)
        {
            StringBuilder arffBuilder = new StringBuilder();

            foreach (string header in tab.ColumnHeaders)
            {
                arffBuilder.AppendLine("@attribute " + header + " {" + tab.ValuesOf(header).Distinct().Aggregate((a, b) => a + "," + b) + "}");
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

        #endregion

    }
}

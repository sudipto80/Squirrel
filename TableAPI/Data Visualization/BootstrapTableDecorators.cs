using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    /// <summary>
    /// Home for Bootstrap table decoration methods. 
    /// </summary>
    public  static class BootstrapTableDecorators
    {
        /// <summary>
        /// Bootstrap  HTML tables classes
        /// </summary>
        public enum BootstrapTableClasses 
        { 
            /// <summary>
            /// The vanilla table class from bootstrap
            /// </summary>
            Table,
            /// <summary>
            /// table-striped class of Bootstrap table
            /// </summary>
            Table_Striped,
            /// <summary>
            /// table-bordered class of Bootstrap table
            /// </summary>
            Table_Bordered,
            /// <summary>
            /// table-hover class of Bootstrap table
            /// </summary>
            Table_Hover,
            /// <summary>
            /// table-condensed class of Bootstrap table
            /// </summary>
            Table_Condensed 
        };
        /// <summary>
        /// Bootstrap classes for table rows 
        /// </summary>
        public enum BootstrapTableRowClasses 
        {
            /// <summary>
            /// 
            /// </summary>
            None, 
            /// <summary>
            /// 
            /// </summary>
            Success,
            /// <summary>
            /// 
            /// </summary>
            Active,
            /// <summary>
            /// 
            /// </summary>
            Info, 
            /// <summary>
            /// 
            /// </summary>
            Warning,
            /// <summary>
            /// 
            /// </summary>
            Danger 
        }
        /// <summary>
        /// Finds indices of the rows for which the given predicate returns true.
        /// </summary>
        /// <param name="tab">The table</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>A list of integers depicting the row indices where the predicate returns true.</returns>
        private static List<int> GetTheRowsWhere(this Table tab,
                          Func<Dictionary<string, string>, bool> predicate)
        {
            List<int> rowIndices = new List<int>();
            if (predicate != null)
            {
                for (int i = 0; i < tab.RowCount; i++)
                {
                    if (predicate(tab[i]))
                        rowIndices.Add(i);
                }
            }
            return rowIndices;
        }
        /// <summary>
        /// This method is used to determing the bootstrap table class of the row.
        /// </summary>
        /// <param name="i">The index of the row</param>
        /// <param name="activeRows">Indices of the table rows which are in active class</param>
        /// <param name="warningRows">Indices of the table rows which are in the warning class</param>
        /// <param name="dangerRows">Indices of the table rows which are in the danger class</param>
        /// <param name="infoRows">Indices of the table rows which are in the info class</param>
        /// <param name="successRows">Indices of the table rows in the success classs</param>
        /// <returns>Returns any of the few possible BootstrapTableRowClasses enum values</returns>
        private static BootstrapTableRowClasses DeterMineRowClass(int i,
                                     List<int> activeRows,
                                     List<int> warningRows,
                                     List<int> dangerRows, 
                                     List<int> infoRows,
                                     List<int> successRows)
        {
            if (activeRows.Contains(i))
                return BootstrapTableRowClasses.Active;
            if (warningRows.Contains(i))
                return BootstrapTableRowClasses.Warning;
            if (dangerRows.Contains(i))
                return BootstrapTableRowClasses.Danger;
            if (infoRows.Contains(i))
                return BootstrapTableRowClasses.Info;
            else
                return BootstrapTableRowClasses.None;
        }
        /// <summary>
        /// Bootstrap offers functionalities to color rows of a given table.
        /// </summary>
        /// <param name="tab">The table for which the corresponding bootstrap representation has to be generated.</param>
        /// <param name="successPredicate">The predicate to use to find rows that passes the success scenario</param>
        /// <param name="activePredicate">The predicate to use to find rows that passes the active scenario</param>
        /// <param name="infoPredicate">The predicate to use to find rows that passes the info scenario</param>
        /// <param name="warningPredicate">The predicate to use to find rows that passes the warning scenario</param>
        /// <param name="dangerPredicate">The predicate to use to find rows that passes the danger scenario</param>
        /// <param name="tableClass">The class of the table. Possible values are 
        /// Table, Table_Striped, Table_Bordered, Table_Hover, Table_Condensed</param>       
        /// <returns>A string representing the table in bootstrap contextual table</returns>
        /// <example>//You can provide just a single predicate</example>
        /// <example>//Let's say you have a column called "StockPrice"</example>
        /// <example>//Func&lt;Dictionary&lt;string,string&gt;,bool&gt; warning = x => Convert.ToDouble(x["StockPrice"])>10.5;</example>
        /// <example>//tab.ToBootstrapHTMLTableWithColoredRows(warningPredicate:warning);</example>
        /// <remarks>Watch this video to understand it better: </remarks>
        public static string ToBootstrapHTMLTableWithColoredRows(this Table tab,                                                                    
                                                                      //All are optional. Use any number of predicates you want.
                                                                      Func<Dictionary<string,string>,bool> successPredicate  = null,
                                                                      Func<Dictionary<string, string>, bool> activePredicate = null,
                                                                      Func<Dictionary<string, string>, bool> infoPredicate = null,
                                                                      Func<Dictionary<string, string>, bool> warningPredicate = null,
                                                                      Func<Dictionary<string, string>, bool> dangerPredicate = null,
                                                                      BootstrapTableClasses tableClass = BootstrapTableClasses.Table
                                                                 )
        {
            List<int> successRowIndices = tab.GetTheRowsWhere(successPredicate);
            List<int> activeRowIndices = tab.GetTheRowsWhere(activePredicate);
            List<int> dangerRowIndices = tab.GetTheRowsWhere(dangerPredicate);
            List<int> infoRowIndices = tab.GetTheRowsWhere(infoPredicate);
            List<int> warningRowIndices = tab.GetTheRowsWhere(warningPredicate);

            StringBuilder tableBuilder = new StringBuilder();
            if (tab == null)
                throw new ArgumentNullException("tab");

            string content = @"<link rel=""stylesheet"" href=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css"">
                                <link rel=""stylesheet"" href=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap-theme.min.css"">
                                <script src=""http://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js""></script>
                                <script src=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/js/bootstrap.min.js""></script>
                                <style type=""text/css"">
                                    .bs-example{
    	                                margin: 20px;
                                    }
                                </style>";

           
            tableBuilder.AppendLine("<html>");
            tableBuilder.AppendLine(content);
            tableBuilder.AppendLine(@"<div class=""bs-example"">");
            if (tableClass == BootstrapTableClasses.Table_Bordered)
                tableBuilder.AppendLine(@"<table class = ""table table-bordered"">");
            else if (tableClass == BootstrapTableClasses.Table_Hover)
                tableBuilder.AppendLine(@"<table class = ""table table-hover"">");
            else if (tableClass == BootstrapTableClasses.Table_Condensed)
                tableBuilder.AppendLine(@"<table class = ""table table-condensed"">");
            else if (tableClass == BootstrapTableClasses.Table_Striped)
                tableBuilder.AppendLine(@"<table class = ""table table-striped"">");
            else
                tableBuilder.AppendLine(@"<table class = ""table"">");
            tableBuilder.AppendLine("<thead>");
            foreach (string header in tab.ColumnHeaders)
            {
                tableBuilder.AppendLine("<th>" + header + "</th>");
            }
            tableBuilder.AppendLine("</thead>");
            tableBuilder.AppendLine("<tbody>");
            for (int i = 0; i < tab.RowCount; i++)
            {
                BootstrapTableRowClasses determinedRowClass = DeterMineRowClass(i, activeRowIndices, warningRowIndices, dangerRowIndices, infoRowIndices, successRowIndices);
                    
                if(determinedRowClass == BootstrapTableRowClasses.None)
                    tableBuilder.AppendLine("<tr>");
                else if(determinedRowClass == BootstrapTableRowClasses.Success)
                    tableBuilder.AppendLine(@"<tr class=""success"">");
                else if (determinedRowClass == BootstrapTableRowClasses.Danger)
                    tableBuilder.AppendLine(@"<tr class=""danger"">");
                else if (determinedRowClass == BootstrapTableRowClasses.Info)
                    tableBuilder.AppendLine(@"<tr class=""info"">");
                else if (determinedRowClass == BootstrapTableRowClasses.Active)
                    tableBuilder.AppendLine(@"<tr class=""active"">");
                else if (determinedRowClass == BootstrapTableRowClasses.Warning)
                    tableBuilder.AppendLine(@"<tr class=""warning"">");

                foreach (string header in tab.ColumnHeaders)
                {
                        tableBuilder.AppendLine("<td>" + tab[header][i] + "</td>");
                    
                }
                tableBuilder.AppendLine("</tr>");
            }
            tableBuilder.AppendLine("</tbody>");
            tableBuilder.AppendLine("</table>");
            tableBuilder.AppendLine("</div>");
            tableBuilder.AppendLine("</html>");
           
            return tableBuilder.ToString();
           
        }
       
        /// <summary>
        /// Returns a basic HTML table in bootstrap format.
        /// </summary>
        /// <param name="tab">The table which has to be transformed to bootstrap table implementation.</param>
        /// <param name="tableClass">The boostrap table class. Possible values are 
        /// Table, Table_Striped, Table_Bordered, Table_Hover, Table_Condensed.
        /// The default class is "Table"</param>
        /// <returns>A table in Bootstrap format with the given table class.</returns>
        public static string ToBasicBootstrapHTMLTable(this Table tab, BootstrapTableClasses tableClass = BootstrapTableClasses.Table)
        {
            if (tab == null)
                throw new ArgumentNullException("tab");

            string content = @"<link rel=""stylesheet"" href=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css"">
                                <link rel=""stylesheet"" href=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap-theme.min.css"">
                                <script src=""http://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js""></script>
                                <script src=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/js/bootstrap.min.js""></script>
                                <style type=""text/css"">
                                    .bs-example{
    	                                margin: 20px;
                                    }
                                </style>";

         
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.AppendLine("<html>");
            tableBuilder.AppendLine(content);
            tableBuilder.AppendLine(@"<div class=""bs-example"">");
            if (tableClass == BootstrapTableClasses.Table_Bordered)
                tableBuilder.AppendLine(@"<table class = ""table table-bordered"">");
            else if (tableClass == BootstrapTableClasses.Table_Hover)
                tableBuilder.AppendLine(@"<table class = ""table table-hover"">");
            else if (tableClass == BootstrapTableClasses.Table_Condensed)
                tableBuilder.AppendLine(@"<table class = ""table table-condensed"">");
            else if (tableClass == BootstrapTableClasses.Table_Striped)
                tableBuilder.AppendLine(@"<table class = ""table table-striped"">");
            else
                tableBuilder.AppendLine(@"<table class = ""table"">");
            tableBuilder.AppendLine("<thead>");
            foreach (string header in tab.ColumnHeaders)
            {
                tableBuilder.AppendLine("<th>" + header + "</th>");
            }
            tableBuilder.AppendLine("</thead>");
            tableBuilder.AppendLine("<tbody>");
            for (int i = 0; i < tab.RowCount; i++)
            {
                tableBuilder.AppendLine("<tr>");
                foreach (string header in tab.ColumnHeaders)
                    tableBuilder.AppendLine("<td>" + tab[header][i] + "</td>");
                tableBuilder.AppendLine("</tr>");
            }
            
            tableBuilder.AppendLine("</tbody>");
            tableBuilder.AppendLine("</table>");
            tableBuilder.AppendLine("</div>");
            tableBuilder.AppendLine("</html>");
            return tableBuilder.ToString();
        }

    }
}

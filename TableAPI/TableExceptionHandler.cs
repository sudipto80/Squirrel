using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squirrel;

namespace TableAPI
{
    /// <summary>
    /// 
    /// </summary>
    public static class TableExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab"></param>
        public static void ThrowIfTableIsNull(this Table tab)
        {
            if(tab == null)
                throw new ArgumentNullException("Table instance " + nameof(tab) + " is null");

        }
        public static void ThrowIfColumnsAreNotPresentInTable(this Table tab, params string[] columns)
        {
            const int threshold = 3; 
            var exceptionBuilder = new StringBuilder();

            foreach (var column in columns)
            {
                if (tab.ColumnHeaders.Contains(column)) continue;
                exceptionBuilder.AppendLine($"Column {column} isn't present in the table instance");
                var probableIntendedColumns = tab.ColumnHeaders
                    .Where(thisColumn => HammingDistance(thisColumn, column) <= threshold);

                foreach (var probableColumn in probableIntendedColumns)
                {
                    exceptionBuilder.AppendLine($"Did you mean to use {probableColumn} instead of {column}?");
                }
            }

            var message = exceptionBuilder.ToString().Trim();
            if (message.Length > 0)
                throw new ArgumentException(message);

            
        }
        static int HammingDistance(string x, string y)
            => x.Zip(y, (m, n) => m == n).Count(equal => !equal);
    }
}

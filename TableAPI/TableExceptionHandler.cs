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
        /// <summary>
        /// Checks if the table has all the columns presented. 
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="columns"></param>
        public static void ThrowIfColumnsAreNotPresentInTable(this Table tab, 
                                                               params string[] columns)
        {
            Func<string, string, int> hammingDistance = (x, y)
                => x.ToCharArray().Zip(y.ToCharArray(), (m, n) => m == n).Count(z => z == false);

            int threshold = 3; 
                  
            StringBuilder exceptionBuilder = new StringBuilder();
            foreach (var column in columns)
            {
                if (!tab.ColumnHeaders.Contains(column))
                {
                    exceptionBuilder.AppendLine($"Column {column} isn't present in the table instance");
                    var probableIntendedColumns = tab.ColumnHeaders
                        .Where(thisColumn => hammingDistance(thisColumn, column) <= threshold);
                    foreach (var probableColumn in probableIntendedColumns)
                    {
                        exceptionBuilder.AppendLine($"Did you mean to use {probableColumn} instead of {column} ?");
                    }
                }
            }
            string message = exceptionBuilder.ToString().Trim();
            if(message.Length > 0)
                throw new ArgumentException(message);
        }
    }
}

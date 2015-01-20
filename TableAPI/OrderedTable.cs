using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
 
    /// <summary>
    /// Representation of a table which is already sorted by a given column.
    /// </summary>
    public  class OrderedTable : Table
    {
        /// <summary>
        /// Sorts an already sorted table by another column
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <param name="how">Direction for sorting.</param>
        /// <returns>A sorted table</returns>
        public  OrderedTable ThenBy(string columnName, SortDirection how = SortDirection.Ascending)
        {
            return this.SortBy(columnName, how);
        }
    }
}

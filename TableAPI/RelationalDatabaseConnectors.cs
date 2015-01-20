using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    /// <summary>
    /// Sweet home for all database (
    /// </summary>
    public static class RelationalDatabaseConnectors
    {
        //SQL
        //MySQL
        //Oracle
        //Access
        #region SQL Server
        #endregion 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Table LoadFromSQLServer2008(string connectionString, string tableName)
        {
            return new Table();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Table LoadFromMongoDB()
        {
            Table t = new Table();
            return t;
        }
      //  public static Table 
    }
}

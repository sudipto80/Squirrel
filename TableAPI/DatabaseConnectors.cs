using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    public static class DatabaseConnectors
    {
        //SQL
        //MySQL
        //Oracle
        //MongoDB
        //CouchDB
        //Redis
        //
        public static Table LoadFromSQLServer2008(string connectionString, string tableName)
        {
            return new Table();
        }
        public static Table LoadFromMongoDB()
        {
            Table t = new Table();
            return t;
        }
      //  public static Table 
    }
}

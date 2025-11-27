using System.Data.SqlClient;

namespace Squirrel;

public class DatabaseConnectors
{
    /// <summary>
    /// Loads data from a SQL Server database.
    /// </summary>
    /// <param name="connectionString">The connection string to the SQL Server database.</param>
    /// <param name="query">The SQL query to execute.</param>
    /// <returns>A table with all the values from the SQL Server database.</returns>
    public static Table LoadSqlServer(string connectionString, string query)
    {
        var conn = new SqlConnection(connectionString);
        conn.Open();
        // Method implementation goes here
        var cmd = new SqlCommand(query, conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            //reader.GetValue()
        }
        return new Table();
    }

    /// <summary>
    /// Loads data from a SQLite database.
    /// </summary>
    /// <param name="connectionString">The connection string to the SQLite database.</param>
    /// <param name="query">The SQL query to execute.</param>
    /// <returns>A table with all the values from the SQLite database.</returns>
    public static Table LoadSQLite(string connectionString, string query)
    {
        // Method implementation goes here
            
        return new Table();
    }


}
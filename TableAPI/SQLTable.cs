using System.Data;
using System.Text;
using Microsoft.VisualBasic;
using Squirrel;

namespace TableAPI;



public class SqlTable<T>
{
    
    /// <summary>
    /// Columns of the table
    /// </summary>
    public HashSet<string> ColumnNames { get; set; }
    /// <summary>
    /// Rows of the table 
    /// </summary>
    public List<T> Rows { get; set; }
    /// <summary>
    /// Total Row Count 
    /// </summary>
    public int RowCount => Rows.Count;
    /// <summary>
    /// Should we drop the table before recreating it?
    /// </summary>
    public bool ShouldDrop { get; set; }
    
    /// <summary>
    /// Name of the table given
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Default constructor 
    /// </summary>
    public SqlTable()
    {
        ColumnNames = new HashSet<string>();
        Name = string.Empty;
        Rows = new List<T>();
    }

    private Dictionary<string, string> _dataTypes
        = new()
        {
            { "Int32", "int" },
            { "Bool", "bit" },
            { "String", "varchar(max)" },
            { "DateTime", "datetime" }
        };
    
    /// <summary>
    /// The SQL script to create the table in a given
    /// database. 
    /// </summary>
    public string CreateTableScript
    {
        get
        {
            StringBuilder builder = new StringBuilder();
            var props = typeof(T).GetProperties()
                .ToList()
                .Select(t =>
                    new
                    {
                        Name = t.Name,
                        PropType = _dataTypes[t.PropertyType.Name],
                    })
                .ToList();

            builder.AppendLine($"CREATE TABLE {Name} (");
            List<string> columnDefs = new List<string>();
            foreach (var prop in props)
            {
                columnDefs.Add($"\t{prop.Name} {prop.PropType}");
            }

            var allDefs = columnDefs.Aggregate((a, b) => a + "," + b);
            builder.AppendLine($"\t{allDefs});");
            if (ShouldDrop)
            {
                return "Drop Table " + Name + Environment.NewLine
                       + builder;
            }
            //Otherwise 
            return builder.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public List<string> RowInsertCommands
    {
        get
        {
            
            StringBuilder colBuilder 
                = new StringBuilder();  
            
            colBuilder.AppendLine($"INSERT INTO {Name} (");
            colBuilder.AppendLine(ColumnNames.Aggregate((a,b) => a + "," + b));
            colBuilder.AppendLine(") VALUES (");
            
            List<string> insertCommands = new List<string>();
            
            foreach (var row in Rows)
            {
                StringBuilder builder = new StringBuilder();
                List<string> values = new List<string>();
                foreach (var col in ColumnNames)
                {
                    var prop = typeof(T).GetProperty(col);
                    if (prop.PropertyType == typeof(string))
                    {
                        values.Add("\"" + prop.GetValue(row).ToString() + "\"");
                    }
                    else
                    {
                        values.Add(prop.GetValue(row).ToString());
                    }
                }
                builder.AppendLine(colBuilder.ToString() + string.Join(",", values) +")");
                insertCommands.Add(builder.ToString());
            }
            return insertCommands;
        }
    }
}
using Squirrel;
using System.Collections.Generic;

namespace Squirrel;

public static class TableListExtensions
{
    public static void ForEach(this List<Table> tables,
        Action<Table> action) => tables.ForEach(action);

    public static Dictionary<string, Table> IndexByName(this List<Table> tables)
    => tables.ToLookup(t => t.Name)
            .ToDictionary(t => t.Key, t => t.First());
    

    public static Dictionary<string, int> RowCountsByName(this List<Table> tables)
        => tables.ToDictionary(t => t.Name, t => t.RowCount);



}
using Squirrel;

namespace TableAPI;

public class EmptyTable : Table
{
    private List<Dictionary<string, string>> _rows = new(); 
    public EmptyTable()
    {
        _rows = new List<Dictionary<string, string>>();
    }
    public int RowCount => _rows.Count;
}
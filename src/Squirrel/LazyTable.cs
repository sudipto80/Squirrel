using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Squirrel;

public class LazyTable
{
    private ObservableCollection<Dictionary<string, string>> _rows;
    private Dictionary<string,Func<Dictionary<string, string>, decimal>> _folders;
    public Dictionary<string, decimal> FolderStates;
    public int RowCount => _rows.Count;
    public int FolderCount => _folders.Count;
    
    public LazyTable()
    {
        _rows = new ObservableCollection<Dictionary<string, string>>();
        _folders = new Dictionary<string, Func<Dictionary<string, string>, decimal>>();
        FolderStates = new Dictionary<string, decimal>();
        _rows.CollectionChanged += RowsChanged;
    }

    private void RowsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var folder in _folders.Keys)
        {
            if (!FolderStates.ContainsKey(folder))
            {
                FolderStates.Add(folder,
                    _folders[folder].Invoke(_rows[^1]));
            }
            else
            {
                FolderStates[folder] = _folders[folder].Invoke(_rows[^1]);
            }
        }
        
    }

    public void AddFolder(string folderName, Func<Dictionary<string, string>, decimal> folder)
    {
        _folders.Add(folderName, folder);
    }
    public void AddRow(Dictionary<string, string> row)
    {
        _rows.Add(row);
    }

    public LazyTable(IEnumerable<Dictionary<string, string>> rows)
    {
        foreach (var row in rows)
        {
            AddRow(row);
        }
    }

    
 
}
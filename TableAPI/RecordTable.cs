using Squirrel;

namespace TableAPI;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class RecordTable<T>
{
    /// <summary>
    /// 
    /// </summary>
    public required List<T> _rows = new List<T>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyValues"></param>
    /// <typeparam name="TRecordType"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private static TRecordType ToRecord<TRecordType>(params IEnumerable<string> propertyValues)
    {
        var values = propertyValues.ToArray();

        var constructor = typeof(TRecordType).GetConstructors().FirstOrDefault();
        if (constructor == null)
            throw new InvalidOperationException($"Type {typeof(TRecordType).Name} has no public constructor.");

        var parameters = constructor.GetParameters();

        if (parameters.Length != values.Length)
            throw new ArgumentException("Number of values does not match number of constructor parameters.");

        var typedValues = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            typedValues[i] = Convert.ChangeType(values[i], parameters[i].ParameterType);
        }

        return (TRecordType)constructor.Invoke(typedValues);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    public static RecordTable<T> FromTable(Table tab)
    {
        var table = new RecordTable<T>
        {
            _rows = []
        };
        foreach (var rowValues
                 in tab.Rows.Select(row 
                     => row.Keys.Select(k => row[k]).ToList()))
        {
            table._rows.Add(ToRecord<T>(rowValues));
        }

        return table;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public T this[int index] => _rows[index];
    /// <summary>
    /// 
    /// </summary>
    public int RowCount => _rows.Count;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public RecordTable<T> Filter(Func<T, bool> predicate)
    {
        var result = new RecordTable<T>
        {
            _rows = _rows.Where(predicate).ToList()
        };
        return result;
    }

   
    public List<T> Filter(Func<T,int,bool> predicate) => 
         this._rows.Where(predicate.Invoke).ToList();
    
    
    
}
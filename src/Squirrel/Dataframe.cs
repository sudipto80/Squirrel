namespace Squirrel;

public class DataFrame
{
    // Store column names in order
    private readonly List<string> _columnNames = new List<string>();
    
    // Store typed arrays for each column
    private readonly Dictionary<string, Array> _columns = new Dictionary<string, Array>();
    
    // Store column types for type checking and conversion
    private readonly Dictionary<string, Type> _columnTypes = new Dictionary<string, Type>();
    
    public int RowCount { get; private set; }
    public IReadOnlyList<string> Columns => _columnNames;

    // Add a strongly typed column
    public void AddColumn<T>(string name, T[] values)
    {
        if (RowCount == 0)
        {
            RowCount = values.Length;
        }
        else if (values.Length != RowCount)
        {
            throw new ArgumentException($"Column length {values.Length} does not match DataFrame row count {RowCount}");
        }

        _columnNames.Add(name);
        _columns[name] = values;
        _columnTypes[name] = typeof(T);
    }

    // Get column as typed array - no boxing/unboxing
    public T[] GetColumn<T>(string columnName)
    {
        if (!_columns.TryGetValue(columnName, out var column))
            throw new KeyNotFoundException($"Column '{columnName}' not found");

        if (column is T[] typedArray)
            return typedArray;

        throw new InvalidCastException(
            $"Cannot cast column '{columnName}' from {_columnTypes[columnName]} to {typeof(T)}");
    }

    // Efficient row accessor that creates a view without copying data
    public Row this[int index]
    {
        get
        {
            if (index < 0 || index >= RowCount)
                throw new IndexOutOfRangeException();
            return new Row(this, index);
        }
    }

    // Row representation as a view into the DataFrame
    public class Row
    {
        private readonly DataFrame _dataFrame;
        private readonly int _index;

        internal Row(DataFrame dataFrame, int index)
        {
            _dataFrame = dataFrame;
            _index = index;
        }

        public T Get<T>(string columnName)
        {
            var array = _dataFrame.GetColumn<T>(columnName);
            return array[_index];
        }

        public object this[string columnName]
        {
            get => _dataFrame._columns[columnName].GetValue(_index);
        }
    }

    // Efficient data loading
    public static DataFrame FromCsv(string path)
    {
        var df = new DataFrame();
        
        // First pass: detect types and count rows
        using (var reader = new StreamReader(path))
        {
            string[] headers = reader.ReadLine()?.Split(',') ?? Array.Empty<string>();
            var typeDetectors = headers.Select(_ => new TypeDetector()).ToArray();
            
            string line;
            int rowCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                for (int i = 0; i < values.Length && i < headers.Length; i++)
                {
                    typeDetectors[i].AddSample(values[i]);
                }
                rowCount++;
            }

            // Create typed arrays based on detected types
            var types = typeDetectors.Select(d => d.GetMostLikelyType()).ToArray();
            for (int i = 0; i < headers.Length; i++)
            {
                df._columnNames.Add(headers[i]);
                df._columnTypes[headers[i]] = types[i];
                df._columns[headers[i]] = Array.CreateInstance(types[i], rowCount);
            }

            df.RowCount = rowCount;
        }

        // Second pass: load data into typed arrays
        using (var reader = new StreamReader(path))
        {
            reader.ReadLine(); // Skip header
            int currentRow = 0;
            string line;
            
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                for (int i = 0; i < values.Length && i < df._columnNames.Count; i++)
                {
                    var colName = df._columnNames[i];
                    var colType = df._columnTypes[colName];
                    df._columns[colName].SetValue(ParseValue(values[i], colType), currentRow);
                }
                currentRow++;
            }
        }

        return df;
    }

    // Efficient numeric operations
    public double Sum(string columnName)
    {
        var column = _columns[columnName];
        if (column is int[] intArray)
            return intArray.Sum();
        if (column is double[] doubleArray)
            return doubleArray.Sum();
        if (column is decimal[] decimalArray)
            return (double)decimalArray.Sum();
        
        throw new InvalidOperationException($"Cannot sum column of type {_columnTypes[columnName]}");
    }

    public double Mean(string columnName)
    {
        return Sum(columnName) / RowCount;
    }

    private class TypeDetector
    {
        private int intCount, doubleCount, dateCount, boolCount, totalCount;

        public void AddSample(string value)
        {
            totalCount++;
            if (string.IsNullOrEmpty(value)) return;

            if (int.TryParse(value, out _))
                intCount++;
            else if (double.TryParse(value, out _))
                doubleCount++;
            else if (DateTime.TryParse(value, out _))
                dateCount++;
            else if (bool.TryParse(value, out _))
                boolCount++;
        }

        public Type GetMostLikelyType()
        {
            const double threshold = 0.8;
            
            if (intCount > totalCount * threshold)
                return typeof(int);
            if ((intCount + doubleCount) > totalCount * threshold)
                return typeof(double);
            if (dateCount > totalCount * threshold)
                return typeof(DateTime);
            if (boolCount > totalCount * threshold)
                return typeof(bool);
            return typeof(string);
        }
    }

    private static object ParseValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return type.IsValueType ? Activator.CreateInstance(type) : null;

        try
        {
            if (type == typeof(int))
                return int.Parse(value);
            if (type == typeof(double))
                return double.Parse(value);
            if (type == typeof(DateTime))
                return DateTime.Parse(value);
            if (type == typeof(bool))
                return bool.Parse(value);
            return value;
        }
        catch
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
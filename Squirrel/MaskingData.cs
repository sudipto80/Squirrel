namespace Squirrel;

public static class MaskingData
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tab"></param>
    /// <param name="columnName"></param>
    /// <param name="strategy"></param>
    /// <returns></returns>
    public static Table MaskColumn(this Table tab, string columnName,
        MaskingStrategy strategy = MaskingStrategy.StarExceptFirstAndLast)
    {
        
        var newVals = tab.ValuesOf(columnName).MaskValues(strategy);
        var maskedTable = tab;
        maskedTable.RemoveColumn(columnName);
        maskedTable.AddColumn(columnName, newVals);
        return maskedTable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tab"></param>
    /// <param name="columnNames"></param>
    /// <param name="strategy"></param>
    /// <returns></returns>
    public static Table MaskColumns(this Table tab, IEnumerable<string> columnNames,
        MaskingStrategy strategy = MaskingStrategy.StarExceptFirstAndLast)
    {
        var maskedTable = tab;
        foreach (var columnName in columnNames)
        {
            maskedTable = maskedTable.MaskColumn(columnName, strategy);
        }
        return maskedTable;
    }

    /// <summary>
    /// Mask multiple columns with given strategies 
    /// </summary>
    /// <param name="tab"></param>
    /// <param name="columnNames"></param>
    /// <param name="strategies"></param>
    /// <returns></returns>
    public static Table MaskColumnsWithStrategies(this Table tab,
        IEnumerable<string> columnNames, IEnumerable<MaskingStrategy> strategies)
    {
        var maskedTable = tab;
        for(int i = 0; i < strategies.Count(); i++)
        {
            maskedTable = maskedTable.MaskColumn(columnNames.ElementAt(i), strategies.ElementAt(i));
        }
        return maskedTable;
    }
    /// <summary>
    /// Masks a table with different Masking strategies for different columns
    /// </summary>
    /// <param name="tab">The table to mask</param>
    /// <param name="strategies">Different strategies</param>
    /// <returns></returns>
    public static Table MaskColumnsWithStrategies(this Table tab,
        Dictionary<string,MaskingStrategy> strategies)
    {
        var maskedTable = tab;
        foreach (var key in strategies.Keys)
        {
            maskedTable = maskedTable.MaskColumn(key, strategies[key]);
        }
        return maskedTable;
    }
}
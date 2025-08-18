namespace Squirrel;

public static class MaskingData
{
    public static Table MaskColumn(this Table tab, string columnName,
        MaskingStrategy strategy = MaskingStrategy.StarExceptFirstAndLast)
    {
        //TODO: Mask column using Masking strategy 
        var newVals = tab.ValuesOf(columnName).MaskValues(strategy);
        Table maskedTable = tab;
        maskedTable.RemoveColumn(columnName);
        maskedTable.AddColumn(columnName, newVals);
        return maskedTable;
    }

    public static Table MaskColumns(this Table tab, IEnumerable<string> columnNames,
        MaskingStrategy strategy = MaskingStrategy.StarExceptFirstAndLast)
    {
        return tab;
    }

    public static Table MaskColumnsWithStrategies(this Table tab,
        IEnumerable<string> columnNames, IEnumerable<MaskingStrategy> strategies)
    {
        return tab;
    }
   
}
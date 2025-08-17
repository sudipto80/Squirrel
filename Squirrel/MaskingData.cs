namespace Squirrel;

public static class MaskingData
{
    public static Table MaskColumn(this Table tab, string columnName,
        MaskingStrategy strategy = MaskingStrategy.StarExceptFirstAndLast)
    {
        //TODO: Mask column using Masking strategy 
        return tab;
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
namespace Squirrel;

public static class MaskingHelpers
{
    public static List<string> MaskValues(this List<string> values, MaskingStrategy strategy)
    {
        List<string> maskedValues = new List<string>();
        switch (strategy)
        {
            case MaskingStrategy.StarExceptFirstAndLast:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2)
                        return value; // Too short to mask meaningfully
                    return value[0] + new string('*', value.Length - 2) + value[^1];
                }));
                break;
            
            case MaskingStrategy.StarExceptLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4)
                        return value; // Too short to mask meaningfully
                    return new string('*', value.Length - 4) + value[^4..];
                }));
                break;
            
            case MaskingStrategy.StarExceptFirstFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4)
                        return value; // Too short to mask meaningfully
                    return value[..4] + new string('*', value.Length - 4);
                }));
                break;
            
            case MaskingStrategy.StarExceptFirstTwoAndLastTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4)
                        return value; // Too short to mask meaningfully
                    return value[..2] + new string('*', value.Length - 4) + value[^2..];
                }));
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
        }

        if (maskedValues.Count == 0)
        {
            return values;
        }
        return maskedValues;
    }
}
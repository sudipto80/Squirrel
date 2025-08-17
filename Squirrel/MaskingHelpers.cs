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
                    value[0] + new string('*', value.Length - 2) + value[^1]));
                break;
            case MaskingStrategy.StartExceptLastFour:
                maskedValues.AddRange(values.Select(value =>
                    new string('*', value.Length - 4) + value[^4]));
                break;
            case MaskingStrategy.StarExceptFirstFour:
                break;
            case MaskingStrategy.StarExceptFirstTwoAndLastTwo:
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
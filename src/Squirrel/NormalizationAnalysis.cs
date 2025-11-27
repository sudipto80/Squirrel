namespace Squirrel;

public class NormalizationAnalysis
{
    public double CaseInconsistencyScore { get; set; }
    public double DuplicatePercentage { get; set; }
    public bool HasPersonalNames { get; set; }
    public bool HasSentenceStructure { get; set; }
    public bool HasSpaceSeparatedData { get; set; }
    public bool HasMixedAlphaNumeric { get; set; }
    public bool HasSpecialCharacters { get; set; }
    public double UppercasePercentage { get; set; }
    public double NumberSuffixPercentage { get; set; }
    public double SpecialCharPercentage { get; set; }
    public double AverageWordsPerValue { get; set; }
}
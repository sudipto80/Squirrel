using Squirrel;
using Squirrel.Cleansing;

namespace ConsoleApp13;

class Program
{
    static void Main(string[] args)
    {
        var tab = DataAcquisition.LoadCsv(dataFile);
        tab.PrettyDump(header:"Before Text Normalization");

        // Normalize text case
        tab = tab.Normalize(columnName:"ProductName", 
                            strategy: NormalizationStrategy.SentenceCase);
        // Multiple column normalization
        var normalizationStrategies = new Dictionary<string, NormalizationStrategy>
        {
            ["FirstName"] = NormalizationStrategy.SentenceCase,
            ["LastName"] = NormalizationStrategy.SentenceCase,
            ["Email"] = NormalizationStrategy.LowerCase,
            ["ProductCode"] = NormalizationStrategy.UpperCase,
            
        };
        tab = tab.Normalize(normalizationSchemes: normalizationStrategies);
        // Truncate long descriptions
        tab = tab.Truncate("Description", 100);
        // Multiple column truncation
        var truncateRules = new Dictionary<string, int>
        {
            ["Title"] = 50,
            ["Summary"] = 200
        };
        tab = tab.Truncate(truncateRules);
        tab.PrettyDump(header:"After Text Normalization");

    }
}
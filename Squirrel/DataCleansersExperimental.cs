using System.Text.RegularExpressions;
using Squirrel;

namespace Squirrel.DataCleansing;

public static class DataCleansersExperimental
{
    
		/// <summary>
		/// Automatically cleans a table instance 
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public static Table AutoNormalize(this Table tab)
		{
			//Find bad values from teach column 
			//Remove bad values from each column 
			//Return the cleaned table 
			
			//if the character set used in a value of a cell in a column
			//is drastically different from the rest of the values of rest of the cells 
			//then it is probably a bad value 
			
			//Once all the bad values from each of the column is identified, we need to put them in a dictionary
			//With column names and bad values and then we need to iterte over this dictionary to clean the data
			//for each column.
            Table normalizedTab = tab;
            foreach (var col in tab.ColumnHeaders)
            {
                try
                {
                    normalizedTab = normalizedTab.AutoNormalize(col);
                }
                catch
                {
                    continue;
                }
            }
            return normalizedTab;
		}
		/// <summary>
		/// Auto Clean an entire column
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public static Table AutoNormalize(this Table tab, string column)
		{
			var colValues = tab.ValuesOf(column);
			var strategy = DetectNormalizationStrategy(colValues,column);
            return tab.NormalizeColumn(column, strategy);
		}
		
		/// <summary>
		/// A way to find out which Normalization strategy will work best for this 
        /// particular column values.
		/// </summary>
        /// <param name="columnValues"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static NormalizationStrategy DetectNormalizationStrategy(this List<string> columnValues, string columnName = "")
        {
            if (columnValues == null || columnValues.Count == 0)
                return NormalizationStrategy.LowerCase; // Default fallback

            // Filter out null/empty values for analysis
            var validValues = columnValues.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
            if (validValues.Count == 0)
                return NormalizationStrategy.LowerCase;

            // Analyze column name for hints
            var lowerColumnName = columnName.ToLowerInvariant();
            
            // Check for specific data types based on column name
            if (Utilities.IsColumnNameMatch(lowerColumnName, new[] { "name", "firstname", "lastname", "fullname", "person", "author", "customer" }))
                return NormalizationStrategy.NameCase;
                
            if (Utilities.IsColumnNameMatch(lowerColumnName, new[] { "title", "description", "subject", "heading" }))
                return NormalizationStrategy.SentenceCase;
                
            if (Utilities.IsColumnNameMatch(lowerColumnName, new[] { "code", "id", "key", "reference", "sku" }))
                return NormalizationStrategy.UpperCase;
                
            if (Utilities.IsColumnNameMatch(lowerColumnName, new[] { "email", "url", "website", "domain" }))
                return NormalizationStrategy.LowerCase;

            // Analyze actual data patterns
            var dataAnalysis = AnalyzeNormalizationPatterns(validValues);
            
            // Decision tree based on data analysis
            
            // Check for inconsistent casing in names
            if (dataAnalysis.HasPersonalNames && dataAnalysis.CaseInconsistencyScore > 0.3)
                return NormalizationStrategy.NameCase;
                
            // Check for sentence-like content
            if (dataAnalysis.HasSentenceStructure && dataAnalysis.CaseInconsistencyScore > 0.2)
                return NormalizationStrategy.SentenceCase;
                
            // Check if data contains extra information that should be truncated
            if (dataAnalysis.HasSpaceSeparatedData && dataAnalysis.AverageWordsPerValue > 2)
                return NormalizationStrategy.TerminateAtSpace;
                
            // Check for data with numbers that might need separation
            if (dataAnalysis.HasMixedAlphaNumeric && dataAnalysis.NumberSuffixPercentage > 0.6)
                return NormalizationStrategy.TerminateAtNumber;
                
            // Check for highly repetitive data
            if (dataAnalysis.DuplicatePercentage > 0.7)
                return NormalizationStrategy.MostFrequentOne;
                
            // Check for code-like patterns (mostly uppercase/mixed case)
            if (dataAnalysis.UppercasePercentage > 0.6)
                return NormalizationStrategy.UpperCase;
                
            // Check for inconsistent casing in general
            if (dataAnalysis.CaseInconsistencyScore > 0.4)
                return NormalizationStrategy.LowerCase;
                
            // Check for data that contains special characters/punctuation that should be truncated
            if (dataAnalysis.HasSpecialCharacters && dataAnalysis.SpecialCharPercentage > 0.3)
                return NormalizationStrategy.TerminateAtFirstNonAlphaNumeric;
                
            // Default strategy based on content type
            if (dataAnalysis.HasPersonalNames)
                return NormalizationStrategy.NameCase;
            else if (dataAnalysis.HasSentenceStructure)
                return NormalizationStrategy.SentenceCase;
            else
                return NormalizationStrategy.LowerCase; // Safe default
        }
		private static NormalizationAnalysis AnalyzeNormalizationPatterns(List<string> values)
        {
            var analysis = new NormalizationAnalysis();
            var sampleSize = Math.Min(values.Count, 100); // Sample for performance
            var sample = values.Take(sampleSize).ToList();
            
            if (sample.Count == 0) return analysis;
            
            var caseVariations = 0;
            var uppercaseCount = 0;
            var hasPersonalNames = false;
            var hasSentenceStructure = false;
            var hasSpaceSeparated = false;
            var hasMixedAlphaNumeric = 0;
            var hasSpecialChars = 0;
            var numberSuffixCount = 0;
            var totalWords = 0;
            
            // Calculate duplicates
            var uniqueValues = sample.Distinct().Count();
            analysis.DuplicatePercentage = 1.0 - ((double)uniqueValues / sample.Count);
            
            foreach (var value in sample)
            {
                var trimmedValue = value.Trim();
                if (string.IsNullOrEmpty(trimmedValue)) continue;
                
                // Count words
                var words = trimmedValue.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                totalWords += words.Length;
                
                // Check case inconsistency
                if (HasInconsistentCasing(trimmedValue))
                    caseVariations++;
                    
                // Check if mostly uppercase
                if (trimmedValue.Count(char.IsUpper) > trimmedValue.Count(char.IsLower))
                    uppercaseCount++;
                    
                // Check for personal names
                if (IsLikelyPersonalName(trimmedValue))
                    hasPersonalNames = true;
                    
                // Check for sentence structure
                if (HasSentenceStructure(trimmedValue))
                    hasSentenceStructure = true;
                    
                // Check for space-separated data
                if (trimmedValue.Contains(' '))
                    hasSpaceSeparated = true;
                    
                // Check for mixed alphanumeric
                if (HasMixedAlphaNumeric(trimmedValue))
                    hasMixedAlphaNumeric++;
                    
                // Check for special characters
                if (HasSpecialCharacters(trimmedValue))
                    hasSpecialChars++;
                    
                // Check for number suffix
                if (Regex.IsMatch(trimmedValue, @"[a-zA-Z]+\d+"))
                    numberSuffixCount++;
            }
            
            analysis.CaseInconsistencyScore = (double)caseVariations / sample.Count;
            analysis.UppercasePercentage = (double)uppercaseCount / sample.Count;
            analysis.HasPersonalNames = hasPersonalNames;
            analysis.HasSentenceStructure = hasSentenceStructure;
            analysis.HasSpaceSeparatedData = hasSpaceSeparated;
            analysis.HasMixedAlphaNumeric = hasMixedAlphaNumeric > 0;
            analysis.HasSpecialCharacters = hasSpecialChars > 0;
            analysis.NumberSuffixPercentage = (double)numberSuffixCount / sample.Count;
            analysis.SpecialCharPercentage = (double)hasSpecialChars / sample.Count;
            analysis.AverageWordsPerValue = (double)totalWords / sample.Count;
            
            return analysis;
        }
		
		private static bool HasInconsistentCasing(string value)
        {
            // Check if the string has mixed casing that doesn't follow standard patterns
            var words = value.Split(' ');
            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word)) continue;
                
                // Check for mixed case within a word (excluding proper nouns)
                if (word.Any(char.IsUpper) && word.Any(char.IsLower))
                {
                    // Allow proper noun pattern (first letter uppercase, rest lowercase)
                    if (!(char.IsUpper(word[0]) && word.Skip(1).All(char.IsLower)))
                        return true;
                }
            }
            return false;
        }
        
        private static bool IsLikelyPersonalName(string value)
        {
            // Enhanced personal name detection
            var nameParts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length == 0 || nameParts.Length > 4) return false;
            
            // Check if it follows name patterns
            var namePattern = @"^[A-Z][a-z]{1,}$"; // Standard name part
            var initialPattern = @"^[A-Z]\.$"; // Middle initial
            var suffixPattern = @"^(Jr|Sr|III|IV|MD|PhD|Dr)\.?$"; // Suffixes/titles
            
            return nameParts.All(part =>
                Regex.IsMatch(part, namePattern) ||
                Regex.IsMatch(part, initialPattern) ||
                Regex.IsMatch(part, suffixPattern, RegexOptions.IgnoreCase)) &&
                value.Length >= 2 && value.Length <= 50;
        }
        
        private static bool HasSentenceStructure(string value)
        {
            // Check for sentence-like patterns
            return value.Length > 10 && 
                   (value.Contains('.') || value.Contains(',') || value.Contains(':') ||
                    (value.Split(' ').Length > 3 && char.IsUpper(value.Trim()[0])));
        }
        
        private static bool HasMixedAlphaNumeric(string value)
        {
            return value.Any(char.IsLetter) && value.Any(char.IsDigit);
        }
        
        private static bool HasSpecialCharacters(string value)
        {
            return value.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }
        
}
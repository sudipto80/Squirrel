using System.Text;
using System.Text.RegularExpressions;
using Squirrel;


public static class MaskingHelpers
{


    private static readonly Random _random = new Random();
    
    private static bool IsColumnNameMatch(string columnName, string[] keywords)
    {
        return keywords.Any(keyword => columnName.Contains(keyword));
    }
    /// <summary>
    /// Automatically detects the most appropriate masking strategy based on column values
    /// </summary>
    /// <param name="columnValues">The values in the column to analyze</param>
    /// <param name="columnName">Optional column name for additional context</param>
    /// <returns>Recommended masking strategy</returns>
    public static MaskingStrategy DetectMaskingStrategy(this List<string> columnValues, string columnName = "")
    {
        if (columnValues == null || columnValues.Count == 0)
            return MaskingStrategy.None;

        // Filter out null/empty values for analysis
        var validValues = columnValues.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        if (validValues.Count == 0)
            return MaskingStrategy.None;

        // Analyze column name for hints
        var lowerColumnName = columnName.ToLowerInvariant();
        
        // Check for specific data types based on column name
        if (IsColumnNameMatch(lowerColumnName, new[] { "email", "mail", "e-mail" }))
            return MaskingStrategy.EmailPartial;
            
        if (IsColumnNameMatch(lowerColumnName, new[] { "phone", "telephone", "mobile", "cell" }))
            return MaskingStrategy.PhoneAreaCodeAndLastFour;
            
        if (IsColumnNameMatch(lowerColumnName, new[] { "credit", "card", "cc", "payment" }))
            return MaskingStrategy.CreditCardLastFour;
            
        if (IsColumnNameMatch(lowerColumnName, new[] { "age", "years", "yr" }))
            return MaskingStrategy.AgeGroup;
            
        if (IsColumnNameMatch(lowerColumnName, new[] { "ssn", "social", "security", "tax", "id" }))
            return MaskingStrategy.StarExceptLastFour;

        // Analyze actual data patterns
        var dataAnalysis = AnalyzeDataPatterns(validValues);
        
        // Decision tree based on data analysis
        if (dataAnalysis.EmailPercentage > 0.7)
            return MaskingStrategy.EmailPartial;
            
        if (dataAnalysis.PhonePercentage > 0.7)
            return MaskingStrategy.PhoneAreaCodeAndLastFour;
            
        if (dataAnalysis.CreditCardPercentage > 0.7)
            return MaskingStrategy.CreditCardLastFour;
            
        if (dataAnalysis.AgePercentage > 0.7)
            return MaskingStrategy.AgeGroup;
            
        if (dataAnalysis.NumericPercentage > 0.8)
        {
            // Numeric data - check if it looks like sensitive IDs
            if (dataAnalysis.AverageLength >= 8)
                return MaskingStrategy.StarExceptFirstAndLastFour;
            else
                return MaskingStrategy.StarExceptLastFour;
        }
        
        // String data analysis
        if (dataAnalysis.AverageLength <= 3)
            return MaskingStrategy.StarAll; // Very short strings, probably codes
            
        if (dataAnalysis.AverageLength <= 8)
            return MaskingStrategy.StarExceptFirstAndLast;
            
        if (dataAnalysis.AverageLength <= 15)
            return MaskingStrategy.StarExceptFirstTwoAndLastTwo;
            
        if (dataAnalysis.HasPersonalNames)
            return MaskingStrategy.StarExceptFirst; // Names - show first letter only
            
        // Long strings - probably descriptions or comments
        if (dataAnalysis.AverageLength > 50)
            return MaskingStrategy.TruncateWithEllipsis;
            
        // Default strategy for medium-length strings
        return MaskingStrategy.StarExceptFirstAndLastFour;
    }
    private static bool IsLikelyPersonalName(string value)
    {
        // Common name patterns and characteristics
        var nameParts = value.Split(' ');
        if (nameParts.Length > 4) return false; // Too many parts for a typical name
        
        // Check if it contains common name patterns
        var commonNamePatterns = new[]
        {
            @"^[A-Z][a-z]{2,}$", // Standard first/last name pattern
            @"^[A-Z]\.$",        // Middle initial
            @"^(Jr|Sr|III|IV)\.?$" // Suffixes
        };
        
        return nameParts.All(part => 
                   commonNamePatterns.Any(pattern => Regex.IsMatch(part, pattern))) &&
               value.Length >= 2 && value.Length <= 50;
    }

    private static ColumnStatistics AnalyzeDataPatterns(List<string> values)
    {
        var analysis = new ColumnStatistics();
        var sampleSize = Math.Min(values.Count, 100); // Sample for performance
        var sample = values.Take(sampleSize).ToList();
        
        int emailCount = 0, phoneCount = 0, creditCardCount = 0, ageCount = 0, numericCount = 0;
        double totalLength = 0;
        bool hasPersonalNames = false;
        
        foreach (var value in sample)
        {
            totalLength += value.Length;
            
            // Email pattern
            if (Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                emailCount++;
                
            // Phone pattern (various formats)
            if (Regex.IsMatch(value, @"^[\+]?[1-9]?[\d\s\-\(\)\.]{7,15}$"))
                phoneCount++;
                
            // Credit card pattern
            if (Regex.IsMatch(value, @"^[\d\s\-]{13,19}$"))
                creditCardCount++;
                
            // Age pattern (reasonable age range)
            if (int.TryParse(value, out int age) && age >= 0 && age <= 150)
                ageCount++;
                
            // Numeric pattern
            if (decimal.TryParse(value, out _))
                numericCount++;
                
            // Personal names pattern (starts with capital, contains common name patterns)
            if (Regex.IsMatch(value, @"^[A-Z][a-z]+(\s[A-Z][a-z]+)*$") && 
                IsLikelyPersonalName(value))
                hasPersonalNames = true;
        }
        
        analysis.EmailPercentage = (double)emailCount / sampleSize;
        analysis.PhonePercentage = (double)phoneCount / sampleSize;
        analysis.CreditCardPercentage = (double)creditCardCount / sampleSize;
        analysis.AgePercentage = (double)ageCount / sampleSize;
        analysis.NumericPercentage = (double)numericCount / sampleSize;
        analysis.AverageLength = totalLength / sampleSize;
        analysis.HasPersonalNames = hasPersonalNames;
        
        return analysis;
    }
    public static List<string> MaskValues(this List<string> values, MaskingStrategy strategy)
    {
        if (values == null || values.Count == 0)
            return values ?? new List<string>();

        List<string> maskedValues = new List<string>();

        switch (strategy)
        {
            // === Original Strategies ===
            case MaskingStrategy.StarExceptFirstAndLast:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2) return value;
                    return value[0] + new string('*', value.Length - 2) + value[^1];
                }));
                break;

            case MaskingStrategy.StarExceptFirstAndLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 5) return value;
                    return value[0] + new string('*', value.Length - 5) + value[^4..];
                }));
                break;

            case MaskingStrategy.StarExceptFirstFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4) return value;
                    return value[..4] + new string('*', value.Length - 4);
                }));
                break;

            case MaskingStrategy.StarExceptFirstTwoAndLastTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4) return value;
                    return value[..2] + new string('*', value.Length - 4) + value[^2..];
                }));
                break;

            case MaskingStrategy.StarExceptLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4) return value;
                    return new string('*', value.Length - 4) + value[^4..];
                }));
                break;

            // === Complete Masking ===
            case MaskingStrategy.StarAll:
                maskedValues.AddRange(values.Select(value => new string('*', value.Length)));
                break;

            case MaskingStrategy.Redacted:
                maskedValues.AddRange(values.Select(_ => "[REDACTED]"));
                break;

            case MaskingStrategy.Hidden:
                maskedValues.AddRange(values.Select(_ => "[HIDDEN]"));
                break;

            case MaskingStrategy.Masked:
                maskedValues.AddRange(values.Select(_ => "[MASKED]"));
                break;

            // === Partial Display Strategies ===
            case MaskingStrategy.StarExceptFirst:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 1) return value;
                    return value[0] + new string('*', value.Length - 1);
                }));
                break;

            case MaskingStrategy.StarExceptFirstTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2) return value;
                    return value[..2] + new string('*', value.Length - 2);
                }));
                break;

            case MaskingStrategy.StarExceptFirstThree:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 3) return value;
                    return value[..3] + new string('*', value.Length - 3);
                }));
                break;

            case MaskingStrategy.StarExceptLast:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 1) return value;
                    return new string('*', value.Length - 1) + value[^1];
                }));
                break;

            case MaskingStrategy.StarExceptLastTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2) return value;
                    return new string('*', value.Length - 2) + value[^2..];
                }));
                break;

            case MaskingStrategy.StarExceptLastThree:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 3) return value;
                    return new string('*', value.Length - 3) + value[^3..];
                }));
                break;

            case MaskingStrategy.StarExceptFirstThreeAndLastThree:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 6) return value;
                    return value[..3] + new string('*', value.Length - 6) + value[^3..];
                }));
                break;

            case MaskingStrategy.StarExceptFirstFourAndLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 8) return value;
                    return value[..4] + new string('*', value.Length - 8) + value[^4..];
                }));
                break;

            // === Email-Specific Strategies ===
            case MaskingStrategy.EmailPartial:
                maskedValues.AddRange(values.Select(MaskEmailPartial));
                break;

            case MaskingStrategy.EmailDomainOnly:
                maskedValues.AddRange(values.Select(MaskEmailDomainOnly));
                break;

            case MaskingStrategy.EmailUsernameOnly:
                maskedValues.AddRange(values.Select(MaskEmailUsernameOnly));
                break;

            // === Phone Number Strategies ===
            case MaskingStrategy.PhoneAreaCodeAndLastFour:
                maskedValues.AddRange(values.Select(MaskPhoneAreaCodeAndLastFour));
                break;

            case MaskingStrategy.PhoneLastFour:
                maskedValues.AddRange(values.Select(MaskPhoneLastFour));
                break;

            case MaskingStrategy.PhoneAreaCodeOnly:
                maskedValues.AddRange(values.Select(MaskPhoneAreaCodeOnly));
                break;

            // === Credit Card Strategies ===
            case MaskingStrategy.CreditCardLastFour:
                maskedValues.AddRange(values.Select(MaskCreditCardLastFour));
                break;

            case MaskingStrategy.CreditCardFirstFourLastFour:
                maskedValues.AddRange(values.Select(MaskCreditCardFirstFourLastFour));
                break;

            case MaskingStrategy.CreditCardFirstSixLastFour:
                maskedValues.AddRange(values.Select(MaskCreditCardFirstSixLastFour));
                break;

            // === Custom Character Strategies ===
            case MaskingStrategy.XMask:
                maskedValues.AddRange(values.Select(value => new string('X', value.Length)));
                break;

            case MaskingStrategy.HashMask:
                maskedValues.AddRange(values.Select(value => new string('#', value.Length)));
                break;

            case MaskingStrategy.DashMask:
                maskedValues.AddRange(values.Select(value => new string('-', value.Length)));
                break;

            case MaskingStrategy.BulletMask:
                maskedValues.AddRange(values.Select(value => new string('•', value.Length)));
                break;

            // === Percentage-Based Strategies ===
            case MaskingStrategy.MaskMiddle25Percent:
                maskedValues.AddRange(values.Select(value => MaskMiddlePercent(value, 0.25f)));
                break;

            case MaskingStrategy.MaskMiddle50Percent:
                maskedValues.AddRange(values.Select(value => MaskMiddlePercent(value, 0.50f)));
                break;

            case MaskingStrategy.MaskMiddle75Percent:
                maskedValues.AddRange(values.Select(value => MaskMiddlePercent(value, 0.75f)));
                break;

            // === Pattern-Based Strategies ===
            case MaskingStrategy.ShowEveryThird:
                maskedValues.AddRange(values.Select(ShowEveryThird));
                break;

            case MaskingStrategy.ShowRandom30Percent:
                maskedValues.AddRange(values.Select(value => ShowRandomPercent(value, 0.30f)));
                break;

            case MaskingStrategy.AlternatingOddShow:
                maskedValues.AddRange(values.Select(AlternatingOddShow));
                break;

            case MaskingStrategy.AlternatingEvenShow:
                maskedValues.AddRange(values.Select(AlternatingEvenShow));
                break;

            // === Length-Preserving Strategies ===
            case MaskingStrategy.PreserveLengthStar:
                maskedValues.AddRange(values.Select(value => new string('*', value.Length)));
                break;

            case MaskingStrategy.PreserveLengthX:
                maskedValues.AddRange(values.Select(value => new string('X', value.Length)));
                break;

            // === Special Cases ===
            case MaskingStrategy.None:
                maskedValues.AddRange(values);
                break;

            case MaskingStrategy.TruncateWithEllipsis:
                maskedValues.AddRange(values.Select(value =>
                    value.Length > 10 ? value[..7] + "..." : value));
                break;

            case MaskingStrategy.ShowLengthOnly:
                maskedValues.AddRange(values.Select(value => $"[{value.Length} characters]"));
                break;

            case MaskingStrategy.ShowTypeOnly:
                maskedValues.AddRange(values.Select(GetDataType));
                break;
            
            case MaskingStrategy.AgeGroup:
                maskedValues.AddRange(values.Select(MaskAgeGroup));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy,
                    $"Masking strategy '{strategy}' is not implemented.");
        }

        return maskedValues.Count == 0 ? values : maskedValues;
    }

    // === Helper Methods ===
    private static string MaskAgeGroup(string ageValue)
    {
        // Try to parse the value as an integer
        if (!int.TryParse(ageValue.Trim(), out int age))
        {
            // If it's not a valid number, return as-is or indicate invalid
            return "[INVALID_AGE]";
        }
        
        // Categorize age into groups
        return age switch
        {
            < 0 => "[INVALID_AGE]",
            >= 0 and <= 2 => "Infant",
            >= 3 and <= 5 => "Toddler", 
            >= 6 and <= 12 => "Child",
            >= 13 and <= 19 => "Teenager",
            >= 20 and <= 35 => "Young Adult",
            >= 36 and <= 55 => "Adult",
            >= 56 and <= 70 => "Middle-Aged",
            >= 71 and <= 85 => "Senior",
            _ => "Elderly" // > 85
        };
    }
    private static string MaskEmailPartial(string email)
    {
        if (!email.Contains('@')) return email;

        var parts = email.Split('@');
        var username = parts[0];
        var domain = parts[1];

        var maskedUsername = username.Length > 2
            ? username[..2] + new string('*', Math.Max(0, username.Length - 2))
            : username;

        var maskedDomain = domain.Length > 2
            ? domain[..2] + new string('*', Math.Max(0, domain.Length - 2))
            : domain;

        return $"{maskedUsername}@{maskedDomain}";
    }

    private static string MaskEmailDomainOnly(string email)
    {
        if (!email.Contains('@')) return email;
        var parts = email.Split('@');
        return new string('*', parts[0].Length) + "@" + parts[1];
    }

    private static string MaskEmailUsernameOnly(string email)
    {
        if (!email.Contains('@')) return email;
        var parts = email.Split('@');
        return parts[0] + "@" + new string('*', parts[1].Length);
    }

    private static string MaskPhoneAreaCodeAndLastFour(string phone)
    {
        // Extract digits only
        var digits = Regex.Replace(phone, @"[^\d]", "");
        if (digits.Length < 10) return phone;

        var areaCode = digits[..3];
        var lastFour = digits[^4..];
        return $"({areaCode}) ***-{lastFour}";
    }

    private static string MaskPhoneLastFour(string phone)
    {
        var digits = Regex.Replace(phone, @"[^\d]", "");
        if (digits.Length < 4) return phone;

        var lastFour = digits[^4..];
        return "***-***-" + lastFour;
    }

    private static string MaskPhoneAreaCodeOnly(string phone)
    {
        var digits = Regex.Replace(phone, @"[^\d]", "");
        if (digits.Length < 10) return phone;

        var areaCode = digits[..3];
        return $"({areaCode}) ***-****";
    }

    private static string MaskCreditCardLastFour(string cardNumber)
    {
        var digits = Regex.Replace(cardNumber, @"[^\d]", "");
        if (digits.Length < 4) return cardNumber;

        var lastFour = digits[^4..];
        return "**** **** **** " + lastFour;
    }

    private static string MaskCreditCardFirstFourLastFour(string cardNumber)
    {
        var digits = Regex.Replace(cardNumber, @"[^\d]", "");
        if (digits.Length < 8) return cardNumber;

        var firstFour = digits[..4];
        var lastFour = digits[^4..];
        return $"{firstFour} **** **** {lastFour}";
    }

    private static string MaskCreditCardFirstSixLastFour(string cardNumber)
    {
        var digits = Regex.Replace(cardNumber, @"[^\d]", "");
        if (digits.Length < 10) return cardNumber;

        var firstSix = digits[..6];
        var lastFour = digits[^4..];
        return $"{firstSix}** ****{lastFour}";
    }

    private static string MaskMiddlePercent(string value, float percentage)
    {
        if (value.Length <= 2) return value;

        int maskLength = Math.Max(1, (int)(value.Length * percentage));
        int startPos = (value.Length - maskLength) / 2;

        var sb = new StringBuilder(value);
        for (int i = startPos; i < startPos + maskLength && i < value.Length; i++)
        {
            sb[i] = '*';
        }

        return sb.ToString();
    }

    private static string ShowEveryThird(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append((i + 1) % 3 == 0 ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string ShowRandomPercent(string value, float percentage)
    {
        if (value.Length == 0) return value;

        int showCount = Math.Max(1, (int)(value.Length * percentage));
        var indicesToShow = Enumerable.Range(0, value.Length)
            .OrderBy(_ => _random.Next())
            .Take(showCount)
            .ToHashSet();

        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append(indicesToShow.Contains(i) ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string AlternatingOddShow(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append(i % 2 == 0 ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string AlternatingEvenShow(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append(i % 2 == 1 ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string GetDataType(string value)
    {
        if (string.IsNullOrEmpty(value)) return "[EMPTY]";
        if (int.TryParse(value, out _)) return "[INTEGER]";
        if (decimal.TryParse(value, out _)) return "[DECIMAL]";
        if (DateTime.TryParse(value, out _)) return "[DATETIME]";
        if (value.Contains('@')) return "[EMAIL]";
        if (Regex.IsMatch(value, @"^\d{3}-?\d{3}-?\d{4}$")) return "[PHONE]";
        if (Regex.IsMatch(value, @"^\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}$")) return "[CREDITCARD]";
        return "[STRING]";
    }
    
    /// <summary>
    /// Analyzes an entire table and returns suggested masking strategies for each column
    /// </summary>
    /// <param name="table">Dictionary where key is column name and value is list of column values</param>
    /// <returns>Dictionary of column names to recommended masking strategies</returns>
    public static Dictionary<string, MaskingStrategy> DetectTableMaskingStrategies(
        this Table table)
    {
        var strategies = new Dictionary<string, MaskingStrategy>();
        
        foreach (var column in table.ColumnHeaders)
        {
            strategies[column] = DetectMaskingStrategy(table.ValuesOf(column), column);
        }
        
        return strategies;
    }
}
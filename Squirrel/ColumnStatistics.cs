namespace Squirrel;

/// <summary>
/// Encapsulates statistical metrics derived from analyzing a dataset column.
/// Provides insights into the types of data and distribution patterns detected in the column.
/// </summary>
public class ColumnStatistics
{
    /// <summary>
    /// Represents the percentage of email-like values within the analyzed column data.
    /// </summary>
    /// <remarks>
    /// This property calculates the proportion of values in a data column that appear to be email addresses
    /// based on pattern matching or other evaluation criteria. It is primarily used for determining the
    /// appropriate masking strategy or categorizing columns containing email data.
    /// </remarks>
    public double EmailPercentage { get; set; }

    /// <summary>
    /// Represents the percentage of phone-like values within the analyzed column data.
    /// </summary>
    /// <remarks>
    /// This property calculates the proportion of values in a data column that appear to be phone numbers
    /// based on pattern matching or other recognizable criteria. It is primarily used for identifying columns
    /// containing phone data to apply appropriate masking strategies or categorization.
    /// </remarks>
    public double PhonePercentage { get; set; }

    /// <summary>
    /// Represents the percentage of values within the analyzed column data that appear to be credit card numbers.
    /// </summary>
    /// <remarks>
    /// This property evaluates the proportion of values in a data column that match credit card patterns
    /// (e.g., using common numeric formats or validation algorithms such as Luhn's check). It can be used to
    /// identify columns potentially containing sensitive payment information and to apply appropriate
    /// data-masking strategies for such data.
    /// </remarks>
    public double CreditCardPercentage { get; set; }

    /// <summary>
    /// Represents the percentage of age-related values within the analyzed column data.
    /// </summary>
    /// <remarks>
    /// This property measures the proportion of values in a data column that correspond to age-related data
    /// based on pattern matching or predefined criteria. It is useful for identifying columns that potentially
    /// contain age information, enabling appropriate categorization or masking strategies.
    /// </remarks>
    public double AgePercentage { get; set; }

    /// <summary>
    /// Represents the percentage of numeric values within the analyzed column data.
    /// </summary>
    /// <remarks>
    /// This property calculates the proportion of values in a data column that are recognized as numeric.
    /// It is typically used to assess the column's composition and determine applicable data processing
    /// or masking strategies.
    /// </remarks>
    public double NumericPercentage { get; set; }

    /// <summary>
    /// Represents the average length of the values within a column of data.
    /// </summary>
    /// <remarks>
    /// This property calculates the mean number of characters across all non-null and non-empty values
    /// in a column. It is used to profile the data and to assist in selecting appropriate handling
    /// or masking strategies for the column's contents.
    /// </remarks>
    public double AverageLength { get; set; }

    /// <summary>
    /// Indicates whether the analyzed column data contains personal name-like values.
    /// </summary>
    /// <remarks>
    /// This property evaluates the presence of personal names within the column data
    /// based on specific patterns or predefined criteria. It is typically used to guide
    /// data masking strategies or to identify columns that may contain sensitive
    /// personally identifiable information (PII).
    /// </remarks>
    public bool HasPersonalNames { get; set; }

}
# Data Cleansing & Transformation

Data cleaning is a critical step in any analytics pipeline. Squirrel offers a comprehensive set of methods to clean, validate, and transform your data.

## Table of Contents
- [Removing Rows - Numeric Conditions](#removing-rows---numeric-conditions)
- [Removing Rows - Date Conditions](#removing-rows---date-conditions)
- [Removing Rows - Pattern Matching](#removing-rows---pattern-matching)
- [Removing Rows - String Patterns](#removing-rows---string-patterns)
- [Removing Rows - Generic Conditions](#removing-rows---generic-conditions)
- [Handling Missing Values](#handling-missing-values)
- [Duplicate Removal](#duplicate-removal)
- [Outlier Detection & Removal](#outlier-detection--removal)
- [Data Transformation](#data-transformation)
- [Normalization](#normalization)
- [Data Anonymization](#data-anonymization)
- [Value Replacement](#value-replacement)

---

## Removing Rows - Numeric Conditions

### RemoveGreaterThan
Removes rows where column values are greater than a specified threshold.

**Signature:**
```csharp
public static Table RemoveGreaterThan(this Table tab, string columnName, decimal value)
```

**Example:**
```csharp
// Remove all rows where Age is greater than 120
var cleaned = table.RemoveGreaterThan("Age", 120);

// Remove unrealistic prices
var realistic = products.RemoveGreaterThan("Price", 1000000m);
```

### RemoveGreaterThanOrEqualTo
Removes rows where column values are greater than or equal to a specified value.

**Signature:**
```csharp
public static Table RemoveGreaterThanOrEqualTo(this Table tab, string columnName, decimal value)
```

**Example:**
```csharp
// Remove rows where age is 65 or older (retirement age)
var preRetirement = employees.RemoveGreaterThanOrEqualTo("Age", 65);

// Remove scores at or above threshold
var belowThreshold = results.RemoveGreaterThanOrEqualTo("Score", 100m);
```

### RemoveLessThan
Removes rows where column values are less than a specified minimum.

**Signature:**
```csharp
public static Table RemoveLessThan(this Table tab, string columnName, decimal value)
```

**Example:**
```csharp
// Remove rows where quantity is less than minimum order
var validOrders = orders.RemoveLessThan("Quantity", 5);

// Remove below-threshold measurements
var valid = measurements.RemoveLessThan("Temperature", -40m);
```

### RemoveLessThanOrEqualTo
Removes rows where column values are less than or equal to a specified value.

**Signature:**
```csharp
public static Table RemoveLessThanOrEqualTo(this Table tab, string columnName, decimal value)
```

**Example:**
```csharp
// Remove rows where score is 50 or below
var passing = students.RemoveLessThanOrEqualTo("Score", 50);

// Remove zero or negative prices
var validPrices = products.RemoveLessThanOrEqualTo("Price", 0m);
```

### RemoveIfBetween
Removes rows where numeric values fall within a specified range (inclusive).

**Signatures:**
```csharp
public static Table RemoveIfBetween(this Table tab, string columnName, decimal low, decimal high)
public static Table RemoveIfBetween(this Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
```

**Examples:**
```csharp
// Remove rows where price is between 50 and 100
var cleaned = table.RemoveIfBetween("Price", 50m, 100m);

// Remove suspicious age range
var validated = census.RemoveIfBetween("Age", 150m, 200m);
```

### RemoveIfNotBetween
Removes rows where values are NOT within a specified range.

**Signatures:**
```csharp
public static Table RemoveIfNotBetween(this Table tab, string columnName, decimal low, decimal high)
public static Table RemoveIfNotBetween(this Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
```

**Examples:**
```csharp
// Keep only rows where age is between 18 and 65
var workingAge = population.RemoveIfNotBetween("Age", 18m, 65m);

// Keep only valid temperature range
var validTemp = sensors.RemoveIfNotBetween("Temperature", -20m, 50m);
```

---

## Removing Rows - Date Conditions

### RemoveIfBefore
Removes rows where date values occur before a specified date.

**Signature:**
```csharp
public static Table RemoveIfBefore(this Table tab, string dateColumnName, DateTime date)
```

**Example:**
```csharp
// Remove all orders before January 1, 2023
var recent = orders.RemoveIfBefore("OrderDate", new DateTime(2023, 1, 1));

// Remove old records
var current = records.RemoveIfBefore("CreatedDate", DateTime.Now.AddYears(-1));
```

### RemoveIfAfter
Removes rows where date values occur after a specified date.

**Signature:**
```csharp
public static Table RemoveIfAfter(this Table tab, string dateColumnName, DateTime date)
```

**Example:**
```csharp
// Remove all orders after December 31, 2023
var historical = orders.RemoveIfAfter("OrderDate", new DateTime(2023, 12, 31));

// Remove future-dated entries (data errors)
var valid = transactions.RemoveIfAfter("TransactionDate", DateTime.Now);
```

### RemoveIfBetween (Date Range)
Removes rows where dates fall within a specified date range.

**Signature:**
```csharp
public static Table RemoveIfBetween(this Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
```

**Example:**
```csharp
// Remove vacation period transactions
var cleaned = transactions.RemoveIfBetween("Date", 
    new DateTime(2023, 12, 24), 
    new DateTime(2024, 1, 2));
```

### RemoveIfNotBetween (Date Range)
Removes rows where dates are NOT within a specified date range.

**Signature:**
```csharp
public static Table RemoveIfNotBetween(this Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
```

**Example:**
```csharp
// Keep only Q3 2023 orders
var q3Orders = orders.RemoveIfNotBetween("OrderDate", 
    new DateTime(2023, 7, 1), 
    new DateTime(2023, 9, 30));

// Keep only fiscal year data
var fiscalYear = data.RemoveIfNotBetween("Date",
    new DateTime(2023, 4, 1),
    new DateTime(2024, 3, 31));
```

---

## Removing Rows - Pattern Matching

### RemoveMatches
Removes rows where column values match a regular expression pattern.

**Signature:**
```csharp
public static Table RemoveMatches(this Table tab, string columnName, string regexPattern)
```

**Examples:**
```csharp
// Remove rows where Name contains numbers
var cleaned = table.RemoveMatches("Name", "[0-9]+");

// Remove test accounts
var production = users.RemoveMatches("Email", @"test.*@example\.com");

// Remove rows with special characters in names
var validNames = customers.RemoveMatches("Name", @"[^a-zA-Z\s]");
```

### RemoveNonMatches
Removes rows where column values do NOT match a regular expression pattern.

**Signature:**
```csharp
public static Table RemoveNonMatches(this Table tab, string columnName, string regexPattern)
```

**Examples:**
```csharp
// Keep only valid emails
var validEmails = table.RemoveNonMatches("Email", @"^[^@]+@[^@]+\.[^@]+$");

// Keep only numeric IDs
var validIds = records.RemoveNonMatches("ID", @"^\d+$");

// Keep only phone numbers in correct format
var validPhones = contacts.RemoveNonMatches("Phone", @"^\d{3}-\d{3}-\d{4}$");
```

---

## Removing Rows - String Patterns

### RemoveIfContains
Removes rows where column values contain any of the specified substrings.

**Signature:**
```csharp
public static Table RemoveIfContains(this Table tab, string columnName, params string[] badValues)
```

**Example:**
```csharp
// Remove rows containing profanity or spam markers
var cleaned = comments.RemoveIfContains("Text", "spam", "viagra", "casino");

// Remove test data
var production = data.RemoveIfContains("Description", "test", "debug", "temp");
```

### RemoveIfDoesNotContain
Removes rows where column values do NOT contain all specified substrings.

**Signature:**
```csharp
public static Table RemoveIfDoesNotContain(this Table tab, string columnName, params string[] goodValues)
```

**Example:**
```csharp
// Keep only descriptions that contain all required keywords
var relevant = products.RemoveIfDoesNotContain("Description", "certified", "approved");

// Keep only complete addresses
var complete = addresses.RemoveIfDoesNotContain("Address", "Street", "City", "ZIP");
```

### RemoveIfStartsWith
Removes rows where column values start with any of the specified prefixes.

**Signature:**
```csharp
public static Table RemoveIfStartsWith(this Table tab, string columnName, params string[] badPrefixes)
```

**Example:**
```csharp
// Remove test users
var production = users.RemoveIfStartsWith("Username", "test_", "debug_", "temp_");

// Remove draft entries
var published = articles.RemoveIfStartsWith("Status", "Draft", "Pending");
```

### RemoveIfDoesNotStartWith
Removes rows where column values do NOT start with any of the specified prefixes.

**Signature:**
```csharp
public static Table RemoveIfDoesNotStartWith(this Table tab, string columnName, params string[] goodPrefixes)
```

**Example:**
```csharp
// Keep only entries with valid prefixes
var valid = codes.RemoveIfDoesNotStartWith("Code", "PRD-", "DEV-", "STG-");

// Keep only specific product categories
var filtered = products.RemoveIfDoesNotStartWith("SKU", "ELEC", "FURN", "APPL");
```

### RemoveIfEndsWith
Removes rows where column values end with any of the specified suffixes.

**Signature:**
```csharp
public static Table RemoveIfEndsWith(this Table tab, string columnName, params string[] badSuffixes)
```

**Example:**
```csharp
// Remove temporary files
var permanent = files.RemoveIfEndsWith("Filename", ".tmp", ".temp", ".bak");

// Remove test domains
var production = emails.RemoveIfEndsWith("Email", "@test.com", "@example.com");
```

### RemoveIfDoesNotEndWith
Removes rows where column values do NOT end with any of the specified suffixes.

**Signature:**
```csharp
public static Table RemoveIfDoesNotEndWith(this Table tab, string columnName, params string[] goodSuffixes)
```

**Example:**
```csharp
// Keep only valid image files
var images = files.RemoveIfDoesNotEndWith("Filename", ".jpg", ".png", ".gif");

// Keep only completed quarters
var complete = reports.RemoveIfDoesNotEndWith("Quarter", "Q1", "Q2", "Q3", "Q4");
```

---

## Removing Rows - Generic Conditions

### RemoveIf
Generic removal based on a predicate function with type conversion.

**Signature:**
```csharp
public static Table RemoveIf<T>(this Table tab, string columnName, Func<T, bool> predicate)
    where T : IEquatable<T>
```

**Examples:**
```csharp
// Remove rows where Age is less than 18
var adults = table.RemoveIf<int>("Age", age => age < 18);

// Remove inactive accounts
var active = users.RemoveIf<string>("Status", status => status == "Inactive");

// Remove low scores
var passing = students.RemoveIf<decimal>("Score", score => score < 60m);

// Complex condition
var filtered = data.RemoveIf<DateTime>("Date", 
    date => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
```

### RemoveIfNot
Removes rows that do NOT satisfy a given condition (inverse of RemoveIf).

**Signature:**
```csharp
public static Table RemoveIfNot<T>(this Table tab, string columnName, Func<T, bool> predicate)
    where T : IEquatable<T>
```

**Examples:**
```csharp
// Keep only active records
var active = table.RemoveIfNot<string>("Status", status => status == "Active");

// Keep only adults
var adults = people.RemoveIfNot<int>("Age", age => age >= 18);

// Keep only valid prices
var valid = products.RemoveIfNot<decimal>("Price", price => price > 0);
```

### RemoveIfAnyOf
Removes rows if column values match any value in a list of illegal values.

**Signature:**
```csharp
public static Table RemoveIfAnyOf(this Table tab, string columnName, params string[] illegalValues)
```

**Examples:**
```csharp
// Remove rows with invalid statuses
var cleaned = table.RemoveIfAnyOf("Status", "Deleted", "Archived", "Suspended");

// Remove test data
var production = data.RemoveIfAnyOf("Environment", "test", "dev", "staging");

// Remove invalid grades
var valid = students.RemoveIfAnyOf("Grade", "F", "I", "W");
```

### RemoveIfNotAnyOf
Removes rows if column values do NOT match any value in a list of expected values.

**Signature:**
```csharp
public static Table RemoveIfNotAnyOf(this Table tab, string columnName, params string[] expectedValues)
```

**Examples:**
```csharp
// Keep only specific statuses
var filtered = table.RemoveIfNotAnyOf("Status", "Active", "Pending", "Processing");

// Keep only valid departments
var valid = employees.RemoveIfNotAnyOf("Department", "IT", "HR", "Sales", "Marketing");

// Keep only approved grades
var passing = students.RemoveIfNotAnyOf("Grade", "A", "B", "C", "D");
```

### RemoveCombination
Removes rows based on a two-column predicate.

**Signature:**
```csharp
public static Table RemoveCombination<T, U>(this Table tab, string column1, string column2, 
    Func<T, U, bool> predicate)
```

**Example:**
```csharp
// Remove impossible gender-pregnancy combinations
var valid = medical.RemoveCombination<string, string>("Gender", "Pregnant",
    (gender, pregnant) => gender == "M" && pregnant == "Y");

// Remove age-grade mismatches
var consistent = students.RemoveCombination<int, int>("Age", "Grade",
    (age, grade) => age < grade + 5 || age > grade + 10);
```

---

## Handling Missing Values

### ReplaceMissingValuesByDefault
Replaces missing values according to a specified strategy.

**Signature:**
```csharp
public static Table ReplaceMissingValuesByDefault(this Table tab,
    MissingValueHandlingStrategy strategy = MissingValueHandlingStrategy.MarkWithNa,
    params string[] missingValueRepresentations)
```

**Missing Value Handling Strategies:**
- `MissingValueHandlingStrategy.Zero` - Replace with 0
- `MissingValueHandlingStrategy.Infinity` - Replace with infinity
- `MissingValueHandlingStrategy.MarkWithNa` - Mark as "NA" (default)
- `MissingValueHandlingStrategy.Average` - Replace with column average
- `MissingValueHandlingStrategy.Default` - Replace with type default
- `MissingValueHandlingStrategy.Min` - Replace with column minimum
- `MissingValueHandlingStrategy.Max` - Replace with column maximum

**Examples:**
```csharp
// Replace empty strings and "N/A" with "NA"
var cleaned = table.ReplaceMissingValuesByDefault(
    MissingValueHandlingStrategy.MarkWithNa, 
    "", "N/A", "null");

// Replace missing ages with average age
var filled = census.ReplaceMissingValuesByDefault(
    MissingValueHandlingStrategy.Average,
    "", "NA", "-1");

// Replace missing prices with 0
var products = catalog.ReplaceMissingValuesByDefault(
    MissingValueHandlingStrategy.Zero,
    "", "NULL", "N/A");

// Replace missing values with column maximum
var scores = results.ReplaceMissingValuesByDefault(
    MissingValueHandlingStrategy.Max,
    "", "NA");
```

### MarkAsMissingIfNotAnyOf
Marks values as missing if they don't match expected values.

**Signature:**
```csharp
public static Table MarkAsMissingIfNotAnyOf(this Table tab, string columnName, 
    string missingMarker, params string[] possibleValues)
```

**Example:**
```csharp
// Mark invalid gender values as "NA"
var cleaned = survey.MarkAsMissingIfNotAnyOf("Gender", "NA", "M", "F", "Other");

// Mark invalid status codes
var validated = orders.MarkAsMissingIfNotAnyOf("Status", "INVALID", 
    "Pending", "Shipped", "Delivered", "Cancelled");
```

### RemoveIncompleteRows
Removes rows that have missing values (holes) in any column.

**Signature:**
```csharp
public static Table RemoveIncompleteRows(this Table tab, params string[] missingMarkers)
```

**Example:**
```csharp
// Remove rows with any empty cells
var complete = table.RemoveIncompleteRows("", "NA", "NULL", "N/A");

// Remove rows with missing data markers
var cleaned = survey.RemoveIncompleteRows("", "?", "-", "NA", "N/A", "Unknown");
```

---

## Duplicate Removal

### Distinct
Removes duplicate rows from the table based on all columns.

**Signature:**
```csharp
public static Table Distinct(this Table tab)
```

**Example:**
```csharp
// Remove all duplicate rows
var unique = table.Distinct();

// Remove duplicate customer records
var uniqueCustomers = customers.Distinct();
```

**Note:** This method calculates a hash by combining all column values to identify duplicates. May be slower for tables with many columns.

### DistinctBy
Removes duplicates based on specific columns only.

**Signature:**
```csharp
public static Table DistinctBy(this Table tab, params string[] columnNames)
```

**Examples:**
```csharp
// Remove duplicates based on Email only
var uniqueEmails = users.DistinctBy("Email");

// Remove duplicates based on FirstName and LastName
var uniqueNames = people.DistinctBy("FirstName", "LastName");

// Remove duplicate products by SKU
var uniqueProducts = inventory.DistinctBy("SKU");
```

---

## Outlier Detection & Removal

### RemoveOutliers
Automatically detects and removes statistical outliers from a numeric column.

**Signature:**
```csharp
public static Table RemoveOutliers(this Table tab, string columnName,
    OutlierDetectionAlgorithm algo = OutlierDetectionAlgorithm.IqrInterval)
```

**Outlier Detection Algorithms:**
- `OutlierDetectionAlgorithm.IqrInterval` - Interquartile Range method (default)
- `OutlierDetectionAlgorithm.ZScore` - Z-Score method (threshold: 3.0)
- `OutlierDetectionAlgorithm.ModifiedZScore` - Modified Z-Score method (threshold: 3.5)
- `OutlierDetectionAlgorithm.StandardDeviation` - Standard Deviation method (2σ)
- `OutlierDetectionAlgorithm.Percentile` - Percentile method (5th-95th percentile)

**Examples:**
```csharp
// Remove outliers using IQR method (default)
var cleaned = salaries.RemoveOutliers("Salary");

// Remove outliers using Z-Score
var normalized = measurements.RemoveOutliers("Temperature", 
    OutlierDetectionAlgorithm.ZScore);

// Remove outliers using Modified Z-Score
var filtered = prices.RemoveOutliers("Price", 
    OutlierDetectionAlgorithm.ModifiedZScore);

// Remove outliers using Standard Deviation
var validated = sensors.RemoveOutliers("Reading",
    OutlierDetectionAlgorithm.StandardDeviation);

// Remove outliers using Percentile method
var trimmed = data.RemoveOutliers("Value",
    OutlierDetectionAlgorithm.Percentile);
```

### ExtractOutliers
Extracts rows with outlier values (opposite of RemoveOutliers).

**Signature:**
```csharp
public static Table ExtractOutliers(this Table tab, string columnName,
    OutlierDetectionAlgorithm algo = OutlierDetectionAlgorithm.IqrInterval)
```

**Example:**
```csharp
// Get outlier records for analysis
var outliers = salaries.ExtractOutliers("Salary");

// Extract anomalies using Z-Score
var anomalies = transactions.ExtractOutliers("Amount", 
    OutlierDetectionAlgorithm.ZScore);

// Identify extreme values
var extremes = measurements.ExtractOutliers("Temperature",
    OutlierDetectionAlgorithm.ModifiedZScore);
```

---

## Data Transformation

### Transform
Applies a transformation function to every value in a column.

**Signature:**
```csharp
public static Table Transform(this Table tab, string columnName, Func<string, string> transformer)
```

**Examples:**
```csharp
// Convert names to uppercase
var upper = table.Transform("Name", name => name.ToUpper());

// Trim whitespace from all values
var trimmed = data.Transform("Description", desc => desc.Trim());

// Extract first word
var firstWords = table.Transform("FullName", name => name.Split(' ')[0]);

// Clean phone numbers
var cleaned = contacts.Transform("Phone", 
    phone => phone.Replace("-", "").Replace("(", "").Replace(")", ""));

// Add prefix to all codes
var prefixed = products.Transform("Code", code => "PRD-" + code);

// Format currency
var formatted = prices.Transform("Price", 
    price => decimal.Parse(price).ToString("C2"));
```

### TransformCurrencyToNumeric
Removes currency symbols and formatting from numeric columns.

**Signature:**
```csharp
public static Table TransformCurrencyToNumeric(this Table tab, params string[] columns)
```

**Example:**
```csharp
// Clean currency columns
// "$1,234.56" becomes "1234.56"
// "€999,99" becomes "999.99"
var cleaned = financial.TransformCurrencyToNumeric("Price", "Tax", "Total");

// Clean single column
var products = catalog.TransformCurrencyToNumeric("UnitPrice");
```

**Supported Currency Symbols:** $, £, €, ¥ among many others. 

### KeepJustNumbersAndDecimal
String extension that removes all non-numeric characters except decimals.

**Signature:**
```csharp
public static string KeepJustNumbersAndDecimal(this string input)
```

**Example:**
```csharp
// Use with Transform
var cleaned = table.Transform("Price", price => price.KeepJustNumbersAndDecimal());

// Direct usage
string clean = "$1,234.56 USD".KeepJustNumbersAndDecimal(); // Returns "1234.56"
string number = "ABC123.45XYZ".KeepJustNumbersAndDecimal(); // Returns "123.45"
```

### ConvertOnesAndZerosToBoolean
Converts numeric 1s and 0s to boolean text representations.

**Signature:**
```csharp
public static Table ConvertOnesAndZerosToBoolean(this Table tab, string column)
```

**Example:**
```csharp
// Convert binary flags to boolean
// "1" becomes "true", "0" becomes "false"
var converted = users.ConvertOnesAndZerosToBoolean("IsActive");

// Multiple columns
var cleaned = survey
    .ConvertOnesAndZerosToBoolean("Married")
    .ConvertOnesAndZerosToBoolean("Employed")
    .ConvertOnesAndZerosToBoolean("HasChildren");
```

### Truncate
Truncates string values to a specified length.

**Signatures:**
```csharp
public static Table Truncate(this Table tab, string columnName, int length)
public static Table Truncate(this Table tab, Dictionary<string, int> truncateLengths)
```

**Examples:**
```csharp
// Truncate descriptions to 50 characters
var short = products.Truncate("Description", 50);

// Truncate multiple columns with different lengths
var truncated = data.Truncate(new Dictionary<string, int>
{
    { "Title", 100 },
    { "Summary", 250 },
    { "Author", 50 }
});
```

---

## Normalization

### Normalize
Normalizes string values according to various strategies.

**Signatures:**
```csharp
public static Table Normalize(this Table tab, string columnName, 
    NormalizationStrategy strategy = NormalizationStrategy.SentenceCase)
public static Table Normalize(this Table tab, 
    Dictionary<string, NormalizationStrategy> normalizationSchemes)
```

**Normalization Strategies:**
- `NormalizationStrategy.SentenceCase` - First letter uppercase, rest lowercase
- `NormalizationStrategy.NameCase` - Capitalize each word (Title Case)
- `NormalizationStrategy.UpperCase` - All uppercase
- `NormalizationStrategy.LowerCase` - All lowercase
- `NormalizationStrategy.TerminateAtSpace` - Keep only text before first space
- `NormalizationStrategy.TerminateAtNumber` - Keep only text before first number
- `NormalizationStrategy.TerminateAtFirstNonAlphaNumeric` - Keep only alphanumeric prefix
- `NormalizationStrategy.MostFrequentOne` - Replace all with most common value

**Examples:**
```csharp
// Normalize names to proper case
var normalized = table.Normalize("Name", NormalizationStrategy.NameCase);

// Normalize multiple columns with different strategies
var cleaned = data.Normalize(new Dictionary<string, NormalizationStrategy>
{
    { "FirstName", NormalizationStrategy.NameCase },
    { "LastName", NormalizationStrategy.NameCase },
    { "Email", NormalizationStrategy.LowerCase },
    { "ProductCode", NormalizationStrategy.UpperCase },
    { "Description", NormalizationStrategy.SentenceCase }
});

// Extract clean data before extra information
var cleaned = messy.Normalize("Name", NormalizationStrategy.TerminateAtSpace);
// "John Doe (Manager)" becomes "John"

// Remove suffixes with numbers
var products = catalog.Normalize("SKU", NormalizationStrategy.TerminateAtNumber);
// "PROD123" becomes "PROD"

// Standardize inconsistent data
var standard = survey.Normalize("Country", NormalizationStrategy.MostFrequentOne);
```

### AutoNormalize
Automatically detects and applies the best normalization strategy.

**Signatures:**
```csharp
public static Table AutoNormalize(this Table tab)
public static Table AutoNormalize(this Table tab, string column)
```

**Examples:**
```csharp
// Auto-normalize all columns
var cleaned = messyData.AutoNormalize();

// Auto-normalize specific column
var normalized = data.AutoNormalize("CustomerName");
```

**Detection Logic:**
- Analyzes column names and data patterns
- Detects personal names, titles, codes, emails, URLs
- Identifies case inconsistencies
- Determines optimal normalization strategy

---

## Data Anonymization

### Anonymize
Anonymizes sensitive data by replacing values with generated identifiers.

**Signature:**
```csharp
public static Table Anonymize(this Table tab, string columnName, string anonymizationPrefix)
```

**Examples:**
```csharp
// Anonymize customer names
// "John Smith" → "CUST-0001", "Jane Doe" → "CUST-0002"
var anonymous = customers.Anonymize("CustomerName", "CUST-");

// Anonymize email addresses
var protected = users.Anonymize("Email", "USER-");

// Anonymize multiple columns
var fullyAnonymized = data
    .Anonymize("Name", "NAME-")
    .Anonymize("Email", "EMAIL-")
    .Anonymize("Phone", "PHONE-");

// Anonymize SSN
var secure = records.Anonymize("SSN", "ID-");
```

**Features:**
- Each unique value gets a unique identifier
- Maintains referential integrity (same input = same output)
- Generates sequential IDs with 4-digit padding (0001, 0002, etc.)
- Preserves data relationships while protecting privacy

---

## Value Replacement

### ReplaceXWithY
Replaces specific values with new values in a column.

**Signatures:**
```csharp
public static Table ReplaceXWithY(this Table tab, string columnName, string x, string y)
public static Table ReplaceXsWithY(this Table tab, string columnName, string y, params string[] xs)
public static Table ReplaceXWithY(this Table tab, 
    Dictionary<string, KeyValuePair<string, string>> values)
```

**Examples:**
```csharp
// Simple replacement
var cleaned = table.ReplaceXWithY("Status", "N/A", "Unknown");

// Replace multiple values with one
var normalized = data.ReplaceXsWithY("Gender", "Unknown", "N/A", "NA", "", "?");

// Replace multiple columns
var cleaned = survey.ReplaceXWithY(new Dictionary<string, KeyValuePair<string, string>>
{
    { "Gender", new KeyValuePair<string, string>("M", "Male") },
    { "Status", new KeyValuePair<string, string>("A", "Active") },
    { "Type", new KeyValuePair<string, string>("P", "Premium") }
});

// Standardize boolean values
var standardized = data
    .ReplaceXsWithY("Active", "Yes", "Y", "yes", "1", "true")
    .ReplaceXsWithY("Active", "No", "N", "no", "0", "false");
```

---

## Data Validation

### RemoveRowsWithImpossibleValues
Removes rows with impossible or invalid value combinations.

**Signatures:**
```csharp
public static Table RemoveRowsWithImpossibleValues(this Table tab, 
    string columnName, params string[] possibleValues)
public static Table RemoveRowsWithImpossibleValues(this Table tab, 
    Dictionary<string, string[]> possibleValueCombos)
```

**Examples:**
```csharp
// Remove rows with invalid gender values
var valid = census.RemoveRowsWithImpossibleValues("Gender", "M", "F", "Other");

// Remove rows with invalid status codes
var cleaned = orders.RemoveRowsWithImpossibleValues("Status", 
    "Pending", "Shipped", "Delivered", "Cancelled");

// Validate multiple columns
var validated = data.RemoveRowsWithImpossibleValues(
    new Dictionary<string, string[]>
    {
        { "Gender", new[] { "M", "F", "Other" } },
        { "MaritalStatus", new[] { "Single", "Married", "Divorced", "Widowed" } },
        { "EmploymentStatus", new[] { "Employed", "Unemployed", "Retired", "Student" } }
    });
```

---

## Summary of Common Use Cases

### Cleaning Numeric Data
```csharp
// Remove outliers and invalid ranges
var cleaned = data
    .RemoveOutliers("Salary")
    .RemoveGreaterThan("Age", 120)
    .RemoveLessThan("Age", 0)
    .RemoveIfNotBetween("Temperature", -50m, 50m);
```

### Cleaning Date Data
```csharp
// Keep only valid date ranges
var valid = transactions
    .RemoveIfBefore("Date", new DateTime(2023, 1, 1))
    .RemoveIfAfter("Date", DateTime.Now)
    .RemoveIfNotBetween("Date", startDate, endDate);
```

### Cleaning Text Data
```csharp
// Normalize and clean text
var cleaned = customers
    .Normalize("Name", NormalizationStrategy.NameCase)
    .Normalize("Email", NormalizationStrategy.LowerCase)
    .RemoveMatches("Phone", @"[^0-9\-\(\)\s]")
    .Transform("Description", d => d.Trim());
```

### Handling Missing Data
```csharp
// Multiple strategies for missing values
var filled = survey
    .ReplaceMissingValuesByDefault(MissingValueHandlingStrategy.Average, "", "NA")
    .RemoveIncompleteRows("", "N/A", "Unknown");
```

### Data Privacy
```csharp
// Anonymize sensitive information
var protected = patients
    .Anonymize("PatientName", "PAT-")
    .Anonymize("SSN", "ID-")
    .Anonymize("Email", "EMAIL-");
```

### Comprehensive Cleaning Pipeline
```csharp
// Full data cleaning workflow
var cleaned = rawData
    .Distinct()                                    // Remove duplicates
    .RemoveIncompleteRows("", "NA", "N/A")        // Remove incomplete rows
    .RemoveOutliers("Salary")                      // Remove outliers
    .RemoveIfNotBetween("Age", 18m, 120m)         // Valid age range
    .RemoveMatches("Email", @"test.*@")           // Remove test emails
    .Normalize("Name", NormalizationStrategy.NameCase)  // Normalize names
    .TransformCurrencyToNumeric("Salary", "Bonus")      // Clean currency
    .ReplaceXsWithY("Status", "Active", "A", "1", "Yes")  // Standardize status
    .ConvertOnesAndZerosToBoolean("IsManager");   // Convert to boolean
```

---

## Error Handling

All cleansing methods include built-in validation:
- Throws `ArgumentNullException` if table or required parameters are null
- Throws `ArgumentException` if column doesn't exist in table
- Throws `FormatException` if data cannot be converted to expected type
- Throws `ArgumentOutOfRangeException` if numeric parameters are invalid

**Example with error handling:**
```csharp
try
{
    var cleaned = data
        .RemoveOutliers("Salary")
        .RemoveIfNotBetween("Age", 0m, 120m);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Column not found: {ex.Message}");
}
catch (FormatException ex)
{
    Console.WriteLine($"Data type conversion failed: {ex.Message}");
}
```

---

This comprehensive guide covers all data cleansing methods available in Squirrel. For more information on data acquisition, analysis, and visualization, see the other documentation sections.

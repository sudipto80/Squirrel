# Data Cleaning & Transformation

Data cleaning is a critical step in any analytics pipeline. Squirrel offers a wide range of methods to filter, clean, and transform your data.

## Adding and Removing Columns

### AddColumn
Adds a new column to the table.

**Signatures:**
```csharp
void AddColumn(string columnName, string formula, int decimalDigits)
void AddColumn(string columnName, List<string> values)
```

**Examples:**
```csharp
// Add a calculated column
table.AddColumn("Total", "Price * Quantity", 2);

// Add a column with predefined values
var statuses = new List<string> { "Active", "Pending", "Active" };
table.AddColumn("Status", statuses);
```

### RemoveColumn
Removes a specified column from the table.

```csharp
table.RemoveColumn("TemporaryColumn");
```

### ModifyColumnName
Modifies the name of an existing column.

```csharp
table.ModifyColumnName("OldName", "NewName");
```

### ExtractAndAddAsColumn
Extracts values from a column based on a regex pattern and adds them as a new column.

```csharp
// Extract area code from phone numbers
table.ExtractAndAddAsColumn("Phone", @"\((\d{3})\)", "AreaCode");
```

## Adding and Removing Rows

### AddRow
Adds a new row to the table.

```csharp
table.AddRow("John", "30", "Engineer");
```

### AddRows
Adds multiple rows to the table.

```csharp
var newRows = new List<string[]>
{
    new[] { "Alice", "28", "Designer" },
    new[] { "Bob", "35", "Manager" }
};
table.AddRows(newRows);
```

### AddRowsByShortHand
Adds multiple rows to the table using a shorthand syntax.

```csharp
// Add 5 rows with incrementing ID
table.AddRowsByShortHand("ID++", 5);
```

### Drop
Removes rows from the table.

```csharp
// Drop all columns except specified ones
var simplified = table.Drop("TempColumn1", "TempColumn2");
```

### Shuffle
Randomly shuffles the rows of the table.

```csharp
var shuffled = table.Shuffle();
```

## Filtering and Subsetting

### Filter
Filters the table to find specific values.

**Signatures:**
```csharp
Table Filter(Func<Dictionary<string,string>,bool> predicate)
Table Filter(Dictionary<string,List<string>> fieldSearchValuesMap)
Table Filter(Dictionary<string,string> _fieldSearchValueMap)
Table Filter(string column, string[] values)
```

**Examples:**
```csharp
// Filter with a predicate
var result = table.Filter(row => row["Age"] == "30" && row["City"] == "Seattle");

// Filter by specific column values
var techEmployees = table.Filter("Department", new[] { "IT", "Engineering" });

// Filter using a dictionary
var filterMap = new Dictionary<string, string> { { "Status", "Active" }, { "Type", "Premium" } };
var filtered = table.Filter(filterMap);
```

### FilterByRegex
Filters the table based on a regex pattern in a specific column.

```csharp
// Filter for email addresses from gmail.com
var gmailUsers = table.FilterByRegex("Email", @"@gmail\.com$");
```

### Pick
Selects specific rows from the table.

```csharp
// Pick only Name and Email columns
var subset = table.Pick("Name", "Email");
```

### Bottom
Selects the bottom N rows of the table.

```csharp
// Get the last 10 rows
var lastTen = table.Bottom(10);
```

### BottomNPercent
Selects the bottom N percent of rows from the table.

```csharp
// Get the bottom 25% of rows
var bottomQuarter = table.BottomNPercent(25);
```

### Middle
Selects the middle rows of the table.

```csharp
// Get 20 rows from the middle, starting at row 50
var middleRows = table.Middle(50, 20);
```

### Exclusive
Returns rows that are exclusive to the table when compared with another.

```csharp
// Find customers who are in table1 but not in table2
var exclusiveCustomers = table1.Exclusive(table2);
```

### RemoveGreaterThan
Removes rows where the value in a specified column is greater than a given value.

```csharp
// Remove all rows where price is greater than 100
table = table.RemoveGreaterThan("Price", 100);
```

### RemoveGreaterThanOrEqualTo
Removes rows where the value in a specified column is greater than or equal to a given value.

```csharp
// Remove rows where age is 65 or older
table = table.RemoveGreaterThanOrEqualTo("Age", 65);
```

### RemoveLessThan
Removes rows where the value in a specified column is less than a given value.

```csharp
// Remove rows where quantity is less than 5
table = table.RemoveLessThan("Quantity", 5);
```

### RemoveLessThanOrEqualTo
Removes rows where the value in a specified column is less than or equal to a given value.

```csharp
// Remove rows where score is 50 or below
table = table.RemoveLessThanOrEqualTo("Score", 50);
```

### RemoveIf
Remove rows based on a condition.
```csharp
// Remove rows where Age is less than 18
table.RemoveIf("Age", age => int.Parse(age) < 18);
```

### RemoveIfAfter
Removes rows if a date in a specified column is after a given date.

```csharp
// Remove all orders after December 31, 2023
table = table.RemoveIfAfter("OrderDate", new DateTime(2023, 12, 31));
```

### RemoveIfBefore
Removes rows if a date in a specified column is before a given date.

```csharp
// Remove all orders before January 1, 2023
table = table.RemoveIfBefore("OrderDate", new DateTime(2023, 1, 1));
```

### RemoveIfBetween
Removes rows if a value in a specified column is between two given values.

**Signatures:**
```csharp
Table RemoveIfBetween(Table tab, string columnName, decimal low, decimal high)
Table RemoveIfBetween(Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
```

**Examples:**
```csharp
// Remove rows where price is between 50 and 100
table = table.RemoveIfBetween("Price", 50, 100);

// Remove rows with dates in a specific range
table = table.RemoveIfBetween("OrderDate", 
    new DateTime(2023, 6, 1), 
    new DateTime(2023, 12, 31));
```

### RemoveIfNot
Removes rows that do not satisfy a given condition.

```csharp
// Keep only active records
table = table.RemoveIfNot("Status", status => status == "Active");
```

### RemoveIfAnyOf
Removes rows if a value in a specified column matches any of the provided values.

```csharp
// Remove rows with invalid statuses
table = table.RemoveIfAnyOf("Status", new[] { "Deleted", "Archived", "Suspended" });
```

### RemoveIfNotAnyOf
Removes rows if a value in a specified column does not match any of the provided values.

```csharp
// Keep only rows with specific statuses
table = table.RemoveIfNotAnyOf("Status", new[] { "Active", "Pending", "Processing" });
```

### RemoveIfNotBetween
Removes rows if a value in a specified column is not between two given values.

**Signatures:**
```csharp
Table RemoveIfNotBetween(Table tab, string columnName, decimal low, decimal high)
Table RemoveIfNotBetween(Table tab, string dateColumnName, DateTime startDate, DateTime endDate)
```

**Examples:**
```csharp
// Keep only rows where age is between 18 and 65
table = table.RemoveIfNotBetween("Age", 18, 65);

// Keep only orders from Q3 2023
table = table.RemoveIfNotBetween("OrderDate", 
    new DateTime(2023, 7, 1), 
    new DateTime(2023, 9, 30));
```

### RemoveMatches
Removes rows that match a specific pattern.

```csharp
// Remove all test accounts
table = table.RemoveMatches("Email", @"test.*@example\.com");
```

### RemoveNonMatches
Keep only rows where a column matches a specific pattern (Regex).
```csharp
// Keep only valid emails
table.RemoveNonMatches("Email", @"^[^@]+@[^@]+\.[^@]+$");
```

### RemoveOutliers
Automatically detect and remove statistical outliers from a numeric column.
```csharp
table.RemoveOutliers("Salary");
```

### RemoveImpossibleCombinations
Removes impossible combinations from data, like gender:male, pregnant:yes.

**Signature:**
```csharp
public static Table RemoveImpssibleCombinations(this Table tab, Dictionary<string,string> impossibleCombos)
```
**Example:**
```csharp
Dictionary<string, string> impComb = new Dictionary<string, string>();
impComb.Add("Gender", "M");
impComb.Add("Pregnant", "Y");
tab = tab.RemoveImpossibleCombinations(impossibleCombos:impComb);
```

## Transforming Data

### Transform
Apply a transformation function to every value in a column.
```csharp
// Convert names to uppercase
table.Transform("Name", name => name.ToUpper());
```

### ConvertOnesAndZerosToBoolean
Converts all 1s and 0s in a column to boolean `true` or `false`.

```csharp
table.ConvertOnesAndZerosToBoolean("IsActive");
```

### KeepJustNumbersAndDecimal
Removes all non-numeric characters from a column, except for decimals.

```csharp
// Clean currency values: "$1,234.56" becomes "1234.56"
table.KeepJustNumbersAndDecimal("Price");
```

### RoundOffTo
Rounds the numeric values in columns to a specified number of decimal places.

**Examples:**
```csharp
// Round all numeric columns to 2 decimal places
table = table.RoundOffTo(2);

// Specify different rounding for each column
table = table.RoundOffTo("Price:2,Tax:2,Weight:1");
```

## Merging Data

### Merge
Merges two tables.

```csharp
// Combine two tables, removing duplicates
var combined = table1.Merge(table2);
```

### MergeByColumns
Merges two tables based on matching column values.

```csharp
// Join customers and orders on CustomerID
var result = customers.MergeByColumns(orders, "CustomerID");
```

### MergeColumns
Merges two or more columns into a single column.

```csharp
// Combine FirstName and LastName into FullName
table.MergeColumns("FullName", " ", "FirstName", "LastName");
```

## Handling Missing Values

### ReplaceMissingValues
Replaces missing values in the table with a specified value.

```csharp
// Replace missing ages with 0
table.ReplaceMissingValues("Age", "0");

// Replace missing values with column average
table.ReplaceMissingValues("Salary", ReplacementStrategy.Average);
```

## Data Masking

Protect sensitive information using masking strategies.

```csharp
// Mask credit card numbers, keeping only the last 4 digits
table.MaskColumn("CreditCard", MaskingStrategy.StarExceptLastFour);
```

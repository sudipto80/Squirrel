# Analysis & Statistics

Squirrel includes built-in statistical functions to help you understand your data.

## Basic Statistics

The `BasicStatistics` class and `Table` extensions provide common statistical measures.

- **Mean / Average**: Calculate the average value of a column.
- **Median**: Find the middle value.
- **Mode**: Find the most frequent value.
- **Standard Deviation**: Measure the amount of variation.
- **Variance**: Calculate the variance of the sequence.
- **Kurtosis**: Measure the "tailedness" of the probability distribution.
- **Skewness**: Measure the asymmetry of the probability distribution.
- **Range**: Calculate the difference between the maximum and minimum values.

```csharp
// Basic statistics examples
var avgAge = table.GetColumn("Age").Average();
var medianIncome = table.GetColumn("Income").Median();
var stdDev = table.GetColumn("Salary").StandardDeviation();
var variance = table.GetColumn("Score").Variance();
var kurtosis = table.GetColumn("Values").Kurtosis();
var range = table.GetColumn("Price").Range();
```

## Aggregation

### Aggregate
The `Aggregate` method flattens a table as per a given aggregation scheme. It has two overloaded versions.

**Signatures:**
```csharp
Table Aggregate(string columnName, AggregationMethod how = AggregationMethod.Sum)
Table Aggregate(string columnName, Func<List<string>, string> how)
```

The first parameter is the name of the non-numeric column that should be present in the final result, and the second is the aggregation scheme.

```csharp
// Group by Department and calculate average Salary
var summary = table.Aggregate("Department", AggregationMethod.Average);

// Find range of values for all numeric fields grouped by Name
iris.Aggregate("Name", AggregationMethod.Range).PrettyDump();
```

**Supported Aggregation Methods:**
- **Sum**: Summation of the sequence
- **Average** (Mean): Average of the sequence
- **Max**: Maximum value of the sequence
- **Min**: Minimum value of the sequence
- **Count**: Total count of the sequence
- **StandardDeviation**: Standard deviation of the sequence
- **Variance**: Variance of the sequence
- **AboveAverageCount**: Number of instances above average
- **BelowAverageCount**: Number of instances below average
- **AverageCount**: Number of instances equal to average
- **Skew**: Measures the skewness of the sequence
- **Kurtosis**: Measures the kurtosis of the sequence
- **Range**: Range of values in the sequence

**Custom Aggregation:**
You can provide a custom aggregation function using the second overload:
```csharp
var result = table.Aggregate("Category", values => 
{
    // Custom aggregation logic
    return values.Count().ToString();
});
```

### AggregateColumns
Aggregates columns in the table.

```csharp
// Aggregate multiple columns
var result = table.AggregateColumns(
    new[] { "Sales", "Revenue", "Profit" },
    AggregationMethod.Sum
);
```

## Data Exploration

### Distinct
Get distinct values from the table.

```csharp
// Remove duplicate rows
var uniqueRows = table.Distinct();
```

### ValuesOf
Extracts all values from a specified column.

**Signatures:**
```csharp
List<string> ValuesOf(string columnName)
List<T> ValuesOf<T>(string columnName)
```

```csharp
// Get all values from a column as strings
var names = table.ValuesOf("Name");

// Get typed values
var ages = table.ValuesOf<int>("Age");
```

### HowMany
Counts the number of rows in the table.

```csharp
// Get total row count
var rowCount = table.HowMany();
Console.WriteLine($"Total rows: {rowCount}");
```

### DoesMatchingEntryExist
Checks if a matching entry exists in the table.

```csharp
// Check if a specific entry exists
var exists = table.DoesMatchingEntryExist(row => 
    row["Name"] == "John" && row["Age"] == "30"
);
```

### IsSubset
Determines if one table is a subset of another.

```csharp
// Check if table2 is a subset of table1
var isSubset = table1.IsSubset(table2);
```

## Outlier Detection

### ExtractOutliers
Identify anomalies in your dataset using outlier detection algorithms.

```csharp
var outliers = table.ExtractOutliers("Temperature");
```

### IQRRange
Calculate the Interquartile Range (IQR) for a column.

```csharp
// Calculate IQR for a numeric column
var iqr = table.IQRRange("Salary");
Console.WriteLine($"IQR: {iqr}");
```

## Percentages and Ratios

### GetPercentage
Calculate percentages for values in a column.

```csharp
// Get percentage of a specific value
var percentage = table.GetPercentage("Status", "Active");
Console.WriteLine($"Active records: {percentage}%");
```

## Sampling

### RandomSample
Get a random sample from the table.

```csharp
// Get 100 random rows
var sample = table.RandomSample(100);
```

## Advanced Analysis

### CumulativeFold
Apply a cumulative fold operation on a column.

```csharp
// Calculate running total
var runningTotal = table.CumulativeFold("Sales", (acc, val) => acc + val);
```

### Bottom
Get the bottom N rows from the table.

```csharp
// Get the last 10 rows
var bottomRows = table.Bottom(10);
```

### Middle
Get the middle rows from the table.

```csharp
// Get 50 rows from the middle, starting at row 100
var middleRows = table.Middle(100, 50);
```

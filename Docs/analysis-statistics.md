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
var avgAge = table.GetColumn("Age").Average();
var medianIncome = table.GetColumn("Income").Median();
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

**Example:**
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

## Data Exploration

### Distinct
Get distinct values from the table.

### ValuesOf
Extracts all values from a specified column.

**Signatures:**
```csharp
List<string> ValuesOf(string columnName)
List<T> ValuesOf<T>(string columnName)
```

### HowMany
Counts the number of rows in the table.

### DoesMatchingEntryExist
Checks if a matching entry exists in the table.

### IsSubset
Determines if one table is a subset of another.

## Outlier Detection

### ExtractOutliers
Identify anomalies in your dataset using outlier detection algorithms.

```csharp
var outliers = table.ExtractOutliers("Temperature");
```

### IQRRange
Calculate the Interquartile Range (IQR) for a column.

## Percentages and Ratios

### GetPercentage
Calculate percentages for values in a column.

## Sampling

### RandomSample
Get a random sample from the table.

## Advanced Analysis

### CumulativeFold
Apply a cumulative fold operation on a column.

### Bottom
Get the bottom N rows from the table.

### Middle
Get the middle rows from the table.

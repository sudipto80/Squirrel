# Basic Statistics

Squirrel provides a comprehensive set of statistical functions for analyzing numeric data. These methods work seamlessly with Table columns and provide essential statistical measures for data analysis.

## Table of Contents
- [Central Tendency](#central-tendency)
  - [Median](#median)
- [Dispersion Measures](#dispersion-measures)
  - [Range](#range)
  - [Variance](#variance)
  - [Median Absolute Deviation](#median-absolute-deviation)
  - [Interquartile Range](#interquartile-range)
- [Distribution Shape](#distribution-shape)
  - [Kurtosis](#kurtosis)
- [Percentiles](#percentiles)
  - [Percentile Calculation](#percentile-calculation)
- [Comparative Counts](#comparative-counts)
  - [Above Average Count](#above-average-count)
  - [Below Average Count](#below-average-count)
  - [Average Count](#average-count)
- [Usage with Tables](#usage-with-tables)
- [Complete Examples](#complete-examples)

---

## Central Tendency

### Median

#### Median
Calculates the median (middle value) of a list of numbers.

**Signature:**
```csharp
public static decimal Median(List<decimal> numbers)
```

**Parameters:**
- `numbers` - List of decimal values

**Returns:**
- For odd-length lists: the middle value
- For even-length lists: the average of the two middle values

**Examples:**
```csharp
// Odd number of values
var values1 = new List<decimal> { 1, 3, 5, 7, 9 };
var median1 = BasicStatistics.Median(values1); // Returns 5

// Even number of values
var values2 = new List<decimal> { 1, 2, 3, 4, 5, 6 };
var median2 = BasicStatistics.Median(values2); // Returns 3.5

// With Table data - using ValuesOf
var salaries = employees["Salary"].Select(Convert.ToDecimal).ToList();
var medianSalary = BasicStatistics.Median(salaries);
Console.WriteLine($"Median Salary: {medianSalary:C}");

// Alternative: using ValuesOf method
var salariesAlt = employees.ValuesOf("Salary").Select(decimal.Parse).ToList();
var medianAlt = BasicStatistics.Median(salariesAlt);

// Unsorted data (automatically sorted)
var unsorted = new List<decimal> { 45, 12, 78, 23, 56, 34 };
var median = BasicStatistics.Median(unsorted); // Returns 39.5
```

**Features:**
- Automatically sorts the data
- Handles both odd and even-length lists
- More robust than mean for skewed distributions
- Not affected by extreme outliers

---

## Dispersion Measures

### Range

#### Range
Calculates the range (difference between max and min) of numeric values.

**Signature:**
```csharp
public static decimal Range(this IEnumerable<decimal> values)
```

**Examples:**
```csharp
// Simple range
var values = new List<decimal> { 10, 25, 15, 30, 20 };
var range = values.Range(); // Returns 20 (30 - 10)

// With Table data - using indexer
var prices = products["Price"].Select(decimal.Parse).ToList();
var priceRange = prices.Range();
Console.WriteLine($"Price Range: {priceRange:C}");

// Temperature range
var temps = weather["Temperature"].Select(decimal.Parse).ToList();
var tempRange = temps.Range();
Console.WriteLine($"Temperature varies by {tempRange}°");

// Analyze multiple columns
var ageRange = census["Age"].Select(decimal.Parse).ToList().Range();
var incomeRange = census["Income"].Select(decimal.Parse).ToList().Range();
Console.WriteLine($"Age Range: {ageRange} years");
Console.WriteLine($"Income Range: ${incomeRange:N0}");
```

**Use Cases:**
- Quick measure of data spread
- Identifying variability
- Quality control (acceptable ranges)
- Data validation

---

### Variance

#### Variance
Calculates the variance of numeric values (measure of spread around the mean).

**Signatures:**
```csharp
public static double Variance(this IEnumerable<decimal> values)
public static double Variance(this IEnumerable<double> values)
```

**Formula:**
```
Variance = Σ(x - mean)² / n
```

**Examples:**
```csharp
// Calculate variance
var values = new List<decimal> { 2, 4, 6, 8, 10 };
var variance = values.Variance(); // Returns 8.0

// With Table data - measure score consistency
var scores = students["TestScore"].Select(decimal.Parse).ToList();
var scoreVariance = scores.Variance();
Console.WriteLine($"Score Variance: {scoreVariance:F2}");

// Compare variance across groups
var groupA = salesTable
    .Filter("Region", "North")
    ["Sales"].Select(decimal.Parse).ToList()
    .Variance();

var groupB = salesTable
    .Filter("Region", "South")
    ["Sales"].Select(decimal.Parse).ToList()
    .Variance();

Console.WriteLine($"North Region Variance: {groupA:F2}");
Console.WriteLine($"South Region Variance: {groupB:F2}");

if (groupA > groupB)
    Console.WriteLine("North region has more variable sales");
else
    Console.WriteLine("South region has more variable sales");
```

**Interpretation:**
- **Low variance**: Values are close to the mean (consistent)
- **High variance**: Values are spread out (variable)
- **Variance = 0**: All values are identical

**Use Cases:**
- Measuring consistency
- Quality control
- Risk assessment
- Comparing variability between groups

---

### Median Absolute Deviation

#### MedianAbsoluteDeviation
Calculates the Median Absolute Deviation (MAD) - a robust measure of variability less sensitive to outliers than standard deviation.

**Signature:**
```csharp
public static decimal MedianAbsoluteDeviation(this List<decimal> values)
```

**Formula:**
```
MAD = median(|xi - median(x)|)
```

**Examples:**
```csharp
// Calculate MAD
var values = new List<decimal> { 2, 4, 6, 8, 10, 100 }; // Note: 100 is an outlier
var mad = values.MedianAbsoluteDeviation();
Console.WriteLine($"MAD: {mad}"); // Robust to the outlier (100)

// Compare with regular deviation
var median = BasicStatistics.Median(values);
Console.WriteLine($"Median: {median}");
Console.WriteLine($"MAD: {mad}");

// With Table data - robust outlier detection
var incomes = census["Income"].Select(decimal.Parse).ToList();
var medianIncome = BasicStatistics.Median(incomes);
var mad = incomes.MedianAbsoluteDeviation();

// Flag values more than 3 MADs from median (robust outliers)
var threshold = 3m * mad;
var outliers = census.GetRowsWhere(row =>
{
    var income = decimal.Parse(row["Income"]);
    return Math.Abs(income - medianIncome) > threshold;
});

Console.WriteLine($"Found {outliers.RowCount} outliers using MAD");

// Compare different regions
var regions = salesData.SplitOn("Region");
foreach (var region in regions)
{
    var sales = region.Value["Sales"].Select(decimal.Parse).ToList();
    var regionMAD = sales.MedianAbsoluteDeviation();
    Console.WriteLine($"{region.Key} MAD: {regionMAD:F2}");
}
```

**Advantages over Standard Deviation:**
- **Robust**: Not affected by extreme outliers
- **Reliable**: More stable for non-normal distributions
- **Interpretable**: Based on median, easier to understand

**Use Cases:**
- Outlier detection in real-world data
- Robust statistical analysis
- Data with extreme values
- Non-normal distributions

---

### Interquartile Range

#### IqrRange
Calculates the Interquartile Range (IQR) boundaries for outlier detection.

**Signature:**
```csharp
public static Tuple<decimal, decimal> IqrRange(List<decimal> numbers)
```

**Returns:**
- Tuple containing (LowerBound, UpperBound)
- LowerBound = Q1 - 1.5 × IQR
- UpperBound = Q3 + 1.5 × IQR

**Formula:**
```
Q1 = 25th percentile
Q3 = 75th percentile
IQR = Q3 - Q1
Lower Bound = Q1 - 1.5 × IQR
Upper Bound = Q3 + 1.5 × IQR
```

**Examples:**
```csharp
// Calculate IQR boundaries
var values = new List<decimal> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 100 };
var iqrBounds = BasicStatistics.IqrRange(values);
Console.WriteLine($"Lower Bound: {iqrBounds.Item1}");
Console.WriteLine($"Upper Bound: {iqrBounds.Item2}");

// Identify outliers
var outliers = values.Where(v => 
    v < iqrBounds.Item1 || v > iqrBounds.Item2).ToList();
Console.WriteLine($"Outliers: {string.Join(", ", outliers)}");

// Use with Table data
var salaries = employees["Salary"].Select(decimal.Parse).ToList();
var salaryBounds = BasicStatistics.IqrRange(salaries);

// Filter out salary outliers
var cleanedEmployees = employees.GetRowsWhere(row =>
{
    var salary = decimal.Parse(row["Salary"]);
    return salary >= salaryBounds.Item1 && salary <= salaryBounds.Item2;
});

Console.WriteLine($"Removed {employees.RowCount - cleanedEmployees.RowCount} outliers");

// Visualize distribution
var prices = products["Price"].Select(decimal.Parse).ToList();
var bounds = BasicStatistics.IqrRange(prices);
var medianVal = BasicStatistics.Median(prices);
var q1 = BasicStatistics.Median(prices.Where(p => p < medianVal).ToList());
var q3 = BasicStatistics.Median(prices.Where(p => p > medianVal).ToList());

Console.WriteLine("Box Plot Statistics:");
Console.WriteLine($"Min (excluding outliers): {prices.Where(p => p >= bounds.Item1).Min()}");
Console.WriteLine($"Q1: {q1}");
Console.WriteLine($"Median: {medianVal}");
Console.WriteLine($"Q3: {q3}");
Console.WriteLine($"Max (excluding outliers): {prices.Where(p => p <= bounds.Item2).Max()}");
Console.WriteLine($"Outliers: {prices.Count(p => p < bounds.Item1 || p > bounds.Item2)}");
```

**Use Cases:**
- Outlier detection (standard method)
- Box plot creation
- Robust data cleaning
- Quality control limits

**Note:** This method is used internally by `RemoveOutliers()` with the IQR algorithm.

---

## Distribution Shape

### Kurtosis

#### Kurtosis
Measures the "tailedness" of the distribution - characterizes peakedness or flatness compared to a normal distribution.

**Signature:**
```csharp
public static double Kurtosis(this IList<double> values)
```

**Formula:**
```
Kurtosis = (n+1)n * Σ((xi - mean)/s)⁴ / ((n-1)(n-2)(n-3)) - 3(n-1)² / ((n-2)(n-3))
```

**Examples:**
```csharp
// Calculate kurtosis
var values = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var kurt = values.Kurtosis();
Console.WriteLine($"Kurtosis: {kurt:F4}");

// With Table data - convert to double and calculate
var returns = stockData["DailyReturn"]
    .Select(Convert.ToDouble)
    .ToList();
var kurtosis = returns.Kurtosis();

// Interpret kurtosis
if (kurtosis > 0)
    Console.WriteLine("Positive kurtosis: Heavy tails, more outliers than normal");
else if (kurtosis < 0)
    Console.WriteLine("Negative kurtosis: Light tails, fewer outliers than normal");
else
    Console.WriteLine("Kurtosis ≈ 0: Similar to normal distribution");

// Risk analysis example
var portfolioReturns = portfolio["Return"]
    .Select(Convert.ToDouble)
    .ToList();
var portfolioKurtosis = portfolioReturns.Kurtosis();

Console.WriteLine($"Portfolio Kurtosis: {portfolioKurtosis:F4}");
if (portfolioKurtosis > 3)
    Console.WriteLine("⚠️ High risk: More extreme returns than expected");

// Compare distributions
var groupA = dataTable.Filter("Group", "A")
    ["Measurement"].Select(Convert.ToDouble).ToList();
var groupB = dataTable.Filter("Group", "B")
    ["Measurement"].Select(Convert.ToDouble).ToList();

Console.WriteLine($"Group A Kurtosis: {groupA.Kurtosis():F4}");
Console.WriteLine($"Group B Kurtosis: {groupB.Kurtosis():F4}");
```

**Interpretation:**
- **Positive kurtosis (Leptokurtic)**: Peaked distribution with heavy tails
  - More outliers than normal distribution
  - Higher risk in financial contexts
- **Negative kurtosis (Platykurtic)**: Flat distribution with light tails
  - Fewer outliers than normal distribution
  - More uniform distribution
- **Zero kurtosis (Mesokurtic)**: Similar to normal distribution

**Performance:**
- Uses SIMD (Single Instruction Multiple Data) optimization when available
- Hardware-accelerated for large datasets
- Significantly faster than traditional implementations

**Use Cases:**
- Financial risk assessment
- Quality control
- Distribution analysis
- Identifying extreme values
- Comparing data distributions

---

## Percentiles

### Percentile Calculation

#### Percentile
Calculates the value at a specified percentile using the nearest-rank method.

**Signature:**
```csharp
public static decimal Percentile(this List<decimal> values, decimal percentile)
```

**Parameters:**
- `values` - List of decimal values
- `percentile` - Percentile to calculate (0-100)

**Examples:**
```csharp
// Basic percentile calculation
var scores = new List<decimal> { 55, 67, 72, 85, 90, 92, 95, 98 };

Console.WriteLine($"25th percentile: {scores.Percentile(25m)}"); // Q1
Console.WriteLine($"50th percentile: {scores.Percentile(50m)}"); // Median
Console.WriteLine($"75th percentile: {scores.Percentile(75m)}"); // Q3
Console.WriteLine($"90th percentile: {scores.Percentile(90m)}");
Console.WriteLine($"95th percentile: {scores.Percentile(95m)}");

// With Table data - income analysis
var incomes = census["Income"].Select(decimal.Parse).ToList();

Console.WriteLine("Income Distribution:");
Console.WriteLine($"10th percentile: {incomes.Percentile(10m):C}");
Console.WriteLine($"25th percentile: {incomes.Percentile(25m):C}");
Console.WriteLine($"50th percentile (Median): {incomes.Percentile(50m):C}");
Console.WriteLine($"75th percentile: {incomes.Percentile(75m):C}");
Console.WriteLine($"90th percentile: {incomes.Percentile(90m):C}");
Console.WriteLine($"95th percentile: {incomes.Percentile(95m):C}");
Console.WriteLine($"99th percentile: {incomes.Percentile(99m):C}");

// Performance benchmarking
var responseTimes = logs["ResponseTime"].Select(decimal.Parse).ToList();

Console.WriteLine("API Performance (ms):");
Console.WriteLine($"P50 (median): {responseTimes.Percentile(50m)}");
Console.WriteLine($"P90: {responseTimes.Percentile(90m)}");
Console.WriteLine($"P95: {responseTimes.Percentile(95m)}");
Console.WriteLine($"P99: {responseTimes.Percentile(99m)}");

// Salary bands
var salaries = employees["Salary"].Select(decimal.Parse).ToList();

var bands = new Dictionary<string, decimal>
{
    { "Entry Level (0-25%)", salaries.Percentile(25m) },
    { "Mid Level (25-50%)", salaries.Percentile(50m) },
    { "Senior Level (50-75%)", salaries.Percentile(75m) },
    { "Executive Level (75-90%)", salaries.Percentile(90m) },
    { "Top Tier (90%+)", salaries.Percentile(90m) }
};

foreach (var band in bands)
{
    Console.WriteLine($"{band.Key}: up to {band.Value:C}");
}

// Grade distribution
var grades = students["FinalScore"].Select(decimal.Parse).ToList();

var gradeCutoffs = new Dictionary<string, decimal>
{
    { "A (90%+)", grades.Percentile(90m) },
    { "B (75%+)", grades.Percentile(75m) },
    { "C (50%+)", grades.Percentile(50m) },
    { "D (25%+)", grades.Percentile(25m) }
};

Console.WriteLine("Grade Cutoffs:");
foreach (var cutoff in gradeCutoffs)
{
    Console.WriteLine($"{cutoff.Key}: {cutoff.Value:F1}");
}
```

**Features:**
- Uses linear interpolation between values
- Handles edge cases (0th and 100th percentile)
- Automatically sorts data
- Returns exact value when percentile aligns with data point

**Common Percentiles:**
- **P25 (Q1)**: 25th percentile, first quartile
- **P50 (Q2)**: 50th percentile, median
- **P75 (Q3)**: 75th percentile, third quartile
- **P90**: 90th percentile, common in SLA monitoring
- **P95**: 95th percentile, performance benchmarks
- **P99**: 99th percentile, tail latency analysis

**Use Cases:**
- Performance monitoring (P95, P99 latency)
- Salary benchmarking
- Grade distributions
- Income analysis
- Quality control limits
- Risk assessment

---

## Comparative Counts

### Above Average Count

#### AboveAverageCount
Counts how many values are above the average.

**Signature:**
```csharp
public static int AboveAverageCount(this IEnumerable<decimal> values)
```

**Examples:**
```csharp
// Simple count
var scores = new List<decimal> { 50, 60, 70, 80, 90 };
var aboveAvg = scores.AboveAverageCount();
Console.WriteLine($"{aboveAvg} scores above average"); // Returns 2

// With Table data - performance analysis
var sales = salesData["Amount"].Select(decimal.Parse).ToList();
var aboveAvgCount = sales.AboveAverageCount();
var avgSales = sales.Average();

Console.WriteLine($"Average Sales: {avgSales:C}");
Console.WriteLine($"{aboveAvgCount} employees performed above average");
Console.WriteLine($"{sales.Count() - aboveAvgCount} employees below average");

// Percentage above average
var percentAbove = (decimal)sales.AboveAverageCount() / sales.Count() * 100;
Console.WriteLine($"{percentAbove:F1}% of sales were above average");

// Regional comparison
var regions = salesTable.SplitOn("Region");
foreach (var region in regions)
{
    var regionSales = region.Value["Sales"].Select(decimal.Parse).ToList();
    var regionAboveAvg = regionSales.AboveAverageCount();
    var regionAvg = regionSales.Average();
    
    Console.WriteLine($"{region.Key}:");
    Console.WriteLine($"  Average: {regionAvg:C}");
    Console.WriteLine($"  Above Average: {regionAboveAvg}/{regionSales.Count()}");
}

// Student performance
var testScores = students["Score"].Select(decimal.Parse).ToList();
var aboveAvgStudents = testScores.AboveAverageCount();
var avgScore = testScores.Average();

Console.WriteLine($"Class Average: {avgScore:F1}");
Console.WriteLine($"{aboveAvgStudents} students scored above average");
Console.WriteLine($"That's {(decimal)aboveAvgStudents / students.RowCount * 100:F1}% of the class");
```

**Use Cases:**
- Performance rankings
- Student grading
- Employee evaluations
- Sales analysis
- Quality metrics

---

### Below Average Count

#### BelowAverageCount
Counts how many values are below the average.

**Signature:**
```csharp
public static int BelowAverageCount(this IEnumerable<decimal> values)
```

**Examples:**
```csharp
// Simple count
var prices = new List<decimal> { 10, 20, 30, 40, 50 };
var belowAvg = prices.BelowAverageCount();
Console.WriteLine($"{belowAvg} prices below average"); // Returns 2

// With Table data - identify underperformers
var sales = salesReps["Sales"].Select(decimal.Parse).ToList();
var belowAvgCount = sales.BelowAverageCount();
var avgSales = sales.Average();

Console.WriteLine($"Average Sales: {avgSales:C}");
Console.WriteLine($"{belowAvgCount} reps need improvement");

// Performance distribution
var scores = employees["PerformanceScore"].Select(decimal.Parse).ToList();
var avg = scores.Average();
var above = scores.AboveAverageCount();
var below = scores.BelowAverageCount();
var at = scores.Count() - above - below;

Console.WriteLine("Performance Distribution:");
Console.WriteLine($"Above Average: {above} ({(decimal)above/scores.Count()*100:F1}%)");
Console.WriteLine($"At Average: {at} ({(decimal)at/scores.Count()*100:F1}%)");
Console.WriteLine($"Below Average: {below} ({(decimal)below/scores.Count()*100:F1}%)");

// Quality control
var measurements = production["Measurement"].Select(decimal.Parse).ToList();
var target = measurements.Average();
var belowTarget = measurements.BelowAverageCount();

if ((decimal)belowTarget / measurements.Count() > 0.5m)
{
    Console.WriteLine("⚠️ WARNING: More than 50% below average quality");
}
```

**Use Cases:**
- Identifying underperformers
- Quality control
- Training needs assessment
- Resource allocation
- Performance improvement tracking

---

### Average Count

#### AverageCount
Counts values that are at or above the average (>= average).

**Signature:**
```csharp
public static int AverageCount(this IEnumerable<decimal> values)
```

**Example:**
```csharp
// Count at or above average
var values = new List<decimal> { 10, 20, 30, 40, 50 };
var atOrAbove = values.AverageCount();
Console.WriteLine($"{atOrAbove} values at or above average"); // Returns 3

// With Table data
var scores = tests["Score"].Select(decimal.Parse).ToList();
var avgOrAbove = scores.AverageCount();
var avg = scores.Average();

Console.WriteLine($"Average Score: {avg:F1}");
Console.WriteLine($"{avgOrAbove}/{scores.Count()} met or exceeded average");
```

**Note:** This includes values equal to the average, unlike `AboveAverageCount()`.

---

## Usage with Tables

All statistical functions work seamlessly with Table data using the indexer `table["ColumnName"]` or `ValuesOf("ColumnName")`:

```csharp
// Load data
var sales = DataAcquisition.LoadCsv("sales.csv");

// Extract numeric column using indexer (recommended)
var amounts = sales["Amount"].Select(decimal.Parse).ToList();

// Alternative: using ValuesOf method
var amountsAlt = sales.ValuesOf("Amount").Select(decimal.Parse).ToList();

// Calculate statistics
var median = BasicStatistics.Median(amounts);
var range = amounts.Range();
var variance = amounts.Variance();
var mad = amounts.MedianAbsoluteDeviation();
var p95 = amounts.Percentile(95m);
var aboveAvg = amounts.AboveAverageCount();

// Display results
Console.WriteLine($"Median: {median:C}");
Console.WriteLine($"Range: {range:C}");
Console.WriteLine($"Variance: {variance:F2}");
Console.WriteLine($"MAD: {mad:C}");
Console.WriteLine($"95th Percentile: {p95:C}");
Console.WriteLine($"Above Average: {aboveAvg}/{amounts.Count()}");
```

---

## Complete Examples

### Comprehensive Statistical Analysis

```csharp
// Load sales data
var sales = DataAcquisition.LoadCsv("sales.csv");
var amounts = sales["Amount"].Select(decimal.Parse).ToList();

Console.WriteLine("=== SALES ANALYSIS ===\n");

// Central tendency
var median = BasicStatistics.Median(amounts);
var average = amounts.Average();
Console.WriteLine("Central Tendency:");
Console.WriteLine($"  Mean: {average:C}");
Console.WriteLine($"  Median: {median:C}");

// Dispersion
Console.WriteLine("\nDispersion:");
Console.WriteLine($"  Range: {amounts.Range():C}");
Console.WriteLine($"  Variance: {amounts.Variance():F2}");
Console.WriteLine($"  MAD: {amounts.MedianAbsoluteDeviation():C}");

// Percentiles
Console.WriteLine("\nPercentiles:");
Console.WriteLine($"  P25 (Q1): {amounts.Percentile(25m):C}");
Console.WriteLine($"  P50 (Median): {amounts.Percentile(50m):C}");
Console.WriteLine($"  P75 (Q3): {amounts.Percentile(75m):C}");
Console.WriteLine($"  P90: {amounts.Percentile(90m):C}");
Console.WriteLine($"  P95: {amounts.Percentile(95m):C}");

// Outlier detection
var iqrBounds = BasicStatistics.IqrRange(amounts);
var outlierCount = amounts.Count(a => 
    a < iqrBounds.Item1 || a > iqrBounds.Item2);
Console.WriteLine("\nOutlier Analysis:");
Console.WriteLine($"  Lower Bound: {iqrBounds.Item1:C}");
Console.WriteLine($"  Upper Bound: {iqrBounds.Item2:C}");
Console.WriteLine($"  Outliers: {outlierCount}");

// Performance distribution
Console.WriteLine("\nPerformance Distribution:");
Console.WriteLine($"  Above Average: {amounts.AboveAverageCount()}");
Console.WriteLine($"  Below Average: {amounts.BelowAverageCount()}");
```

### Regional Comparison

```csharp
// Compare statistics across regions
var salesData = DataAcquisition.LoadCsv("regional_sales.csv");
var regions = salesData.SplitOn("Region");

Console.WriteLine("=== REGIONAL COMPARISON ===\n");

foreach (var region in regions.OrderBy(r => r.Key))
{
    var sales = region.Value["Sales"].Select(decimal.Parse).ToList();
    
    Console.WriteLine($"{region.Key}:");
    Console.WriteLine($"  Count: {sales.Count()}");
    Console.WriteLine($"  Median: {BasicStatistics.Median(sales):C}");
    Console.WriteLine($"  Range: {sales.Range():C}");
    Console.WriteLine($"  Variance: {sales.Variance():F2}");
    Console.WriteLine($"  P90: {sales.Percentile(90m):C}");
    Console.WriteLine($"  Above Avg: {sales.AboveAverageCount()}/{sales.Count()}");
    Console.WriteLine();
}
```

### Financial Risk Analysis

```csharp
// Analyze investment returns
var portfolio = DataAcquisition.LoadCsv("returns.csv");
var returns = portfolio["DailyReturn"].Select(Convert.ToDouble).ToList();

Console.WriteLine("=== RISK ANALYSIS ===\n");

// Calculate kurtosis for tail risk
var kurtosis = returns.Kurtosis();
Console.WriteLine($"Kurtosis: {kurtosis:F4}");

if (kurtosis > 3)
    Console.WriteLine("⚠️ Fat tails detected - higher risk of extreme events");
else if (kurtosis < -1)
    Console.WriteLine("✓ Thin tails - more predictable distribution");

// Value at Risk (VaR) using percentiles
var returnsDecimal = portfolio["DailyReturn"].Select(decimal.Parse).ToList();
var var95 = returnsDecimal.Percentile(5m); // 5th percentile for losses
var var99 = returnsDecimal.Percentile(1m); // 1st percentile for extreme losses

Console.WriteLine($"\nValue at Risk:");
Console.WriteLine($"  95% VaR: {var95:P2}");
Console.WriteLine($"  99% VaR: {var99:P2}");

// Consistency measure
var mad = returnsDecimal.MedianAbsoluteDeviation();
Console.WriteLine($"\nMAD: {mad:P4} (lower is more stable)");
```

### Student Performance Analysis

```csharp
// Analyze test scores
var students = DataAcquisition.LoadCsv("test_scores.csv");
var scores = students["Score"].Select(decimal.Parse).ToList();

Console.WriteLine("=== CLASS PERFORMANCE ===\n");

// Basic stats
var median = BasicStatistics.Median(scores);
var average = scores.Average();
Console.WriteLine($"Class Average: {average:F1}");
Console.WriteLine($"Class Median: {median:F1}");

// Grade distribution using percentiles
Console.WriteLine("\nGrade Boundaries:");
Console.WriteLine($"  A (90%+): {scores.Percentile(90m):F1}+");
Console.WriteLine($"  B (75%+): {scores.Percentile(75m):F1}+");
Console.WriteLine($"  C (50%+): {scores.Percentile(50m):F1}+");
Console.WriteLine($"  D (25%+): {scores.Percentile(25m):F1}+");

// Performance distribution
var above = scores.AboveAverageCount();
var below = scores.BelowAverageCount();
Console.WriteLine("\nStudent Distribution:");
Console.WriteLine($"  Above Average: {above} ({(decimal)above/scores.Count()*100:F1}%)");
Console.WriteLine($"  Below Average: {below} ({(decimal)below/scores.Count()*100:F1}%)");

// Identify struggling students
var strugglingThreshold = scores.Percentile(25m);
var strugglingStudents = students.GetRowsWhere(row =>
    decimal.Parse(row["Score"]) < strugglingThreshold);
Console.WriteLine($"\n{strugglingStudents.RowCount} students need additional support");
```

---

## Summary

Squirrel's BasicStatistics module provides:

✅ **Robust Measures**: MAD and IQR for outlier-resistant analysis  
✅ **Distribution Analysis**: Kurtosis for understanding data shape  
✅ **Percentile Calculation**: Comprehensive percentile support  
✅ **Performance Optimized**: SIMD acceleration for large datasets  
✅ **Table Integration**: Seamless work with Table columns  
✅ **Practical Functions**: Above/below average counts  
✅ **Outlier Detection**: IQR range calculation built-in  

These functions form the foundation for data analysis, quality control, risk assessment, and performance monitoring in Squirrel.

For data cleaning operations, see [Data Cleansing](Data_Cleansing.md).  
For data loading, see [Data Acquisition](Data_Acquisition.md).

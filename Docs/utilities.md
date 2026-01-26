# Display & Utility Functions

A comprehensive guide to Squirrel's utility functions that make data exploration, debugging, and presentation effortless.

## Table of Contents
- [PrettyDump - The Game Changer](#prettydump---the-game-changer)
- [Gist - Statistical Summary](#gist---statistical-summary)
- [Data Inspection Methods](#data-inspection-methods)
- [Column and Row Utilities](#column-and-row-utilities)
- [Export Utilities](#export-utilities)
- [Comparison: Console Output Methods](#comparison-console-output-methods)

---

## PrettyDump - The Game Changer

### Overview

`PrettyDump()` is Squirrel's most beloved utility function - it transforms raw data into beautifully formatted, color-coded console tables with a single method call. Think of it as "pretty print" on steroids.

### Why PrettyDump is Revolutionary

**The Problem It Solves:**
```csharp
// ❌ WITHOUT PrettyDump - Manual formatting nightmare
Console.WriteLine("=== Top Products ===");
Console.WriteLine("Product          | Sales    | Growth");
Console.WriteLine("-----------------|----------|--------");
foreach (var row in topProducts.Rows)
{
    Console.WriteLine($"{row["Product"],-16} | {row["Sales"],8} | {row["Growth"],6}");
}
Console.WriteLine();
// 😫 Tedious, error-prone, hard to maintain
```

```csharp
// ✅ WITH PrettyDump - One elegant line
topProducts.PrettyDump(header: "Top Products", 
                      rowColor: ConsoleColor.DarkGreen);
// 😍 Beautiful, automatic, perfect
```

### Signature

```csharp
public Table PrettyDump(
    string header = null,
    ConsoleColor rowColor = ConsoleColor.Gray,
    Alignment? alignment = null)
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `header` | string | null | Optional title displayed above the table |
| `rowColor` | ConsoleColor | Gray | Color for table rows (16 console colors available) |
| `alignment` | Alignment? | null | Column alignment: Left, Right, Center, or null for auto |

### Available Console Colors

| Color | Best Used For |
|-------|---------------|
| **DarkGreen** | ✅ Positive results, opportunities, good values, "go" indicators |
| **DarkRed** | ⚠️ Warnings, errors, depreciation, alerts, "stop" indicators |
| **DarkCyan** | 📊 Neutral data, location info, general information |
| **DarkMagenta** | 💎 Premium items, luxury segments, high-value |
| **DarkYellow** | ⚡ Categories, budgets, caution areas |
| **DarkBlue** | 📈 Metrics, analytics, calculated data |
| **Green** | 🎉 Success messages, confirmations |
| **Red** | 🚨 Critical errors, urgent items |
| **Cyan** | 💬 Information, neutral highlights |
| **Yellow** | ⚠️ Warnings, attention needed |
| **Blue** | 🔍 Details, secondary information |
| **Magenta** | 🎨 Creative, special items |
| **White** | 📄 Default, clean presentation |
| **Gray** | 📋 Standard output |

### Real-World Examples

#### Example 1: Sales Analysis

```csharp
using Squirrel;
using System;

var sales = DataAcquisition.LoadCsv("sales.csv");

// Top performers - show in green (positive)
var topSales = sales
    .SortBy("Amount", how: SortDirection.Descending)
    .Top(10)
    .Pick("SalesRep", "Region", "Amount", "Quarter");

topSales.PrettyDump(header: "🏆 Top 10 Sales Representatives", 
                    rowColor: ConsoleColor.DarkGreen);
Console.ResetColor();

// Underperformers - show in red (attention needed)
var bottomSales = sales
    .SortBy("Amount")
    .Top(10);

bottomSales.PrettyDump(header: "⚠️ Need Coaching & Support",
                      rowColor: ConsoleColor.DarkRed);
Console.ResetColor();

// Regional summary - neutral cyan
var byRegion = sales
    .Aggregate("Region", AggregationMethod.Sum)
    .SortBy("Amount", how: SortDirection.Descending);

byRegion.PrettyDump(header: "📊 Sales by Region",
                   rowColor: ConsoleColor.DarkCyan);
Console.ResetColor();
```

**Output:**
```
╔════════════════════════════════════════════╗
║   🏆 Top 10 Sales Representatives          ║
╠═══════════════╦═════════╦══════════╦═══════╣
║ SalesRep      ║ Region  ║ Amount   ║Quarter║
╠═══════════════╬═════════╬══════════╬═══════╣
║ Alice Johnson ║ West    ║ 234567.8 ║  Q4   ║  [in DarkGreen]
║ Bob Smith     ║ East    ║ 198765.4 ║  Q4   ║
║ ...           ║ ...     ║ ...      ║  ...  ║
╚═══════════════╩═════════╩══════════╩═══════╝
```

#### Example 2: Data Exploration Workflow

```csharp
using Squirrel;

// 1. Quick peek at data structure
var customers = DataAcquisition.LoadCsv("customers.csv");

Console.WriteLine("=== Data Exploration ===\n");

// Show first 5 rows to understand structure
customers.Top(5).PrettyDump(header: "📋 Sample Records");
Console.ResetColor();

// Show column summary
Console.WriteLine($"\n📊 Dataset: {customers.RowCount} rows, " +
                 $"{customers.ColumnHeaders.Count} columns");
Console.WriteLine($"Columns: {string.Join(", ", customers.ColumnHeaders)}");

// 2. Quick stats on key metrics
var highValue = customers
    .Where("LifetimeValue", ">", 10000m)
    .SortBy("LifetimeValue", how: SortDirection.Descending);

highValue.Top(10).PrettyDump(
    header: "💎 Top 10 High-Value Customers",
    rowColor: ConsoleColor.DarkMagenta);
Console.ResetColor();

// 3. Distribution analysis
var segments = customers
    .Aggregate("Segment", AggregationMethod.Count)
    .SortBy("Count", how: SortDirection.Descending);

segments.PrettyDump(
    header: "📊 Customer Segments",
    rowColor: ConsoleColor.DarkYellow);
Console.ResetColor();
```

#### Example 3: Pipeline Debugging

```csharp
using Squirrel;

var raw = DataAcquisition.LoadCsv("messy_data.csv");

Console.WriteLine("=== Data Cleaning Pipeline ===\n");
Console.WriteLine($"1. Starting with: {raw.RowCount} rows");

// Show sample of raw data
raw.Top(3).PrettyDump(header: "🔍 Raw Data Sample");
Console.ResetColor();

// Clean outliers
var step1 = raw.RemoveOutliers("amount");
Console.WriteLine($"\n2. After removing outliers: {step1.RowCount} rows");
step1.Top(3).PrettyDump(header: "After Outlier Removal");
Console.ResetColor();

// Remove incomplete rows
var step2 = step1.RemoveIncompleteRows();
Console.WriteLine($"\n3. After removing incomplete: {step2.RowCount} rows");
step2.Top(3).PrettyDump(header: "After Quality Check");
Console.ResetColor();

// Final result
var final = step2
    .SortBy("amount", how: SortDirection.Descending)
    .Top(10);

Console.WriteLine($"\n4. Final top 10:");
final.PrettyDump(
    header: "✅ Clean Data - Top 10",
    rowColor: ConsoleColor.DarkGreen);
Console.ResetColor();
```

### Why PrettyDump is So Useful

#### 1. **Zero Configuration**
No need to calculate column widths, add padding, or draw borders. It just works.

#### 2. **Instant Visual Feedback**
During development, immediately see if your data looks right:
```csharp
// Quick sanity check during development
myData
    .RemoveOutliers("value")
    .Top(10)
    .PrettyDump();  // ← See results instantly
```

#### 3. **Interactive Development**
Perfect for REPL-style development and exploration:
```csharp
// Try different filters and see results immediately
data.Filter("status", "Active").PrettyDump();
data.Filter("amount", ">", 1000).PrettyDump();
data.Filter("date", ">", DateTime.Now.AddDays(-30)).PrettyDump();
```

#### 4. **Debugging Complex Pipelines**
Add PrettyDump at any stage to inspect intermediate results:
```csharp
var result = data
    .RemoveOutliers("amount")
    .PrettyDump(header: "After outlier removal")  // ← Debug point 1
    .Filter("region", "West")
    .PrettyDump(header: "After region filter")    // ← Debug point 2
    .Aggregate("product")
    .PrettyDump(header: "Final aggregation");     // ← Debug point 3
```

#### 5. **Professional Console Applications**
Create polished CLI tools without complex UI frameworks:
```csharp
Console.WriteLine("╔════════════════════════════════╗");
Console.WriteLine("║   QUARTERLY SALES REPORT       ║");
Console.WriteLine("╚════════════════════════════════╝\n");

q4Data.PrettyDump(header: "Q4 2024 Performance",
                  rowColor: ConsoleColor.DarkCyan);
```

#### 6. **Teaching and Presentations**
Clear output for demonstrations and teaching:
```csharp
// Live coding demo
Console.WriteLine("Let's analyze customer segments...\n");
customers
    .SplitOn("Segment")
    .First()
    .Value
    .Top(5)
    .PrettyDump(header: "Sample from Premium Segment");
```

### Best Practices

#### ✅ DO: Always Reset Color
```csharp
table.PrettyDump(rowColor: ConsoleColor.DarkGreen);
Console.ResetColor();  // ← Important! Prevents color bleeding
```

#### ✅ DO: Use Descriptive Headers
```csharp
// Good: Clear, informative headers
results.PrettyDump(header: "Top 10 Customers by Revenue");

// Better: Add context and emoji
results.PrettyDump(header: "💰 Top 10 Customers by Revenue (2024)");
```

#### ✅ DO: Chain with Pick() for Focused Output
```csharp
// Only show relevant columns
largeTable
    .Pick("CustomerName", "Revenue", "Region")
    .PrettyDump(header: "Key Metrics");
```

#### ✅ DO: Use Semantic Colors
```csharp
// Green for positive/success
profitable.PrettyDump(rowColor: ConsoleColor.DarkGreen);

// Red for warnings/issues
losses.PrettyDump(rowColor: ConsoleColor.DarkRed);

// Cyan for neutral info
summary.PrettyDump(rowColor: ConsoleColor.DarkCyan);
```

#### ❌ DON'T: Forget to Reset Color
```csharp
// Bad: Color will bleed into subsequent output
table.PrettyDump(rowColor: ConsoleColor.Red);
Console.WriteLine("This text will be red too!");  // ← Problem

// Good:
table.PrettyDump(rowColor: ConsoleColor.Red);
Console.ResetColor();
Console.WriteLine("This text is normal");  // ← Correct
```

#### ❌ DON'T: Dump Huge Tables
```csharp
// Bad: 10,000 rows to console
hugeTable.PrettyDump();  // ← Will scroll forever

// Good: Show relevant subset
hugeTable.Top(20).PrettyDump(header: "First 20 rows");
hugeTable.Bottom(20).PrettyDump(header: "Last 20 rows");
```

---

## Gist - Statistical Summary

### Overview

`Gist()` generates comprehensive statistical summaries for numeric columns in your table. Think of it as "describe()" in pandas or "summary()" in R.

### Signature

```csharp
public Dictionary<string, Dictionary<string, decimal>> Gist()
```

### Returns

A nested dictionary where:
- **Outer key:** Column name
- **Inner dictionary:** Statistical measures
  - `"count"` - Number of non-null values
  - `"mean"` - Average value
  - `"std"` - Standard deviation
  - `"min"` - Minimum value
  - `"25%"` - 25th percentile (Q1)
  - `"50%"` - 50th percentile (median)
  - `"75%"` - 75th percentile (Q3)
  - `"max"` - Maximum value

### Examples

#### Example 1: Quick Data Overview

```csharp
using Squirrel;
using System;

var sales = DataAcquisition.LoadCsv("sales.csv");

// Get statistical summary
var stats = sales.Gist();

Console.WriteLine("=== SALES DATA SUMMARY ===\n");

foreach (var column in stats)
{
    Console.WriteLine($"📊 {column.Key}");
    Console.WriteLine("─────────────────────");
    foreach (var stat in column.Value)
    {
        Console.WriteLine($"  {stat.Key,-8}: {stat.Value:N2}");
    }
    Console.WriteLine();
}
```

**Output:**
```
=== SALES DATA SUMMARY ===

📊 Amount
─────────────────────
  count   : 1,234.00
  mean    : 5,678.90
  std     : 2,345.67
  min     : 100.00
  25%     : 3,456.78
  50%     : 5,234.56
  75%     : 7,890.12
  max     : 25,000.00

📊 Quantity
─────────────────────
  count   : 1,234.00
  mean    : 12.50
  ...
```

#### Example 2: Quality Checks

```csharp
using Squirrel;

var data = DataAcquisition.LoadCsv("product_data.csv");
var stats = data.Gist();

// Check for outliers
foreach (var column in stats)
{
    var mean = column.Value["mean"];
    var std = column.Value["std"];
    var max = column.Value["max"];
    var min = column.Value["min"];
    
    // Flag columns with extreme ranges
    var range = max - min;
    var coefficient = std / mean;  // Coefficient of variation
    
    if (coefficient > 1.0m)
    {
        Console.WriteLine($"⚠️ {column.Key} has high variability " +
                         $"(CV: {coefficient:P})");
    }
    
    // Check for potential outliers using 3-sigma rule
    var lowerBound = mean - (3 * std);
    var upperBound = mean + (3 * std);
    
    if (min < lowerBound || max > upperBound)
    {
        Console.WriteLine($"🔍 {column.Key} may have outliers");
        Console.WriteLine($"   Expected range: [{lowerBound:N2}, {upperBound:N2}]");
        Console.WriteLine($"   Actual range: [{min:N2}, {max:N2}]");
    }
}
```

#### Example 3: Before/After Comparison

```csharp
using Squirrel;

var raw = DataAcquisition.LoadCsv("raw_data.csv");
var cleaned = raw.RemoveOutliers("amount");

Console.WriteLine("=== DATA CLEANING IMPACT ===\n");

var rawStats = raw.Gist();
var cleanStats = cleaned.Gist();

Console.WriteLine("BEFORE Cleaning:");
Console.WriteLine($"  Count: {rawStats["amount"]["count"]}");
Console.WriteLine($"  Mean:  {rawStats["amount"]["mean"]:C}");
Console.WriteLine($"  Std:   {rawStats["amount"]["std"]:C}");
Console.WriteLine($"  Range: {rawStats["amount"]["min"]:C} to " +
                 $"{rawStats["amount"]["max"]:C}");

Console.WriteLine("\nAFTER Cleaning:");
Console.WriteLine($"  Count: {cleanStats["amount"]["count"]}");
Console.WriteLine($"  Mean:  {cleanStats["amount"]["mean"]:C}");
Console.WriteLine($"  Std:   {cleanStats["amount"]["std"]:C}");
Console.WriteLine($"  Range: {cleanStats["amount"]["min"]:C} to " +
                 $"{cleanStats["amount"]["max"]:C}");

var improvement = (rawStats["amount"]["std"] - cleanStats["amount"]["std"]) 
                 / rawStats["amount"]["std"] * 100;
Console.WriteLine($"\n✅ Reduced std deviation by {improvement:F1}%");
```

#### Example 4: Display Gist as Table

```csharp
using Squirrel;

var data = DataAcquisition.LoadCsv("metrics.csv");
var gist = data.Gist();

// Convert Gist to Table for display
var gistTable = new Table();
gistTable.AddColumn("Statistic", new[] { 
    "Count", "Mean", "Std Dev", "Min", "Q1", "Median", "Q3", "Max" 
}.ToList());

foreach (var column in gist)
{
    gistTable.AddColumn(column.Key, new[] {
        column.Value["count"].ToString("N0"),
        column.Value["mean"].ToString("N2"),
        column.Value["std"].ToString("N2"),
        column.Value["min"].ToString("N2"),
        column.Value["25%"].ToString("N2"),
        column.Value["50%"].ToString("N2"),
        column.Value["75%"].ToString("N2"),
        column.Value["max"].ToString("N2")
    }.ToList());
}

gistTable.PrettyDump(header: "📊 Statistical Summary");
Console.ResetColor();
```

---

## Data Inspection Methods

### Head() - First N Rows

Returns the first N rows of the table.

```csharp
public Table Head(int n = 10)
```

**Example:**
```csharp
var customers = DataAcquisition.LoadCsv("customers.csv");

// Quick peek at data structure
customers.Head(5).PrettyDump(header: "First 5 Customers");
```

### Tail() - Last N Rows

Returns the last N rows of the table.

```csharp
public Table Tail(int n = 10)
```

**Example:**
```csharp
var logs = DataAcquisition.LoadCsv("system_logs.csv");

// See most recent logs
logs.Tail(10).PrettyDump(header: "Latest 10 Log Entries");
```

### Top() - First N Rows After Sorting

Get first N rows (alias for Head, useful in pipeline context).

```csharp
public Table Top(int n)
```

**Example:**
```csharp
// Get top 10 by revenue
sales
    .SortBy("Revenue", how: SortDirection.Descending)
    .Top(10)
    .PrettyDump(header: "Top 10 by Revenue");
```

### Bottom() - Last N Rows After Sorting

Get last N rows (alias for Tail, useful in pipeline context).

```csharp
public Table Bottom(int n)
```

**Example:**
```csharp
// Get bottom 10 performers
sales
    .SortBy("Revenue", how: SortDirection.Descending)
    .Bottom(10)
    .PrettyDump(header: "Bottom 10 Performers");
```

### Pick() - Select Specific Columns

Select only specific columns to display.

```csharp
public Table Pick(params string[] columns)
```

**Example:**
```csharp
// Show only relevant columns
customers
    .Pick("Name", "Email", "TotalPurchases", "LastActive")
    .Head(10)
    .PrettyDump(header: "Customer Contact List");
```

### Every() - Sample Every Nth Row

Get every Nth row for large dataset sampling.

```csharp
public Table Every(int n)
```

**Example:**
```csharp
// Sample 1 in every 100 rows from large dataset
hugeData
    .Every(100)
    .PrettyDump(header: "1% Sample of Data");
```

---

## Column and Row Utilities

### ValuesOf() - Get Column Values

Extract all values from a specific column.

```csharp
public List<string> ValuesOf(string columnName)
// OR use indexer
public List<string> this[string columnName]
```

**Example:**
```csharp
var sales = DataAcquisition.LoadCsv("sales.csv");

// Get all amounts
var amounts = sales["Amount"].Select(decimal.Parse).ToList();

// Calculate statistics
var total = amounts.Sum();
var average = amounts.Average();
var median = BasicStatistics.Median(amounts);

Console.WriteLine($"Total Sales: {total:C}");
Console.WriteLine($"Average: {average:C}");
Console.WriteLine($"Median: {median:C}");
```

### RowCount Property

Get number of rows in the table.

```csharp
public int RowCount { get; }
```

**Example:**
```csharp
var data = DataAcquisition.LoadCsv("data.csv");
Console.WriteLine($"Loaded {data.RowCount:N0} records");

var cleaned = data.RemoveOutliers("value");
var removed = data.RowCount - cleaned.RowCount;
Console.WriteLine($"Removed {removed} outliers ({removed * 100.0 / data.RowCount:F1}%)");
```

### ColumnHeaders Property

Get all column names.

```csharp
public HashSet<string> ColumnHeaders { get; }
```

**Example:**
```csharp
var data = DataAcquisition.LoadCsv("data.csv");

Console.WriteLine("Available Columns:");
foreach (var col in data.ColumnHeaders.OrderBy(c => c))
{
    Console.WriteLine($"  - {col}");
}
```

### Histogram() - Frequency Distribution

Count occurrences of each unique value in a column.

```csharp
public Dictionary<string, int> Histogram(string columnName)
```

**Example:**
```csharp
var orders = DataAcquisition.LoadCsv("orders.csv");

// Distribution of order status
var statusDist = orders.Histogram("Status");

Console.WriteLine("📊 Order Status Distribution:");
foreach (var status in statusDist.OrderByDescending(s => s.Value))
{
    var pct = status.Value * 100.0 / orders.RowCount;
    var bar = new string('█', (int)(pct / 2));
    Console.WriteLine($"{status.Key,-15} {bar} {status.Value} ({pct:F1}%)");
}
```

**Output:**
```
📊 Order Status Distribution:
Completed       ████████████████ 487 (48.7%)
Pending         ████████ 234 (23.4%)
Shipped         ██████ 156 (15.6%)
Cancelled       ███ 123 (12.3%)
```

---

## Export Utilities

### ToCsv() - Export to CSV

Export table to CSV format string.

```csharp
public string ToCsv()
```

**Example:**
```csharp
var report = sales
    .Aggregate("Region", AggregationMethod.Sum)
    .SortBy("Amount", how: SortDirection.Descending);

// Save to file
File.WriteAllText("regional_sales.csv", report.ToCsv());

// Or display
Console.WriteLine(report.ToCsv());
```

### ToHtmlTable() - Export to HTML

Export table to HTML table format.

```csharp
public string ToHtmlTable()
```

**Example:**
```csharp
var topProducts = products
    .SortBy("Sales", how: SortDirection.Descending)
    .Top(10);

// Create HTML report
var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Top Products Report</title>
    <style>
        table {{ border-collapse: collapse; width: 100%; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        th {{ background-color: #4CAF50; color: white; }}
    </style>
</head>
<body>
    <h1>Top 10 Products</h1>
    {topProducts.ToHtmlTable()}
</body>
</html>";

File.WriteAllText("top_products.html", html);
```

### ToTsv() - Export to TSV

Export table to tab-separated values format.

```csharp
public string ToTsv()
```

**Example:**
```csharp
var data = sales.Aggregate("Product", AggregationMethod.Sum);
File.WriteAllText("product_summary.tsv", data.ToTsv());
```

---

## Comparison: Console Output Methods

| Method | Use Case | Output Quality | Setup Required |
|--------|----------|----------------|----------------|
| **WriteLine** | Simple messages | Plain text | None |
| **String.Format** | Custom formatting | Moderate | Manual alignment |
| **PrettyDump** | Table display | Excellent | None |
| **ToCsv + Display** | Data export | Raw format | None |
| **Custom formatting** | Special needs | Varies | Lots of code |

### Evolution of Console Output

```csharp
// ═══════════════════════════════════════════
// LEVEL 1: Basic WriteLine (Ugly)
// ═══════════════════════════════════════════
Console.WriteLine("Top Sales:");
foreach (var row in topSales.Rows)
{
    Console.WriteLine(row["Name"] + " - " + row["Amount"]);
}
// Output:
// Top Sales:
// Alice - 5000
// Bob - 4500

// ═══════════════════════════════════════════
// LEVEL 2: String Formatting (Better)
// ═══════════════════════════════════════════
Console.WriteLine("Top Sales:");
Console.WriteLine($"{"Name",-20} {"Amount",10}");
Console.WriteLine(new string('-', 30));
foreach (var row in topSales.Rows)
{
    Console.WriteLine($"{row["Name"],-20} {decimal.Parse(row["Amount"]),10:C}");
}
// Output:
// Top Sales:
// Name                    Amount
// ------------------------------
// Alice                $5,000.00
// Bob                  $4,500.00

// ═══════════════════════════════════════════
// LEVEL 3: PrettyDump (Perfect!)
// ═══════════════════════════════════════════
topSales
    .Pick("Name", "Amount")
    .PrettyDump(header: "Top Sales", rowColor: ConsoleColor.DarkGreen);
Console.ResetColor();
// Output:
// ╔════════════════════════════════╗
// ║          Top Sales             ║
// ╠════════════════════╦═══════════╣
// ║ Name               ║ Amount    ║
// ╠════════════════════╬═══════════╣
// ║ Alice              ║ $5,000.00 ║  [in green]
// ║ Bob                ║ $4,500.00 ║
// ╚════════════════════╩═══════════╝
```

---

## Complete Example: Data Exploration Session

Here's a real-world example showing how these utilities work together:

```csharp
using Squirrel;
using System;
using System.Linq;
using System.IO;

namespace DataExploration
{
    class Program
    {
        static void Main()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║    INTERACTIVE DATA EXPLORATION        ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            
            // ═══════════════════════════════════════
            // 1. LOAD & INSPECT
            // ═══════════════════════════════════════
            var sales = DataAcquisition.LoadCsv("sales.csv");
            
            Console.WriteLine($"📁 Loaded: {sales.RowCount:N0} records");
            Console.WriteLine($"📊 Columns: {sales.ColumnHeaders.Count}");
            Console.WriteLine($"   {string.Join(", ", sales.ColumnHeaders)}\n");
            
            // Quick peek at structure
            sales.Head(3).PrettyDump(header: "First 3 Records");
            Console.ResetColor();
            
            // ═══════════════════════════════════════
            // 2. STATISTICAL OVERVIEW
            // ═══════════════════════════════════════
            Console.WriteLine("\n📈 STATISTICAL SUMMARY\n");
            
            var stats = sales.Gist();
            foreach (var col in stats.Take(2))  // Show first 2 numeric columns
            {
                Console.WriteLine($"📊 {col.Key}:");
                Console.WriteLine($"   Mean:   {col.Value["mean"]:N2}");
                Console.WriteLine($"   Median: {col.Value["50%"]:N2}");
                Console.WriteLine($"   Range:  {col.Value["min"]:N2} to {col.Value["max"]:N2}");
                Console.WriteLine();
            }
            
            // ═══════════════════════════════════════
            // 3. CLEAN DATA
            // ═══════════════════════════════════════
            Console.WriteLine("🧹 CLEANING DATA\n");
            
            var cleaned = sales
                .RemoveOutliers("Amount")
                .RemoveIncompleteRows();
            
            var removed = sales.RowCount - cleaned.RowCount;
            Console.WriteLine($"Removed: {removed} rows ({removed * 100.0 / sales.RowCount:F1}%)");
            Console.WriteLine($"Clean data: {cleaned.RowCount:N0} rows\n");
            
            // ═══════════════════════════════════════
            // 4. TOP PERFORMERS
            // ═══════════════════════════════════════
            Console.WriteLine("🏆 TOP PERFORMERS\n");
            
            var topSales = cleaned
                .SortBy("Amount", how: SortDirection.Descending)
                .Top(10)
                .Pick("SalesRep", "Region", "Amount", "Date");
            
            topSales.PrettyDump(
                header: "Top 10 Sales",
                rowColor: ConsoleColor.DarkGreen);
            Console.ResetColor();
            
            // ═══════════════════════════════════════
            // 5. REGIONAL ANALYSIS
            // ═══════════════════════════════════════
            Console.WriteLine("\n🌎 REGIONAL PERFORMANCE\n");
            
            var byRegion = cleaned
                .Aggregate("Region", AggregationMethod.Sum)
                .SortBy("Amount", how: SortDirection.Descending);
            
            byRegion.PrettyDump(
                header: "Sales by Region",
                rowColor: ConsoleColor.DarkCyan);
            Console.ResetColor();
            
            // ═══════════════════════════════════════
            // 6. DISTRIBUTION ANALYSIS
            // ═══════════════════════════════════════
            Console.WriteLine("\n📊 PRODUCT DISTRIBUTION\n");
            
            var productDist = cleaned.Histogram("Product");
            foreach (var product in productDist
                .OrderByDescending(p => p.Value)
                .Take(10))
            {
                var pct = product.Value * 100.0 / cleaned.RowCount;
                var bar = new string('█', (int)(pct));
                Console.WriteLine($"{product.Key,-20} {bar} {product.Value} ({pct:F1}%)");
            }
            
            // ═══════════════════════════════════════
            // 7. EXPORT RESULTS
            // ═══════════════════════════════════════
            Console.WriteLine("\n💾 EXPORTING RESULTS\n");
            
            File.WriteAllText("top_sales.csv", topSales.ToCsv());
            Console.WriteLine("✅ Exported: top_sales.csv");
            
            File.WriteAllText("regional_summary.csv", byRegion.ToCsv());
            Console.WriteLine("✅ Exported: regional_summary.csv");
            
            File.WriteAllText("regional_summary.html", byRegion.ToHtmlTable());
            Console.WriteLine("✅ Exported: regional_summary.html");
            
            // ═══════════════════════════════════════
            // 8. SUMMARY
            // ═══════════════════════════════════════
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         EXPLORATION COMPLETE           ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            
            var amounts = cleaned["Amount"].Select(decimal.Parse).ToList();
            Console.WriteLine($"\n📊 Final Stats:");
            Console.WriteLine($"   Records Analyzed: {cleaned.RowCount:N0}");
            Console.WriteLine($"   Total Sales: {amounts.Sum():C}");
            Console.WriteLine($"   Average: {amounts.Average():C}");
            Console.WriteLine($"   Median: {BasicStatistics.Median(amounts):C}");
            Console.WriteLine($"\n✨ Happy analyzing!");
        }
    }
}
```

---

## Quick Reference

### Display Methods
```csharp
table.PrettyDump(header: "Title", rowColor: ConsoleColor.DarkGreen)  // Beautiful table output
table.Gist()                            // Statistical summary
table.Head(10)                          // First 10 rows
table.Tail(10)                          // Last 10 rows
table.Top(10)                           // First 10 (pipeline context)
table.Bottom(10)                        // Last 10 (pipeline context)
table.Every(100)                        // Sample every 100th row
table.Pick("col1", "col2")              // Select columns
```

### Data Access
```csharp
table["ColumnName"]                     // Get column values
table.ValuesOf("ColumnName")            // Get column values (alternative)
table.RowCount                          // Number of rows
table.ColumnHeaders                     // Column names
table.Histogram("Column")               // Value distribution
```

### Export Methods
```csharp
table.ToCsv()                           // CSV format
table.ToTsv()                           // Tab-separated
table.ToHtmlTable()                     // HTML table
```

---

## Best Practices Summary

1. **Use PrettyDump liberally during development** - It's your window into the data
2. **Always call Console.ResetColor()** after colored output
3. **Use semantic colors** - Green for good, Red for warnings, Cyan for neutral
4. **Add descriptive headers** - Make output self-documenting
5. **Chain with Pick()** - Show only relevant columns
6. **Limit output size** - Use Head(), Top(), or Every() for large datasets
7. **Use Gist() for quality checks** - Catch outliers and data issues early
8. **Export results** - Save interesting findings with ToCsv() or ToHtmlTable()

---

These utility functions transform Squirrel from a data processing library into a complete data exploration and presentation platform. Happy analyzing! 🎉

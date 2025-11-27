# Core Concepts

Squirrel is a comprehensive data processing and analytics framework designed specifically for .NET developers. It transforms messy data into insights through an intuitive, business-readable API that makes data cleaning, analysis, and visualization accessible to both developers and business users.

## Table of Contents
- [Philosophy & Design](#philosophy--design)
- [The Data Pipeline](#the-data-pipeline)
- [The Table: Core Data Structure](#the-table-core-data-structure)
- [Data Flow Architecture](#data-flow-architecture)
- [Key Concepts](#key-concepts)

---

## Philosophy & Design

### Why Squirrel?

**The .NET Data Gap**  
While Python has pandas and R has comprehensive data tools, .NET developers have lacked a mature, integrated solution for data processing. Squirrel fills this gap by providing a complete data platform for .NET.

**The "Tiny to Medium Data" Reality**  
Most business datasets aren't "Big Data" – they're what Professor Alex Smola classified as "Tiny to Medium":

| Dataset Size | Name | Example |
|--------------|------|---------|
| Fits in mobile phone | Tiny | < 100 MB |
| Fits in laptop | Small | ~ 1 GB |
| Fits in workstation | Medium | ~ 4 GB |
| Fits in server | Large | > 4 GB |
| Requires clusters | Big | Petabytes |

**Business Reality**: Most business datasets have only a few thousand rows. Squirrel is optimized for this reality, processing 100k rows in under 1 second.

### Core Design Principles

1. **Business-Readable Code**
   - Pipelines read like specifications, not technical implementation
   - Method names use natural language (e.g., `RemoveIfNotBetween`, `NormalizeColumn`)
   - Self-documenting fluent API

2. **Complete Data Platform**
   - Generate data from mathematical formulas
   - Acquire data from multiple sources
   - Clean and validate data
   - Analyze with statistics
   - Visualize results
   - Export to various formats

3. **Developer Productivity**
   - Chainable operations (fluent interface)
   - Strong typing with `RecordTable<T>`
   - Minimal boilerplate code
   - IntelliSense-friendly

4. **Enterprise Ready**
   - Built-in compliance features (GDPR, HIPAA)
   - Data masking and anonymization
   - Robust error handling
   - Performance optimized

---

## The Data Pipeline

Squirrel follows a complete data pipeline architecture that mirrors real-world analytics workflows:

```
┌─────────────────┐
│ Data Generation │  Generate from formulas, create test data
│   & Acquisition │  Load from CSV, Excel, JSON, databases, APIs
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Data Cleansing  │  Remove outliers, handle missing values
│  & Validation   │  Normalize text, validate formats
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Transformation  │  Filter, sort, aggregate, pivot
│   & Analysis    │  Calculate statistics, split/merge tables
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Visualization  │  Charts, graphs, formatted output
│   & Export      │  Export to CSV, HTML, Excel, JSON
└─────────────────┘
```

### Pipeline Stages

#### 1. **Data Acquisition**
Load data from various sources:
```csharp
// From files
var data = DataAcquisition.LoadCsv("sales.csv");
var excel = DataAcquisition.LoadExcel("report.xlsx", "Sheet1");
var html = DataAcquisition.LoadHtmlTable("data.html");

// From cloud
var s3Data = await DataAcquisition.LoadFromS3(accessKey, secretKey, bucket, key);

// From memory
var generated = Enumerable.Range(1, 100)
    .Select(n => new { ID = n, Value = n * 10 })
    .ToTableFromAnonList();
```

#### 2. **Data Cleansing**
Clean and validate data:
```csharp
var clean = rawData
    .RemoveOutliers("amount")
    .RemoveIfNotBetween("age", 18, 120)
    .RemoveNonMatches("email", @"^[^@]+@[^@]+\.[^@]+$")
    .RemoveIncompleteRows("", "NA", "NULL")
    .Normalize("name", NormalizationStrategy.NameCase);
```

#### 3. **Transformation & Analysis**
Transform and analyze:
```csharp
var analysis = cleanData
    .Filter("region", "West", "North")
    .SortBy("revenue", how: SortDirection.Descending)
    .Aggregate("month", AggregationMethod.Sum)
    .AddColumn("profit", "[revenue] - [costs]", 2);
```

#### 4. **Visualization & Export**
Visualize and export results:
```csharp
// Display to console
analysis.PrettyDump(header: "Sales Analysis");

// Export to files
File.WriteAllText("report.csv", analysis.ToCsv());
File.WriteAllText("report.html", analysis.ToHtmlTable());

// Visualize (with visualization libraries)
analysis.ToBarChartByGoogleDataVisualization("month", "revenue", "Monthly Revenue");
```

---

## The Table: Core Data Structure

The `Table` class is the heart of Squirrel. It represents tabular data with rows and columns, similar to:
- A database table
- An Excel spreadsheet
- A pandas DataFrame
- A DataTable in ADO.NET

### Table Structure

```csharp
public class Table
{
    // Core properties
    public List<Dictionary<string, string>> Rows { get; set; }
    public HashSet<string> ColumnHeaders { get; }
    public int RowCount { get; }
    public string Name { get; set; }
    
    // Indexers for easy access
    public Dictionary<string, string> this[int index] { get; set; }      // Row access
    public List<string> this[string columnName] { get; set; }            // Column access
    public string this[string columnName, int index] { get; }             // Cell access
}
```

### Creating Tables

**From Files:**
```csharp
var customers = DataAcquisition.LoadCsv("customers.csv");
var sales = DataAcquisition.LoadExcel("sales.xlsx", "Q4");
```

**From Code:**
```csharp
// Empty table
var table = new Table();

// Add columns
table.AddColumn("Name", new List<string> { "Alice", "Bob", "Charlie" });
table.AddColumn("Age", new List<string> { "25", "30", "35" });

// Add rows
table.AddRow(new Dictionary<string, string>
{
    { "Name", "Diana" },
    { "Age", "28" }
});
```

**From Anonymous Types:**
```csharp
var data = new[]
{
    new { Name = "Alice", Age = 25, City = "Seattle" },
    new { Name = "Bob", Age = 30, City = "Portland" },
    new { Name = "Charlie", Age = 35, City = "Denver" }
}.ToTableFromAnonList();
```

**From Mathematical Formulas:**
```csharp
// Generate time series data
var model = new Table();
model.AddColumn("Time", Enumerable.Range(1, 60).Select(x => x.ToString()).ToList());
model.AddColumn("Position", "[Time] * 9.81 / 2", 2);
model.AddColumn("Velocity", "[Time] * 9.81", 2);
```

### Accessing Data

**Access Rows:**
```csharp
// Get single row
var firstRow = table[0];
Console.WriteLine(firstRow["Name"]); // Dictionary access

// Get range of rows
var rows = table[5, 10]; // Rows 5 through 10 (inclusive)

// Get specific rows
var selectedRows = table[new[] { 0, 5, 10 }];
```

**Access Columns:**
```csharp
// Get entire column
var names = table["Name"]; // Returns List<string>

// Get column with conversion
var ages = table["Age"].Select(int.Parse).ToList();
var salaries = table["Salary"].Select(decimal.Parse).ToList();
```

**Access Cells:**
```csharp
// Direct cell access
var value = table["Name", 5]; // Name at row 5

// Modify cell
table[5]["Name"] = "Updated Name";
```

---

## Data Flow Architecture

### Immutable Operations

Most Squirrel operations return a **new Table** rather than modifying the original:

```csharp
var original = DataAcquisition.LoadCsv("data.csv");
var filtered = original.Filter("status", "Active"); // Returns new table
var sorted = filtered.SortBy("amount");             // Returns new table

// Original remains unchanged
Console.WriteLine(original.RowCount);  // Original count
Console.WriteLine(sorted.RowCount);     // May be different
```

**Why Immutable?**
- Allows experimentation without data loss
- Enables easy comparison of before/after
- Supports parallel processing
- Makes debugging easier

### Fluent Interface (Method Chaining)

Operations can be chained together for readable pipelines:

```csharp
var result = DataAcquisition.LoadCsv("sales.csv")
    .RemoveOutliers("amount")
    .RemoveIfBefore("date", new DateTime(2024, 1, 1))
    .Normalize("customer", NormalizationStrategy.NameCase)
    .Filter("region", "West", "North")
    .SortBy("amount", how: SortDirection.Descending)
    .Top(100)
    .Aggregate("month", AggregationMethod.Sum);

result.PrettyDump(header: "Top 100 Sales by Month");
```

**Equivalent Non-Chained Version:**
```csharp
var raw = DataAcquisition.LoadCsv("sales.csv");
var cleaned = raw.RemoveOutliers("amount");
var recent = cleaned.RemoveIfBefore("date", new DateTime(2024, 1, 1));
var normalized = recent.Normalize("customer", NormalizationStrategy.NameCase);
var filtered = normalized.Filter("region", "West", "North");
var sorted = filtered.SortBy("amount", how: SortDirection.Descending);
var top = sorted.Top(100);
var result = top.Aggregate("month", AggregationMethod.Sum);
```

The chained version is more concise and reads like a specification.

### Type Safety with RecordTable

For strongly-typed scenarios, use `RecordTable<T>`:

```csharp
// Define record type
public record Customer(int Id, string Name, string Email, decimal Balance);

// Load as strongly-typed table
var customers = DataAcquisition.LoadJson<Customer>("customers.json");

// Type-safe operations
var highValue = customers.Filter(c => c.Balance > 10000m);
var sorted = highValue.Rows.OrderByDescending(c => c.Balance);

// Access with full IntelliSense
foreach (var customer in sorted)
{
    Console.WriteLine($"{customer.Name}: {customer.Balance:C}");
}

// Convert to regular Table for more operations
var table = customers.ToSqlTable();
```

---

## Key Concepts

### 1. Column-Oriented Operations

Squirrel is designed for column-based operations, making it natural to:

```csharp
// Extract column values
var amounts = sales["Amount"].Select(decimal.Parse).ToList();

// Calculate statistics
var median = BasicStatistics.Median(amounts);
var range = amounts.Range();
var p95 = amounts.Percentile(95m);

// Add calculated columns
sales.AddColumn("Tax", "[Amount] * 0.08", 2);
sales.AddColumn("Total", "[Amount] + [Tax]", 2);
```

### 2. Aggregation & Grouping

Squirrel provides powerful grouping and aggregation:

```csharp
// Split by category
var byRegion = sales.SplitOn("Region");
foreach (var region in byRegion)
{
    Console.WriteLine($"{region.Key}: {region.Value.RowCount} sales");
}

// Aggregate
var monthlySales = sales.Aggregate("Month", AggregationMethod.Sum);
var avgByProduct = sales.Aggregate("Product", AggregationMethod.Average);

// Custom aggregation
var custom = sales.Aggregate("Category", values => 
    values.Select(decimal.Parse).Max().ToString());
```

### 3. Filtering & Selection

Multiple ways to filter data:

```csharp
// By predicate
var result = sales.Filter(row => 
    decimal.Parse(row["Amount"]) > 1000 && 
    row["Status"] == "Completed");

// By column values
var westSales = sales.Filter("Region", "West");
var priorityCustomers = sales.Filter("Tier", "Gold", "Platinum");

// By pattern
var gmailUsers = sales.FilterByRegex("Email", @"@gmail\.com$");

// Extension methods
var expensive = sales.Where("Amount", ">", 1000m);
var recent = sales.Where("Date", ">", new DateTime(2024, 1, 1));
```

### 4. Missing Value Handling

Squirrel provides comprehensive missing value handling:

```csharp
// Replace with strategy
var filled = data.ReplaceMissingValuesByDefault(
    MissingValueHandlingStrategy.Average,
    "", "NA", "NULL", "N/A");

// Remove incomplete rows
var complete = data.RemoveIncompleteRows("", "NA");

// Mark as missing
var marked = data.MarkAsMissingIfNotAnyOf("Status", "MISSING", 
    "Active", "Pending", "Completed");
```

### 5. Data Transformation

Transform data in multiple ways:

```csharp
// Transform column values
var cleaned = data.Transform("Name", name => name.Trim().ToUpper());

// Normalize
var normalized = data.Normalize("Name", NormalizationStrategy.NameCase);

// Pivot
var pivoted = data.PivotSimple(
    indexColumn: "Product",
    pivotColumn: "Month", 
    valueColumn: "Sales");

// Transpose
var transposed = data.Transpose("Metric");

// Explode delimited values
var exploded = data.Explode("Tags", delim: ';');
```

### 6. Joining & Merging

Combine tables in various ways:

```csharp
// Inner join
var joined = customers.InnerJoin(orders, "CustomerID");

// Merge by columns (same as inner join)
var merged = customers.MergeByColumns(orders, "CustomerID");

// Union (vertical merge)
var combined = table1.Merge(table2, removeDups: true);

// Find common rows
var common = table1.Common(table2);

// Find exclusive rows
var exclusive = table1.Exclusive(table2);
```


Squirrel brings professional data analytics capabilities to .NET, making data science accessible to every developer. Start building your data pipelines today!

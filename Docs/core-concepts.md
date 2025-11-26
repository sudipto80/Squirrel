# Core Concepts

The core of the Squirrel library is the `Table` class, which represents a dataset in a tabular format.

## The Table Class

The `Table` class is the primary data structure used to hold data. It is designed to be intuitive and easy to manipulate.

### Key Features
- **Row-based and Column-based Access**: Access data by row index or column name.
- **Dynamic**: Columns can be added or removed dynamically.
- **Fluent API**: Most operations return a `Table` (or a modified copy), allowing for method chaining.

### Creating a Table

```csharp
using Squirrel;

// Create an empty table
var table = new Table();

// Add columns
table.AddColumn("Name");
table.AddColumn("Age");

// Add rows
table.AddRow("Alice", "30");
table.AddRow("Bob", "25");
```

## OrderedTable

`OrderedTable` is a specialized version of `Table` that is returned when a sorting operation is performed. It maintains the order of rows based on the sort criteria.

```csharp
// Sort by Age to get an OrderedTable
var orderedTable = table.SortBy("Age");

// Apply secondary sorting
var multiSorted = orderedTable.ThenBy("Name");

// OrderedTable supports all Table operations
var filtered = orderedTable.Filter("Age", new[] { "25", "30" });
```

## Dataframe

The `Dataframe` class provides advanced data manipulation capabilities, similar to dataframes in Python's pandas or R.

```csharp
// Create a Dataframe from a Table
var dataframe = new Dataframe(table);

// Perform advanced operations
var grouped = dataframe.GroupBy("Department");
var aggregated = dataframe.Aggregate("Salary", AggregationMethod.Average);

// Statistical analysis
var correlation = dataframe.Correlation("Age", "Salary");
var summary = dataframe.Describe();
```

## LazyTable

`LazyTable` is used for deferred execution or handling large datasets where loading everything into memory at once is not feasible.

```csharp
// Create a LazyTable for large datasets
var lazyTable = new LazyTable("large_dataset.csv");

// Operations are deferred until materialization
var filtered = lazyTable
    .Filter("Status", new[] { "Active" })
    .RemoveOutliers("Amount");

// Materialize only when needed
var results = filtered.Take(100); // Load only first 100 rows
var count = filtered.Count(); // Count without loading all data
```

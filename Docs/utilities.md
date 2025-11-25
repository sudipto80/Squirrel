# Utilities

Helper classes and extensions to simplify common tasks.

## Display and Output

### PrettyDump
Dumps the table in a pretty format to the console with customizable colors and alignment.

```csharp
void PrettyDump(
    this Table tab, 
    ConsoleColor headerColor = ConsoleColor.Green, 
    ConsoleColor rowColor = ConsoleColor.White,
    string header = "None", 
    Alignment align = Alignment.Right)
```

**Examples:**
```csharp
// Default dump
tab.PrettyDump();

// Dump with a header
tab.PrettyDump(header: "Sales Report");

// Dump with left alignment (right is default)
tab.PrettyDump(header: "Sales Report", align: Alignment.Left);
```

### ShowMe
Display table information.

```csharp
// Show rows matching a condition
table.ShowMe(row => row["Age"] == "30");
```

## Query and Inspection

### ColumnNamesInvolved
Get the names of columns involved in an operation.

```csharp
var columns = table.ColumnNamesInvolved();
foreach (var col in columns)
{
    Console.WriteLine(col);
}
```

### MethodInvolved
Identify the methods involved in a query or operation.

```csharp
var methodInfo = table.MethodInvolved();
Console.WriteLine($"Method: {methodInfo}");
```

### GetCommand
Retrieve command information.

```csharp
var command = table.GetCommand();
Console.WriteLine($"Command: {command}");
```

### PseudoNaturalQuery
Enable pseudo-natural language queries on your data.

```csharp
// Query using natural language
var result = table.PseudoNaturalQuery("show me all employees in IT department");
```

## Data Helpers

### Common
Common utility functions.

```csharp
// Find common rows between two tables
var commonRows = table1.Common(table2);
```

### Gist
Work with table gists for quick summaries.

```csharp
// Get a summary/gist of the table
var gist = table.Gist();
gist.PrettyDump();
```

### HandleIt
Error handling and data processing utilities.

```csharp
// Handle errors gracefully during data processing
table.HandleIt(row => 
{
    // Processing logic that might throw exceptions
    return ProcessRow(row);
});
```

### GetSmartDefaultValues
Automatically determine smart default values when data is missing or ambiguous, improving the robustness of data processing pipelines.

```csharp
// Get smart defaults for missing values
var defaults = table.GetSmartDefaultValues("Age");
table.ReplaceMissingValues("Age", defaults);
```

## Date Extensions

Utilities for handling and parsing dates.

```csharp
// Determine the frequency of date entries (Daily, Weekly, etc.)
var frequency = table.GetFrequency("OrderDate");
Console.WriteLine($"Frequency: {frequency}");

// Convert strings to DateTime objects safely
var dateColumn = table.ToDate("DateString");
```

## Generic Utilities

`GenericUtil` provides general-purpose helper methods used throughout the library.

```csharp
// Use generic utilities for common operations
var result = GenericUtil.ParseValue<int>("123");
var isNumeric = GenericUtil.IsNumeric("456.78");
```

# Visualization

Squirrel makes it easy to visualize your data by integrating with popular charting libraries like Google Charts.

## Google Data Visualization

You can generate HTML/JavaScript code to render charts using Google Charts.

### Bar Chart
```csharp
table.ToBarChartByGoogleDataVisualization(
    xAxisColumn: "Product",
    yAxisColumn: "Sales",
    title: "Product Sales"
);
```

### Pie Chart
```csharp
table.ToPieChartByGoogleDataVisualization(
    labelColumn: "Category",
    valueColumn: "Count",
    title: "Category Distribution"
);
```

### Histogram
Generate histogram visualizations to show the distribution of values.

```csharp
// Generate a histogram for age distribution
var histogramHtml = table.ToHistogramByGoogleDataVisualization(
    columnName: "Age",
    title: "Age Distribution",
    bucketSize: 10
);

// Save or display the histogram
File.WriteAllText("age_histogram.html", histogramHtml);
```

## Dashboards

### QuickDashboard
Create quick dashboards for data visualization and exploration.

```csharp
// Create a dashboard with multiple visualizations
var dashboard = table.QuickDashboard(
    title: "Sales Dashboard",
    charts: new[]
    {
        new { type = "bar", column = "Product", value = "Sales" },
        new { type = "pie", column = "Category", value = "Count" }
    }
);

// Save the dashboard
File.WriteAllText("dashboard.html", dashboard);
```

## HTML Tables

Convert your `Table` directly to an HTML table string for embedding in web pages or reports.

```csharp
string htmlTable = table.ToHtmlTable();
```

## Bootstrap Integration

Use `BootstrapTableDecorators` to style your HTML tables with Bootstrap classes for a modern look.

### Basic Bootstrap Table

Generate a standard Bootstrap table with optional styling classes like Striped, Bordered, Hover, etc.

```csharp
using static Squirrel.BootstrapTableDecorators;

// Generate a striped Bootstrap table
string bootstrapHtml = table.ToBasicBootstrapHtmlTable(BootstrapTableClasses.TableStriped);
```

### Conditional Row Coloring

You can apply Bootstrap contextual classes (success, warning, danger, etc.) to rows based on specific conditions.

```csharp
// Highlight rows based on sales figures
string coloredHtml = table.ToBootstrapHtmlTableWithColoredRows(
    // Green rows for high sales
    successPredicate: row => double.Parse(row["Sales"]) > 5000,
    
    // Red rows for low sales
    dangerPredicate: row => double.Parse(row["Sales"]) < 1000,
    
    // Yellow rows for warnings
    warningPredicate: row => row["Status"] == "Pending",
    
    // Apply hover effect to the table
    tableClass: BootstrapTableClasses.TableHover
);
```

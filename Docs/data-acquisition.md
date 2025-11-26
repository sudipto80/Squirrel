# Data Acquisition & Export

Data acquisition is the foundation of any data analysis workflow. Squirrel provides comprehensive methods to load data from various file formats and sources, as well as export data to multiple formats.

## Table of Contents
- [Loading Data from Files](#loading-data-from-files)
  - [CSV Files](#csv-files)
  - [TSV Files](#tsv-files)
  - [Excel Files](#excel-files)
  - [Fixed-Length Files](#fixed-length-files)
  - [ARFF Files](#arff-files)
  - [HTML Tables](#html-tables)
  - [JSON Files](#json-files)
- [Loading from Cloud Storage](#loading-from-cloud-storage)
  - [AWS S3](#aws-s3)
- [Creating Tables from Objects](#creating-tables-from-objects)
  - [From Anonymous Lists](#from-anonymous-lists)
  - [From DataTables](#from-datatables)
- [Exporting Data](#exporting-data)
  - [To CSV](#to-csv)
  - [To TSV](#to-tsv)
  - [To HTML](#to-html)
  - [To ARFF](#to-arff)
  - [To DataTable](#to-datatable)
- [Display & Debugging](#display--debugging)
  - [Console Output](#console-output)

---

## Loading Data from Files

### CSV Files

#### LoadCsv
Loads data from a CSV (Comma-Separated Values) file.

**Signature:**
```csharp
public static Table LoadCsv(string csvFileName, bool hasHeader = true)
```

**Parameters:**
- `csvFileName` - Path to the CSV file
- `hasHeader` - Whether the first row contains column headers (default: true)

**Examples:**
```csharp
// Load CSV with headers (default)
var customers = DataAcquisition.LoadCsv("customers.csv");

// Load CSV without headers
var data = DataAcquisition.LoadCsv("data.csv", hasHeader: false);

// Load and immediately work with data
var sales = DataAcquisition.LoadCsv("sales.csv")
    .Filter("Region", "West")
    .SortBy("Amount");
```

**Features:**
- Handles quoted fields containing commas
- Handles escaped quotes within quoted fields
- Automatically trims whitespace
- Preserves empty cells
- Fast parsing using optimized algorithms

**CSV Format Notes:**
```csv
Name,Age,City
"John Doe",30,"New York"
"Jane Smith",25,"Los Angeles"
"Bob Johnson",35,"Chicago"
```

---

### TSV Files

#### LoadTsv
Loads data from a TSV (Tab-Separated Values) file.

**Signature:**
```csharp
public static Table LoadTsv(string tsvFileName, bool hasHeader)
```

**Parameters:**
- `tsvFileName` - Path to the TSV file
- `hasHeader` - Whether the first row contains column headers

**Example:**
```csharp
// Load TSV file with headers
var data = DataAcquisition.LoadTsv("data.tsv", hasHeader: true);

// Load TSV file without headers
var raw = DataAcquisition.LoadTsv("raw_data.tsv", hasHeader: false);
```

**TSV Format Example:**
```
Name	Age	City
John Doe	30	New York
Jane Smith	25	Los Angeles
```

---

### Excel Files

#### LoadExcel
Loads data from an Excel file (.xls, .xlsx, .xlsb).

**Signature:**
```csharp
public static Table LoadExcel(string filePath, string sheetName = "Sheet1")
```

**Parameters:**
- `filePath` - Path to the Excel file
- `sheetName` - Name of the sheet to load (default: "Sheet1")

**Examples:**
```csharp
// Load default sheet
var data = DataAcquisition.LoadExcel("report.xlsx");

// Load specific sheet
var sales = DataAcquisition.LoadExcel("workbook.xlsx", "Sales Data");

// Load and process
var monthly = DataAcquisition.LoadExcel("finances.xlsx", "Monthly")
    .Aggregate("Month", AggregationMethod.Sum);
```

**Supported Formats:**
- Binary Excel files (Excel 2.0-2003, .xls)
- OpenXml Excel files (Excel 2007+, .xlsx)
- Excel Binary Workbook (.xlsb)

**Features:**
- Automatically detects file format
- First row treated as headers
- Returns data with proper column names
- Preserves table name from sheet name

---

### Fixed-Length Files

#### LoadFixedLength (with Dictionary)
Loads data from fixed-column-length files using a field length map.

**Signature:**
```csharp
public static Table LoadFixedLength(string fileName, 
    Dictionary<string, int> fieldLengthMap)
```

**Parameters:**
- `fileName` - Path to the fixed-length file
- `fieldLengthMap` - Dictionary mapping field names to their character widths

**Example:**
```csharp
// Define field lengths
var fieldLengths = new Dictionary<string, int>
{
    { "Name", 20 },
    { "Age", 3 },
    { "City", 15 },
    { "ZIP", 5 }
};

// Load file
var data = DataAcquisition.LoadFixedLength("data.txt", fieldLengths);
```

**Input File Format:**
```
John Doe            30 New York       10001
Jane Smith          25 Los Angeles    90001
Bob Johnson         35 Chicago        60601
```

#### LoadFixedLength (with String Format)
Loads data from fixed-length files using a formatted string specification.

**Signature:**
```csharp
public static Table LoadFixedLength(string fileName, string headersWithLength)
```

**Parameters:**
- `fileName` - Path to the fixed-length file
- `headersWithLength` - Format string: "name(20),age(3),city(15),zip(5)"

**Example:**
```csharp
// Load with format string
var data = DataAcquisition.LoadFixedLength("data.txt", 
    "Name(20),Age(3),City(15),ZIP(5)");

// Another example
var records = DataAcquisition.LoadFixedLength("payroll.txt",
    "EmployeeID(10),FirstName(15),LastName(15),Salary(10)");
```

**Format String Syntax:**
- Format: `columnName(width),columnName(width),...`
- Width in characters
- No spaces in format string
- Last column reads to end of line

---

### ARFF Files

#### LoadArff
Loads data from ARFF (Attribute-Relation File Format) files, commonly used with Weka data mining toolkit.

**Signature:**
```csharp
public static Table LoadArff(string fileName)
```

**Example:**
```csharp
// Load ARFF file
var weather = DataAcquisition.LoadArff("weather.arff");

// Load and analyze
var iris = DataAcquisition.LoadArff("iris.arff")
    .Aggregate("species", AggregationMethod.Count);
```

**ARFF Format Example:**
```
@relation weather

@attribute outlook {sunny, overcast, rainy}
@attribute temperature numeric
@attribute humidity numeric
@attribute windy {TRUE, FALSE}
@attribute play {yes, no}

@data
sunny,85,85,FALSE,no
sunny,80,90,TRUE,no
overcast,83,86,FALSE,yes
rainy,70,96,FALSE,yes
```

**Features:**
- Parses @attribute declarations
- Extracts column headers automatically
- Skips comments (lines starting with %)
- Reads data after @data declaration
- Handles nominal and numeric attributes

---

### HTML Tables

#### LoadHtmlTable
Loads data from an HTML table in a file.

**Signature:**
```csharp
public static Table LoadHtmlTable(string htmlTable)
```

**Parameters:**
- `htmlTable` - Path to the HTML file containing a table

**Example:**
```csharp
// Load HTML table
var data = DataAcquisition.LoadHtmlTable("table.html");

// Load web-scraped data
var prices = DataAcquisition.LoadHtmlTable("scraped_prices.html")
    .RemoveOutliers("Price");
```

**Supported HTML Format:**
```html
<table>
  <thead>
    <tr>
      <th>Name</th>
      <th>Age</th>
      <th>City</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>John Doe</td>
      <td>30</td>
      <td>New York</td>
    </tr>
    <tr>
      <td>Jane Smith</td>
      <td>25</td>
      <td>Los Angeles</td>
    </tr>
  </tbody>
</table>
```

**Features:**
- Handles both `<th>` headers and first-row headers
- Uses HtmlAgilityPack for robust parsing
- Trims whitespace from cell values
- Sets table name from filename
- Handles nested HTML in cells

---

### JSON Files

#### LoadJson
Loads data from a JSON file into a strongly-typed RecordTable.

**Signature:**
```csharp
public static RecordTable<T> LoadJson<T>(string jsonFileName)
```

**Parameters:**
- `jsonFileName` - Path to the JSON file
- `T` - The record type to deserialize into

**Example:**
```csharp
// Define a record type
public record Customer(string Name, int Age, string Email);

// Load JSON
var customers = DataAcquisition.LoadJson<Customer>("customers.json");

// Access data
foreach (var customer in customers.Rows)
{
    Console.WriteLine($"{customer.Name}: {customer.Email}");
}
```

**JSON Format Example:**
```json
[
  { "Name": "John Doe", "Age": 30, "Email": "john@example.com" },
  { "Name": "Jane Smith", "Age": 25, "Email": "jane@example.com" }
]
```

---

## Loading from Cloud Storage

### AWS S3

#### LoadFromS3
Loads data from an AWS S3 bucket as a Table.

**Signature:**
```csharp
public static async Task<Table> LoadFromS3(string accessKey, string secretKey,
    string bucketName, string resourceName)
```

**Parameters:**
- `accessKey` - AWS access key
- `secretKey` - AWS secret key
- `bucketName` - S3 bucket name
- `resourceName` - File path/key in S3

**Example:**
```csharp
// Load CSV from S3
var sales = await DataAcquisition.LoadFromS3(
    accessKey: "YOUR_ACCESS_KEY",
    secretKey: "YOUR_SECRET_KEY",
    bucketName: "my-data-bucket",
    resourceName: "data/sales.csv");

// Process loaded data
var summary = sales.Aggregate("Region", AggregationMethod.Sum);
```

**Features:**
- Uses AWS SDK
- Automatically detects file format (currently supports CSV)
- Downloads to temporary file
- Region: US-East-1 (default)

#### LoadFromS3AsString
Loads S3 object content as a string.

**Signature:**
```csharp
public static async Task<string> LoadFromS3AsString(string accessKey, 
    string secretKey, string bucketName, string resourceName)
```

**Example:**
```csharp
// Load file content as string
var content = await DataAcquisition.LoadFromS3AsString(
    accessKey: "YOUR_ACCESS_KEY",
    secretKey: "YOUR_SECRET_KEY",
    bucketName: "my-bucket",
    resourceName: "data/config.txt");

Console.WriteLine(content);
```

#### ReadS3ObjectAsString
Alternative method to read S3 object as string.

**Signature:**
```csharp
public static async Task<string> ReadS3ObjectAsString(string accessKey,
    string secretKey, string bucketName, string objectKey)
```

**Example:**
```csharp
// Read object content
var json = await DataAcquisition.ReadS3ObjectAsString(
    accessKey: "YOUR_ACCESS_KEY",
    secretKey: "YOUR_SECRET_KEY",
    bucketName: "my-bucket",
    objectKey: "config.json");
```

#### ListAllS3Objects
Lists all objects in an S3 bucket.

**Signature:**
```csharp
public static async Task<List<S3Object>> ListAllS3Objects(string accessKey,
    string secretKey, string bucketName)
```

**Example:**
```csharp
// List all files in bucket
var objects = await DataAcquisition.ListAllS3Objects(
    accessKey: "YOUR_ACCESS_KEY",
    secretKey: "YOUR_SECRET_KEY",
    bucketName: "my-data-bucket");

// Display files
foreach (var obj in objects)
{
    Console.WriteLine($"{obj.Key} - {obj.Size} bytes - {obj.LastModified}");
}
```

**Features:**
- Handles pagination automatically
- Retrieves up to 1000 objects per request
- Returns complete list of all objects
- Includes file metadata (size, last modified, etc.)

---

## Creating Tables from Objects

### From Anonymous Lists

#### ToTableFromAnonList
Creates a Table from a list of anonymous objects or any class instances.

**Signature:**
```csharp
public static Table ToTableFromAnonList<T>(this IEnumerable<T> list) 
    where T : class
```

**Examples:**
```csharp
// Create from anonymous objects
var data = new[]
{
    new { Name = "John", Age = 30, City = "New York" },
    new { Name = "Jane", Age = 25, City = "Los Angeles" },
    new { Name = "Bob", Age = 35, City = "Chicago" }
};

var table = data.ToTableFromAnonList();

// Create from LINQ query
var summary = orders
    .GroupBy(o => o.Region)
    .Select(g => new
    {
        Region = g.Key,
        TotalSales = g.Sum(o => o.Amount),
        OrderCount = g.Count()
    })
    .ToTableFromAnonList();

// Create from class instances
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}

var products = new List<Product>
{
    new Product { Name = "Laptop", Price = 999.99m, Category = "Electronics" },
    new Product { Name = "Mouse", Price = 29.99m, Category = "Electronics" }
};

var productTable = products.ToTableFromAnonList();
```

**Features:**
- Works with any class or anonymous type
- Automatically extracts property names as columns
- Converts all values to strings
- Preserves property order

#### ToRecordTableFromAnonList
Creates a strongly-typed RecordTable from a list.

**Signature:**
```csharp
public static RecordTable<T> ToRecordTableFromAnonList<T>(this IEnumerable<T> list)
```

**Example:**
```csharp
// Create strongly-typed table
public record Sale(string Product, decimal Amount, DateTime Date);

var sales = new List<Sale>
{
    new("Laptop", 999.99m, DateTime.Now),
    new("Mouse", 29.99m, DateTime.Now)
};

var salesTable = sales.ToRecordTableFromAnonList();

// Access with type safety
foreach (var sale in salesTable.Rows)
{
    Console.WriteLine($"{sale.Product}: ${sale.Amount}");
}
```

---

### From DataTables

#### LoadDataTable
Converts an ADO.NET DataTable to a Squirrel Table.

**Signature:**
```csharp
public static Table LoadDataTable(DataTable dt)
```

**Example:**
```csharp
// Load from database
using var connection = new SqlConnection(connectionString);
var adapter = new SqlDataAdapter("SELECT * FROM Customers", connection);
var dataTable = new DataTable();
adapter.Fill(dataTable);

// Convert to Squirrel Table
var customers = DataAcquisition.LoadDataTable(dataTable);

// Now use Squirrel's analysis features
var summary = customers
    .Aggregate("Region", AggregationMethod.Count)
    .SortBy("Count", how: SortDirection.Descending);
```

**Features:**
- Preserves all columns and rows
- Converts all values to strings
- Maintains column names
- Works with any ADO.NET data source

---

## Exporting Data

### To CSV

#### ToCsv
Exports a Table to CSV format.

**Signature:**
```csharp
public static string ToCsv(this Table tab)
```

**Example:**
```csharp
// Export to CSV string
var csvContent = table.ToCsv();

// Save to file
File.WriteAllText("output.csv", table.ToCsv());

// Export filtered data
var westernSales = sales
    .Filter("Region", "West")
    .ToCsv();
File.WriteAllText("western_sales.csv", westernSales);
```

**Output Format:**
```csv
Name,Age,City
"John Doe","30","New York"
"Jane Smith","25","Los Angeles"
"Bob Johnson","35","Chicago"
```

**Features:**
- Includes column headers
- Quotes all values
- Handles commas within values
- Standard CSV format

---

### To TSV

#### ToTsv
Exports a Table to TSV (Tab-Separated Values) format.

**Signature:**
```csharp
public static string ToTsv(this Table tab)
```

**Example:**
```csharp
// Export to TSV string
var tsvContent = table.ToTsv();

// Save to file
File.WriteAllText("output.tsv", table.ToTsv());
```

**Output Format:**
```
Name	Age	City
"John Doe"	"30"	"New York"
"Jane Smith"	"25"	"Los Angeles"
```

---

### To HTML

#### ToHtmlTable
Exports a Table to HTML table format.

**Signature:**
```csharp
public static string ToHtmlTable(this Table tab)
```

**Examples:**
```csharp
// Export to HTML
var html = table.ToHtmlTable();

// Save as HTML file
File.WriteAllText("report.html", $@"
<!DOCTYPE html>
<html>
<head>
    <title>Report</title>
    <style>
        table {{ border-collapse: collapse; }}
        th, td {{ border: 1px solid black; padding: 8px; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    {table.ToHtmlTable()}
</body>
</html>");

// Use in web applications
var salesHtml = sales
    .Aggregate("Month", AggregationMethod.Sum)
    .ToHtmlTable();
```

**Output Format:**
```html
<table>
<thead><tr>
<th>Name</th>
<th>Age</th>
<th>City</th>
</tr></thead>
<tbody>
<tr>
<td>John Doe</td>
<td>30</td>
<td>New York</td>
</tr>
<tr>
<td>Jane Smith</td>
<td>25</td>
<td>Los Angeles</td>
</tr>
</tbody>
</table>
```

**Features:**
- Proper HTML table structure
- Includes `<thead>` and `<tbody>`
- Ready for styling with CSS
- Valid HTML5

---

### To ARFF

#### ToArff
Exports a Table to ARFF format (for Weka data mining).

**Signature:**
```csharp
public static string ToArff(this Table tab)
```

**Example:**
```csharp
// Export to ARFF
var arffContent = table.ToArff();

// Save to file
File.WriteAllText("data.arff", table.ToArff());

// Export for machine learning
var iris = DataAcquisition.LoadCsv("iris.csv");
File.WriteAllText("iris.arff", iris.ToArff());
```

**Output Format:**
```
@attribute Name {John Doe,Jane Smith,Bob Johnson}
@attribute Age {30,25,35}
@attribute City {New York,Los Angeles,Chicago}
@data
John Doe,30,New York
Jane Smith,25,Los Angeles
Bob Johnson,35,Chicago
```

**Features:**
- Generates @attribute declarations
- Includes all distinct values for each attribute
- Proper @data section
- Compatible with Weka toolkit

---

### To DataTable

#### ToDataTable
Converts a Squirrel Table to an ADO.NET DataTable.

**Signature:**
```csharp
public static DataTable ToDataTable(this Table tab)
```

**Example:**
```csharp
// Convert to DataTable
var dataTable = table.ToDataTable();

// Use with ADO.NET
using var connection = new SqlConnection(connectionString);
using var bulkCopy = new SqlBulkCopy(connection);
bulkCopy.DestinationTableName = "Customers";
connection.Open();
bulkCopy.WriteToServer(dataTable);

// Bind to UI controls
dataGridView.DataSource = table.ToDataTable();
```

**Features:**
- Creates proper DataTable structure
- All columns as string type
- Preserves all rows and columns
- Compatible with ADO.NET operations

---

### Convert to RecordTable

#### AsRecordTable
Converts a Table to a strongly-typed RecordTable.

**Signature:**
```csharp
public static RecordTable<T> AsRecordTable<T>(this Table tab)
```

**Example:**
```csharp
// Define record type
public record Customer(string Name, int Age, string City);

// Load and convert
var table = DataAcquisition.LoadCsv("customers.csv");
var typedTable = table.AsRecordTable<Customer>();

// Type-safe access
foreach (var customer in typedTable.Rows)
{
    Console.WriteLine($"{customer.Name} is {customer.Age} years old");
}
```

---

## Display & Debugging

### Console Output

#### PrettyDump
Displays a table in a formatted, aligned console output.

**Signature:**
```csharp
public static void PrettyDump(this Table tab, 
    ConsoleColor headerColor = ConsoleColor.Green,
    ConsoleColor rowColor = ConsoleColor.White,
    string header = "None", 
    Alignment align = Alignment.Right)
```

**Parameters:**
- `headerColor` - Color for column headers (default: Green)
- `rowColor` - Color for data rows (default: White)
- `header` - Optional table header text
- `align` - Column alignment: `Alignment.Right` or `Alignment.Left`

**Examples:**
```csharp
// Basic dump
table.PrettyDump();

// With custom colors
table.PrettyDump(
    headerColor: ConsoleColor.Cyan,
    rowColor: ConsoleColor.Yellow);

// With header and left alignment
table.PrettyDump(
    header: "Sales Report - Q4 2024",
    align: Alignment.Left);

// Custom styling
sales
    .Aggregate("Region", AggregationMethod.Sum)
    .SortBy("Total", how: SortDirection.Descending)
    .PrettyDump(
        headerColor: ConsoleColor.Blue,
        rowColor: ConsoleColor.White,
        header: "Regional Sales Summary",
        align: Alignment.Right);
```

**Output Example:**
```
Sales Report - Q4 2024
   Name              Age       City
   John Doe           30  New York
   Jane Smith         25  Los Angeles
   Bob Johnson        35  Chicago
```

**Features:**
- Auto-aligns columns based on content width
- Color-coded headers and rows
- Optional table header
- Left or right alignment
- Handles long strings gracefully

---

## Complete Workflow Examples

### Data Pipeline Example
```csharp
// Load data
var sales = DataAcquisition.LoadCsv("sales.csv");

// Clean and analyze
var report = sales
    .RemoveOutliers("Amount")
    .RemoveIfBefore("Date", new DateTime(2024, 1, 1))
    .Aggregate("Region", AggregationMethod.Sum)
    .SortBy("Amount", how: SortDirection.Descending);

// Export results
File.WriteAllText("report.csv", report.ToCsv());
File.WriteAllText("report.html", report.ToHtmlTable());
report.PrettyDump(header: "Regional Sales Report");
```

### Multi-Format Processing
```csharp
// Load from different sources
var csvData = DataAcquisition.LoadCsv("sales.csv");
var excelData = DataAcquisition.LoadExcel("inventory.xlsx", "Stock");
var htmlData = DataAcquisition.LoadHtmlTable("prices.html");

// Merge and analyze
var combined = csvData
    .MergeByColumns(excelData, "ProductID")
    .MergeByColumns(htmlData, "ProductID");

// Export
File.WriteAllText("combined.csv", combined.ToCsv());
```

### Cloud Data Pipeline
```csharp
// Load from S3
var rawData = await DataAcquisition.LoadFromS3(
    accessKey: config["AWS:AccessKey"],
    secretKey: config["AWS:SecretKey"],
    bucketName: "data-warehouse",
    resourceName: "raw/sales.csv");

// Process
var cleaned = rawData
    .RemoveIncompleteRows("", "NA", "NULL")
    .RemoveOutliers("Amount")
    .Normalize("CustomerName", NormalizationStrategy.NameCase);

// Export locally
File.WriteAllText("processed.csv", cleaned.ToCsv());
```

### In-Memory Data Creation
```csharp
// Create from code
var data = new[]
{
    new { Month = "Jan", Sales = 15000, Expenses = 8000 },
    new { Month = "Feb", Sales = 18000, Expenses = 9000 },
    new { Month = "Mar", Sales = 22000, Expenses = 10000 }
}.ToTableFromAnonList();

// Analyze
data.AddColumn("Profit", "[Sales] - [Expenses]", 2);
data.PrettyDump(header: "Monthly Financial Summary");

// Export
File.WriteAllText("monthly_summary.csv", data.ToCsv());
```

---

## Error Handling

All data acquisition methods include error handling:

```csharp
try
{
    var data = DataAcquisition.LoadCsv("data.csv");
}
catch (FileNotFoundException)
{
    Console.WriteLine("File not found");
}
catch (IOException ex)
{
    Console.WriteLine($"IO Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading data: {ex.Message}");
}
```

For S3 operations:
```csharp
try
{
    var data = await DataAcquisition.LoadFromS3(accessKey, secretKey, 
        bucketName, resourceName);
}
catch (AmazonS3Exception ex)
{
    Console.WriteLine($"S3 Error: {ex.Message}");
}
```

---

## Performance Tips

1. **Large CSV Files**: Use streaming for very large files
2. **Excel Files**: Load specific sheets rather than entire workbook
3. **S3 Files**: Consider downloading once and caching locally
4. **Memory**: Process data in chunks for very large datasets
5. **Export**: Use `ToCsv()` and write to file stream for large exports

---

## Summary

Squirrel's DataAcquisition class provides:
- ✅ Multiple file format support (CSV, TSV, Excel, ARFF, HTML, JSON)
- ✅ Cloud storage integration (AWS S3)
- ✅ In-memory data creation
- ✅ Multiple export formats
- ✅ Type-safe operations with RecordTable
- ✅ Easy conversion to/from ADO.NET
- ✅ Console debugging with PrettyDump
- ✅ Robust parsing and error handling

For data cleaning operations, see [Data Cleansing](Data_Cleansing.md) documentation.

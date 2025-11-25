# Data Acquisition

Squirrel provides a robust `DataAcquisition` class for loading data from various sources and saving it to different formats.

## Loading Data

### CSV
Load data from Comma Separated Value files.
```csharp
var table = DataAcquisition.LoadCsv("path/to/file.csv");
// With custom delimiter
var table = DataAcquisition.LoadCsv("path/to/file.txt", delimiter: '\t');
```

### TSV
Load data from Tab Separated Value files.
```csharp
var table = DataAcquisition.LoadTsv("path/to/file.tsv");
```

### Excel
Load data from Excel files (.xls, .xlsx).
```csharp
var table = DataAcquisition.LoadXls("path/to/file.xlsx");
```

### HTML
Parse HTML tables directly from a string or file.
```csharp
var table = DataAcquisition.LoadHtml(htmlString);
```

### Parquet
Load data from Parquet files for efficient storage and retrieval.
```csharp
var table = DataAcquisition.LoadParquet("path/to/file.parquet");
```

### ARFF
Loads ARFF file. ARFF files are the file formats for the Weka project.
```csharp
var table = DataAcquisition.LoadARFF("path/to/weather.arff");
```

### Fixed-Length
Load data from fixed-width text files. You can specify the column widths using a dictionary or a formatted string.
```csharp
// Using a dictionary
var fieldLengthMap = new Dictionary<string, int> { { "col1", 10 }, { "col2", 15 } };
var table = DataAcquisition.LoadFixedLength("path/to/file.txt", fieldLengthMap);

// Using a formatted string
var table = DataAcquisition.LoadFixedLength("path/to/file.txt", "col1:10,col2:15");
```

### Flat File
Load data from any flat file format.
```csharp
var table = DataAcquisition.LoadFlatFile("path/to/file.dat");
```

### Linear Entries
Load data from linear entry format.
```csharp
var table = DataAcquisition.LoadLinearEntries("path/to/file.txt");
```

### ADO.NET DataTable
Load data directly from an ADO.NET `DataTable`.
```csharp
var table = DataAcquisition.LoadDataTable(myDataTable);
```

### Databases
Connect to SQL Server, MongoDB, and other databases using `DatabaseConnectors`.

#### SQL Server
```csharp
var table = DataAcquisition.LoadFromSQLServer2008(connectionString, "SELECT * FROM MyTable");
```

#### MongoDB
```csharp
var table = DataAcquisition.LoadFromMongoDB(connectionString, "MyCollection");
```

### Run SQL Query
Execute a SQL query against a database and load the results.
```csharp
var table = DataAcquisition.RunSQLQuery(connection, "SELECT * FROM MyTable");
```

## Saving Data

You can export your `Table` objects back to various formats.

```csharp
// Save to CSV
table.ToCsv("output.csv");

// Save to TSV
table.ToTsv("output.tsv");

// Save to HTML
var html = table.ToHtml();

// Convert to ADO.NET DataTable
var dataTable = table.ToDataTable();

// Save to ARFF format
var arffString = table.ToArff();
File.WriteAllText("output.arff", arffString);

// Convert Gist to Table
var gist = table.Gist();
var tableFromGist = gist.ToTable();
```


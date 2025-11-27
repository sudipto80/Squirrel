# Squirrel (TableAPI) Documentation

Welcome to the documentation for Squirrel!
A comprehensive data processing and analytics framework for .NET.

## Overview

Squirrel provides a fluent, business-readable API for:
- **Data Acquisition**: Loading data from CSV, Excel, Databases, HTML, and more.
- **Data Cleaning**: Filtering, transforming, and normalizing data.
- **Analysis**: Statistical analysis, outlier detection, and aggregation.
- **Visualization**: Generating charts and reports.

## Documentation Sections

- [Core Concepts](core-concepts.md): Understand the `Table` data structure and basic operations.
- [Data Acquisition](data-acquisition.md): How to load and save data.
- [Data Cleaning & Transformation](data-cleaning.md): Methods for cleaning and transforming your datasets.
- [Analysis & Statistics](analysis-statistics.md): Statistical functions and data analysis tools.
- [Utilities](utilities.md): Helper functions and extensions.

## Quick Start

```csharp
using Squirrel;

// Load data
var data = DataAcquisition.LoadCsv("data.csv");

// Clean and Analyze
var result = data
    .RemoveOutliers("Amount")
    .Filter("Category", "Electronics")
    .SortBy("Date");

// Visualize
result.ToBarChartByGoogleDataVisualization("Date", "Amount", "Sales Over Time");
```

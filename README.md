# Squirrel

<img src="squirrel_logo.png" alt="Squirrel Logo" height="250" width="300">

[Logo designed by Pirog Tetyana from The Noun Project](https://raw.github.com/sudipto80/Squirrel/newb/img/license.txt)

## Cross-Platform Agile Data Analytics for .NET

<img src="https://www.pocketsolution.net/img/mac-linux-windows.png" alt="Cross-Platform Support">

---

## Introduction

Squirrel is a cross-platform agile data analytics framework built on .NET Standard 2.0. It is designed to empower .NET developers with an end-to-end data analysis solution, offering seamless integration for data acquisition, transformation, and visualization. With Squirrel, you can:

- Handle tiny to medium datasets efficiently.
- Leverage modern C# features to create clean and intuitive data pipelines.
- Generate professional visualizations using adapters for Google Charts and Highcharts.

---

## Features

- **Clean and Intuitive API:** Perform complex operations like filtering, sorting, and aggregations with ease.
- **Built-in Visualization Support:** Create charts and graphs using Google Charts or Highcharts directly from your data.
- **End-to-End Workflow:** Cover data acquisition, transformation, and visualization in a single framework.
- **Optimized for Small Data:** Perfect for datasets up to a few hundred thousand rows.
- **Modern C# Integration:** Use LINQ, pattern matching, and other modern features seamlessly.
- **Embeddable Outputs:** Generate HTML-ready visualizations for integration into web apps.

---

## Quick Start

### Installation
Install the Squirrel package via NuGet:

```bash
PM> Install-Package TableAPI
```

### Example Usage

Here’s how you can generate a pie chart of the top 10 gold-winning nations in the Olympics:

```csharp
using Squirrel;

// Load data
Table olympics = DataAcquisition.LoadCSV("olympics.csv");

// Generate a pie chart
string html = olympics
             .SortBy("Gold Medals", SortDirection.Descending)
             .Top(10)
             .ToPieByGoogleDataVisualization("Country", "Top Gold Winning Nations");

System.Diagnostics.Process.Start("dashboard.html");
```

---

## Why Choose Squirrel?

### Comparison with Deedle

| Feature                     | Squirrel                        | Deedle              |
|-----------------------------|----------------------------------|---------------------|
| API Cleanliness             | Intuitive, chainable methods    | Verbose             |
| Built-in Visualizations     | Yes (Google/Highcharts support) | No                  |
| Workflow Coverage           | End-to-end                     | Data manipulation   |
| Modern C# Features          | Fully supported                 | Limited             |
| Target Dataset Size         | Tiny to Medium                  | Similar             |

---

## Architecture Overview

<img src="squirrel_block.png" alt="Squirrel Architecture">

### Core Components:

1. **Data Modeling:**
   - Stores data in a ubiquitous `Table` format.
2. **I/O Blocks:**
   - Handles data input and output in various formats (CSV, TSV, HTML, etc.).
3. **Data Cleansing:**
   - Offers tools to clean and prepare raw datasets.
4. **Data Generation:**
   - Programmatically generate new datasets.
5. **Data Visualization Adapters:**
   - Create visually appealing charts and graphs.

---

## Detailed Examples

Explore more examples in the [Examples Section](https://github.com/sudipto80/Squirrel/tree/master/ScreenCastDemos):

1. [Do women pay more tips than men?](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-01.md)
2. [Iris dataset aggregation](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-02.md)
3. [Finding top gold-winning nations in Olympics](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-04.md)

---

## Upcoming Book

### "Data Analysis with C#13 and .NET 9"

Dive deeper into Squirrel with the upcoming book, "Data Analysis with C#13 and .NET 9: A Practical Guide Using Squirrel," published by Apress. Learn real-world use cases, advanced tutorials, and industry applications.

**Release Date:** April 2025  
**Publisher:** Apress

---

## Contributing

We welcome contributions! Here’s how you can help:

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Submit a pull request.

Check the [Contributing Guidelines](https://github.com/sudipto80/Squirrel/blob/master/CONTRIBUTING.md) for more details.

---

## Community and Support

Join our community to ask questions, share ideas, and collaborate:

- [GitHub Discussions](https://github.com/sudipto80/Squirrel/discussions)
- [Slack Channel](#)
- [Twitter](https://twitter.com/squirrel_analytics)

---

## License

Squirrel is licensed under the [MIT License](https://github.com/sudipto80/Squirrel/blob/master/LICENSE).

---

## Acknowledgments

Special thanks to all contributors and users who make Squirrel better every day!

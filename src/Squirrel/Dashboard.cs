using System.Text;

namespace Squirrel;

public class Dashboard
{
    private readonly int _rows;
    private readonly int _columns;
    private readonly string _title;

    // Row index → Column index → DashboardBox
    private readonly Dictionary<int, Dictionary<int, DashboardBox>>
        _cells = new();

    // ──────────────────────────────────────────
    // Constructors
    // ──────────────────────────────────────────

    /// <summary>
    /// Creates a Dashboard with an explicit grid size.
    /// Use AddCell() to place boxes.
    /// </summary>
    public Dashboard(int rows, int columns,string title = "")
    {
        _rows    = rows;
        _columns = columns;
        _title    = title; 
    }

    /// <summary>
    /// Creates a Dashboard from a layout preset and a total number of cells.
    /// Automatically computes rows/columns from the layout.
    /// </summary>
    public Dashboard(DashboardLayout layout, int totalCellCount,string title = "")
    {
        (_rows, _columns) = ResolveGrid(layout, totalCellCount);
        _title            = title;
    }

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>
    /// Places a DashboardBox at the given (row, col) position.
    /// Supports method chaining.
    /// </summary>
    public Dashboard AddCell(int row, int col, DashboardBox cell)
    {
        if (row < 0 || row >= _rows)
            throw new ArgumentOutOfRangeException(nameof(row),
                $"Row must be between 0 and {_rows - 1}.");

        if (col < 0 || col >= _columns)
            throw new ArgumentOutOfRangeException(nameof(col),
                $"Column must be between 0 and {_columns - 1}.");

        if (!_cells.ContainsKey(row))
            _cells[row] = new Dictionary<int, DashboardBox>();

        if (_cells[row].ContainsKey(col))
            throw new InvalidOperationException(
                $"Cell ({row},{col}) is already occupied.");

        _cells[row][col] = cell;
        return this;
    }

    /// <summary>
    /// Adds boxes sequentially, filling left-to-right then top-to-bottom.
    /// Useful when working with the KeepAdding / Automatic layouts.
    /// </summary>
    public Dashboard AddCells(IEnumerable<DashboardBox> boxes)
    {
        int row = 0, col = 0;
        foreach (var box in boxes)
        {
            AddCell(row, col, box);
            col++;
            if (col >= _columns) { col = 0; row++; }
        }
        return this;
    }

    /// <summary>
    /// Renders all boxes into a single self-contained HTML dashboard page.
    /// </summary>
    public string Render_old()
    {
        string cellWidth = $"{Math.Floor(100.0 / _columns)}%";

        var rowsHtml = new StringBuilder();

        for (int r = 0; r < _rows; r++)
        {
            rowsHtml.AppendLine(@"  <div class=""dashboard-row"">");

            for (int c = 0; c < _columns; c++)
            {
                if (_cells.TryGetValue(r, out var cols) &&
                    cols.TryGetValue(c, out var box))
                {
                    rowsHtml.AppendLine(box.Render(cellWidth));
                }
                else
                {
                    // Empty placeholder keeps the grid aligned
                    rowsHtml.AppendLine(
                        $@"    <div class=""dashboard-box dashboard-box--empty"" style=""width:{cellWidth};""></div>");
                }
            }

            rowsHtml.AppendLine("  </div>");
        }

       return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8""/>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>Dashboard</title>
  <style>
    *, *::before, *::after {{ box-sizing: border-box; margin: 0; padding: 0; }}

    body {{
      font-family: 'Segoe UI', Arial, sans-serif;
      background: #f0f2f5;
      padding: 24px;
    }}

    .dashboard-grid {{
      display: flex;
      flex-direction: column;
      gap: 16px;
    }}

    .dashboard-row {{
      display: flex;
      flex-direction: row;
      gap: 16px;
      width: 100%;
    }}

    .dashboard-box {{
      background: #ffffff;
      border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 12px;
      overflow: hidden;
      min-height: 480px;
    }}

    .dashboard-box--empty {{
      background: transparent;
      box-shadow: none;
      min-height: 0;
    }}

    .dashboard-box-title {{
      font-size: 1rem;
      font-weight: 600;
      color: #333;
      border-bottom: 2px solid #f0f2f5;
      padding-bottom: 8px;
      flex-shrink: 0;
    }}

    .dashboard-box-content {{
      flex: 1;
      height: 420px;
      overflow: hidden;
      position: relative;
    }}

    .dashboard-box-content canvas {{
      display: block;
      width: 100% !important;
      height: 100% !important;
      max-width: 100%;
    }}

    .dashboard-box-content iframe,
    .dashboard-box-content div[id^=""chart""] {{
      width: 100% !important;
      height: 100% !important;
    }}

    .dashboard-box-content table {{
      width: 100%;
      border-collapse: collapse;
      font-size: 0.875rem;
    }}

    .dashboard-box-content th {{
      background: #f0f2f5;
      padding: 8px 12px;
      text-align: left;
      font-weight: 600;
      color: #444;
      border-bottom: 2px solid #ddd;
    }}

    .dashboard-box-content td {{
      padding: 7px 12px;
      border-bottom: 1px solid #eee;
      color: #555;
    }}

    .dashboard-box-content tr:last-child td {{
      border-bottom: none;
    }}

    .dashboard-box-content tr:hover td {{
      background: #f9f9f9;
    }}
  </style>
</head>
<body>
  <div class=""dashboard-grid"">
{rowsHtml}  </div>
</body>
</html>";
    }

   public string Render()
{
    bool isVertical = _columns == 1;
    string cellWidth = isVertical
        ? "100%"
        : $"{Math.Floor(100.0 / _columns)}%";

    var rowsHtml = new StringBuilder();
    int chartCounter = 0;

    for (int r = 0; r < _rows; r++)
    {
        rowsHtml.AppendLine(@"  <div class=""dashboard-row"">");

        for (int c = 0; c < _columns; c++)
        {
            if (_cells.TryGetValue(r, out var cols) &&
                cols.TryGetValue(c, out var box))
            {
                string boxWidth = box.ColSpan > 1
                    ? $"{Math.Floor(100.0 / _columns * box.ColSpan)}%"
                    : cellWidth;

                rowsHtml.AppendLine(box.Render(boxWidth, chartCounter++));
            }
            else
            {
                rowsHtml.AppendLine(
                    $@"    <div class=""dashboard-box dashboard-box--empty"" style=""width:{cellWidth};""></div>");
            }
        }

        rowsHtml.AppendLine("  </div>");
    }

    string titleHtml = string.IsNullOrEmpty(_title)
        ? ""
        : $@"<div class=""dashboard-title"">{_title}</div>";

    return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8""/>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>{(_title == "" ? "Dashboard" : _title)}</title>
  <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
  <style>
    *, *::before, *::after {{ box-sizing: border-box; margin: 0; padding: 0; }}

    body {{
      font-family: 'Segoe UI', Arial, sans-serif;
      background: #f0f2f5;
      padding: 24px;
    }}

    .dashboard-title {{
      font-size: 1.6rem;
      font-weight: 700;
      color: #222;
      margin-bottom: 20px;
      padding-bottom: 12px;
      border-bottom: 3px solid #4285F4;
    }}

    .dashboard-grid {{
      display: flex;
      flex-direction: column;
      gap: 16px;
    }}

    .dashboard-row {{
      display: flex;
      flex-direction: row;
      gap: 16px;
      width: 100%;
    }}

    .dashboard-box {{
      background: #ffffff;
      border-radius: 10px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 12px;
      overflow: hidden;
      min-height: 480px;
    }}

    .dashboard-box--empty {{
      background: transparent;
      box-shadow: none;
      min-height: 0;
    }}

    .dashboard-box-title {{
      font-size: 1rem;
      font-weight: 600;
      color: #333;
      border-bottom: 2px solid #f0f2f5;
      padding-bottom: 8px;
      flex-shrink: 0;
    }}

    .dashboard-box-content {{
      flex: 1;
      height: 420px;
      overflow: hidden;
      position: relative;
    }}

    .dashboard-box-content canvas {{
      display: block;
      width: 100% !important;
      height: 100% !important;
      max-width: 100%;
    }}

    .dashboard-box-content iframe,
    .dashboard-box-content div[id^=""chart""] {{
      width: 100% !important;
      height: 100% !important;
    }}

    .dashboard-box-content table {{
      width: 100%;
      border-collapse: collapse;
      font-size: 0.875rem;
    }}

    .dashboard-box-content th {{
      background: #f0f2f5;
      padding: 8px 12px;
      text-align: left;
      font-weight: 600;
      color: #444;
      border-bottom: 2px solid #ddd;
    }}

    .dashboard-box-content td {{
      padding: 7px 12px;
      border-bottom: 1px solid #eee;
      color: #555;
    }}

    .dashboard-box-content tr:last-child td {{
      border-bottom: none;
    }}

    .dashboard-box-content tr:hover td {{
      background: #f9f9f9;
    }}
  </style>
</head>
<body>
  <div class=""dashboard-grid"">
    {titleHtml}
{rowsHtml}  </div>
</body>
</html>";
}

    // ──────────────────────────────────────────
    // Private helpers
    // ──────────────────────────────────────────

    /// <summary>
    /// Computes (rows, columns) from a layout preset and cell count.
    /// </summary>
    private static (int rows, int cols) ResolveGrid(DashboardLayout layout, int cellCount)
    {
        return layout switch
        {
            // Everything in one row
            DashboardLayout.Horizontal => (1, cellCount),

            // Everything in one column
            DashboardLayout.Vertical => (cellCount, 1),


            // Closest to a square grid
            DashboardLayout.Automatic => AutoGrid(cellCount),

            // Wide | Narrow | Wide — 3-column grid with defined proportions
            DashboardLayout.RectangleSquareRectangle =>
                ((int)(Math.Ceiling(cellCount / 3.0)), 3),

            _ => AutoGrid(cellCount)
        };
    }

    /// <summary>
    /// Picks the most square-like (rows × cols) grid that fits all cells.
    /// </summary>
    private static (int rows, int cols) AutoGrid(int cellCount)
    {
        int cols = (int)Math.Ceiling(Math.Sqrt(cellCount));
        int rows = (int)Math.Ceiling((double)cellCount / cols);
        return (rows, cols);
    }
}
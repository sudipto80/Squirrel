using System.Text;
using Squirrel.Data_Visualization;

namespace Squirrel.DataVisualization;

public static class TailwindTableCreator
{
  /// <summary>
  /// A generic helper row 
  /// </summary>
  /// <param name="table"></param>
  /// <param name="headerRowClass"></param>
  /// <returns></returns>
  private static string GetHeaders(this Table table, string headerRowClass)
  {
    return table.ColumnHeaders
      .Select(t => $"<th class=\"{headerRowClass}\">{t}</th>")
      .Aggregate((f, s) => f + Environment.NewLine + s);
  }

  private static string GetRows(this Table table,  string[] trClasses, string[] tdClasses  )
  {
    StringBuilder sb = new StringBuilder();
    string rowClass = string.Empty;
    for (int i = 0; i < table.Rows.Count; i++)
    {
      var row = table.Rows[i];
      if (trClasses.Length  == 0)
      {
        sb.AppendLine($"<tr>");
      }
      else
      {
       
        if (trClasses.Length == 1)
        {
          rowClass = trClasses[0];
        }
        else
        {
          rowClass = trClasses[i % trClasses.Length];
        }

        sb.AppendLine($"<tr class=\"{rowClass}\">");
      }


      int rowIndex = 0;
      foreach (var cell in row)
      {
          var tdRowClass = tdClasses.Length == 0 ? string.Empty : tdClasses[rowIndex % tdClasses.Length];
          rowIndex++;
          if (tdRowClass == string.Empty)
          {
            sb.AppendLine($"<td>{cell.Value}</td>");
          }
          else
          {
            sb.AppendLine($"<td class=\"{tdRowClass}\">{cell.Value}</td>");
          } // sb.AppendLine($"<td class=\"px-3 py-2 text-stone-800\">{cell.Value}</td>");

      }

      sb.AppendLine("</tr>");
    }

    return sb.ToString();
  }


  public static string ToStripedColorPurpleHeader(this Table table)
    {
     

      /*
       *  <th class="text-left font-medium text-white bg-violet-700 px-3 py-2">Name</th>
          <th class="text-left font-medium text-white bg-violet-700 px-3 py-2">Role</th>
          <th class="text-left font-medium text-white bg-violet-700 px-3 py-2">Score</th>
          <th class="text-left font-medium text-white bg-violet-700 px-3 py-2">Grade</th>
       */
      string tableTemplate = """
                                                                  
                                                                   <html lang="en">
                                                                     <head>
                                                                     <meta charset="UTF-8">
                                                                     <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                                                     <title>!TITLE!</title>
                                                                     <script src="https://cdn.tailwindcss.com"></script>
                                                                     </head>
                                                                     <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                                                                       <div class="w-full max-w-2xl overflow-hidden rounded-lg">
                                                                         <table class="w-full text-sm border-collapse">
                                                                           <thead>
                                                                             <tr>
                                                                              !HEADERS!
                                                                             </tr>
                                                                           </thead>
                                                                           <tbody>
                                                                             !TABLE_ROWS!
                                                                         </tbody>
                                                                         </table>
                                                                       </div>
                                                                     </body>
                                                                     </html>
                             """;
      
        return tableTemplate.Replace("!HEADERS!", table.GetHeaders("text-left font-medium text-white bg-violet-700 px-3 py-2"))
          .Replace("!TABLE_ROWS!", table.GetRows(trClasses : ["bg-violet-50", "bg-white"], tdClasses:[]))
          .Replace("!TITLE!", "Table Visualization by Squirrel");
      
    }

  public static string ToFullGridTable(this Table table)
  {
       string tableTemplate = """
                                                                  
                                                                 <!DOCTYPE html>
                             <html lang="en">
                             <head>
                             <meta charset="UTF-8">
                             <meta name="viewport" content="width=device-width, initial-scale=1.0">
                             <title>!TITLE!</title>
                             <script src="https://cdn.tailwindcss.com"></script>
                             </head>
                             <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                               <div class="w-full max-w-2xl">
                                 <table class="w-full text-sm border-collapse">
                                   <thead>
                                     <tr>
                                     !HEADERS!
                                     </tr>
                                   </thead>
                                   <tbody>
                                   !TABLE_ROWS!
                                   </tbody>
                                 </table>
                               </div>
                             </body>
                             </html>
                             """;
      
        return tableTemplate.Replace("!HEADERS!", table.GetHeaders("text-left font-medium text-stone-800 bg-stone-100 border border-stone-200 px-3 py-2"))
          .Replace("!TABLE_ROWS!", table.GetRows(trClasses:[], tdClasses : ["border border-stone-200 px-3 py-2 text-stone-800"]))
          .Replace("!TITLE!", "Table Visualization by Squirrel");
  }

  public static string ToPlainHtmlTable(this Table table)
  {
    string tableTemplate = """

                           <!DOCTYPE html>
                           <html lang="en">
                           <head>
                           <meta charset="UTF-8">
                           <meta name="viewport" content="width=device-width, initial-scale=1.0">
                           <title>Table 01 — Plain / No Borders</title>
                           <script src="https://cdn.tailwindcss.com"></script>
                           </head>
                           <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                             <div class="bg-white p-4 rounded-lg w-full max-w-2xl">
                               <table class="w-full text-sm border-collapse">
                                 <thead>
                                   <tr>
                                     !HEADERS!
                                 </thead>
                                 <tbody>
                                  !TABLE_ROWS!
                                 </tbody>
                               </table>
                             </div>
                           </body>
                           </html>
                           """;
    
    return tableTemplate
      .Replace("!HEADERS!", table.GetHeaders("text-left text-[11px] uppercase tracking-wider text-stone-400 font-normal border-b border-stone-200 px-3 py-2"))
      .Replace("!TABLE_ROWS!", table.GetRows( trClasses:[], tdClasses : ["px-3 py-2 text-stone-800"]))
      .Replace("!TITLE!", "Table Visualization by Squirrel");
  }

  public static string ToHoverTailwindTable(this Table table)
  {
    string tableTemplate = """
                           <!DOCTYPE html>
                           <html lang="en">
                           <head>
                           <meta charset="UTF-8">
                           <meta name="viewport" content="width=device-width, initial-scale=1.0">
                           <title>Table 06 — Hover Rows</title>
                           <script src="https://cdn.tailwindcss.com"></script>
                           </head>
                           <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                             <div class="bg-white w-full max-w-2xl rounded-lg overflow-hidden">
                               <table class="w-full text-sm border-collapse">
                                 <thead>
                                   <tr>
                                   !HEADERS!
                                   </tr>
                                 </thead>
                                 <tbody>
                                  !TABLE_ROWS!
                                 </tbody>
                               </table>
                             </div>
                           </body>
                           </html>

                           """;
    
    return tableTemplate
      .Replace("!HEADERS!", table.GetHeaders("text-left text-[11px] uppercase tracking-wider text-stone-400 font-medium border-b border-stone-300 px-3 py-2"))
      .Replace("!TABLE_ROWS!", table.GetRows(trClasses:["border-b border-stone-200 hover:bg-stone-100 transition-colors duration-100 cursor-pointer"], tdClasses:["px-3 py-2 text-stone-800"]))
      .Replace("!TITLE!", "Table Visualization by Squirrel");
  }

  public static string ToNumberedRowsTable(this Table table)
  {
    string tableTemplate = """
                           <!DOCTYPE html>
                           <html lang="en">
                           <head>
                           <meta charset="UTF-8">
                           <meta name="viewport" content="width=device-width, initial-scale=1.0">
                           <title>Table 18 — Numbered Rows</title>
                           <script src="https://cdn.tailwindcss.com"></script>
                           </head>
                           <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                             <div class="bg-white w-full max-w-2xl rounded-lg overflow-hidden">
                               <table class="w-full text-sm border-collapse">
                                 <thead>
                                   <tr>
                                    !HEADERS!
                                   </tr>
                                 </thead>
                                 <tbody>
                                   !TABLE_ROWS!
                                 </tbody>
                               </table>
                             </div>
                           </body>
                           </html>
                           """;

    var headers =
      """<th class="text-left text-[11px] uppercase tracking-wider font-medium text-stone-400 border-b border-stone-300 px-3 py-2 w-8">#</th>""";
    headers += table.GetHeaders(
      "text-left text-[11px] uppercase tracking-wider font-medium text-stone-400 border-b border-stone-300 px-3 py-2");

    // Build rows manually so we can inject the 1-based row number as the first <td>
    var sb = new StringBuilder();
    for (int i = 0; i < table.Rows.Count; i++)
    {
      bool isLastRow = i == table.Rows.Count - 1;
      string trClass = isLastRow ? "" : "border-b border-stone-200";
      sb.AppendLine(trClass == "" ? "<tr>" : $"<tr class=\"{trClass}\">");

      // Row-number cell
      sb.AppendLine($"""<td class="px-3 py-2 text-stone-400 font-mono text-[11px]">{i + 1}</td>""");

      // Data cells
      foreach (var cell in table.Rows[i])
      {
        sb.AppendLine($"""<td class="px-3 py-2 text-stone-800">{cell.Value}</td>""");
      }

      sb.AppendLine("</tr>");
    }

    return tableTemplate
      .Replace("!HEADERS!", headers)
      .Replace("!TABLE_ROWS!", sb.ToString())
      .Replace("!TITLE!", "Table Visualization by Squirrel");
  }
  public static string ToCompactTable(this Table table)
  {
    string tableTemplate = """
                           <!DOCTYPE html>
                           <html lang="en">
                           <head>
                           <meta charset="UTF-8">
                           <meta name="viewport" content="width=device-width, initial-scale=1.0">
                           <title>Table 11 — Compact / Dense</title>
                           <script src="https://cdn.tailwindcss.com"></script>
                           </head>
                           <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                             <div class="bg-white w-full max-w-2xl rounded-lg overflow-hidden">
                               <table class="w-full border-collapse" style="font-size:11.5px;">
                                 <thead>
                                   <tr class="bg-stone-100 border-b border-stone-300">
                                    !HEADERS!
                                   </tr>
                                 </thead>
                                 <tbody>
                                   !TABLE_ROWS!
                                 </tbody>
                               </table>
                             </div>
                           </body>
                           </html>
                           """;

    return tableTemplate
      .Replace("!HEADERS!", table.GetHeaders("text-left font-medium text-stone-800 px-2.5 py-1"))
      .Replace("!TABLE_ROWS!", table.GetRows(trClasses: ["border-b border-stone-200"], tdClasses: ["px-2.5 py-1 text-stone-800"]))
      .Replace("!TITLE!", "Table Visualization by Squirrel");
  }

  private static string _toHighlightedColumnTable(this Table table, int highlightColumnIndex = 0)
  {
    string tableTemplate = """
                           <!DOCTYPE html>
                           <html lang="en">
                           <head>
                           <meta charset="UTF-8">
                           <meta name="viewport" content="width=device-width, initial-scale=1.0">
                           <title>Table 17 — Highlighted Column</title>
                           <script src="https://cdn.tailwindcss.com"></script>
                           </head>
                           <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                             <div class="bg-white w-full max-w-2xl rounded-lg overflow-hidden">
                               <table class="w-full text-sm border-collapse">
                                 <thead>
                                   <tr>
                                    !HEADERS!
                                   </tr>
                                 </thead>
                                 <tbody>
                                   !TABLE_ROWS!
                                 </tbody>
                               </table>
                             </div>
                           </body>
                           </html>
                           """;

    // Build headers — highlighted column gets blue header, rest get standard
    var headerSb = new StringBuilder();
    for (int i = 0; i < table.ColumnHeaders.Count; i++)
    {
      string thClass = i == highlightColumnIndex
        ? "text-left font-medium text-white bg-blue-700 border-b border-stone-300 px-3 py-2"
        : "text-left font-medium text-stone-800 border-b border-stone-300 px-3 py-2";
      headerSb.AppendLine($"<th class=\"{thClass}\">{table.ColumnHeaders.ElementAt(i)}</th>");
    }

    // Build rows — highlighted column gets blue cell, rest get standard
    var rowSb = new StringBuilder();
    for (int i = 0; i < table.Rows.Count; i++)
    {
      bool isLastRow = i == table.Rows.Count - 1;
      string trClass = isLastRow ? "" : "border-b border-stone-200";
      rowSb.AppendLine(trClass == "" ? "<tr>" : $"<tr class=\"{trClass}\">");

      var row = table.Rows[i];
      for (int j = 0; j < row.Count; j++)
      {
        string tdClass = j == highlightColumnIndex
          ? "px-3 py-2 font-medium text-blue-900 bg-blue-50"
          : "px-3 py-2 text-stone-800";
        rowSb.AppendLine($"<td class=\"{tdClass}\">{row.ElementAt(j).Value}</td>");
      }

      rowSb.AppendLine("</tr>");
    }

    return tableTemplate
      .Replace("!HEADERS!", headerSb.ToString())
      .Replace("!TABLE_ROWS!", rowSb.ToString())
      .Replace("!TITLE!", "Table Visualization by Squirrel");
  }
  
  public static string ToSemanticRowsTable(this Table table,
    params Func<IReadOnlyList<string>, string>[] rowColorResolvers)
{
    string tableTemplate = """
                           <!DOCTYPE html>
                           <html lang="en">
                           <head>
                           <meta charset="UTF-8">
                           <meta name="viewport" content="width=device-width, initial-scale=1.0">
                           <title>Table 10 — Semantic / Colored Rows</title>
                           <script src="https://cdn.tailwindcss.com"></script>
                           </head>
                           <body class="bg-stone-100 min-h-screen flex items-center justify-center p-8">
                             <div class="bg-white w-full max-w-2xl rounded-lg overflow-hidden">
                               <table class="w-full text-sm border-collapse">
                                 <thead>
                                   <tr>
                                    !HEADERS!
                                   </tr>
                                 </thead>
                                 <tbody>
                                   !TABLE_ROWS!
                                 </tbody>
                               </table>
                             </div>
                           </body>
                           </html>
                           """;

    var rowSb = new StringBuilder();
    foreach (var row in table.Rows)
    {
        // Extract cell values as strings for the resolvers
        var values = row.Select(c => c.Value).ToList().AsReadOnly();

        // Try each resolver in order, first non-null/empty result wins
        string color = rowColorResolvers
            .Select(resolver => resolver(values))
            .FirstOrDefault(c => !string.IsNullOrEmpty(c)) ?? "stone";

        bool isFirstCell = true;
        rowSb.AppendLine($"<tr class=\"bg-{color}-50\">");
        foreach (var cell in row)
        {
            if (isFirstCell)
            {
                rowSb.AppendLine($"<td class=\"px-3 py-2 text-{color}-900\">" +
                                 $"<span class=\"inline-block w-2 h-2 rounded-full bg-{color}-600 mr-2\"></span>" +
                                 $"{cell.Value}</td>");
                isFirstCell = false;
            }
            else
            {
                rowSb.AppendLine($"<td class=\"px-3 py-2 text-{color}-900\">{cell.Value}</td>");
            }
        }
        rowSb.AppendLine("</tr>");
    }

    return tableTemplate
        .Replace("!HEADERS!", table.GetHeaders("text-left text-[11px] uppercase tracking-wider font-medium text-stone-400 border-b border-stone-300 px-3 py-2"))
        .Replace("!TABLE_ROWS!", rowSb.ToString())
        .Replace("!TITLE!", "Table Visualization by Squirrel");
}

  public static string ToHighlightedColumnTable(this Table table, int columnIndexToHighlight = 0)
  {
    return table._toHighlightedColumnTable(highlightColumnIndex: columnIndexToHighlight);
  }
  public static string ToTailwindTable(this Table table, TailwindTableClass tableClass = TailwindTableClass.Plain)
  {
    return tableClass switch
    {
      TailwindTableClass.StripedPurple => table.ToStripedColorPurpleHeader(),
      TailwindTableClass.FullGrid => table.ToFullGridTable(),
      TailwindTableClass.Plain => table.ToPlainHtmlTable(),
      TailwindTableClass.NumberedRows => table.ToNumberedRowsTable(),
      TailwindTableClass.HoverRows => table.ToHoverTailwindTable(),
      TailwindTableClass.Compact => table.ToCompactTable(),
      TailwindTableClass.SemanticRows => table.ToSemanticRowsTable(),
      TailwindTableClass.HighlighitedColumn => throw new Exception("Use ToHighlightedColumnTable() method for this class since it requires a column index parameter"),
      _ => table.ToHtmlTable()
    };
  }
    
}
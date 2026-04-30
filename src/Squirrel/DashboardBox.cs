using System.Text.RegularExpressions;

namespace Squirrel;

public class DashboardBox
{
    public string Title { get; set; } = string.Empty;
    public string Html  { get; set; } = string.Empty;

    /// <summary>
    /// Optional: how many grid columns this box should span (default 1).
    /// </summary>
    public int ColSpan { get; set; } = 1;

    /// <summary>
    /// Renders this box as a self-contained div card.
    /// chartIndex is supplied by Dashboard.Render() to guarantee unique canvas IDs.
    /// </summary>
    internal string Render(string width = "100%", int chartIndex = 0)
    {
        string uniqueId  = $"chart_{chartIndex}";
        string fixedHtml = Html
            .Replace("id=\"chart\"",               $"id=\"{uniqueId}\"")
            .Replace("id='chart'",                 $"id='{uniqueId}'")
            .Replace("getElementById('chart')",    $"getElementById('{uniqueId}')")
            .Replace("getElementById(\"chart\")",  $"getElementById(\"{uniqueId}\")");

        fixedHtml = StripAndFix(fixedHtml);

        return $@"
    <div class=""dashboard-box"" style=""width:{width};"">
        <div class=""dashboard-box-title"">{Title}</div>
        <div class=""dashboard-box-content"">
            {fixedHtml}
        </div>
    </div>";
    }

    private static string StripAndFix(string html)
    {
        // Remove everything up to and including </head>
        int headEnd = html.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
        if (headEnd >= 0)
            html = html[(headEnd + "</head>".Length)..];

        // Remove <body>, </body>, </html>
        html = Regex.Replace(html, @"<body[^>]*>", "",  RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"</body\s*>",  "",  RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"</html\s*>",  "",  RegexOptions.IgnoreCase);

        // Remove fixed-size wrapper divs e.g. <div style="width: 700px; height: 450px;">
        // and their closing </div> immediately before a <script> block
        html = Regex.Replace(html, @"<div\s+style=""[^""]*\d+px[^""]*""[^>]*>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"</div>\s*(?=\s*<script)",                   "", RegexOptions.IgnoreCase);

        // Remove chart.js <script src> — loaded once in dashboard <head>
        html = Regex.Replace(html, @"<script[^>]*chart\.js[^>]*>\s*</script>", "", RegexOptions.IgnoreCase);

        // Wrap inline <script> blocks in an IIFE to prevent const redeclaration errors
        html = Regex.Replace(
            html,
            @"<script>([\s\S]*?)</script>",
            m => $"<script>(function(){{{m.Groups[1].Value}}})();</script>",
            RegexOptions.IgnoreCase);

        return html.Trim();
    }
}
namespace Squirrel;

public enum DashboardLayout
{
    /// <summary>All cells in a single row, side by side.</summary>
    Horizontal,
    /// <summary>All cells in a single column, stacked.</summary>
    Vertical,
    /// <summary>Cells added left-to-right, wrapping to next row automatically.</summary>
    KeepAdding,
    /// <summary>Tries to form the most square-like grid possible.</summary>
    Automatic,
    /// <summary>Alternating layout: wide | narrow | wide per row.</summary>
    RectangleSquareRectangle
}
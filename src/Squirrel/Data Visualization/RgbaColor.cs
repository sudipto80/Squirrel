namespace Squirrel.DataVisualization;

/// <summary>
/// Represents an RGBA color with red, green, blue, and alpha (opacity) channels
/// </summary>
public class RgbaColor
{
    /// <summary>
    /// Red channel (0-255)
    /// </summary>
    public int Red { get; set; }

    /// <summary>
    /// Green channel (0-255)
    /// </summary>
    public int Green { get; set; }

    /// <summary>
    /// Blue channel (0-255)
    /// </summary>
    public int Blue { get; set; }

    /// <summary>
    /// Alpha/opacity channel (0.0-1.0)
    /// </summary>
    public double Alpha { get; set; }

    public RgbaColor() { }

    public RgbaColor(int red, int green, int blue, double alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    /// <summary>
    /// Returns the CSS rgba() string representation
    /// </summary>
    public override string ToString()
    {
        return $"rgba({Red}, {Green}, {Blue}, {Alpha})";
    }

    /// <summary>
    /// Returns the CSS rgba() string representation with custom formatting
    /// </summary>
    /// <param name="alphaDecimals">Number of decimal places for alpha channel</param>
    public string ToString(int alphaDecimals)
    {
        var alphaFormatted = Math.Round(Alpha, alphaDecimals);
        return $"rgba({Red}, {Green}, {Blue}, {alphaFormatted})";
    }

    /// <summary>
    /// Returns a hexadecimal color representation (loses alpha information)
    /// </summary>
    public string ToHex()
    {
        return $"#{Red:X2}{Green:X2}{Blue:X2}";
    }

    /// <summary>
    /// Returns an 8-digit hexadecimal color representation including alpha
    /// </summary>
    public string ToHexWithAlpha()
    {
        byte alphaByte = (byte)(Alpha * 255);
        return $"#{Red:X2}{Green:X2}{Blue:X2}{alphaByte:X2}";
    }

    /// <summary>
    /// Returns the RGB representation (without alpha)
    /// </summary>
    public string ToRgb()
    {
        return $"rgb({Red}, {Green}, {Blue})";
    }

    /// <summary>
    /// Parses an rgba string like "rgba(54, 162, 235, 0.5)"
    /// </summary>
    public static RgbaColor Parse(string rgbaString)
    {
        var cleaned = rgbaString.Replace("rgba(", "").Replace(")", "").Replace(" ", "");
        var parts = cleaned.Split(',');
        
        return new RgbaColor
        {
            
            Red = byte.Parse(parts[0]),
            Green = byte.Parse(parts[1]),
            Blue = byte.Parse(parts[2]),
            Alpha = double.Parse(parts[3])
        };
    }
      /// <summary>
    /// Creates an RgbaColor from a known color name (e.g., "red", "blue", "pink")
    /// </summary>
    /// <param name="colorName">The name of the color</param>
    /// <param name="alpha">Optional alpha/opacity value (0.0-1.0), defaults to 1.0</param>
    /// <returns>RgbaColor object</returns>
    public static RgbaColor FromName(string colorName, double alpha = 1.0)
    {
        var color = System.Drawing.Color.FromName(colorName);
        
        if (!color.IsKnownColor && color.A == 0)
        {
            throw new ArgumentException($"Unknown color name: {colorName}");
        }

        return new RgbaColor(color.R, color.G, color.B, alpha);
    }

    /// <summary>
    /// Tries to create an RgbaColor from a known color name
    /// </summary>
    /// <param name="colorName">The name of the color</param>
    /// <param name="result">The resulting RgbaColor if successful</param>
    /// <param name="alpha">Optional alpha/opacity value (0.0-1.0), defaults to 1.0</param>
    /// <returns>True if the color name was recognized, false otherwise</returns>
    public static bool TryFromName(string colorName, out RgbaColor result, double alpha = 1.0)
    {
        try
        {
            var color = System.Drawing.Color.FromName(colorName);
            
            if (!color.IsKnownColor && color.A == 0)
            {
                result = null;
                return false;
            }

            result = new RgbaColor(color.R, color.G, color.B, alpha);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Parses various color formats: color names, rgba(), rgb(), hex, or hex with alpha
    /// </summary>
    /// <param name="colorString">String representation of the color</param>
    /// <param name="defaultAlpha">Default alpha value if not specified (0.0-1.0)</param>
    /// <returns>RgbaColor object</returns>
    public static RgbaColor ParseAny(string colorString, double defaultAlpha = 1.0)
    {
        colorString = colorString?.Trim();
        
        if (string.IsNullOrEmpty(colorString))
            throw new ArgumentException("Color string cannot be null or empty");

        // Try rgba format
        if (colorString.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase))
        {
            return Parse(colorString);
        }

        // Try rgb format
        if (colorString.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase))
        {
            var cleaned = colorString.Replace("rgb(", "").Replace(")", "").Replace(" ", "");
            var parts = cleaned.Split(',');
            return new RgbaColor(
                byte.Parse(parts[0]),
                byte.Parse(parts[1]),
                byte.Parse(parts[2]),
                defaultAlpha
            );
        }

        // Try hex format
        if (colorString.StartsWith("#"))
        {
            return ParseHex(colorString, defaultAlpha);
        }

        // Try as color name
        return FromName(colorString, defaultAlpha);
    }
    /// <summary>
    /// Parses a hexadecimal color string (#RGB, #RRGGBB, or #RRGGBBAA)
    /// </summary>
    private static RgbaColor ParseHex(string hexString, double defaultAlpha = 1.0)
    {
        hexString = hexString.TrimStart('#');

        if (hexString.Length == 3) // #RGB
        {
            return new RgbaColor(
                Convert.ToInt32(hexString[0].ToString() + hexString[0], 16),
                Convert.ToInt32(hexString[1].ToString() + hexString[1], 16),
                Convert.ToInt32(hexString[2].ToString() + hexString[2], 16),
                defaultAlpha
            );
        }
        else if (hexString.Length == 6) // #RRGGBB
        {
            return new RgbaColor(
                Convert.ToInt32(hexString.Substring(0, 2), 16),
                Convert.ToInt32(hexString.Substring(2, 2), 16),
                Convert.ToInt32(hexString.Substring(4, 2), 16),
                defaultAlpha
            );
        }
        else if (hexString.Length == 8) // #RRGGBBAA
        {
            return new RgbaColor(
                Convert.ToInt32(hexString.Substring(0, 2), 16),
                Convert.ToInt32(hexString.Substring(2, 2), 16),
                Convert.ToInt32(hexString.Substring(4, 2), 16),
                Convert.ToInt32(hexString.Substring(6, 2), 16) / 255.0
            );
        }

        throw new ArgumentException($"Invalid hex color format: #{hexString}");
    }
}
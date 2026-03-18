namespace Squirrel.DataVisualization;

public class ColorPicker
{
    public static RgbaColor[] GetColorsForScheme(ColorScheme scheme, int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        var baseColors = scheme switch
        {
            ColorScheme.Rainbow => GetRainbowColors(),
            ColorScheme.Pastel => GetPastelColors(),
            ColorScheme.Monochrome => GetMonochromeColors(),
            ColorScheme.Vibrant => GetVibrantColors(),
            ColorScheme.EarthTones => GetEarthTonesColors(),
            ColorScheme.CoolTones => GetCoolTonesColors(),
            ColorScheme.WarmTones => GetWarmTonesColors(),
            ColorScheme.Neon => GetNeonColors(),
            ColorScheme.Grayscale => GetGrayscaleColors(),
            ColorScheme.RainyDay => GetRainyDayColors(),
            ColorScheme.Sunset => GetSunsetColors(),
            ColorScheme.Ocean => GetOceanColors(),
            ColorScheme.Forest => GetForestColors(),
            ColorScheme.Desert => GetDesertColors(),
            ColorScheme.Candy => GetCandyColors(),
            ColorScheme.Metallic => GetMetallicColors(),
            ColorScheme.JewelTones => GetJewelTonesColors(),
            ColorScheme.Aquatic => GetAquaticColors(),
            ColorScheme.Fire => GetFireColors(),
            ColorScheme.Ice => GetIceColors(),
            ColorScheme.Spring => GetSpringColors(),
            ColorScheme.Autumn => GetAutumnColors(),
            ColorScheme.Winter => GetWinterColors(),
            ColorScheme.Summer => GetSummerColors(),
            ColorScheme.Retro => GetRetroColors(),
            ColorScheme.Futuristic => GetFuturisticColors(),
            ColorScheme.Custom => GetVibrantColors(),
            _ => GetRainbowColors()
        };

        return DistributeColors(baseColors, count);
    }

// Fixed 10-color palettes
    private static RgbaColor[] GetRainbowColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FF0000"), // Red
            RgbaColor.ParseAny("#FF7F00"), // Orange
            RgbaColor.ParseAny("#FFFF00"), // Yellow
            RgbaColor.ParseAny("#7FFF00"), // Chartreuse
            RgbaColor.ParseAny("#00FF00"), // Green
            RgbaColor.ParseAny("#00FF7F"), // Spring Green
            RgbaColor.ParseAny("#00FFFF"), // Cyan
            RgbaColor.ParseAny("#007FFF"), // Azure
            RgbaColor.ParseAny("#0000FF"), // Blue
            RgbaColor.ParseAny("#7F00FF") // Violet
        };
    }

    private static RgbaColor[] GetPastelColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FFB3BA"), // Pastel Pink
            RgbaColor.ParseAny("#FFDFBA"), // Pastel Peach
            RgbaColor.ParseAny("#FFFFBA"), // Pastel Yellow
            RgbaColor.ParseAny("#BAFFC9"), // Pastel Mint
            RgbaColor.ParseAny("#BAE1FF"), // Pastel Blue
            RgbaColor.ParseAny("#C9C9FF"), // Pastel Periwinkle
            RgbaColor.ParseAny("#FFBAF3"), // Pastel Magenta
            RgbaColor.ParseAny("#FFD4BA"), // Pastel Apricot
            RgbaColor.ParseAny("#E0BBE4"), // Pastel Lavender
            RgbaColor.ParseAny("#D4F1F4") // Pastel Aqua
        };
    }

    private static RgbaColor[] GetMonochromeColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#1a1a2e"), // Very Dark Blue
            RgbaColor.ParseAny("#16213e"), // Dark Blue
            RgbaColor.ParseAny("#0f3460"), // Navy Blue
            RgbaColor.ParseAny("#1e5f8b"), // Medium Dark Blue
            RgbaColor.ParseAny("#2e86ab"), // Steel Blue
            RgbaColor.ParseAny("#3da5d9"), // Medium Blue
            RgbaColor.ParseAny("#64b6d9"), // Light Blue
            RgbaColor.ParseAny("#89cff0"), // Sky Blue
            RgbaColor.ParseAny("#a7d7f0"), // Pale Blue
            RgbaColor.ParseAny("#d4e7f5") // Very Light Blue
        };
    }

    private static RgbaColor[] GetVibrantColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FF1744"), // Vibrant Red
            RgbaColor.ParseAny("#FF6F00"), // Vibrant Orange
            RgbaColor.ParseAny("#FFD600"), // Vibrant Yellow
            RgbaColor.ParseAny("#76FF03"), // Vibrant Lime
            RgbaColor.ParseAny("#00E676"), // Vibrant Green
            RgbaColor.ParseAny("#00E5FF"), // Vibrant Cyan
            RgbaColor.ParseAny("#2979FF"), // Vibrant Blue
            RgbaColor.ParseAny("#651FFF"), // Vibrant Purple
            RgbaColor.ParseAny("#D500F9"), // Vibrant Magenta
            RgbaColor.ParseAny("#FF4081") // Vibrant Pink
        };
    }

    private static RgbaColor[] GetEarthTonesColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#3E2723"), // Dark Chocolate
            RgbaColor.ParseAny("#5D4037"), // Brown
            RgbaColor.ParseAny("#6D4C41"), // Mocha
            RgbaColor.ParseAny("#8D6E63"), // Light Brown
            RgbaColor.ParseAny("#A1887F"), // Tan
            RgbaColor.ParseAny("#827717"), // Olive
            RgbaColor.ParseAny("#9E9D24"), // Moss
            RgbaColor.ParseAny("#AFB42B"), // Lime Green
            RgbaColor.ParseAny("#C0A04C"), // Wheat
            RgbaColor.ParseAny("#D4AF37") // Gold
        };
    }

    private static RgbaColor[] GetCoolTonesColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#0D47A1"), // Deep Blue
            RgbaColor.ParseAny("#1565C0"), // Navy
            RgbaColor.ParseAny("#1976D2"), // Blue
            RgbaColor.ParseAny("#0288D1"), // Light Blue
            RgbaColor.ParseAny("#0097A7"), // Cyan
            RgbaColor.ParseAny("#00897B"), // Teal
            RgbaColor.ParseAny("#00695C"), // Dark Teal
            RgbaColor.ParseAny("#4A148C"), // Deep Purple
            RgbaColor.ParseAny("#6A1B9A"), // Purple
            RgbaColor.ParseAny("#7B1FA2") // Violet
        };
    }

    private static RgbaColor[] GetWarmTonesColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#B71C1C"), // Dark Red
            RgbaColor.ParseAny("#C62828"), // Red
            RgbaColor.ParseAny("#D32F2F"), // Crimson
            RgbaColor.ParseAny("#E64A19"), // Red-Orange
            RgbaColor.ParseAny("#F4511E"), // Orange-Red
            RgbaColor.ParseAny("#FF6F00"), // Orange
            RgbaColor.ParseAny("#FF8F00"), // Amber
            RgbaColor.ParseAny("#FFA000"), // Gold
            RgbaColor.ParseAny("#FFB300"), // Yellow-Orange
            RgbaColor.ParseAny("#FFC107") // Yellow
        };
    }

    private static RgbaColor[] GetNeonColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FF0080"), // Neon Pink
            RgbaColor.ParseAny("#FF00FF"), // Neon Magenta
            RgbaColor.ParseAny("#8000FF"), // Neon Purple
            RgbaColor.ParseAny("#0080FF"), // Neon Blue
            RgbaColor.ParseAny("#00FFFF"), // Neon Cyan
            RgbaColor.ParseAny("#00FF80"), // Neon Mint
            RgbaColor.ParseAny("#80FF00"), // Neon Lime
            RgbaColor.ParseAny("#FFFF00"), // Neon Yellow
            RgbaColor.ParseAny("#FF8000"), // Neon Orange
            RgbaColor.ParseAny("#FF0000") // Neon Red
        };
    }

    private static RgbaColor[] GetGrayscaleColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#000000"), // Black
            RgbaColor.ParseAny("#1C1C1C"), // Almost Black
            RgbaColor.ParseAny("#383838"), // Dark Gray
            RgbaColor.ParseAny("#555555"), // Medium Dark Gray
            RgbaColor.ParseAny("#717171"), // Gray
            RgbaColor.ParseAny("#8D8D8D"), // Medium Gray
            RgbaColor.ParseAny("#A9A9A9"), // Light Gray
            RgbaColor.ParseAny("#C5C5C5"), // Silver
            RgbaColor.ParseAny("#E1E1E1"), // Very Light Gray
            RgbaColor.ParseAny("#FFFFFF") // White
        };
    }

    private static RgbaColor[] GetRainyDayColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#2F4F4F"), // Dark Slate Gray
            RgbaColor.ParseAny("#455A64"), // Blue Gray
            RgbaColor.ParseAny("#546E7A"), // Slate
            RgbaColor.ParseAny("#607D8B"), // Gray Blue
            RgbaColor.ParseAny("#78909C"), // Light Slate
            RgbaColor.ParseAny("#90A4AE"), // Silver Blue
            RgbaColor.ParseAny("#B0BEC5"), // Misty Blue
            RgbaColor.ParseAny("#CFD8DC"), // Pale Gray
            RgbaColor.ParseAny("#ECEFF1"), // Cloud White
            RgbaColor.ParseAny("#F5F5F5") // Off White
        };
    }

    private static RgbaColor[] GetSunsetColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#1A1A2E"), // Deep Night
            RgbaColor.ParseAny("#4A148C"), // Deep Purple
            RgbaColor.ParseAny("#6A1B9A"), // Purple
            RgbaColor.ParseAny("#880E4F"), // Magenta
            RgbaColor.ParseAny("#C62828"), // Deep Red
            RgbaColor.ParseAny("#D84315"), // Red-Orange
            RgbaColor.ParseAny("#EF6C00"), // Orange
            RgbaColor.ParseAny("#F57C00"), // Light Orange
            RgbaColor.ParseAny("#FFA726"), // Peach
            RgbaColor.ParseAny("#FFD54F") // Golden
        };
    }

    private static RgbaColor[] GetOceanColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#001F3F"), // Deep Ocean
            RgbaColor.ParseAny("#003D5C"), // Dark Blue
            RgbaColor.ParseAny("#005B7F"), // Navy Blue
            RgbaColor.ParseAny("#0074A2"), // Ocean Blue
            RgbaColor.ParseAny("#008DC5"), // Medium Blue
            RgbaColor.ParseAny("#00A6E8"), // Sky Blue
            RgbaColor.ParseAny("#33B8EB"), // Light Blue
            RgbaColor.ParseAny("#66CAEE"), // Aqua Blue
            RgbaColor.ParseAny("#99DCF1"), // Pale Aqua
            RgbaColor.ParseAny("#CCEEF4") // Foam
        };
    }

    private static RgbaColor[] GetForestColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#1B5E20"), // Dark Forest
            RgbaColor.ParseAny("#2E7D32"), // Forest Green
            RgbaColor.ParseAny("#388E3C"), // Green
            RgbaColor.ParseAny("#43A047"), // Medium Green
            RgbaColor.ParseAny("#4CAF50"), // Light Green
            RgbaColor.ParseAny("#66BB6A"), // Grass
            RgbaColor.ParseAny("#81C784"), // Sage
            RgbaColor.ParseAny("#558B2F"), // Olive Green
            RgbaColor.ParseAny("#689F38"), // Lime Green
            RgbaColor.ParseAny("#7CB342") // Yellow Green
        };
    }

    private static RgbaColor[] GetDesertColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#3E2723"), // Dark Brown
            RgbaColor.ParseAny("#5D4037"), // Brown
            RgbaColor.ParseAny("#795548"), // Mocha
            RgbaColor.ParseAny("#8D6E63"), // Tan
            RgbaColor.ParseAny("#A1887F"), // Light Tan
            RgbaColor.ParseAny("#BCAAA4"), // Sand
            RgbaColor.ParseAny("#D7CCC8"), // Light Sand
            RgbaColor.ParseAny("#F57F17"), // Gold
            RgbaColor.ParseAny("#F9A825"), // Yellow
            RgbaColor.ParseAny("#FBC02D") // Light Yellow
        };
    }

    private static RgbaColor[] GetCandyColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FFB3E6"), // Cotton Candy Pink
            RgbaColor.ParseAny("#FFD9E6"), // Light Pink
            RgbaColor.ParseAny("#FFF0B3"), // Lemon
            RgbaColor.ParseAny("#E6FFB3"), // Lime Candy
            RgbaColor.ParseAny("#B3FFD9"), // Mint
            RgbaColor.ParseAny("#B3F0FF"), // Blue Raspberry
            RgbaColor.ParseAny("#D9B3FF"), // Grape
            RgbaColor.ParseAny("#FFB3D9"), // Bubblegum
            RgbaColor.ParseAny("#FFD9B3"), // Peach
            RgbaColor.ParseAny("#F0B3FF") // Lavender Candy
        };
    }

    private static RgbaColor[] GetMetallicColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#C0C0C0"), // Silver
            RgbaColor.ParseAny("#D3D3D3"), // Light Silver
            RgbaColor.ParseAny("#A8A8A8"), // Dark Silver
            RgbaColor.ParseAny("#FFD700"), // Gold
            RgbaColor.ParseAny("#DAA520"), // Goldenrod
            RgbaColor.ParseAny("#B87333"), // Copper
            RgbaColor.ParseAny("#CD7F32"), // Bronze
            RgbaColor.ParseAny("#708090"), // Slate Gray
            RgbaColor.ParseAny("#778899"), // Light Slate
            RgbaColor.ParseAny("#696969") // Dim Gray
        };
    }

    private static RgbaColor[] GetJewelTonesColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#9B111E"), // Ruby
            RgbaColor.ParseAny("#CC338B"), // Pink Sapphire
            RgbaColor.ParseAny("#E0115F"), // Rose
            RgbaColor.ParseAny("#50C878"), // Emerald
            RgbaColor.ParseAny("#0F52BA"), // Sapphire
            RgbaColor.ParseAny("#9966CC"), // Amethyst
            RgbaColor.ParseAny("#FC8EAC"), // Rose Quartz
            RgbaColor.ParseAny("#FFA000"), // Amber
            RgbaColor.ParseAny("#40E0D0"), // Turquoise
            RgbaColor.ParseAny("#002FA7") // Lapis
        };
    }

    private static RgbaColor[] GetAquaticColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#006D77"), // Deep Teal
            RgbaColor.ParseAny("#008080"), // Teal
            RgbaColor.ParseAny("#00A896"), // Turquoise
            RgbaColor.ParseAny("#00CED1"), // Dark Turquoise
            RgbaColor.ParseAny("#02C39A"), // Aqua Green
            RgbaColor.ParseAny("#40E0D0"), // Turquoise
            RgbaColor.ParseAny("#48D1CC"), // Medium Turquoise
            RgbaColor.ParseAny("#7FFFD4"), // Aquamarine
            RgbaColor.ParseAny("#AFEEEE"), // Pale Turquoise
            RgbaColor.ParseAny("#E0F2F1") // Mint Cream
        };
    }

    private static RgbaColor[] GetFireColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#4A0000"), // Deep Maroon
            RgbaColor.ParseAny("#8B0000"), // Dark Red
            RgbaColor.ParseAny("#B22222"), // Fire Brick
            RgbaColor.ParseAny("#DC143C"), // Crimson
            RgbaColor.ParseAny("#FF4500"), // Orange Red
            RgbaColor.ParseAny("#FF6347"), // Tomato
            RgbaColor.ParseAny("#FF7F50"), // Coral
            RgbaColor.ParseAny("#FFA500"), // Orange
            RgbaColor.ParseAny("#FFD700"), // Gold
            RgbaColor.ParseAny("#FFFF00") // Yellow
        };
    }

    private static RgbaColor[] GetIceColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#E0F7FA"), // Very Light Cyan
            RgbaColor.ParseAny("#B2EBF2"), // Light Cyan
            RgbaColor.ParseAny("#80DEEA"), // Pale Cyan
            RgbaColor.ParseAny("#4DD0E1"), // Aqua
            RgbaColor.ParseAny("#26C6DA"), // Bright Cyan
            RgbaColor.ParseAny("#ADD8E6"), // Light Blue
            RgbaColor.ParseAny("#B0E0E6"), // Powder Blue
            RgbaColor.ParseAny("#AFEEEE"), // Pale Turquoise
            RgbaColor.ParseAny("#E0FFFF"), // Light Cyan
            RgbaColor.ParseAny("#F0F8FF") // Alice Blue
        };
    }

    private static RgbaColor[] GetSpringColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FFC0CB"), // Pink
            RgbaColor.ParseAny("#FFB6C1"), // Light Pink
            RgbaColor.ParseAny("#FF69B4"), // Hot Pink
            RgbaColor.ParseAny("#98FB98"), // Pale Green
            RgbaColor.ParseAny("#90EE90"), // Light Green
            RgbaColor.ParseAny("#7CFC00"), // Lawn Green
            RgbaColor.ParseAny("#FFFACD"), // Lemon Chiffon
            RgbaColor.ParseAny("#E6E6FA"), // Lavender
            RgbaColor.ParseAny("#DDA0DD"), // Plum
            RgbaColor.ParseAny("#DA70D6") // Orchid
        };
    }

    private static RgbaColor[] GetAutumnColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#8B4513"), // Saddle Brown
            RgbaColor.ParseAny("#A0522D"), // Sienna
            RgbaColor.ParseAny("#D2691E"), // Chocolate
            RgbaColor.ParseAny("#CD853F"), // Peru
            RgbaColor.ParseAny("#DEB887"), // Burlywood
            RgbaColor.ParseAny("#FF8C00"), // Dark Orange
            RgbaColor.ParseAny("#FFA500"), // Orange
            RgbaColor.ParseAny("#DAA520"), // Goldenrod
            RgbaColor.ParseAny("#B8860B"), // Dark Goldenrod
            RgbaColor.ParseAny("#8B0000") // Dark Red
        };
    }

    private static RgbaColor[] GetWinterColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FFFFFF"), // White
            RgbaColor.ParseAny("#F0F8FF"), // Alice Blue
            RgbaColor.ParseAny("#F8F8FF"), // Ghost White
            RgbaColor.ParseAny("#E6F2FF"), // Ice Blue
            RgbaColor.ParseAny("#B0C4DE"), // Light Steel Blue
            RgbaColor.ParseAny("#4682B4"), // Steel Blue
            RgbaColor.ParseAny("#5F9EA0"), // Cadet Blue
            RgbaColor.ParseAny("#708090"), // Slate Gray
            RgbaColor.ParseAny("#778899"), // Light Slate Gray
            RgbaColor.ParseAny("#2F4F4F") // Dark Slate Gray
        };
    }

    private static RgbaColor[] GetSummerColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#FFD700"), // Gold
            RgbaColor.ParseAny("#FFFF00"), // Yellow
            RgbaColor.ParseAny("#87CEEB"), // Sky Blue
            RgbaColor.ParseAny("#00BFFF"), // Deep Sky Blue
            RgbaColor.ParseAny("#7FFF00"), // Chartreuse
            RgbaColor.ParseAny("#9ACD32"), // Yellow Green
            RgbaColor.ParseAny("#FF7F50"), // Coral
            RgbaColor.ParseAny("#FF6347"), // Tomato
            RgbaColor.ParseAny("#FFB6C1"), // Light Pink
            RgbaColor.ParseAny("#FFA07A") // Light Salmon
        };
    }

    private static RgbaColor[] GetRetroColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#E6B333"), // Mustard
            RgbaColor.ParseAny("#CC8800"), // Dark Mustard
            RgbaColor.ParseAny("#D2691E"), // Chocolate
            RgbaColor.ParseAny("#CD5C5C"), // Indian Red
            RgbaColor.ParseAny("#8B4513"), // Saddle Brown
            RgbaColor.ParseAny("#2F4F4F"), // Dark Slate Gray
            RgbaColor.ParseAny("#008080"), // Teal
            RgbaColor.ParseAny("#F4A460"), // Sandy Brown
            RgbaColor.ParseAny("#BC8F8F"), // Rosy Brown
            RgbaColor.ParseAny("#CD853F") // Peru
        };
    }

    private static RgbaColor[] GetFuturisticColors()
    {
        return new[]
        {
            RgbaColor.ParseAny("#00FFFF"), // Cyan
            RgbaColor.ParseAny("#FF00FF"), // Magenta
            RgbaColor.ParseAny("#00FF00"), // Lime
            RgbaColor.ParseAny("#9D00FF"), // Electric Purple
            RgbaColor.ParseAny("#FF1493"), // Deep Pink
            RgbaColor.ParseAny("#1E90FF"), // Dodger Blue
            RgbaColor.ParseAny("#7FFF00"), // Chartreuse
            RgbaColor.ParseAny("#FF4500"), // Orange Red
            RgbaColor.ParseAny("#DA70D6"), // Orchid
            RgbaColor.ParseAny("#00CED1") // Dark Turquoise
        };
    }

    /// <summary>
    /// Distributes colors from a base palette to match the requested count
    /// </summary>
    private static RgbaColor[] DistributeColors(RgbaColor[] baseColors, int count)
    {
        var colors = new RgbaColor[count];

        if (count <= baseColors.Length)
        {
            // If we need fewer colors than we have, evenly distribute them
            for (int i = 0; i < count; i++)
            {
                int index = (int)Math.Round((double)i * (baseColors.Length - 1) / Math.Max(1, count - 1));
                colors[i] = baseColors[index];
            }
        }
        else
        {
            // If we need more colors, interpolate between the base colors
            for (int i = 0; i < count; i++)
            {
                double position = (double)i * (baseColors.Length - 1) / (count - 1);
                int index = (int)Math.Floor(position);
                double fraction = position - index;

                if (index >= baseColors.Length - 1)
                {
                    colors[i] = baseColors[baseColors.Length - 1];
                }
                else if (fraction < 0.0001) // Very close to exact index
                {
                    colors[i] = baseColors[index];
                }
                else
                {
                    // Interpolate between two colors
                    colors[i] = InterpolateColors(baseColors[index], baseColors[index + 1], fraction);
                }
            }
        }

        return colors;
    }

    /// <summary>
    /// Interpolates between two RgbaColor objects
    /// </summary>
    private static RgbaColor InterpolateColors(RgbaColor color1, RgbaColor color2, double fraction)
    {
        int r = (int)(color1.Red + (color2.Red - color1.Red) * fraction);
        int g = (int)(color1.Green + (color2.Green - color1.Green) * fraction);
        int b = (int)(color1.Blue + (color2.Blue - color1.Blue) * fraction);
        double a = color1.Alpha + (color2.Alpha - color1.Alpha) * fraction;

        return new RgbaColor(
            Math.Clamp(r, 0, 255),
            Math.Clamp(g, 0, 255),
            Math.Clamp(b, 0, 255),
            Math.Clamp(a, 0.0, 1.0)
        );
    }
}
namespace Squirrel.DataVisualization;

public class DynamicColorPicker
{
    public static RgbaColor[] GetColorsForScheme(ColorScheme scheme, int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        return scheme switch
        {
            ColorScheme.Rainbow => GenerateRainbow(count),
            ColorScheme.Pastel => GeneratePastel(count),
            ColorScheme.Monochrome => GenerateMonochrome(count),
            ColorScheme.Vibrant => GenerateVibrant(count),
            ColorScheme.EarthTones => GenerateEarthTones(count),
            ColorScheme.CoolTones => GenerateCoolTones(count),
            ColorScheme.WarmTones => GenerateWarmTones(count),
            ColorScheme.Neon => GenerateNeon(count),
            ColorScheme.Grayscale => GenerateGrayscale(count),
            ColorScheme.RainyDay => GenerateRainyDay(count),
            ColorScheme.Sunset => GenerateSunset(count),
            ColorScheme.Ocean => GenerateOcean(count),
            ColorScheme.Forest => GenerateForest(count),
            ColorScheme.Desert => GenerateDesert(count),
            ColorScheme.Candy => GenerateCandy(count),
            ColorScheme.Metallic => GenerateMetallic(count),
            ColorScheme.JewelTones => GenerateJewelTones(count),
            ColorScheme.Aquatic => GenerateAquatic(count),
            ColorScheme.Fire => GenerateFire(count),
            ColorScheme.Ice => GenerateIce(count),
            ColorScheme.Spring => GenerateSpring(count),
            ColorScheme.Autumn => GenerateAutumn(count),
            ColorScheme.Winter => GenerateWinter(count),
            ColorScheme.Summer => GenerateSummer(count),
            ColorScheme.Retro => GenerateRetro(count),
            ColorScheme.Futuristic => GenerateFuturistic(count),
            ColorScheme.Custom => GenerateVibrant(count), // Default to vibrant for custom
            _ => GenerateRainbow(count)
        };
    }

    private static RgbaColor[] GenerateRainbow(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double hue = (double)i / count;
            colors[i] = HsvToRgba(hue, 0.8, 0.95);
        }

        return colors;
    }

    private static RgbaColor[] GeneratePastel(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double hue = (double)i / count;
            colors[i] = HsvToRgba(hue, 0.25, 0.95);
        }

        return colors;
    }

    private static RgbaColor[] GenerateMonochrome(int count)
    {
        var baseHue = 0.6; // Blue
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double value = 0.3 + (0.6 * i / Math.Max(1, count - 1));
            colors[i] = HsvToRgba(baseHue, 0.5, value);
        }

        return colors;
    }

    private static RgbaColor[] GenerateVibrant(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double hue = (double)i / count;
            colors[i] = HsvToRgba(hue, 0.9, 0.9);
        }

        return colors;
    }

    private static RgbaColor[] GenerateEarthTones(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.08, S = 0.6, V = 0.5 }, // Brown
            new { H = 0.1, S = 0.5, V = 0.4 }, // Dark brown
            new { H = 0.15, S = 0.4, V = 0.6 }, // Tan
            new { H = 0.25, S = 0.5, V = 0.4 }, // Olive
            new { H = 0.05, S = 0.7, V = 0.4 } // Rust
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateCoolTones(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double hue = 0.5 + (0.3 * i / Math.Max(1, count - 1)); // Blue to cyan range
            colors[i] = HsvToRgba(hue, 0.6, 0.8);
        }

        return colors;
    }

    private static RgbaColor[] GenerateWarmTones(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double hue = 0.0 + (0.15 * i / Math.Max(1, count - 1)); // Red to yellow range
            colors[i] = HsvToRgba(hue, 0.7, 0.9);
        }

        return colors;
    }

    private static RgbaColor[] GenerateNeon(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double hue = (double)i / count;
            colors[i] = HsvToRgba(hue, 1.0, 1.0);
        }

        return colors;
    }

    private static RgbaColor[] GenerateGrayscale(int count)
    {
        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            int value = (int)(255 * i / Math.Max(1, count - 1));
            colors[i] = new RgbaColor(value, value, value, 1.0);
        }

        return colors;
    }

    private static RgbaColor[] GenerateRainyDay(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.58, S = 0.3, V = 0.5 }, // Gray-blue
            new { H = 0.6, S = 0.4, V = 0.4 }, // Slate
            new { H = 0.55, S = 0.2, V = 0.6 }, // Light gray-blue
            new { H = 0.0, S = 0.0, V = 0.5 } // Gray
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateSunset(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.0, S = 0.8, V = 0.9 }, // Red
            new { H = 0.05, S = 0.9, V = 0.95 }, // Orange
            new { H = 0.1, S = 0.8, V = 1.0 }, // Yellow-orange
            new { H = 0.85, S = 0.6, V = 0.7 }, // Purple
            new { H = 0.75, S = 0.5, V = 0.5 } // Deep purple
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateOcean(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.55, S = 0.9, V = 0.3 }, // Deep blue
            new { H = 0.52, S = 0.7, V = 0.6 }, // Ocean blue
            new { H = 0.5, S = 0.5, V = 0.8 }, // Light blue
            new { H = 0.48, S = 0.3, V = 0.9 } // Aqua
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateForest(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.25, S = 0.7, V = 0.3 }, // Dark green
            new { H = 0.3, S = 0.6, V = 0.5 }, // Forest green
            new { H = 0.35, S = 0.5, V = 0.6 }, // Green
            new { H = 0.12, S = 0.5, V = 0.4 } // Brown-green
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateDesert(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.12, S = 0.6, V = 0.8 }, // Sand
            new { H = 0.08, S = 0.7, V = 0.6 }, // Terracotta
            new { H = 0.05, S = 0.5, V = 0.5 }, // Brown
            new { H = 0.15, S = 0.4, V = 0.9 } // Tan
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateCandy(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.95, S = 0.5, V = 1.0 }, // Pink
            new { H = 0.15, S = 0.4, V = 1.0 }, // Light yellow
            new { H = 0.5, S = 0.4, V = 1.0 }, // Light blue
            new { H = 0.3, S = 0.4, V = 1.0 }, // Mint
            new { H = 0.85, S = 0.4, V = 1.0 } // Lavender
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateMetallic(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.0, S = 0.0, V = 0.75 }, // Silver
            new { H = 0.1, S = 0.2, V = 0.6 }, // Gold
            new { H = 0.05, S = 0.3, V = 0.5 }, // Bronze
            new { H = 0.0, S = 0.0, V = 0.5 } // Steel
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateJewelTones(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.0, S = 0.9, V = 0.6 }, // Ruby
            new { H = 0.3, S = 0.9, V = 0.5 }, // Emerald
            new { H = 0.6, S = 0.9, V = 0.6 }, // Sapphire
            new { H = 0.85, S = 0.7, V = 0.5 }, // Amethyst
            new { H = 0.15, S = 0.8, V = 0.7 } // Topaz
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateAquatic(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.5, S = 0.8, V = 0.7 }, // Teal
            new { H = 0.52, S = 0.6, V = 0.8 }, // Turquoise
            new { H = 0.48, S = 0.5, V = 0.9 }, // Aqua
            new { H = 0.55, S = 0.7, V = 0.6 } // Sea green
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateFire(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.02, S = 1.0, V = 1.0 }, // Bright red
            new { H = 0.05, S = 1.0, V = 1.0 }, // Orange-red
            new { H = 0.08, S = 1.0, V = 1.0 }, // Orange
            new { H = 0.12, S = 0.9, V = 1.0 }, // Yellow-orange
            new { H = 0.15, S = 0.8, V = 1.0 } // Yellow
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateIce(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.55, S = 0.2, V = 1.0 }, // Very light blue
            new { H = 0.52, S = 0.3, V = 0.9 }, // Ice blue
            new { H = 0.5, S = 0.4, V = 0.85 }, // Cool blue
            new { H = 0.0, S = 0.0, V = 0.95 } // White-blue
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateSpring(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.95, S = 0.4, V = 1.0 }, // Pink
            new { H = 0.3, S = 0.5, V = 0.9 }, // Light green
            new { H = 0.15, S = 0.5, V = 1.0 }, // Yellow
            new { H = 0.85, S = 0.3, V = 0.95 } // Lavender
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateAutumn(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.05, S = 0.8, V = 0.8 }, // Orange
            new { H = 0.0, S = 0.7, V = 0.7 }, // Red
            new { H = 0.12, S = 0.7, V = 0.6 }, // Brown
            new { H = 0.08, S = 0.6, V = 0.5 } // Dark orange
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateWinter(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.0, S = 0.0, V = 0.95 }, // White
            new { H = 0.6, S = 0.3, V = 0.7 }, // Icy blue
            new { H = 0.0, S = 0.0, V = 0.6 }, // Gray
            new { H = 0.58, S = 0.4, V = 0.5 } // Dark blue-gray
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateSummer(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.15, S = 0.6, V = 1.0 }, // Sunny yellow
            new { H = 0.52, S = 0.5, V = 0.9 }, // Sky blue
            new { H = 0.3, S = 0.6, V = 0.8 }, // Grass green
            new { H = 0.0, S = 0.6, V = 0.9 } // Coral
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateRetro(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.08, S = 0.7, V = 0.7 }, // Mustard
            new { H = 0.05, S = 0.8, V = 0.6 }, // Burnt orange
            new { H = 0.12, S = 0.6, V = 0.5 }, // Brown
            new { H = 0.48, S = 0.5, V = 0.6 } // Teal
        };
        return InterpolateColors(baseColors, count);
    }

    private static RgbaColor[] GenerateFuturistic(int count)
    {
        var baseColors = new[]
        {
            new { H = 0.75, S = 0.8, V = 0.9 }, // Electric purple
            new { H = 0.5, S = 0.9, V = 1.0 }, // Cyan
            new { H = 0.85, S = 0.7, V = 1.0 }, // Magenta
            new { H = 0.35, S = 0.8, V = 0.9 } // Lime
        };
        return InterpolateColors(baseColors, count);
    }

// Helper method to convert HSV to RGBA
    private static RgbaColor HsvToRgba(double h, double s, double v)
    {
        int hi = (int)(h * 6) % 6;
        double f = h * 6 - Math.Floor(h * 6);

        double p = v * (1 - s);
        double q = v * (1 - f * s);
        double t = v * (1 - (1 - f) * s);

        double r, g, b;
        switch (hi)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            default:
                r = v;
                g = p;
                b = q;
                break;
        }

        return new RgbaColor(
            (int)(r * 255),
            (int)(g * 255),
            (int)(b * 255),
            1.0
        );
    }

// Helper method to interpolate between base colors
    private static RgbaColor[] InterpolateColors(dynamic[] baseColors, int count)
    {
        if (count == 1)
            return [HsvToRgba(baseColors[0].H, baseColors[0].S, baseColors[0].V)];

        var colors = new RgbaColor[count];
        for (int i = 0; i < count; i++)
        {
            double position = (double)i / (count - 1) * (baseColors.Length - 1);
            int index = (int)position;
            double fraction = position - index;

            if (index >= baseColors.Length - 1)
            {
                colors[i] = HsvToRgba(baseColors[baseColors.Length - 1].H,
                    baseColors[baseColors.Length - 1].S,
                    baseColors[baseColors.Length - 1].V);
            }
            else
            {
                double h = baseColors[index].H + (baseColors[index + 1].H - baseColors[index].H) * fraction;
                double s = baseColors[index].S + (baseColors[index + 1].S - baseColors[index].S) * fraction;
                double vVal = baseColors[index].V + (baseColors[index + 1].V - baseColors[index].V) * fraction;
                colors[i] = HsvToRgba(h, s, vVal);
            }
        }

        return colors;
    }
}
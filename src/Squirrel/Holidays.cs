using System.Text.Json;

public class Holidays
{
    private readonly List<DateTime> _fixed = new();
    private readonly List<Func<int, DateTime>> _rules = new();
    private readonly List<Func<int, DateTime>> _easterRelative = new();
    public string CalendarName { get; private set; } = "Default";
    public int Year { get; private set; }

    public static Holidays Load(string path)
    {
        var h = new Holidays();
        var json = JsonDocument.Parse(File.ReadAllText(path));
        var root = json.RootElement;

        h.CalendarName = root.GetProperty("calendar").GetString()!;
        h.Year = root.GetProperty("year").GetInt32();

        // Fixed dates
        if (root.TryGetProperty("fixed", out var fixed_))
            foreach (var item in fixed_.EnumerateArray())
                h._fixed.Add(DateTime.Parse(item.GetProperty("date").GetString()!));

        // Ordinal rules — "4th Thursday of November"
        if (root.TryGetProperty("rules", out var rules))
            foreach (var rule in rules.EnumerateArray())
            {
                int ordinal = rule.GetProperty("ordinal").GetInt32();
                var day     = Enum.Parse<DayOfWeek>(rule.GetProperty("day").GetString()!);
                int month   = rule.GetProperty("month").GetInt32();
                h._rules.Add(year => GetNthDayOfMonth(year, month, day, ordinal));
            }

        // Easter-relative offsets — "Good Friday = Easter - 2"
        if (root.TryGetProperty("easter_relative", out var easterRules))
            foreach (var rule in easterRules.EnumerateArray())
            {
                int offset = rule.GetProperty("offset").GetInt32();
                h._easterRelative.Add(year => GetEaster(year).AddDays(offset));
            }

        return h;
    }

    public bool IsHoliday(DateTime date) =>
        _fixed.Any(h => h.Date == date.Date)                          ||
        _rules.Any(r => r(date.Year).Date == date.Date)               ||
        _easterRelative.Any(r => r(date.Year).Date == date.Date);

    public bool IsTradingDay(DateTime date) =>
        date.DayOfWeek != DayOfWeek.Saturday &&
        date.DayOfWeek != DayOfWeek.Sunday   &&
        !IsHoliday(date);

    public List<DateTime> GetHolidaysForYear(int year)
    {
        var all = new List<DateTime>();
        all.AddRange(_fixed.Where(d => d.Year == year));
        all.AddRange(_rules.Select(r => r(year)));
        all.AddRange(_easterRelative.Select(r => r(year)));
        return all.OrderBy(d => d).ToList();
    }

    // Nth weekday of month — handles both positive and -1 (last)
    private static DateTime GetNthDayOfMonth(int year, int month, DayOfWeek dow, int n)
    {
        if (n > 0)
        {
            var first = new DateTime(year, month, 1);
            int daysUntil = ((int)dow - (int)first.DayOfWeek + 7) % 7;
            return first.AddDays(daysUntil + (n - 1) * 7);
        }
        else // last occurrence
        {
            var last = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            int daysBack = ((int)last.DayOfWeek - (int)dow + 7) % 7;
            return last.AddDays(-daysBack);
        }
    }

    // Meeus/Jones/Butcher algorithm
    private static DateTime GetEaster(int year)
    {
        int a = year % 19, b = year / 100, c = year % 100;
        int d = b / 4,  e = b % 4,  f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4,  k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day   = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateTime(year, month, day);
    }
}
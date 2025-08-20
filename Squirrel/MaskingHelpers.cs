using System.Text;
using System.Text.RegularExpressions;
using Squirrel;


public static class MaskingHelpers
{


    private static readonly Random _random = new Random();

    public static List<string> MaskValues(this List<string> values, MaskingStrategy strategy)
    {
        if (values == null || values.Count == 0)
            return values ?? new List<string>();

        List<string> maskedValues = new List<string>();

        switch (strategy)
        {
            // === Original Strategies ===
            case MaskingStrategy.StarExceptFirstAndLast:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2) return value;
                    return value[0] + new string('*', value.Length - 2) + value[^1];
                }));
                break;

            case MaskingStrategy.StarExceptFirstAndLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 5) return value;
                    return value[0] + new string('*', value.Length - 5) + value[^4..];
                }));
                break;

            case MaskingStrategy.StarExceptFirstFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4) return value;
                    return value[..4] + new string('*', value.Length - 4);
                }));
                break;

            case MaskingStrategy.StarExceptFirstTwoAndLastTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4) return value;
                    return value[..2] + new string('*', value.Length - 4) + value[^2..];
                }));
                break;

            case MaskingStrategy.StarExceptLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 4) return value;
                    return new string('*', value.Length - 4) + value[^4..];
                }));
                break;

            // === Complete Masking ===
            case MaskingStrategy.StarAll:
                maskedValues.AddRange(values.Select(value => new string('*', value.Length)));
                break;

            case MaskingStrategy.Redacted:
                maskedValues.AddRange(values.Select(_ => "[REDACTED]"));
                break;

            case MaskingStrategy.Hidden:
                maskedValues.AddRange(values.Select(_ => "[HIDDEN]"));
                break;

            case MaskingStrategy.Masked:
                maskedValues.AddRange(values.Select(_ => "[MASKED]"));
                break;

            // === Partial Display Strategies ===
            case MaskingStrategy.StarExceptFirst:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 1) return value;
                    return value[0] + new string('*', value.Length - 1);
                }));
                break;

            case MaskingStrategy.StarExceptFirstTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2) return value;
                    return value[..2] + new string('*', value.Length - 2);
                }));
                break;

            case MaskingStrategy.StarExceptFirstThree:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 3) return value;
                    return value[..3] + new string('*', value.Length - 3);
                }));
                break;

            case MaskingStrategy.StarExceptLast:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 1) return value;
                    return new string('*', value.Length - 1) + value[^1];
                }));
                break;

            case MaskingStrategy.StarExceptLastTwo:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 2) return value;
                    return new string('*', value.Length - 2) + value[^2..];
                }));
                break;

            case MaskingStrategy.StarExceptLastThree:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 3) return value;
                    return new string('*', value.Length - 3) + value[^3..];
                }));
                break;

            case MaskingStrategy.StarExceptFirstThreeAndLastThree:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 6) return value;
                    return value[..3] + new string('*', value.Length - 6) + value[^3..];
                }));
                break;

            case MaskingStrategy.StarExceptFirstFourAndLastFour:
                maskedValues.AddRange(values.Select(value =>
                {
                    if (value.Length <= 8) return value;
                    return value[..4] + new string('*', value.Length - 8) + value[^4..];
                }));
                break;

            // === Email-Specific Strategies ===
            case MaskingStrategy.EmailPartial:
                maskedValues.AddRange(values.Select(MaskEmailPartial));
                break;

            case MaskingStrategy.EmailDomainOnly:
                maskedValues.AddRange(values.Select(MaskEmailDomainOnly));
                break;

            case MaskingStrategy.EmailUsernameOnly:
                maskedValues.AddRange(values.Select(MaskEmailUsernameOnly));
                break;

            // === Phone Number Strategies ===
            case MaskingStrategy.PhoneAreaCodeAndLastFour:
                maskedValues.AddRange(values.Select(MaskPhoneAreaCodeAndLastFour));
                break;

            case MaskingStrategy.PhoneLastFour:
                maskedValues.AddRange(values.Select(MaskPhoneLastFour));
                break;

            case MaskingStrategy.PhoneAreaCodeOnly:
                maskedValues.AddRange(values.Select(MaskPhoneAreaCodeOnly));
                break;

            // === Credit Card Strategies ===
            case MaskingStrategy.CreditCardLastFour:
                maskedValues.AddRange(values.Select(MaskCreditCardLastFour));
                break;

            case MaskingStrategy.CreditCardFirstFourLastFour:
                maskedValues.AddRange(values.Select(MaskCreditCardFirstFourLastFour));
                break;

            case MaskingStrategy.CreditCardFirstSixLastFour:
                maskedValues.AddRange(values.Select(MaskCreditCardFirstSixLastFour));
                break;

            // === Custom Character Strategies ===
            case MaskingStrategy.XMask:
                maskedValues.AddRange(values.Select(value => new string('X', value.Length)));
                break;

            case MaskingStrategy.HashMask:
                maskedValues.AddRange(values.Select(value => new string('#', value.Length)));
                break;

            case MaskingStrategy.DashMask:
                maskedValues.AddRange(values.Select(value => new string('-', value.Length)));
                break;

            case MaskingStrategy.BulletMask:
                maskedValues.AddRange(values.Select(value => new string('•', value.Length)));
                break;

            // === Percentage-Based Strategies ===
            case MaskingStrategy.MaskMiddle25Percent:
                maskedValues.AddRange(values.Select(value => MaskMiddlePercent(value, 0.25f)));
                break;

            case MaskingStrategy.MaskMiddle50Percent:
                maskedValues.AddRange(values.Select(value => MaskMiddlePercent(value, 0.50f)));
                break;

            case MaskingStrategy.MaskMiddle75Percent:
                maskedValues.AddRange(values.Select(value => MaskMiddlePercent(value, 0.75f)));
                break;

            // === Pattern-Based Strategies ===
            case MaskingStrategy.ShowEveryThird:
                maskedValues.AddRange(values.Select(ShowEveryThird));
                break;

            case MaskingStrategy.ShowRandom30Percent:
                maskedValues.AddRange(values.Select(value => ShowRandomPercent(value, 0.30f)));
                break;

            case MaskingStrategy.AlternatingOddShow:
                maskedValues.AddRange(values.Select(AlternatingOddShow));
                break;

            case MaskingStrategy.AlternatingEvenShow:
                maskedValues.AddRange(values.Select(AlternatingEvenShow));
                break;

            // === Length-Preserving Strategies ===
            case MaskingStrategy.PreserveLengthStar:
                maskedValues.AddRange(values.Select(value => new string('*', value.Length)));
                break;

            case MaskingStrategy.PreserveLengthX:
                maskedValues.AddRange(values.Select(value => new string('X', value.Length)));
                break;

            // === Special Cases ===
            case MaskingStrategy.None:
                maskedValues.AddRange(values);
                break;

            case MaskingStrategy.TruncateWithEllipsis:
                maskedValues.AddRange(values.Select(value =>
                    value.Length > 10 ? value[..7] + "..." : value));
                break;

            case MaskingStrategy.ShowLengthOnly:
                maskedValues.AddRange(values.Select(value => $"[{value.Length} characters]"));
                break;

            case MaskingStrategy.ShowTypeOnly:
                maskedValues.AddRange(values.Select(GetDataType));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy,
                    $"Masking strategy '{strategy}' is not implemented.");
        }

        return maskedValues.Count == 0 ? values : maskedValues;
    }

    // === Helper Methods ===

    private static string MaskEmailPartial(string email)
    {
        if (!email.Contains('@')) return email;

        var parts = email.Split('@');
        var username = parts[0];
        var domain = parts[1];

        var maskedUsername = username.Length > 2
            ? username[..2] + new string('*', Math.Max(0, username.Length - 2))
            : username;

        var maskedDomain = domain.Length > 2
            ? domain[..2] + new string('*', Math.Max(0, domain.Length - 2))
            : domain;

        return $"{maskedUsername}@{maskedDomain}";
    }

    private static string MaskEmailDomainOnly(string email)
    {
        if (!email.Contains('@')) return email;
        var parts = email.Split('@');
        return new string('*', parts[0].Length) + "@" + parts[1];
    }

    private static string MaskEmailUsernameOnly(string email)
    {
        if (!email.Contains('@')) return email;
        var parts = email.Split('@');
        return parts[0] + "@" + new string('*', parts[1].Length);
    }

    private static string MaskPhoneAreaCodeAndLastFour(string phone)
    {
        // Extract digits only
        var digits = Regex.Replace(phone, @"[^\d]", "");
        if (digits.Length < 10) return phone;

        var areaCode = digits[..3];
        var lastFour = digits[^4..];
        return $"({areaCode}) ***-{lastFour}";
    }

    private static string MaskPhoneLastFour(string phone)
    {
        var digits = Regex.Replace(phone, @"[^\d]", "");
        if (digits.Length < 4) return phone;

        var lastFour = digits[^4..];
        return "***-***-" + lastFour;
    }

    private static string MaskPhoneAreaCodeOnly(string phone)
    {
        var digits = Regex.Replace(phone, @"[^\d]", "");
        if (digits.Length < 10) return phone;

        var areaCode = digits[..3];
        return $"({areaCode}) ***-****";
    }

    private static string MaskCreditCardLastFour(string cardNumber)
    {
        var digits = Regex.Replace(cardNumber, @"[^\d]", "");
        if (digits.Length < 4) return cardNumber;

        var lastFour = digits[^4..];
        return "**** **** **** " + lastFour;
    }

    private static string MaskCreditCardFirstFourLastFour(string cardNumber)
    {
        var digits = Regex.Replace(cardNumber, @"[^\d]", "");
        if (digits.Length < 8) return cardNumber;

        var firstFour = digits[..4];
        var lastFour = digits[^4..];
        return $"{firstFour} **** **** {lastFour}";
    }

    private static string MaskCreditCardFirstSixLastFour(string cardNumber)
    {
        var digits = Regex.Replace(cardNumber, @"[^\d]", "");
        if (digits.Length < 10) return cardNumber;

        var firstSix = digits[..6];
        var lastFour = digits[^4..];
        return $"{firstSix}** ****{lastFour}";
    }

    private static string MaskMiddlePercent(string value, float percentage)
    {
        if (value.Length <= 2) return value;

        int maskLength = Math.Max(1, (int)(value.Length * percentage));
        int startPos = (value.Length - maskLength) / 2;

        var sb = new StringBuilder(value);
        for (int i = startPos; i < startPos + maskLength && i < value.Length; i++)
        {
            sb[i] = '*';
        }

        return sb.ToString();
    }

    private static string ShowEveryThird(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append((i + 1) % 3 == 0 ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string ShowRandomPercent(string value, float percentage)
    {
        if (value.Length == 0) return value;

        int showCount = Math.Max(1, (int)(value.Length * percentage));
        var indicesToShow = Enumerable.Range(0, value.Length)
            .OrderBy(_ => _random.Next())
            .Take(showCount)
            .ToHashSet();

        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append(indicesToShow.Contains(i) ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string AlternatingOddShow(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append(i % 2 == 0 ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string AlternatingEvenShow(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            sb.Append(i % 2 == 1 ? value[i] : '*');
        }

        return sb.ToString();
    }

    private static string GetDataType(string value)
    {
        if (string.IsNullOrEmpty(value)) return "[EMPTY]";
        if (int.TryParse(value, out _)) return "[INTEGER]";
        if (decimal.TryParse(value, out _)) return "[DECIMAL]";
        if (DateTime.TryParse(value, out _)) return "[DATETIME]";
        if (value.Contains('@')) return "[EMAIL]";
        if (Regex.IsMatch(value, @"^\d{3}-?\d{3}-?\d{4}$")) return "[PHONE]";
        if (Regex.IsMatch(value, @"^\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}$")) return "[CREDITCARD]";
        return "[STRING]";
    }
}
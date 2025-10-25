namespace Squirrel;

public static class SymbolConstants
{
    /// <summary>
    /// Comprehensive array of currency symbols for major world currencies.
    /// Includes primary symbols, alternative symbols, and historical currencies still in use.
    /// Organized by frequency of global usage and economic significance.
    /// </summary>
    public static readonly char[] CurrencySymbols =
    [
        // Major global currencies (most common)
        '$', // US Dollar, Canadian Dollar, Australian Dollar, and others
        '€', // Euro
        '£', // British Pound Sterling
        '¥', // Japanese Yen, Chinese Yuan

        // Other widely used currencies
        '₹', // Indian Rupee
        '₽', // Russian Ruble
        '₩', // South Korean Won
        '₦', // Nigerian Naira
        '₪', // Israeli New Shekel
        '₫', // Vietnamese Dong
        '₡', // Costa Rican Colón
        '₢', // Brazilian Cruzeiro (historical, but symbol still used)
        '₣', // French Franc (historical, but symbol still appears)
        '₤', // Italian Lira (historical, but symbol still appears)
        '₥', // Mill (1/10 cent, used in some contexts)
        '₦', // Nigerian Naira
        '₧', // Spanish Peseta (historical)
        '₨', // Multiple currencies (Pakistani Rupee, Sri Lankan Rupee, etc.)
        '₩', // South Korean Won
        '₪', // Israeli New Shekel
        '₫', // Vietnamese Dong
        '€', // Tugrik (Mongolian)
        '₭', // Kip (Laotian)
        '₮', // Tugrik (Mongolian - alternative)
        '₯', // Drachma (Greek - historical)
        '₰', // German Pfennig (historical)
        '₱', // Philippine Peso
        '₲', // Guarani (Paraguayan)
        '₳', // Austral (Argentine - historical)
        '₴', // Hryvnia (Ukrainian)
        '₵', // Cedi (Ghanaian)
        '₶', // Livre Tournois (French historical)
        '₷', // Spesmilo (historical international currency)
        '₸', // Tenge (Kazakhstani)
        '₹', // Indian Rupee
        '₺', // Turkish Lira
        '₻', // Nordic Mark (historical)
        '₼', // Azerbaijani Manat
        '₽', // Russian Ruble
        '₾', // Georgian Lari
        '₿', // Bitcoin (cryptocurrency)

        // Generic and alternative symbols
        '¢', // Cent (used with various currencies)
        '¤', // Generic currency symbol
        '؋', // Afghan Afghani
        '₼', // Azerbaijani Manat

        // Additional symbols that may appear in financial contexts
        'ƒ', // Florin (Dutch historical, also used as function symbol)
        '¥', // Japanese Yen / Chinese Yuan (duplicate removed above)
    ];

    /// <summary>
    /// Alternative implementation using a HashSet for O(1) lookup performance
    /// if you need to frequently check if a character is a currency symbol.
    /// </summary>
    public static readonly HashSet<char> CurrencySymbolsSet = new(CurrencySymbols);

    /// <summary>
    /// Dictionary mapping currency symbols to their common currency names.
    /// Useful for displaying human-readable currency information.
    /// </summary>
    public static readonly Dictionary<char, string[]> CurrencySymbolNames = new()
    {
        ['$'] = ["US Dollar", "Canadian Dollar", "Australian Dollar", "New Zealand Dollar", "Singapore Dollar"],
        ['€'] = ["Euro"],
        ['£'] = ["British Pound Sterling"],
        ['¥'] = ["Japanese Yen", "Chinese Yuan"],
        ['₹'] = ["Indian Rupee"],
        ['₽'] = ["Russian Ruble"],
        ['₩'] = ["South Korean Won"],
        ['₦'] = ["Nigerian Naira"],
        ['₪'] = ["Israeli New Shekel"],
        ['₫'] = ["Vietnamese Dong"],
        ['₡'] = ["Costa Rican Colón"],
        ['₨'] = ["Pakistani Rupee", "Sri Lankan Rupee", "Nepalese Rupee"],
        ['₱'] = ["Philippine Peso"],
        ['₲'] = ["Paraguayan Guarani"],
        ['₴'] = ["Ukrainian Hryvnia"],
        ['₵'] = ["Ghanaian Cedi"],
        ['₸'] = ["Kazakhstani Tenge"],
        ['₺'] = ["Turkish Lira"],
        ['₼'] = ["Azerbaijani Manat"],
        ['₾'] = ["Georgian Lari"],
        ['₿'] = ["Bitcoin"],
        ['¢'] = ["Cent"],
        ['¤'] = ["Generic Currency"],
        ['؋'] = ["Afghan Afghani"],
        ['ƒ'] = ["Florin"]
    };
}
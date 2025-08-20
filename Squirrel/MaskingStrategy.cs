namespace Squirrel;

/// <summary>
/// Defines various masking strategies for sensitive data protection
/// </summary>
public enum MaskingStrategy
{
    
    /// <summary>Show the first and last character, mask everything else with *</summary>
    StarExceptFirstAndLast,
    
    /// <summary>Show the first character, mask everything except last 4 characters</summary>
    StarExceptFirstAndLastFour,
    
    /// <summary>Show the first 4 characters, mask everything else</summary>
    StarExceptFirstFour,
    
    /// <summary>Show first 2 and last 2 characters, mask everything else</summary>
    StarExceptFirstTwoAndLastTwo,
    
    /// <summary>Show the last 4 characters, mask everything else</summary>
    StarExceptLastFour,
    
   
    /// <summary>Mask all characters with *</summary>
    StarAll,
    
    /// <summary>Replace entire value with [REDACTED]</summary>
    Redacted,
    
    /// <summary>Replace entire value with [HIDDEN]</summary>
    Hidden,
    
    /// <summary>Replace entire value with [MASKED]</summary>
    Masked,
    
    // === Partial Display Strategies ===
    /// <summary>Show only the first character, mask rest</summary>
    StarExceptFirst,
    
    /// <summary>Show only the first 2 characters, mask rest</summary>
    StarExceptFirstTwo,
    
    /// <summary>Show only the first 3 characters, mask rest</summary>
    StarExceptFirstThree,
    
    /// <summary>Show only last character, mask rest</summary>
    StarExceptLast,
    
    /// <summary>Show only the last 2 characters, mask rest</summary>
    StarExceptLastTwo,
    
    /// <summary>Show only the last 3 characters, mask rest</summary>
    StarExceptLastThree,
    
    /// <summary>Show first 3 and last 3 characters, mask the middle</summary>
    StarExceptFirstThreeAndLastThree,
    
    /// <summary>Show first 4 and last 4 characters, mask the middle</summary>
    StarExceptFirstFourAndLastFour,
    
    // === Email-Specific Strategies ===
    /// <summary>Show first 2 chars of username and domain, mask rest (e.g., jo***@ex***.com)</summary>
    EmailPartial,
    
    /// <summary>Show domain only, mask username (e.g., ****@example.com)</summary>
    EmailDomainOnly,
    
    /// <summary>Show username only, mask domain (e.g., john@****)</summary>
    EmailUsernameOnly,
    
    // === Phone Number Strategies ===
    /// <summary>Show area code and last 4 digits (e.g., (555) ***-1234)</summary>
    PhoneAreaCodeAndLastFour,
    
    /// <summary>Show only last 4 digits (e.g., ***-***-1234)</summary>
    PhoneLastFour,
    
    /// <summary>Show area code only (e.g., (555) ***-****)</summary>
    PhoneAreaCodeOnly,
    
    // === Credit Card Strategies ===
    /// <summary>Show last 4 digits only (e.g., **** **** **** 1234)</summary>
    CreditCardLastFour,
    
    /// <summary>Show first 4 and last 4 digits (e.g., 1234 **** **** 5678)</summary>
    CreditCardFirstFourLastFour,
    
    /// <summary>Show first 6 and last 4 digits (e.g., 123456** ****1234)</summary>
    CreditCardFirstSixLastFour,
    
    // === Custom Character Strategies ===
    /// <summary>Use X instead of * for masking</summary>
    XMask,
    
    /// <summary>Use # instead of * for masking</summary>
    HashMask,
    
    /// <summary>Use - instead of * for masking</summary>
    DashMask,
    
    /// <summary>Use • instead of * for masking</summary>
    BulletMask,
    
    // === Percentage-Based Strategies ===
    /// <summary>Mask 25% of characters from the middle</summary>
    MaskMiddle25Percent,
    
    /// <summary>Mask 50% of characters from the middle</summary>
    MaskMiddle50Percent,
    
    /// <summary>Mask 75% of characters from the middle</summary>
    MaskMiddle75Percent,
    
    // === Pattern-Based Strategies ===
    /// <summary>Show every 3rd character, mask others</summary>
    ShowEveryThird,
    
    /// <summary>Show random 30% of characters, mask others</summary>
    ShowRandom30Percent,
    
    /// <summary>Alternate masking: show odd positions, mask even</summary>
    AlternatingOddShow,
    
    /// <summary>Alternate masking: show even positions, mask odd</summary>
    AlternatingEvenShow,
    
    // === Length-Preserving Strategies ===
    /// <summary>Replace it with the same number of * characters</summary>
    PreserveLengthStar,
    
    /// <summary>Replace it with the same number of X characters</summary>
    PreserveLengthX,
    
    // === Special Cases ===
    /// <summary>No masking - show original value (for testing/debugging)</summary>
    None,
    
    /// <summary>Truncate to specific length and add ...</summary>
    TruncateWithEllipsis,
    
    /// <summary>Show character counts only (e.g., [8 characters])</summary>
    ShowLengthOnly,
    
    /// <summary>Show data type only (e.g., [STRING], [NUMBER])</summary>
    ShowTypeOnly,
    
    // === Age-Specific Strategies ===
    /// <summary>Convert numeric age to age group (e.g., 25 → "Adult", 16 → "Teenager")</summary>
    AgeGroup
}


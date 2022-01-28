namespace RegExPatterns;

public static class RegExPatterns
{
    private const string UnicodePattern = "[^\u0000-\u007F]+";
    private const string NonAlphaNumericPattern = "[^a-z0-9-]+";
    private const string TrailingHyphensPattern = "[-]{2,}";
}
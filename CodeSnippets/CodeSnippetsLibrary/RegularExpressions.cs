namespace CodeSnippetsLibrary
{
    public class RegularExpressions
    {
        public class Patterns
        {
            public const string StringNotCointainingSubstring = "\\\"(?!sample1)(?!sample2)[A-Za-z0-9\\s]+\\\"";
            
            public const string UnicodePattern = "[^\u0000-\u007F]+";
            public const string NonAlphaNumericPattern = "[^a-z0-9-]+";
            public const string TrailingHyphensPattern = "[-]{2,}";            
        }
    }
}
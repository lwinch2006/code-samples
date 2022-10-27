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

        public class DataAnnotations
        {
            public const string Email = @"^[a-zA-Z0-9\-\._@+]*$";
            public const string Firstname = @"^[\w\x20\p{L}\-\.]*$";
            public const string Lastname = Firstname;
        }
    }
}
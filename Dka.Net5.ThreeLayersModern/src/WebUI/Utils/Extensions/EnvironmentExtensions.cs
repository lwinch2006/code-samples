using System;

namespace WebUI.Utils.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsDevelopment(this string sourceEnvironment)
        {
            return sourceEnvironment.IsEnvironment("Development");
        }
        
        public static bool IsEnvironment(
            this string sourceEnvironment,
            string targetEnvironment)
        {
            return string.Equals(
                sourceEnvironment,
                targetEnvironment,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
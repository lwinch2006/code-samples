namespace Dka.Net5.IdentityWithDapper.Utils.Constants
{
    public static class SystemConstants
    {
        public const string DbConnectionConfigParamName = "Data:ConnectionString";
        public const string LoginProviderName = "IdentityWithDapper";
        
        public static class TwoFA
        {
            public const string AuthenticatorKeyTokenName = "AuthenticatorKey";
            public const string RecoveryCodeTokenName = "RecoveryCodes";
        }
    }
}
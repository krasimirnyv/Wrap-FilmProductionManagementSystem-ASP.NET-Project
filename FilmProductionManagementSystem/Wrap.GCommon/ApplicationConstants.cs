namespace Wrap.GCommon;

public static class ApplicationConstants
{
    /* Constants for Connection strings */
    public const string SecretConnectionString = "ConnectionStrings:MyDevConnection";
    public const string DefaultConnection = "DefaultConnection";
    public const string MissingConnectionStringMessage = "Connection string 'DefaultConnection' not found.";
    
    /* Constants for Session */
    public const int IdleTimeoutMinutes = 5; // 5 min
    public const bool CookieHttpOnly = true;
    public const bool CookieIsEssential = true;
    
    /* Constants for image file size */
    public const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    /* Constants for Application user's identity options */
    public const string SignInRequireConfirmedAccount = "IdentityOptions:SignIn:RequireConfirmedAccount";
    public const string SignInRequireConfirmedEmail = "IdentityOptions:SignIn:RequireConfirmedEmail";
    public const string SignInRequireConfirmedPhoneNumber = "IdentityOptions:SignIn:RequireConfirmedPhoneNumber";

    public const string UserRequireUniqueEmail = "IdentityOptions:User:RequireUniqueEmail";
    
    public const string LockoutMaxFailedAccessAttempts = "IdentityOptions:Lockout:MaxFailedAccessAttempts";
    public const string LockoutDefaultLockoutTimeSpan = "IdentityOptions:Lockout:DefaultLockoutTimeSpanMinutes";

    public const string PasswordRequireDigit = "IdentityOptions:Password:RequireDigit";
    public const string PasswordRequireLowercase = "IdentityOptions:Password:RequireLowercase";
    public const string PasswordRequireUppercase = "IdentityOptions:Password:RequireUppercase";
    public const string PasswordRequireNonAlphanumeric = "IdentityOptions:Password:RequireNonAlphanumeric";
    public const string PasswordRequiredUniqueChars = "IdentityOptions:Password:RequiredUniqueChars";
    public const string PasswordRequiredLength = "IdentityOptions:Password:RequiredLength";

    public const string StoresProtectPersonalData = "IdentityOptions:Stores:ProtectPersonalData";
}

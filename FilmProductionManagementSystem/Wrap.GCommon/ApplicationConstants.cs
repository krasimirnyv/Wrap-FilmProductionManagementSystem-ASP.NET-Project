namespace Wrap.GCommon;

public static class ApplicationConstants
{
    /* Constants for Connection strings */
    public const string SecretConnectionString = "ConnectionStrings:MyDevConnection";
    public const string DefaultConnection = "DefaultConnection";
    public const string MissingConnectionStringMessage = "Connection string 'DefaultConnection' not found.";
    
    /* Constants for image file size */
    public const string MaxFileSizeOptions = "FormOptions:MultipartBodyLengthLimit"; // 10485760 = 10 * 1024 * 1024; 10 MB
    public const byte MegaBytesBase = 10;
    public const long MaxFileSize = MegaBytesBase * 1024 * 1024; // 10 MB
    public const int StreamBufferSize = 81920;

    /* Error Paths */
    public const string StatusCodeErrorPath = "/Home/Error/{0}";
    public const string ExceptionHandlerPath = "/Home/Error";
    
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
    
    /* Constants for Application Cookie options */
    public const string CookieHttpOnly = "CookieOptions:HttpOnly";
    public const string CookieExpireTimeSpan = "CookieOptions:ExpireTimeSpan";
    public const string CookieSlidingExpiration = "CookieOptions:SlidingExpiration";
    public const string CookieLoginPath = "CookieOptions:LoginPath";
    public const string CookieLogoutPath = "CookieOptions:LogoutPath";
    public const string CookieAccessDeniedPath = "CookieOptions:AccessDeniedPath";
    
    /* Constants for Session */
    public const string IdleTimeoutMinutes = "SessionOptions:IdleTimeout";
    public const string SessionCookieHttpOnly = "SessionOptions:HttpOnly";
    public const string SessionCookieIsEssential = "SessionOptions:IsEssential";
    
    /* TempData and ViewData Keys */
    public const string ErrorTempDateKey = "ErrorMessage";
    public const string WarningTempDataKey = "WarningMessage";
    public const string InfoTempDataKey = "InfoMessage";
    public const string SuccessTempDataKey = "SuccessMessage";

    public const string TitleDataKey = "Title";

    public const string RequestQueryReturnUrl = "ReturnUrl";
    public const string RequestQueryProductionKey = "productionId";
    
    /* Production Status strings */
    public const string DefaultStatus = "status-default";
    public const string PreProductionStatus = "status-pre-production";
    public const string ProductionStatus = "status-production";
    public const string PostProductionStatus = "status-post-production";
    public const string DistributionStatus = "status-distribution";
    
    public const string PreProductionKey = "Pre-production";
    public const string ProductionKey = "Production";
    public const string PostProductionKey = "Post-production";
    public const string DistributionKey = "Distribution";
    
    /* Department name / Abstract Roles strings */
    public const string DirectionAndProduction = "Direction & Production";
    public const string WritingAndDevelopment = "Writing & Development";
    public const string CameraDepartment = "Camera Department";
    public const string LightingDepartment = "Lighting Department";
    public const string GripDepartment = "Grip Department";
    public const string ArtDepartment = "Art Department";
    public const string CostumeAndMakeup = "Costume & Makeup";
    public const string SoundDepartment = "Sound Department";
    public const string PostProductionDepartment = "Post-Production";
    public const string MusicDepartment = "Music Department";
    public const string LocationsDepartment = "Locations";
    public const string LogisticsAndTransportationDepartment = "Logistics & Transportation";
    public const string SetOperationsAndSupportDepartment = "Set Operations & Support";
    public const string OtherDepartment = "Other";
    
    /* Profile status badge */
    public const string ActiveStyle = "bg-success";
    public const string InactiveStyle = "bg-secondary";

    public const string Active = "Active";
    public const string Inactive = "Inactive";
    
    // Regular Expressions for splitting text accurately at:
    // 1) Acronym -> word boundary: "ADR Supervisor" (R + Supervisor)
    // 2) Lower -> upper boundary: "Director Of"  (r + O)
    public const string DisplayNameRegEx = @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[a-z])(?=[A-Z])";
    public const string DisplayNameReplacement = " ";
    
    /* Images */
    public const string NullImageStyle = "col-12";
    public const string ImageStyle = "col-md-8";
    public const string ClickToUploadMessage = "Click to upload image";
    public const string ClickToReplaceMessage = "Click to replace thumbnail";
    
    /* Selector items */
    public const string SelectPhase = "— Select Phase first —";
    public const string SelectStatus = "— Select Status —";

    /* Pagination */
    public const int DefaultProductionsPerPage = 9;
}

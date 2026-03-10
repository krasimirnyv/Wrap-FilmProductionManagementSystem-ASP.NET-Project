namespace Wrap.GCommon;

public static class DataFormat
{
    public const string DecimalTypeFormat = "DECIMAL(18,2)";
    public const string DateTimeTypeFormat = "DATETIME2";
    public const string TextTypeFormat = "NVARCHAR(MAX)";
    
    public const string DateFormat = "dd.MM.yyyy";
    public const string CurrencyFormat = "C";
    public const string NotAvailableFormat = "N/A";
    public const string EmptyNickname = " - ";

    public const char CommaSplitter = ',';

    public const string DisplayUsername = "Username";
    public const string DisplayPassword = "Password";

    public const string ImageFolderName = "img";
    public const string ProfileFolderName = "profile";
    public const string ThumbnailFolderName = "thumbnail";
    public const string DefaultProfilePath = "/img/profile/default-profile.png";
    public const string DefaultThumbnailPath = "/img/thumbnail/default-thumbnail.png";
    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif", ".heif", ".heic", ".hif"];
}
namespace Wrap.GCommon.Text;

using System.Text.RegularExpressions;

using static ApplicationConstants;

public static class DisplayNameFormatter
{
    public static string ToDisplayName(string value)
        => Regex.Replace(value, DisplayNameRegEx, DisplayNameReplacement).Trim();

    public static string ToDisplayName(Enum value)
        => ToDisplayName(value.ToString());
}
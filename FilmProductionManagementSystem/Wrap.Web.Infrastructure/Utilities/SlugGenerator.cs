namespace Wrap.Web.Infrastructure.Utilities;

using System.Text.RegularExpressions;

using Interfaces;

public partial class SlugGenerator : ISlugGenerator
{
    public string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        string slug = input.Trim().ToLowerInvariant();

        slug = WhitespaceRegex().Replace(slug, "-");
        slug = InvalidCharactersRegex().Replace(slug, string.Empty);
        slug = MultipleDashesRegex().Replace(slug, "-");
        

        return slug.Trim('-');
    }
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex InvalidCharactersRegex();

    [GeneratedRegex(@"\-+")]
    private static partial Regex MultipleDashesRegex();
}
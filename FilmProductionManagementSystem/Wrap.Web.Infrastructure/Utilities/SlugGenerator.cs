namespace Wrap.Infrastructure.Utilities;

using Interfaces;

public class SlugGenerator : ISlugGenerator
{
    public string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        
        string[] inputDataSplit = input
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToLowerInvariant())
            .ToArray();

        return string.Join("-", inputDataSplit);
    }
}
namespace Wrap.Services.Core.Utilities.ImageLogic;

using Interfaces;

using static GCommon.OutputMessages;

public class VariantImageStrategyResolver (IEnumerable<IVariantImageStrategy> strategies) : IVariantImageStrategyResolver
{
    private readonly IReadOnlyDictionary<string, IVariantImageStrategy> strategiesByKey =
        strategies.ToDictionary(vis => vis.FolderName, StringComparer.OrdinalIgnoreCase);

    public IVariantImageStrategy Resolve(string variantKey)
    {
        bool isValidKey = strategiesByKey.TryGetValue(variantKey, out IVariantImageStrategy? strategy);
        if (isValidKey && strategy is not null) 
            return strategy;

        throw new NotSupportedException(string.Format(NoImageStrategyFound, variantKey));
    }
}
namespace Wrap.GCommon.UI;

using Enums;
using static ApplicationConstants;

using static Text.DisplayNameFormatter;

/// <summary>
/// Groups ProductionStatusTypes enum values by abstract status
/// Used for rendering UI
/// </summary>
public static class ProductionStatusAbstractionCatalog
{
    /// <summary>
    /// Categorizing the status types by their abstract version for easily visualize.
    /// </summary>
    /// <returns>Dictionary with key - abstraction name and value - enum values</returns>
    public static IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> GetStatusTypeByAbstraction()
    {
        return new Dictionary<string, IReadOnlyCollection<ProductionStatusType>>
        {
            [PreProductionKey] =
            [
                ProductionStatusType.Concept,
                ProductionStatusType.Development,
                ProductionStatusType.Preproduction,
                ProductionStatusType.Financing,
                ProductionStatusType.Casting,
                ProductionStatusType.LocationScouting,
                ProductionStatusType.Rehearsals
            ],

            [ProductionKey] =
            [
                ProductionStatusType.Production,
                ProductionStatusType.OnHold,
                ProductionStatusType.Reshoots
            ],

            [PostProductionKey] =
            [
                ProductionStatusType.PostProduction,
                ProductionStatusType.PictureLock,
                ProductionStatusType.SoundDesign,
                ProductionStatusType.ColorGrading,
                ProductionStatusType.VisualEffects,
                ProductionStatusType.MusicComposition
            ],

            [DistributionKey] =
            [
                ProductionStatusType.Marketing,
                ProductionStatusType.Distribution,
                ProductionStatusType.FestivalCircuit,
                ProductionStatusType.Released,
                ProductionStatusType.Completed,
                ProductionStatusType.Cancelled
            ]
        };
    }

    public static string GetDisplayName(string status)
        => ToDisplayName(status);
}
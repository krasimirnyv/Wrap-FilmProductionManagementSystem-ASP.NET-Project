namespace Wrap.Infrastructure.Tests;

using NUnit.Framework;

using GCommon.Enums;
using GCommon.UI;

using static GCommon.ApplicationConstants;

[TestFixture]
public class ProductionStatusAbstractionCatalogTests
{
    [Test]
    public void GetStatusTypeByAbstraction_ReturnsAllExpectedKeys()
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> map 
            = ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction();

        Assert.That(map.ContainsKey(PreProductionKey), Is.True);
        Assert.That(map.ContainsKey(ProductionKey), Is.True);
        Assert.That(map.ContainsKey(PostProductionKey), Is.True);
        Assert.That(map.ContainsKey(DistributionKey), Is.True);
        
        Assert.That(map[PreProductionKey], Is.Not.Empty);
        Assert.That(map[ProductionKey], Is.Not.Empty);
        Assert.That(map[PostProductionKey], Is.Not.Empty);
        Assert.That(map[DistributionKey], Is.Not.Empty);
    }

    [Test]
    public void GetStatusTypeByAbstraction_CoversEveryEnumValueExactlyOnce()
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> map 
            = ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction();

        ProductionStatusType[] allFromMap = map.SelectMany(kvp => kvp.Value).ToArray();
        ProductionStatusType[] allEnumValues = Enum.GetValues<ProductionStatusType>();

        // no missing values
        Assert.That(allEnumValues, Is.SubsetOf(allFromMap));

        // no extra values (normally impossible, but keep symmetric)
        Assert.That(allFromMap, Is.SubsetOf(allEnumValues));

        // no duplicates across groups
        Assert.That(allFromMap.Length, Is.EqualTo(allFromMap.Distinct().Count()),
            "Some ProductionStatusType values appear in more than one group.");
    }

    [TestCase("PreProduction", "Pre Production")]
    [TestCase("PostProduction", "Post Production")]
    [TestCase("PictureLock", "Picture Lock")]
    public void GetDisplayName_FormatsStatusName(string input, string expected)
    {
        string result = ProductionStatusAbstractionCatalog.GetDisplayName(input);

        Assert.That(result, Is.EqualTo(expected));
    }
}
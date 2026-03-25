namespace Wrap.Services.Models.Production.NestedDtos;

using GCommon.Enums;

public class ProductionAssetDto
{
    public ProductionAssetType AssetType { get; set; }
    
    public string Title { get; set; } = null!;
}
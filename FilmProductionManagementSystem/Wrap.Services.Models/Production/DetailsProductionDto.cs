namespace Wrap.Services.Models.Production;

using NestedDtos;
using GCommon.Enums;

public class DetailsProductionDto
{
    public Guid Id { get; set; }

    public string Thumbnail { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Budget { get; set; }

    public ProductionStatusType StatusType { get; set; }

    public string StatusAbstractClass { get; set; } = string.Empty;

    public DateTime StatusStartDate { get; set; }

    public DateTime? StatusEndDate { get; set; }

    public Guid? ScriptId { get; set; }

    public IReadOnlyCollection<ProductionCrewMemberDto> ProductionCrewMembers { get; set; } = [];

    public IReadOnlyCollection<ProductionCastMemberDto> ProductionCastMembers { get; set; } = [];

    public IReadOnlyCollection<ProductionSceneDto> ProductionScenes { get; set; } = [];

    public IReadOnlyCollection<ProductionAssetDto> ProductionAssets { get; set; } = [];

    public IReadOnlyCollection<ProductionShootingDayDto> ProductionShootingDays { get; set; } = [];
}
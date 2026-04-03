namespace Wrap.Data.Dtos.Cast;

public sealed record CastProductionDataDto
{
    public string Title { get; init; } = null!;
    
    public string? Description { get; init; }

    public string RoleInProduction { get; init; } = null!;
}
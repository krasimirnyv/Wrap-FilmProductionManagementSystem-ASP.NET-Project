namespace Wrap.Data.Dtos.Crew;

public sealed record CrewProductionDataDto
{
    public string Title { get; init; } = null!;
    
    public string? Description { get; init; }

    public string RoleInProduction { get; init; } = null!;
}
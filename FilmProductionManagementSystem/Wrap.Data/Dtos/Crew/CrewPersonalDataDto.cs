namespace Wrap.Data.Dtos.Crew;

public sealed record CrewPersonalDataDto
{
    public Guid Id { get; init; }

    public string FirstName { get; init; } = null!;
    
    public string LastName { get; init; } = null!;
    
    public string? Nickname { get; init; }
    
    public string? Biography { get; init; }

    public CrewUserDataDto User { get; init; } = null!;

    public int SkillsCount { get; init; }

    public IReadOnlyCollection<CrewSkillDataDto> Skills { get; init; } = [];

    public int ProductionsCount { get; init; }

    public IReadOnlyCollection<CrewProductionDataDto> Productions { get; init; } = [];

    public int ScenesCount { get; init; }
    
    public IReadOnlyCollection<CrewSceneDataDto> Scenes { get; init; } = [];

}
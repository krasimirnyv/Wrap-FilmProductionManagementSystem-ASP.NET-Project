namespace Wrap.Data.Dtos.Cast;

public sealed record CastPersonalDataDto
{
    public Guid Id { get; init; }

    public string FirstName { get; init; } = null!;
    
    public string LastName { get; init; } = null!;
    
    public string? Nickname { get; init; }
    
    public string? Biography { get; init; }

    public DateTime Birthday { get; init; }
    
    public byte Age { get; init; }
    
    public string Gender { get; init; } = null!;

    public CastUserDataDto User { get; init; } = null!;

    public int ProductionsCount { get; init; }

    public IReadOnlyCollection<CastProductionDataDto> Productions { get; init; } = [];

    public int ScenesCount { get; init; }

    public IReadOnlyCollection<CastSceneDataDto> Scenes { get; init; } = [];
}
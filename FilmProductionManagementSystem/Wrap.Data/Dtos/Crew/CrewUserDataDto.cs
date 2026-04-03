namespace Wrap.Data.Dtos.Crew;

public sealed record CrewUserDataDto
{
    public Guid Id { get; init; }

    public string? UserName { get; init; }
    
    public string? Email { get; init; }
    
    public bool EmailConfirmed { get; init; }

    public string? PhoneNumber { get; init; }
    
    public bool PhoneNumberConfirmed { get; init; }

    public bool LockoutEnabled { get; init; }
    
    public DateTimeOffset? LockoutEnd { get; init; }
}
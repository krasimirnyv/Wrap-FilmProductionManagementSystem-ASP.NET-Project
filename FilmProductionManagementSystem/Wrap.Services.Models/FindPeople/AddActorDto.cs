namespace Wrap.Services.Models.FindPeople;

public class AddActorDto
{
    public Guid ProductionId { get; set; }

    public Guid CastId { get; set; }

    public string RoleName { get; set; } = null!;
}
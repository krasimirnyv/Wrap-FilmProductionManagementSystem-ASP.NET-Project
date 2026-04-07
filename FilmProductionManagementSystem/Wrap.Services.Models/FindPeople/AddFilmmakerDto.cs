namespace Wrap.Services.Models.FindPeople;

using GCommon.Enums;

public class AddFilmmakerDto
{
    public Guid ProductionId { get; set; }

    public Guid CrewId { get; set; }

    public CrewRoleType RoleType { get; set; }
}
namespace Wrap.Services.Models.LoginRegister;

public class CrewRegistrationCompleteDto
{
    public CrewRegistrationDraftDto? Draft { get; set; }

    public IReadOnlyCollection<int>? SkillNumbers { get; set; } = [];
}
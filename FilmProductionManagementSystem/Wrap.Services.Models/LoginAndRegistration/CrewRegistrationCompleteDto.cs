namespace Wrap.Services.Models.LoginAndRegistration;

public class CrewRegistrationCompleteDto
{
    public CrewRegistrationDraftDto? Draft { get; set; }

    public IReadOnlyCollection<int>? SkillNumbers { get; set; } = [];
}
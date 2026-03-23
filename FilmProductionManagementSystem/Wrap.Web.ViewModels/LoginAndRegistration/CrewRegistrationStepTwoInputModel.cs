namespace Wrap.ViewModels.LoginAndRegistration;

using GCommon.Enums;

/// <summary>
/// Step 2: Skills Selection for Crew Registration
/// Потребител селектира множество умения
/// Primary role ще се посочва от Продуцента (в по-късен етап на апликацията)
/// </summary>
public class CrewRegistrationStepTwoInputModel
{
    /// <summary>
    /// Избрани умения - допускат се няколко
    /// </summary>
    public IReadOnlyCollection<int> SelectedSkills { get; set; } =
        new List<int>();

    /// <summary>
    /// Всички налични умения, групирани по департаменти
    /// Използва се за рендиране на потребителския интерфейс на акордеона
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> SkillsByDepartment { get; set; }
        = new Dictionary<string, IReadOnlyCollection<CrewRoleType>>();
}

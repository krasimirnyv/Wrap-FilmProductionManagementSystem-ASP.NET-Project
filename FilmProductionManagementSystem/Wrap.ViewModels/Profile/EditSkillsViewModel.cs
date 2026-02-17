namespace Wrap.ViewModels.Profile;

using System.ComponentModel.DataAnnotations;

using GCommon.Enums;

public class EditSkillsViewModel
{
    public ICollection<CrewRoleType> CurrentSkills { get; set; }
        = new HashSet<CrewRoleType>();
    
    public IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> AllDepartments { get; set; }
        = new Dictionary<string, IReadOnlyCollection<CrewRoleType>>();
    
    [Required]
    public string SelectedSkills { get; set; } = null!;
}

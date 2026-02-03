namespace FilmProductionManagementSystem.Web.ViewModels.Script;

public class ScriptEditorViewModel
{
    public Guid ScriptId { get; set; }

    public Guid ProductionId { get; set; }
    
    public bool IsReadOnly { get; set; }

    public string LastEditedAt { get; set; } = null!;

    public TitlePageViewModel TitlePageViewModel { get; set; } = null!;

    public List<ScriptPageViewModel> ScriptPages { get; set; } = null!;
}
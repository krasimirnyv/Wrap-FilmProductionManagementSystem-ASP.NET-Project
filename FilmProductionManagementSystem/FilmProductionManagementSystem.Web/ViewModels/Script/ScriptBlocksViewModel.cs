namespace FilmProductionManagementSystem.Web.ViewModels.Script;

using Enums;

public class ScriptBlocksViewModel
{
    public ScriptBlockType BlockType { get; set; }
    
    public string? Text { get; set; }

    public bool IsReadOnly { get; set; }
}
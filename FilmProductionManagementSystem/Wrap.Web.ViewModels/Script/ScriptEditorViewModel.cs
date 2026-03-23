namespace Wrap.ViewModels.Script;

public class ScriptEditorViewModel
{ 
    public Guid ScriptId { get; set; }

    public string ScriptTitle { get; set; } = null!;

    public Guid ProductionId { get; set; }
    
    public string ProductionTitle { get; set; } = null!;

    public bool IsReadOnly { get; set; }

    public string LastEditedAt { get; set; } = null!;

    public TitlePageViewModel TitlePage { get; set; } = null!;

    /// <summary>
    /// All script blocks in order - used for initial render
    /// Client-side pagination will split these into pages
    /// </summary>
    public List<ScriptBlocksViewModel> Blocks { get; set; } = new();

    /// <summary>
    /// Character names extracted from script - used for autocomplete
    /// </summary>
    public HashSet<string> CharacterNames { get; set; } = new();
}
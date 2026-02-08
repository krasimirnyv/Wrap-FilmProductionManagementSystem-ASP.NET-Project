namespace Wrap.ViewModels.Script;

public class ScriptPageViewModel
{
    public int PageNumber {get; set;}
    
    public bool IsTitlePage { get; set; }
    public List<ScriptBlocksViewModel> ScriptBlocks { get; set; } = null!;
}
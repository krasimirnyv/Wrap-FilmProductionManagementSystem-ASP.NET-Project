namespace Wrap.ViewModels.Script;

using GCommon.Enums;

public class ScriptBlocksViewModel
{
    /// <summary>
    /// Database ID - used for autosave updates
    /// </summary>
    public Guid? Id { get; set; }

    public ScriptBlockType BlockType { get; set; }
    
    /// <summary>
    /// HTML content of the block (innerHTML)
    /// May contain formatting tags like <br>, <span>, etc.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Optional metadata in JSON format
    /// Example: {"characterName": "JOHN", "sceneNumber": "12"}
    /// </summary>
    public string? Metadata { get; set; }

    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Order of this block in the script (for rendering)
    /// </summary>
    public int OrderIndex { get; set; }
}
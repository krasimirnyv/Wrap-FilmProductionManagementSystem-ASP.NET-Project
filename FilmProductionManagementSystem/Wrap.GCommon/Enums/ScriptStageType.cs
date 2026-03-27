namespace Wrap.GCommon.Enums;

/// <summary>
/// Editorial / workflow stage of the screenplay.
/// </summary>
public enum ScriptStageType
{
    Outline = 1,
    Draft = 2,
    Rewrite = 3,        // structural redraft
    Polish = 4,         // line edit / final polish
    CharacterPass = 5,
    DialoguePass = 6,
    ShootingScript = 7, // locked format (often used with production colors)
    ProductionDraft = 8, // explicitly means "color drafts apply"
    
    Other = 10
}
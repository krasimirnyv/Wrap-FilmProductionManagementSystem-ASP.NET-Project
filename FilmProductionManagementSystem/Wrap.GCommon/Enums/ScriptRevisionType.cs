namespace Wrap.GCommon.Enums;

/// <summary>
/// Color-coded production revision pages (after script is locked).
/// Standard sequence: White, Blue, Pink, Yellow, Green, Goldenrod.
/// </summary>
public enum ScriptRevisionType
{
    None = 0,
    
    WhiteRevision = 1,
    BlueRevision = 2,
    PinkRevision = 3,
    YellowRevision = 4,
    GreenRevision = 5,
    GoldenrodRevision = 6,
    
    Other = 10
}
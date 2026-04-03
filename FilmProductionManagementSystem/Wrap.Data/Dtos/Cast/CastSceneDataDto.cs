namespace Wrap.Data.Dtos.Cast;

public sealed record CastSceneDataDto
{
    public int SceneNumber { get; init; }
    
    public string SceneType { get; init; } = null!;

    public string SceneName { get; init; } = null!;
    
    public string Location { get; init; } = null!;
    
    public string ProductionTitle { get; init; } = null!;
}
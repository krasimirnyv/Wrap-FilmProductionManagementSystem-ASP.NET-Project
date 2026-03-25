namespace Wrap.Services.Models.Production.NestedDtos;

using GCommon.Enums;

public class ProductionSceneDto
{
    public int SceneNumber { get; set; }
    
    public SceneType SceneType { get; set; }
    
    public string SceneName { get; set; } = null!;
    
    public string Location { get; set; } = null!;
}
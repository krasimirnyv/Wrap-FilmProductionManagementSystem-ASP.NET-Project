namespace Wrap.Services.Models.Production;

using GCommon.Enums;

public class ProductionDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string ThumbnailPath { get; set; } = null!;

    public ProductionStatusType StatusType { get; set; }

    public string StatusAbstractClass { get; set; } = string.Empty;
}
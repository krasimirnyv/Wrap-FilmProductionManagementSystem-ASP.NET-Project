namespace Wrap.Services.Models.FindPeople;

public class FindActorsDto
{
    public IReadOnlyCollection<ActorListDto> ActorListDtos { get; set; }
        = new List<ActorListDto>();

    public int TotalCount { get; set; }
}
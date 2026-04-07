namespace Wrap.Services.Models.FindPeople;

public class FindFilmmakersDto
{
    public IReadOnlyCollection<FilmmakerListDto> FilmmakerListDtos { get; set; }
        = new List<FilmmakerListDto>();

    public int TotalCount { get; set; }
}
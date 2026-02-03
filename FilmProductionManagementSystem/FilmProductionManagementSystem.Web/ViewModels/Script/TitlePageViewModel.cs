namespace FilmProductionManagementSystem.Web.ViewModels.Script;

public class TitlePageViewModel
{
    public string Title { get; set; } = null!;

    public string WrittenBy { get; set; } = null!;

    public string? BasedOn { get; set; }

    public string ContactInfo { get; set; } = null!;
}
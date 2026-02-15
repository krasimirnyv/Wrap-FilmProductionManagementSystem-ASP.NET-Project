namespace Wrap.Services.Core.Interface;

using ViewModels.General;

public interface IHomeService
{
    /// <summary>
    /// Gets the data for the home page,
    /// including the number of crew members, cast members, and productions.
    /// </summary>
    /// <returns>GeneralPageViewModel</returns>
    Task<GeneralPageViewModel> GetGeneralInformation();
    
}
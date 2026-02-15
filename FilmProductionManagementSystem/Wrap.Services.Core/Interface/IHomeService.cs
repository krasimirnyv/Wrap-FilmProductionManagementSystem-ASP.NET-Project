namespace Wrap.Services.Core.Interface;

using ViewModels.General;

public interface IHomeService
{
    Task<GeneralPageViewModel> GetGeneralInformation();
    
}
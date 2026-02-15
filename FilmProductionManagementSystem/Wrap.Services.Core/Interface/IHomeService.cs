namespace Wrap.Services.Core.Interface;

public interface IWrapService
{
    Task<string> GetProfileImagePathAsync(Guid userId, string userType);

}
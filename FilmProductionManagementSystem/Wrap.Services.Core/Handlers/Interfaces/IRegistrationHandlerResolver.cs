namespace Wrap.Services.Core.Handlers.Interfaces;

public interface IRegistrationHandlerResolver
{
    /// <summary>
    /// Resolves current DTO that is used for registration.
    /// </summary>
    /// <returns>specified dto wrapped in IRegistrationHandler<TRegistrationDto></returns>
    IRegistrationHandler<TRegistrationDto> Resolve<TRegistrationDto>();
}
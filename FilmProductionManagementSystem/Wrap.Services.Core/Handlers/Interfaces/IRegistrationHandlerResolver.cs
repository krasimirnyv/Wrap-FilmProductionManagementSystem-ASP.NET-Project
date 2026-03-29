namespace Wrap.Services.Core.Handlers.Interfaces;

public interface IRegistrationHandlerResolver
{
    /// <summary>
    /// Resolves current DTO that is used for registration.
    /// </summary>
    /// <typeparam name="TRegistrationDto"></typeparam>
    /// <returns></returns>
    IRegistrationHandler<TRegistrationDto> Resolve<TRegistrationDto>();
}
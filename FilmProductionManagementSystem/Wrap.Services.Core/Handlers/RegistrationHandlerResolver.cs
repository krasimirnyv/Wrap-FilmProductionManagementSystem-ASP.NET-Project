namespace Wrap.Services.Core.Handlers;

using Interfaces;
using Models.LoginAndRegistration;

using static GCommon.OutputMessages.Register;

public class RegistrationHandlerResolver(IRegistrationHandler<CrewRegistrationCompleteDto> crewRegistrationHandler,
                                         IRegistrationHandler<CastRegistrationDto> castRegistrationHandler) : IRegistrationHandlerResolver
{
    public IRegistrationHandler<TRegistrationDto> Resolve<TRegistrationDto>()
    {
        IRegistrationHandler<TRegistrationDto> registrationHandler = typeof(TRegistrationDto) switch
        {
            _ when typeof(TRegistrationDto) == typeof(CrewRegistrationCompleteDto) => (IRegistrationHandler<TRegistrationDto>)crewRegistrationHandler,
            
            _ when typeof(TRegistrationDto) == typeof(CastRegistrationDto) => (IRegistrationHandler<TRegistrationDto>)castRegistrationHandler,
            
            _ => throw new NotSupportedException(string.Format(UnsupportedRegistrationType, typeof(TRegistrationDto).Name))
        };
        
        return registrationHandler;
    }
}
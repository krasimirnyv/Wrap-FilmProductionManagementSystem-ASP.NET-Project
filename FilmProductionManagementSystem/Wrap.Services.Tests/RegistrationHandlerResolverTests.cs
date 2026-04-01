namespace Wrap.Services.Tests;

using Moq;
using NUnit.Framework;

using Core.Handlers;
using Core.Handlers.Interfaces;
using Models.LoginAndRegistration;

using static GCommon.OutputMessages.Register;

[TestFixture]
public class RegistrationHandlerResolverTests
{
    private Mock<IRegistrationHandler<CrewRegistrationCompleteDto>> crewRegistrationHandlerMock = null!;
    private Mock<IRegistrationHandler<CastRegistrationDto>> castRegistrationHandlerMock = null!;

    private IRegistrationHandlerResolver registerResolver = null!;

    [SetUp]
    public void SetUp()
    {
        crewRegistrationHandlerMock = new Mock<IRegistrationHandler<CrewRegistrationCompleteDto>>(MockBehavior.Strict);
        castRegistrationHandlerMock = new Mock<IRegistrationHandler<CastRegistrationDto>>(MockBehavior.Strict);

        registerResolver = new RegistrationHandlerResolver
        (
            crewRegistrationHandler: crewRegistrationHandlerMock.Object,
            castRegistrationHandler: castRegistrationHandlerMock.Object
        );
    }

    [Test]
    public void Resolve_WhenCrewRegistrationCompleteDto_ReturnsCrewRegistrationHandler()
    {
        // Act
        IRegistrationHandler<CrewRegistrationCompleteDto> result = registerResolver.Resolve<CrewRegistrationCompleteDto>();

        // Assert
        Assert.That(result, Is.SameAs(crewRegistrationHandlerMock.Object));

        crewRegistrationHandlerMock.VerifyNoOtherCalls();
        castRegistrationHandlerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Resolve_WhenCastRegistrationDto_ReturnsCastRegistrationHandler()
    {
        // Act
        IRegistrationHandler<CastRegistrationDto> result = registerResolver.Resolve<CastRegistrationDto>();

        // Assert
        Assert.That(result, Is.SameAs(castRegistrationHandlerMock.Object));

        crewRegistrationHandlerMock.VerifyNoOtherCalls();
        castRegistrationHandlerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Resolve_WhenUnsupportedType_ThrowsNotSupportedExceptionWithCorrectMessage()
    {
        // Act
        NotSupportedException ex = Assert.Throws<NotSupportedException>(
            () => registerResolver.Resolve<UnsupportedRegistrationDto>())!;

        // Assert
        Assert.That(ex.Message, Is.EqualTo(string.Format(UnsupportedRegistrationType, nameof(UnsupportedRegistrationDto))));

        crewRegistrationHandlerMock.VerifyNoOtherCalls();
        castRegistrationHandlerMock.VerifyNoOtherCalls();
    }

    private sealed class UnsupportedRegistrationDto // empty - only used for resolver test
    {
    }
}
namespace Wrap.Services.Tests;

using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;
using NUnit.Framework;

using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using Core;
using Core.Handlers.Interfaces;
using Core.Utilities.ImageLogic.Interfaces;
using Models.LoginAndRegistration;

using static GCommon.OutputMessages;
using static GCommon.OutputMessages.Register;
using static GCommon.DataFormat;

[TestFixture]
public class LoginRegisterServiceTests
{
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private Mock<SignInManager<ApplicationUser>> signInManagerMock = null!;

    private Mock<ILoginRegisterRepository> loginRegisterRepositoryMock = null!;
    private Mock<IImageService> imageServiceMock = null!;
    private Mock<IVariantImageStrategyResolver> imageStrategyResolverMock = null!;
    private Mock<IRegistrationHandlerResolver> registrationHandlerResolverMock = null!;
    private Mock<ILogger<LoginRegisterService>> loggerMock = null!;

    private LoginRegisterService loginRegisterService = null!;

    [SetUp]
    public void SetUp()
    {
        userManagerMock = CreateUserManagerMock();
        signInManagerMock = CreateSignInManagerMock(userManagerMock.Object);

        loginRegisterRepositoryMock = new Mock<ILoginRegisterRepository>(MockBehavior.Strict);
        imageServiceMock = new Mock<IImageService>(MockBehavior.Strict);
        imageStrategyResolverMock = new Mock<IVariantImageStrategyResolver>(MockBehavior.Strict);
        registrationHandlerResolverMock = new Mock<IRegistrationHandlerResolver>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<LoginRegisterService>>(MockBehavior.Loose);

        loginRegisterService = new LoginRegisterService(
            userManagerMock.Object,
            signInManagerMock.Object,
            loginRegisterRepositoryMock.Object,
            imageServiceMock.Object,
            imageStrategyResolverMock.Object,
            registrationHandlerResolverMock.Object,
            loggerMock.Object
        );
    }

    [Test]
    public async Task BuildCrewDraftAsync_WhenValidInput_ReturnsDraftWithSavedProfilePath()
    {
        // Arrange
        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        CrewRegistrationStepOneDto dto = new CrewRegistrationStepOneDto
        {
            UserName = "crew.user",
            Email = "crew@wrap.local",
            PhoneNumber = "+359888000000",
            Password = "Pass123!",
            FirstName = "Crew",
            LastName = "User",
            Nickname = "C",
            Biography = "bio",
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3])
        };

        const string savedPath = "/img/profile/saved.webp";

        imageServiceMock
            .Setup(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedPath);

        // Act
        CrewRegistrationDraftDto? result = await loginRegisterService.BuildCrewDraftAsync(dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserName, Is.EqualTo(dto.UserName));
        Assert.That(result.Email, Is.EqualTo(dto.Email));
        Assert.That(result.PhoneNumber, Is.EqualTo(dto.PhoneNumber));
        Assert.That(result.Password, Is.EqualTo(dto.Password));
        Assert.That(result.FirstName, Is.EqualTo(dto.FirstName));
        Assert.That(result.LastName, Is.EqualTo(dto.LastName));
        Assert.That(result.Nickname, Is.EqualTo(dto.Nickname));
        Assert.That(result.Biography, Is.EqualTo(dto.Biography));
        Assert.That(result.ProfilePicturePath, Is.EqualTo(savedPath));

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()), Times.Once);

        registrationHandlerResolverMock.VerifyNoOtherCalls();
        loginRegisterRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public void BuildCrewDraftAsync_WhenImageServiceThrowsNotSupportedException_RethrowsNotSupportedException()
    {
        // Arrange
        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        CrewRegistrationStepOneDto dto = new CrewRegistrationStepOneDto
        {
            UserName = "crew.user",
            Email = "crew@wrap.local",
            PhoneNumber = "+359888000000",
            Password = "Pass123!",
            FirstName = "Crew",
            LastName = "User",
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3])
        };

        imageServiceMock
            .Setup(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotSupportedException("bad image"));

        // Act + Assert
        NotSupportedException ex =
            Assert.ThrowsAsync<NotSupportedException>(() => loginRegisterService.BuildCrewDraftAsync(dto))!;

        Assert.That(ex.Message, Is.EqualTo("bad image"));
        Assert.That(ex.InnerException, Is.Not.Null);

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void BuildCrewDraftAsync_WhenUnexpectedExceptionOccurs_ThrowsWrappedExceptionWithConfiguredMessage()
    {
        // Arrange
        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        CrewRegistrationStepOneDto dto = new CrewRegistrationStepOneDto
        {
            UserName = "crew.user",
            Email = "crew@wrap.local",
            PhoneNumber = "+359888000000",
            Password = "Pass123!",
            FirstName = "Crew",
            LastName = "User",
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3])
        };

        imageServiceMock
            .Setup(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        // Act
        Exception ex = Assert.ThrowsAsync<Exception>(() => loginRegisterService.BuildCrewDraftAsync(dto))!;

        // Assert
        Assert.That(ex.Message, Is.EqualTo(ExceptionBuildingCrewDraft));
        Assert.That(ex.InnerException, Is.Not.Null);

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetNewModelWithSkills_WhenCalled_ReturnsDtoWithSkillsByDepartment()
    {
        // Act
        CrewRegistrationStepTwoDto dto = loginRegisterService.GetNewModelWithSkills();

        // Assert
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto.SkillsByDepartment, Is.Not.Null);
        Assert.That(dto.SkillsByDepartment.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GetSkills_WhenStepTwoDtoProvided_PopulatesSkillsByDepartment()
    {
        // Arrange
        CrewRegistrationStepTwoDto dto = new CrewRegistrationStepTwoDto();

        // Act
        loginRegisterService.GetSkills(dto);

        // Assert
        Assert.That(dto.SkillsByDepartment, Is.Not.Null);
        Assert.That(dto.SkillsByDepartment.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GetSkills_WhenCompleteDtoProvided_PopulatesSkillsByDepartment()
    {
        // Arrange
        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto();

        // Act
        loginRegisterService.GetSkills(dto);

        // Assert
        Assert.That(dto.SkillsByDepartment, Is.Not.Null);
        Assert.That(dto.SkillsByDepartment.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenHandlerReturnsSuccess_ReturnsSuccess()
    {
        // Arrange
        CastRegistrationDto registrationDto = new CastRegistrationDto
        {
            UserName = "cast.user",
            Email = "cast@wrap.local",
            PhoneNumber = "+359888000111",
            Password = "Pass123!",
            FirstName = "A",
            LastName = "B",
            BirthDate = new DateTime(2000, 1, 1),
            Gender = GCommon.Enums.GenderType.Male,
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3]),
            Biography = "bio"
        };

        Mock<IRegistrationHandler<CastRegistrationDto>> handlerMock =
            new Mock<IRegistrationHandler<CastRegistrationDto>>(MockBehavior.Strict);

        registrationHandlerResolverMock
            .Setup(rhr => rhr.Resolve<CastRegistrationDto>())
            .Returns(handlerMock.Object);

        handlerMock
            .Setup(h => h.CompleteRegistrationAsync(registrationDto))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await loginRegisterService.CompleteRegistrationAsync(registrationDto);

        // Assert
        Assert.That(result.Succeeded, Is.True);

        registrationHandlerResolverMock.Verify(rhr => rhr.Resolve<CastRegistrationDto>(), Times.Once);
        handlerMock.Verify(h => h.CompleteRegistrationAsync(registrationDto), Times.Once);

        loginRegisterRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenHandlerReturnsFailed_ReturnsFailed()
    {
        // Arrange
        CrewRegistrationCompleteDto registrationDto = new CrewRegistrationCompleteDto
        {
            Draft = new CrewRegistrationDraftDto
            {
                UserName = "crew.user",
                Email = "crew@wrap.local",
                PhoneNumber = "+359888000000",
                Password = "Pass123!",
                FirstName = "Crew",
                LastName = "User",
                ProfilePicturePath = "/img/profile/default.webp"
            },
            SkillNumbers = [1, 2]
        };

        Mock<IRegistrationHandler<CrewRegistrationCompleteDto>> handlerMock =
            new Mock<IRegistrationHandler<CrewRegistrationCompleteDto>>(MockBehavior.Strict);

        IdentityResult failed = IdentityResult.Failed(new IdentityError { Description = "failed" });

        registrationHandlerResolverMock
            .Setup(rhr => rhr.Resolve<CrewRegistrationCompleteDto>())
            .Returns(handlerMock.Object);

        handlerMock
            .Setup(h => h.CompleteRegistrationAsync(registrationDto))
            .ReturnsAsync(failed);

        // Act
        IdentityResult result = await loginRegisterService.CompleteRegistrationAsync(registrationDto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain("failed"));
    }

    [Test]
    public void CompleteRegistrationAsync_WhenResolverThrows_PropagatesException()
    {
        // Arrange
        CastRegistrationDto registrationDto = new CastRegistrationDto
        {
            UserName = "cast.user",
            Email = "cast@wrap.local",
            PhoneNumber = "+359888000111",
            Password = "Pass123!",
            FirstName = "A",
            LastName = "B",
            BirthDate = new DateTime(2000, 1, 1),
            Gender = GCommon.Enums.GenderType.Male,
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3]),
            Biography = "bio"
        };

        registrationHandlerResolverMock
            .Setup(rhr => rhr.Resolve<CastRegistrationDto>())
            .Throws(new InvalidOperationException("resolver boom"));

        // Act + Assert
        InvalidOperationException ex =
            Assert.ThrowsAsync<InvalidOperationException>(() => loginRegisterService.CompleteRegistrationAsync(registrationDto))!;

        Assert.That(ex.Message, Is.EqualTo("resolver boom"));
    }

    [Test]
    public void CompleteRegistrationAsync_WhenHandlerThrows_PropagatesException()
    {
        // Arrange
        CastRegistrationDto registrationDto = new CastRegistrationDto
        {
            UserName = "cast.user",
            Email = "cast@wrap.local",
            PhoneNumber = "+359888000111",
            Password = "Pass123!",
            FirstName = "A",
            LastName = "B",
            BirthDate = new DateTime(2000, 1, 1),
            Gender = GCommon.Enums.GenderType.Male,
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3]),
            Biography = "bio"
        };

        Mock<IRegistrationHandler<CastRegistrationDto>> handlerMock =
            new Mock<IRegistrationHandler<CastRegistrationDto>>(MockBehavior.Strict);

        registrationHandlerResolverMock
            .Setup(rhr => rhr.Resolve<CastRegistrationDto>())
            .Returns(handlerMock.Object);

        handlerMock
            .Setup(h => h.CompleteRegistrationAsync(registrationDto))
            .ThrowsAsync(new Exception("handler boom"));

        // Act + Assert
        Exception ex = Assert.ThrowsAsync<Exception>(() => loginRegisterService.CompleteRegistrationAsync(registrationDto))!;
        Assert.That(ex.Message, Is.EqualTo("handler boom"));
    }

    [Test]
    public async Task LoginStatusAsync_WhenDtoIsNull_ReturnsFailedWithEmptyRole()
    {
        // Arrange
        LoginRequestDto? dto = null;

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(string.Empty));

        loginRegisterRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task LoginStatusAsync_WhenRoleIsInvalid_ReturnsFailedWithEmptyRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "user",
            Password = "pass",
            RememberMe = false,
            Role = "InvalidRole"
        };

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(string.Empty));

        loginRegisterRepositoryMock.VerifyNoOtherCalls();
        userManagerMock.Verify(um => um.FindByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task LoginStatusAsync_WhenUserDoesNotExist_ReturnsFailedWithEmptyRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "missingUser",
            Password = "pass",
            RememberMe = false,
            Role = CrewString
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(string.Empty));

        userManagerMock.Verify(um => um.FindByNameAsync(dto.UserName), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), false), Times.Never);
        loginRegisterRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task LoginStatusAsync_WhenPasswordCheckFails_ReturnsFailedWithEmptyRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "user",
            Password = "wrong",
            RememberMe = false,
            Role = CastString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(false);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(string.Empty));

        userManagerMock.Verify(um => um.FindByNameAsync(dto.UserName), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(user, dto.Password), Times.Once);
        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), false), Times.Never);
        loginRegisterRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task LoginStatusAsync_WhenCrewRoleButCrewDoesNotExist_ReturnsFailedCrewRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "crewUser",
            Password = "p",
            RememberMe = false,
            Role = CrewString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CrewExistsByUserIdAsync(user.Id))
            .ReturnsAsync(false);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(CrewString));

        userManagerMock.Verify(um => um.FindByNameAsync(dto.UserName), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(user, dto.Password), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CrewExistsByUserIdAsync(user.Id), Times.Once);
        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), false), Times.Never);
    }

    [Test]
    public async Task LoginStatusAsync_WhenCastRoleButCastDoesNotExist_ReturnsFailedCastRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "castUser",
            Password = "p",
            RememberMe = false,
            Role = CastString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CastExistsByUserIdAsync(user.Id))
            .ReturnsAsync(false);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(CastString));

        userManagerMock.Verify(um => um.FindByNameAsync(dto.UserName), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(user, dto.Password), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CastExistsByUserIdAsync(user.Id), Times.Once);
        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), false), Times.Never);
    }

    [Test]
    public async Task LoginStatusAsync_WhenCrewExistsButPasswordSignInFails_ReturnsFailedWithEmptyRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "crewUser",
            Password = "pass",
            RememberMe = false,
            Role = CrewString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CrewExistsByUserIdAsync(user.Id))
            .ReturnsAsync(true);

        signInManagerMock
            .Setup(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(string.Empty));

        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false), Times.Once);
    }

    [Test]
    public async Task LoginStatusAsync_WhenCastExistsButPasswordSignInFails_ReturnsFailedWithEmptyRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "castUser",
            Password = "pass",
            RememberMe = false,
            Role = CastString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CastExistsByUserIdAsync(user.Id))
            .ReturnsAsync(true);

        signInManagerMock
            .Setup(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.False);
        Assert.That(result.Role, Is.EqualTo(string.Empty));

        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false), Times.Once);
    }

    [Test]
    public async Task LoginStatusAsync_WhenCrewRoleAndCrewExists_ReturnsSucceededCrewRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "crewUser",
            Password = "pass",
            RememberMe = true,
            Role = CrewString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CrewExistsByUserIdAsync(user.Id))
            .ReturnsAsync(true);

        signInManagerMock
            .Setup(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.True);
        Assert.That(result.Role, Is.EqualTo(CrewString));

        userManagerMock.Verify(um => um.FindByNameAsync(dto.UserName), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(user, dto.Password), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CrewExistsByUserIdAsync(user.Id), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CastExistsByUserIdAsync(It.IsAny<Guid>()), Times.Never);
        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false), Times.Once);
    }

    [Test]
    public async Task LoginStatusAsync_WhenCastRoleAndCastExists_ReturnsSucceededCastRole()
    {
        // Arrange
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = "castUser",
            Password = "p",
            RememberMe = false,
            Role = CastString
        };

        ApplicationUser user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName
        };

        userManagerMock
            .Setup(um => um.FindByNameAsync(dto.UserName))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CastExistsByUserIdAsync(user.Id))
            .ReturnsAsync(true);

        signInManagerMock
            .Setup(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        LoginStatusDto result = await loginRegisterService.LoginStatusAsync(dto);

        // Assert
        Assert.That(result.IsSucceeded, Is.True);
        Assert.That(result.Role, Is.EqualTo(CastString));

        userManagerMock.Verify(um => um.FindByNameAsync(dto.UserName), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(user, dto.Password), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CastExistsByUserIdAsync(user.Id), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CrewExistsByUserIdAsync(It.IsAny<Guid>()), Times.Never);
        signInManagerMock.Verify(sim => sim.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false), Times.Once);
    }

    [Test]
    public async Task LogoutAsync_WhenCalled_SignsOut()
    {
        // Arrange
        signInManagerMock
            .Setup(sim => sim.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        await loginRegisterService.LogoutAsync();

        // Assert
        signInManagerMock.Verify(sim => sim.SignOutAsync(), Times.Once);
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        Mock<IUserStore<ApplicationUser>> storeMock = new Mock<IUserStore<ApplicationUser>>(MockBehavior.Loose);

        Mock<UserManager<ApplicationUser>> userManager = new Mock<UserManager<ApplicationUser>>
        (
            storeMock.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<ApplicationUser>>>()
        );

        userManager.SetReturnsDefault(Task.FromResult<ApplicationUser?>(null));

        return userManager;
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> userManager)
    {
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        httpContextAccessorMock
            .Setup(hca => hca.HttpContext)
            .Returns(httpContext);

        Mock<SignInManager<ApplicationUser>> signInManager = new Mock<SignInManager<ApplicationUser>>
        (
            userManager,
            httpContextAccessorMock.Object,
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<ApplicationUser>>()
        )
        {
            CallBase = false
        };

        return signInManager;
    }

    private static IFormFile CreateFormFile(string fileName, byte[] content)
    {
        MemoryStream stream = new MemoryStream(content);

        IFormFile file = new FormFile(stream, 0, content.Length, "file", fileName);

        return file;
    }
}
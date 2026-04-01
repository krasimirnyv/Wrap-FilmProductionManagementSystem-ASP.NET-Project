namespace Wrap.Services.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Moq;
using NUnit.Framework;

using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using Core.Handlers;
using Models.LoginAndRegistration;

using static GCommon.OutputMessages.Register;

[TestFixture]
public class CrewRegistrationHandlerTests
{
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private Mock<SignInManager<ApplicationUser>> signInManagerMock = null!;
    private Mock<ILoginRegisterRepository> loginRegisterRepositoryMock = null!;
    private Mock<ILogger<CrewRegistrationHandler>> loggerMock = null!;

    private CrewRegistrationHandler crewHandler = null!;

    [SetUp]
    public void SetUp()
    {
        userManagerMock = CreateUserManagerMock();
        signInManagerMock = CreateSignInManagerMock(userManagerMock.Object);

        loginRegisterRepositoryMock = new Mock<ILoginRegisterRepository>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<CrewRegistrationHandler>>(MockBehavior.Loose);

        crewHandler = new CrewRegistrationHandler
        (
            userManagerMock.Object,
            signInManagerMock.Object,
            loginRegisterRepositoryMock.Object,
            loggerMock.Object
        );
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenDtoIsNull_ReturnsFailedIdentityResult_AndDoesNotStartTransaction()
    {
        // Arrange
        CrewRegistrationCompleteDto? dto = null;

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(ErrorCreatingCrew));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Never);
        userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenDraftIsNull_ReturnsFailedIdentityResult_AndDoesNotStartTransaction()
    {
        // Arrange
        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = null,
            SkillNumbers = new List<int> { 1 }
        };

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(ErrorFoundingCrewDraft));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Never);
        userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenSkillNumbersIsNull_ReturnsFailedIdentityResult_AndDoesNotStartTransaction()
    {
        // Arrange
        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = ValidCrewDraft(),
            SkillNumbers = null
        };

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(NoSelectedSkills));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Never);
        userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenSkillNumbersIsEmpty_ReturnsFailedIdentityResult_AndDoesNotStartTransaction()
    {
        // Arrange
        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = ValidCrewDraft(),
            SkillNumbers = new List<int>()
        };

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(NoSelectedSkills));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Never);
        userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenUserManagerCreateFails_RollsBack_AndReturnsCreateResult()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = ValidCrewDraft(),
            SkillNumbers = new List<int> { 1, 2 }
        };

        IdentityResult createFail = IdentityResult.Failed(new IdentityError { Description = "create failed" });

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Draft.Password))
            .ReturnsAsync(createFail);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain("create failed"));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.RollbackTransactionAsync(transactionMock.Object), Times.Once);

        loginRegisterRepositoryMock.Verify(lrr => lrr.CreateCrewAsync(It.IsAny<Crew>()), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.AddCrewSkillsAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<int>>()), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.SaveAllChangesAsync(), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);

        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenSaveAllChangesLessThanExpected_RollsBack_DeletesUser_ReturnsFailed()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        IReadOnlyCollection<int> skills = new List<int> { 1, 2, 3 };

        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = ValidCrewDraft(),
            SkillNumbers = skills
        };

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Draft.Password))
            .ReturnsAsync(IdentityResult.Success);

        // PersistDomainDataAsync:
        // CreateCrew + AddCrewSkills
        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CreateCrewAsync(It.IsAny<Crew>()))
            .Returns(Task.CompletedTask);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.AddCrewSkillsAsync(It.IsAny<Guid>(), skills))
            .Returns(Task.CompletedTask);

        // expectedRows = 1 + skills.Count = 4; return 3 => failure
        loginRegisterRepositoryMock
            .Setup(lrr => lrr.SaveAllChangesAsync())
            .ReturnsAsync(3);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        userManagerMock
            .Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(RegistrationTransactionFailure));

        loginRegisterRepositoryMock.Verify(lrr => lrr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);

        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenAllStepsSucceed_CommitsAndSignsIn_ReturnsSuccess()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        IReadOnlyCollection<int> skills = new List<int> { 1, 2, 3 };

        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = ValidCrewDraft(),
            SkillNumbers = skills
        };

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        ApplicationUser? createdUser = null;
        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Draft.Password))
            .Callback<ApplicationUser, string>((user, _) => createdUser = user)
            .ReturnsAsync(IdentityResult.Success);

        Crew? capturedCrew = null;
        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CreateCrewAsync(It.IsAny<Crew>()))
            .Callback<Crew>(crew => capturedCrew = crew)
            .Returns(Task.CompletedTask);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.AddCrewSkillsAsync(It.IsAny<Guid>(), skills))
            .Returns(Task.CompletedTask);

        // expectedRows = 1 + skills.Count
        loginRegisterRepositoryMock
            .Setup(lrr => lrr.SaveAllChangesAsync())
            .ReturnsAsync(1 + skills.Count);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CommitTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        signInManagerMock
            .Setup(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.True);

        // domain mapping checks
        Assert.That(capturedCrew, Is.Not.Null);
        Assert.That(capturedCrew.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(capturedCrew.UserId, Is.EqualTo(createdUser!.Id));
        Assert.That(capturedCrew.ProfileImagePath, Is.EqualTo(dto.Draft.ProfilePicturePath));
        Assert.That(capturedCrew.FirstName, Is.EqualTo(dto.Draft.FirstName));
        Assert.That(capturedCrew.LastName, Is.EqualTo(dto.Draft.LastName));
        Assert.That(capturedCrew.Nickname, Is.EqualTo(dto.Draft.Nickname));
        Assert.That(capturedCrew.Biography, Is.EqualTo(dto.Draft.Biography));
        Assert.That(capturedCrew.IsActive, Is.True);
        Assert.That(capturedCrew.IsDeleted, Is.False);

        // transactional calls
        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CreateCrewAsync(It.IsAny<Crew>()), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.AddCrewSkillsAsync(capturedCrew.Id, skills), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.SaveAllChangesAsync(), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(transactionMock.Object), Times.Once);

        signInManagerMock.Verify(sm => sm.SignInAsync(createdUser, false, It.IsAny<string?>()), Times.Once);

        // ensure no rollback/delete on success
        loginRegisterRepositoryMock.Verify(lrr => lrr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenPersistDomainThrows_RollsBack_DeletesUser_ReturnsFailed()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        IReadOnlyCollection<int> skills = new List<int> { 1, 2 };

        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = ValidCrewDraft(),
            SkillNumbers = skills
        };

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Draft!.Password))
            .ReturnsAsync(IdentityResult.Success);

        // Force exception inside PersistDomainDataAsync
        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CreateCrewAsync(It.IsAny<Crew>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        userManagerMock
            .Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await crewHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(RegistrationTransactionFailure));

        loginRegisterRepositoryMock.Verify(lrr => lrr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);

        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    private static CrewRegistrationDraftDto ValidCrewDraft()
    {
        CrewRegistrationDraftDto dto = new CrewRegistrationDraftDto
        {
            UserName = "crew.user",
            Email = "crew@wrap.local",
            PhoneNumber = "+359888000000",
            Password = "Pass123!",
            FirstName = "Crew",
            LastName = "User",
            Nickname = "C",
            Biography = "bio",
            ProfilePicturePath = "/img/profile/crew.webp"
        };
        
        return dto;
    } 
    
    private static Mock<IDbContextTransaction> CreateTransactionMock()
    {
        Mock<IDbContextTransaction> transactionMock = new Mock<IDbContextTransaction>(MockBehavior.Strict);

        transactionMock
            .Setup(t => t.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        return transactionMock;
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        Mock<IUserStore<ApplicationUser>> store = new Mock<IUserStore<ApplicationUser>>();

        Mock<UserManager<ApplicationUser>> userManagerMock = new Mock<UserManager<ApplicationUser>>
        (
            store.Object,
            Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<ApplicationUser>>>()
        );
        
        return userManagerMock;
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(UserManager<ApplicationUser> userManager)
    {
        Mock<SignInManager<ApplicationUser>> signInManagerMock = new Mock<SignInManager<ApplicationUser>>
        (
            userManager,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
            Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<ApplicationUser>>()
        );
        
        return signInManagerMock;
    }
}
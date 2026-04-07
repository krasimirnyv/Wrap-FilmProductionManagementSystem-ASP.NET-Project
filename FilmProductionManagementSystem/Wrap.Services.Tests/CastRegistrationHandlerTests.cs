namespace Wrap.Services.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;
using NUnit.Framework;

using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using Core.Handlers;
using Core.Utilities.ImageLogic.Interfaces;
using Models.LoginAndRegistration;

using static GCommon.OutputMessages.Register;
using static GCommon.DataFormat;
using static GCommon.ApplicationConstants.IdentityRoles;

[TestFixture]
public class CastRegistrationHandlerTests
{
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private Mock<SignInManager<ApplicationUser>> signInManagerMock = null!;
    private Mock<IImageService> imageServiceMock = null!;
    private Mock<IVariantImageStrategyResolver> imageStrategyResolverMock = null!;
    private Mock<ILoginRegisterRepository> loginRegisterRepositoryMock = null!;
    private Mock<ILogger<CastRegistrationHandler>> loggerMock = null!;

    private CastRegistrationHandler castHandler = null!;

    [SetUp]
    public void SetUp()
    {
        userManagerMock = CreateUserManagerMock();
        signInManagerMock = CreateSignInManagerMock(userManagerMock.Object);

        imageServiceMock = new Mock<IImageService>(MockBehavior.Strict);
        imageStrategyResolverMock = new Mock<IVariantImageStrategyResolver>(MockBehavior.Strict);
        loginRegisterRepositoryMock = new Mock<ILoginRegisterRepository>(MockBehavior.Strict);

        loggerMock = new Mock<ILogger<CastRegistrationHandler>>(MockBehavior.Loose);

        castHandler = new CastRegistrationHandler
        (
            userManagerMock.Object,
            signInManagerMock.Object,
            imageServiceMock.Object,
            imageStrategyResolverMock.Object,
            loginRegisterRepositoryMock.Object,
            loggerMock.Object
        );
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenDtoIsNull_ReturnsFailedAndDoesNotTouchInfrastructure()
    {
        // Arrange
        CastRegistrationDto? dto = null;

        // Act
        IdentityResult result = await castHandler.CompleteRegistrationAsync(dto!);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(ErrorCreatingCrew));

        loginRegisterRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenIdentityCreateFails_RollsBackAndReturnsIdentityResult()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        CastRegistrationDto dto = ValidCastDto();

        IdentityResult createFail = IdentityResult.Failed(new IdentityError { Description = "create failed" });

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(createFail);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        IdentityResult result = await castHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain("create failed"));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password), Times.Once);
        loginRegisterRepositoryMock.Verify(repo => repo.RollbackTransactionAsync(transactionMock.Object), Times.Once);

        userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        imageStrategyResolverMock.Verify(isr => isr.Resolve(It.IsAny<string>()), Times.Never);
        imageServiceMock.Verify(img => img.SaveImageAsync(It.IsAny<IFormFile?>(), It.IsAny<IVariantImageStrategy>(), It.IsAny<CancellationToken>()), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CreateCastAsync(It.IsAny<Cast>()), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.SaveAllChangesAsync(), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenRoleAssignmentFails_RollsBackDeletesUserAndReturnsIdentityResult()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        CastRegistrationDto dto = ValidCastDto();

        IdentityResult roleFail = IdentityResult.Failed(new IdentityError { Description = "role failed" });

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor))
            .ReturnsAsync(roleFail);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        userManagerMock
            .Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await castHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain("role failed"));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password), Times.Once);
        userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor), Times.Once);
        loginRegisterRepositoryMock.Verify(repo => repo.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);

        imageStrategyResolverMock.Verify(isr => isr.Resolve(It.IsAny<string>()), Times.Never);
        imageServiceMock.Verify(img => img.SaveImageAsync(It.IsAny<IFormFile?>(), It.IsAny<IVariantImageStrategy>(), It.IsAny<CancellationToken>()), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CreateCastAsync(It.IsAny<Cast>()), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.SaveAllChangesAsync(), Times.Never);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenSaveAllChangesLessThanExpected_RollsBackDeletesUserAndReturnsFailed()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        CastRegistrationDto dto = ValidCastDto();

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        const string savedPath = "/img/profile/cast.webp";

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor))
            .ReturnsAsync(IdentityResult.Success);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedPath);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CreateCastAsync(It.IsAny<Cast>()))
            .Returns(Task.CompletedTask);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.SaveAllChangesAsync())
            .ReturnsAsync(0);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        userManagerMock
            .Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await castHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(RegistrationTransactionFailure));

        userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor), Times.Once);
        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()), Times.Once);

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CreateCastAsync(It.IsAny<Cast>()), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.SaveAllChangesAsync(), Times.Once);
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

        CastRegistrationDto dto = ValidCastDto();

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        const string savedPath = "/img/profile/cast.webp";

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor))
            .ReturnsAsync(IdentityResult.Success);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedPath);

        Cast? capturedCast = null;
        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CreateCastAsync(It.IsAny<Cast>()))
            .Callback<Cast>(c => capturedCast = c)
            .Returns(Task.CompletedTask);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.SaveAllChangesAsync())
            .ReturnsAsync(1);

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.CommitTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        signInManagerMock
            .Setup(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), false, null))
            .Returns(Task.CompletedTask);

        // Act
        IdentityResult result = await castHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.True);

        userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor), Times.Once);
        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.SaveImageAsync(dto.ProfilePicture, strategy, It.IsAny<CancellationToken>()), Times.Once);

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CreateCastAsync(It.IsAny<Cast>()), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.SaveAllChangesAsync(), Times.Once);
        loginRegisterRepositoryMock.Verify(lrr => lrr.CommitTransactionAsync(transactionMock.Object), Times.Once);

        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Once);

        Assert.That(capturedCast, Is.Not.Null);
        Assert.That(capturedCast.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(capturedCast.UserId, Is.EqualTo(Guid.Empty));
        Assert.That(capturedCast.ProfileImagePath, Is.EqualTo(savedPath));
        Assert.That(capturedCast.FirstName, Is.EqualTo(dto.FirstName));
        Assert.That(capturedCast.LastName, Is.EqualTo(dto.LastName));
        Assert.That(capturedCast.Nickname, Is.EqualTo(dto.Nickname));
        Assert.That(capturedCast.Biography, Is.EqualTo(dto.Biography));
        Assert.That(capturedCast.BirthDate, Is.EqualTo(dto.BirthDate));
        Assert.That(capturedCast.Gender, Is.EqualTo(dto.Gender));
        Assert.That(capturedCast.IsActive, Is.False);
        Assert.That(capturedCast.IsDeleted, Is.False);

        loginRegisterRepositoryMock.Verify(lrr => lrr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Test]
    public async Task CompleteRegistrationAsync_WhenPersistDomainThrows_RollsBackDeletesUserAndReturnsFailed()
    {
        // Arrange
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        CastRegistrationDto dto = ValidCastDto();

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor))
            .ReturnsAsync(IdentityResult.Success);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Throws(new InvalidOperationException("boom"));

        loginRegisterRepositoryMock
            .Setup(lrr => lrr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        userManagerMock
            .Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        IdentityResult result = await castHandler.CompleteRegistrationAsync(dto);

        // Assert
        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.Select(e => e.Description), Does.Contain(RegistrationTransactionFailure));

        loginRegisterRepositoryMock.Verify(lrr => lrr.BeginTransactionAsync(), Times.Once);
        userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), Actor), Times.Once);
        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);

        loginRegisterRepositoryMock.Verify(lrr => lrr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);

        loginRegisterRepositoryMock.Verify(repo => repo.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        signInManagerMock.Verify(sm => sm.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string?>()), Times.Never);
    }

    private static CastRegistrationDto ValidCastDto()
    {
        CastRegistrationDto dto = new CastRegistrationDto
        {
            UserName = "cast.user",
            Email = "cast@wrap.local",
            PhoneNumber = "+359888000111",
            Password = "Pass123!",
            FirstName = "A",
            LastName = "B",
            Nickname = "N",
            BirthDate = new DateTime(2000, 1, 1),
            Gender = GCommon.Enums.GenderType.Male,
            ProfilePicture = CreateFormFile("pic.png", [1, 2, 3]),
            Biography = "bio"
        };
        
        return dto;
    }

    private static IFormFile CreateFormFile(string fileName, byte[] content)
    {
        MemoryStream stream = new MemoryStream(content);
        IFormFile file = new FormFile(stream, 0, content.Length, "file", fileName);

        return file;
    }

    private static Mock<IDbContextTransaction> CreateTransactionMock()
    {
        Mock<IDbContextTransaction> transactionMock = new Mock<IDbContextTransaction>(MockBehavior.Strict);

        transactionMock
            .Setup(tx => tx.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        return transactionMock;
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        Mock<IUserStore<ApplicationUser>> store = new Mock<IUserStore<ApplicationUser>>();

        Mock<UserManager<ApplicationUser>> userManagerMock = new Mock<UserManager<ApplicationUser>>
        (
            store.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
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
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<ApplicationUser>>()
        );

        return signInManagerMock;
    }
}
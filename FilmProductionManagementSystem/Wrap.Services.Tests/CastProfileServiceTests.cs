namespace Wrap.Services.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Moq;
using NUnit.Framework;

using Data.Models;
using Data.Models.Infrastructure;
using Data.Models.MappingEntities;
using Data.Repository.Interfaces;
using Core;
using Core.Utilities.ImageLogic.Interfaces;
using Models.Profile;
using Models.Profile.NestedDtos;
using GCommon.Enums;

using static GCommon.OutputMessages.Profile;
using static GCommon.DataFormat;

[TestFixture]
public class CastProfileServiceTests
{
    private Mock<IProfileRepository> profileRepositoryMock = null!;
    private Mock<IImageService> imageServiceMock = null!;
    private Mock<IVariantImageStrategyResolver> imageStrategyResolverMock = null!;
    private Mock<ILogger<CastProfileService>> loggerMock = null!;

    private CastProfileService castProfileService = null!;

    [SetUp]
    public void SetUp()
    {
        profileRepositoryMock = new Mock<IProfileRepository>(MockBehavior.Strict);
        imageServiceMock = new Mock<IImageService>(MockBehavior.Strict);
        imageStrategyResolverMock = new Mock<IVariantImageStrategyResolver>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<CastProfileService>>(MockBehavior.Loose);

        castProfileService = new CastProfileService
        (
            profileRepositoryMock.Object,
            imageServiceMock.Object,
            imageStrategyResolverMock.Object,
            loggerMock.Object
        );
    }

    [Test]
    public void GetCastProfileDataAsync_WhenCastNotFound_ThrowsArgumentNullExceptionWithMessage()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsync(username))
            .ReturnsAsync((Cast?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => castProfileService.GetCastProfileDataAsync(username));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CastNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetCastProfileDataAsync_WhenCastExists_MapsProfileAndCollectionsCorrectly()
    {
        // Arrange
        const string username = "cast.user";

        Cast cast = CreateCast(
            username: username,
            email: "cast@wrap.local",
            phone: "+359888000111",
            firstName: "A",
            lastName: "B",
            nickname: null, // should become EmptyNickname
            profileImagePath: "/img/profile/cast.webp",
            birthDate: new DateTime(2000, 1, 1),
            isActive: true,
            biography: "bio");

        Production production1 = new Production
        {
            Id = Guid.NewGuid(),
            Title = "Production One",
            Description = "desc",
            StatusType = ProductionStatusType.Production,
        };

        Production production2 = new Production
        {
            Id = Guid.NewGuid(),
            Title = "Production Two",
            Description = null,
            StatusType = ProductionStatusType.PostProduction,
        };

        Scene scene1 = new Scene
        {
            Id = Guid.NewGuid(),
            SceneName = "Scene 1",
            Production = production1,
            ProductionId = production1.Id,
            Location = "loc",
            SceneType = SceneType.Interior,
        };

        Scene scene2 = new Scene
        {
            Id = Guid.NewGuid(),
            SceneName = "Scene 2",
            Production = production2,
            ProductionId = production2.Id,
            Location = "loc2",
            SceneType = SceneType.Exterior,
        };

        ProductionCast pc1 = new ProductionCast
        {
            ProductionId = production1.Id,
            Production = production1,
            CastMemberId = cast.Id,
            CastMember = cast,
            Role = "Detective"
        };

        ProductionCast pc2 = new ProductionCast
        {
            ProductionId = production2.Id,
            Production = production2,
            CastMemberId = cast.Id,
            CastMember = cast,
            Role = "Villain"
        };

        SceneCast sc1 = new SceneCast
        {
            SceneId = scene1.Id,
            Scene = scene1,
            CastMemberId = cast.Id,
            CastMember = cast,
            Role = "Detective"
        };

        SceneCast sc2 = new SceneCast
        {
            SceneId = scene2.Id,
            Scene = scene2,
            CastMemberId = cast.Id,
            CastMember = cast,
            Role = "Villain"
        };

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsync(username))
            .ReturnsAsync(cast);

        profileRepositoryMock
            .Setup(pr => pr.GetCastProductionsAsync(cast.Id))
            .ReturnsAsync(new List<ProductionCast> { pc1, pc2 });

        profileRepositoryMock
            .Setup(pr => pr.GetCastScenesAsync(cast.Id))
            .ReturnsAsync(new List<SceneCast> { sc1, sc2 });

        // Act
        CastProfileDto result = await castProfileService.GetCastProfileDataAsync(username);

        // Assert
        Assert.That(result.FirstName, Is.EqualTo(cast.FirstName));
        Assert.That(result.LastName, Is.EqualTo(cast.LastName));
        Assert.That(result.ProfileImagePath, Is.EqualTo(cast.ProfileImagePath));
        Assert.That(result.Nickname, Is.EqualTo(EmptyNickname));
        Assert.That(result.UserName, Is.EqualTo(cast.User.UserName));
        Assert.That(result.Email, Is.EqualTo(cast.User.Email));
        Assert.That(result.PhoneNumber, Is.EqualTo(cast.User.PhoneNumber));
        Assert.That(result.IsActive, Is.EqualTo(cast.IsActive));
        Assert.That(result.Biography, Is.EqualTo(cast.Biography));

        Assert.That(result.Age, Is.EqualTo(cast.Age.ToString()));
        Assert.That(result.Gender, Is.EqualTo(cast.Gender.ToString()));

        Assert.That(result.Productions.Count, Is.EqualTo(2));
        
        CastMemberProductionDto prodDto1 = result.Productions.Single(p => p.ProductionTitle == "Production One");
        Assert.That(prodDto1.ProductionId, Is.EqualTo(production1.Id.ToString()));
        Assert.That(prodDto1.ProjectStatus, Is.EqualTo(production1.StatusType.ToString()));
        Assert.That(prodDto1.CharacterName, Is.EqualTo("Detective"));

        CastMemberProductionDto prodDto2 = result.Productions.Single(p => p.ProductionTitle == "Production Two");
        Assert.That(prodDto2.ProductionId, Is.EqualTo(production2.Id.ToString()));
        Assert.That(prodDto2.ProjectStatus, Is.EqualTo(production2.StatusType.ToString()));
        Assert.That(prodDto2.CharacterName, Is.EqualTo("Villain"));

        Assert.That(result.Scenes.Count, Is.EqualTo(2));
        
        CastMemberSceneDto sceneDto1 = result.Scenes.Single(s => s.SceneName == "Scene 1");
        Assert.That(sceneDto1.SceneId, Is.EqualTo(scene1.Id.ToString()));
        Assert.That(sceneDto1.ProductionTitle, Is.EqualTo("Production One"));
        Assert.That(sceneDto1.CharacterName, Is.EqualTo("Detective"));

        CastMemberSceneDto sceneDto2 = result.Scenes.Single(s => s.SceneName == "Scene 2");
        Assert.That(sceneDto2.SceneId, Is.EqualTo(scene2.Id.ToString()));
        Assert.That(sceneDto2.ProductionTitle, Is.EqualTo("Production Two"));
        Assert.That(sceneDto2.CharacterName, Is.EqualTo("Villain"));

        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastProductionsAsync(cast.Id), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastScenesAsync(cast.Id), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public void GetEditCastProfileAsync_WhenCastNotFound_ThrowsArgumentNullExceptionWithMessage()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsync(username))
            .ReturnsAsync((Cast?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => castProfileService.GetEditCastProfileAsync(username));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CastNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetEditCastProfileAsync_WhenCastExists_MapsEditDtoCorrectly()
    {
        // Arrange
        const string username = "cast.user";

        Cast cast = CreateCast(
            username: username,
            email: "cast@wrap.local",
            phone: "+359888000111",
            firstName: "A",
            lastName: "B",
            nickname: "Nick",
            profileImagePath: "/img/profile/cast.webp",
            birthDate: new DateTime(2000, 1, 1),
            isActive: true,
            biography: "bio");

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsync(username))
            .ReturnsAsync(cast);

        // Act
        EditCastProfileDto result = await castProfileService.GetEditCastProfileAsync(username);

        // Assert
        Assert.That(result.FirstName, Is.EqualTo(cast.FirstName));
        Assert.That(result.LastName, Is.EqualTo(cast.LastName));
        Assert.That(result.Nickname, Is.EqualTo(cast.Nickname));
        Assert.That(result.PhoneNumber, Is.EqualTo(cast.User.PhoneNumber));
        Assert.That(result.Biography, Is.EqualTo(cast.Biography));

        Assert.That(result.ProfileImage, Is.Null);

        Assert.That(result.Email, Is.EqualTo(cast.User.Email));
        Assert.That(result.CurrentProfileImagePath, Is.EqualTo(cast.ProfileImagePath));
        Assert.That(result.Age, Is.EqualTo(cast.Age.ToString()));
        Assert.That(result.Gender, Is.EqualTo(cast.Gender.ToString()));

        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public void UpdateCastProfileAsync_WhenCastNotFound_RollsBackAndThrowsException()
    {
        // Arrange
        const string username = "missing.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        EditCastProfileDto castDto = new EditCastProfileDto
        {
            FirstName = "New",
            LastName = "Name",
            Nickname = "Nick",
            Biography = "bio",
            PhoneNumber = "+359",
            ProfileImage = null,
            CurrentProfileImagePath = "/img/profile/old.webp"
        };

        profileRepositoryMock
            .Setup(pr => pr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        profileRepositoryMock
            .Setup(pr => pr.GetCastForUpdateAsync(username))
            .ReturnsAsync((Cast?)null);

        profileRepositoryMock
            .Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(() => castProfileService.UpdateCastProfileAsync(username, castDto));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CastNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.BeginTransactionAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastForUpdateAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateCastProfileAsync_WhenNoNewImage_UpdatesFields_SavesAndCommits()
    {
        // Arrange
        const string username = "cast.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Cast cast = CreateCast(
            username: username,
            email: "cast@wrap.local",
            phone: "+359888000111",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            birthDate: new DateTime(2000, 1, 1),
            isActive: true,
            biography: "old bio");

        EditCastProfileDto castDto = new EditCastProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = null, // no image branch
            CurrentProfileImagePath = cast.ProfileImagePath
        };

        profileRepositoryMock
            .Setup(pr => pr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        profileRepositoryMock
            .Setup(pr => pr.GetCastForUpdateAsync(username))
            .ReturnsAsync(cast);

        profileRepositoryMock
            .Setup(pr => pr.SaveAllChangesAsync())
            .ReturnsAsync(1);

        profileRepositoryMock
            .Setup(pr => pr.CommitTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        await castProfileService.UpdateCastProfileAsync(username, castDto);

        // Assert
        Assert.That(cast.FirstName, Is.EqualTo("NewFirst"));
        Assert.That(cast.LastName, Is.EqualTo("NewLast"));
        Assert.That(cast.Nickname, Is.EqualTo("NewNick"));
        Assert.That(cast.Biography, Is.EqualTo("new bio"));
        Assert.That(cast.User.PhoneNumber, Is.EqualTo("+359999"));
        Assert.That(cast.ProfileImagePath, Is.EqualTo("/img/profile/old.webp")); // unchanged

        profileRepositoryMock.Verify(pr => pr.BeginTransactionAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastForUpdateAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageStrategyResolverMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateCastProfileAsync_WhenNewImageProvided_ReplacesImage_SavesAndCommits()
    {
        // Arrange
        const string username = "cast.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Cast cast = CreateCast(
            username: username,
            email: "cast@wrap.local",
            phone: "+359888000111",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            birthDate: new DateTime(2000, 1, 1),
            isActive: true,
            biography: "old bio");

        IFormFile newImage = CreateFormFile("pic.png", [1, 2, 3]);

        EditCastProfileDto castDto = new EditCastProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = newImage, // image branch
            CurrentProfileImagePath = cast.ProfileImagePath
        };

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        const string newWebPath = "/img/profile/new.webp";

        profileRepositoryMock
            .Setup(pr => pr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        profileRepositoryMock
            .Setup(pr => pr.GetCastForUpdateAsync(username))
            .ReturnsAsync(cast);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.ReplaceAsync(castDto.CurrentProfileImagePath, castDto.ProfileImage, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newWebPath);

        profileRepositoryMock
            .Setup(pr => pr.SaveAllChangesAsync())
            .ReturnsAsync(1);

        profileRepositoryMock
            .Setup(pr => pr.CommitTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        await castProfileService.UpdateCastProfileAsync(username, castDto);

        // Assert
        Assert.That(cast.ProfileImagePath, Is.EqualTo(newWebPath));

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.ReplaceAsync(castDto.CurrentProfileImagePath, castDto.ProfileImage, strategy, It.IsAny<CancellationToken>()), Times.Once);

        profileRepositoryMock.Verify(pr => pr.BeginTransactionAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastForUpdateAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageStrategyResolverMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateCastProfileAsync_WhenImageReplaceThrowsNotSupported_RollsBackAndRethrowsNotSupported()
    {
        // Arrange
        const string username = "cast.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Cast cast = CreateCast(
            username: username,
            email: "cast@wrap.local",
            phone: "+359888000111",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            birthDate: new DateTime(2000, 1, 1),
            isActive: true,
            biography: "old bio");

        IFormFile newImage = CreateFormFile("pic.png", [1, 2, 3]);

        EditCastProfileDto castDto = new EditCastProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = newImage,
            CurrentProfileImagePath = cast.ProfileImagePath
        };

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        NotSupportedException nse = new NotSupportedException("bad image");

        profileRepositoryMock
            .Setup(pr => pr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        profileRepositoryMock
            .Setup(pr => pr.GetCastForUpdateAsync(username))
            .ReturnsAsync(cast);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.ReplaceAsync(castDto.CurrentProfileImagePath, castDto.ProfileImage, strategy, It.IsAny<CancellationToken>()))
            .ThrowsAsync(nse);

        profileRepositoryMock
            .Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        NotSupportedException? ex = Assert.ThrowsAsync<NotSupportedException>(() => castProfileService.UpdateCastProfileAsync(username, castDto));

        // Assert
        Assert.That(ex.Message, Does.Contain("bad image"));
        Assert.That(ex.InnerException, Is.Not.Null);

        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.ReplaceAsync(castDto.CurrentProfileImagePath, castDto.ProfileImage, strategy, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void UpdateCastProfileAsync_WhenSaveAllChangesThrowsException_RollsBackAndThrowsException()
    {
        // Arrange
        const string username = "cast.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Cast cast = CreateCast(
            username: username,
            email: "cast@wrap.local",
            phone: "+359888000111",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            birthDate: new DateTime(2000, 1, 1),
            isActive: true,
            biography: "old bio");

        EditCastProfileDto castDto = new EditCastProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = null,
            CurrentProfileImagePath = cast.ProfileImagePath
        };

        profileRepositoryMock
            .Setup(pr => pr.BeginTransactionAsync())
            .ReturnsAsync(transactionMock.Object);

        profileRepositoryMock
            .Setup(pr => pr.GetCastForUpdateAsync(username))
            .ReturnsAsync(cast);

        profileRepositoryMock
            .Setup(pr => pr.SaveAllChangesAsync())
            .ThrowsAsync(new InvalidOperationException("db fail"));

        profileRepositoryMock
            .Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(() => castProfileService.UpdateCastProfileAsync(username, castDto));

        // Assert
        Assert.That(ex.Message, Does.Contain("db fail"));

        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    // ---------------------------
    // Helpers
    // ---------------------------

    private static Cast CreateCast(string username, string email, string phone, string firstName, string lastName, 
        string? nickname, string profileImagePath, DateTime birthDate, bool isActive, string? biography)
    {
        Cast newCast = new Cast
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Nickname = nickname,
            ProfileImagePath = profileImagePath,
            BirthDate = birthDate,
            Gender = GenderType.Male,
            IsActive = isActive,
            IsDeleted = false,
            Biography = biography,
            User = new ApplicationUser
            {
                UserName = username,
                Email = email,
                PhoneNumber = phone
            }
        };
        
        return newCast;
    }

    private static Mock<IDbContextTransaction> CreateTransactionMock()
    {
        Mock<IDbContextTransaction> transactionMock = new Mock<IDbContextTransaction>(MockBehavior.Strict);
        transactionMock.Setup(tx => tx.DisposeAsync()).Returns(ValueTask.CompletedTask);
        
        return transactionMock;
    }

    private static IFormFile CreateFormFile(string fileName, byte[] content)
    {
        MemoryStream stream = new MemoryStream(content);
        IFormFile file = new FormFile(stream, 0, content.Length, "file", fileName);
        
        return file;
    }
}
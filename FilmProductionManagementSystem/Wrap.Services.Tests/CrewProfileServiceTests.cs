namespace Wrap.Services.Tests;

using System.Security.Claims;

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
using Data.Models.MappingEntities;
using Data.Repository.Interfaces;
using Data.Dtos.Crew;
using Core;
using Core.Utilities.ImageLogic.Interfaces;
using Models.Profile;
using Models.Profile.NestedDtos;

using GCommon.Enums;
using GCommon.UI;

using static GCommon.OutputMessages.Profile;
using static GCommon.DataFormat;

[TestFixture]
public class CrewProfileServiceTests
{
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private Mock<SignInManager<ApplicationUser>> signInManagerMock = null!;
    private Mock<IProfileRepository> profileRepositoryMock = null!;
    private Mock<IImageService> imageServiceMock = null!;
    private Mock<IVariantImageStrategyResolver> imageStrategyResolverMock = null!;
    private Mock<ILogger<CrewProfileService>> loggerMock = null!;

    private CrewProfileService crewProfileService = null!;

    [SetUp]
    public void SetUp()
    {
        userManagerMock = CreateUserManagerMock();
        signInManagerMock = CreateSignInManagerMock(userManagerMock.Object);
        
        profileRepositoryMock = new Mock<IProfileRepository>(MockBehavior.Strict);
        imageServiceMock = new Mock<IImageService>(MockBehavior.Strict);
        imageStrategyResolverMock = new Mock<IVariantImageStrategyResolver>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<CrewProfileService>>(MockBehavior.Loose);

        crewProfileService = new CrewProfileService
        (
            userManagerMock.Object,
            signInManagerMock.Object,
            profileRepositoryMock.Object,
            imageServiceMock.Object,
            imageStrategyResolverMock.Object,
            loggerMock.Object
        );
    }

    [Test]
    public void GetCrewProfileDataAsync_WhenCrewNotFound_ThrowsArgumentNullExceptionWithMessage()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.GetCrewProfileDataAsync(username));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CrewNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetCrewProfileDataAsync_WhenCrewExists_MapsProfileAndGroupsSkillsAndMapsCollections()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: null, // should become EmptyNickname
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        // Pick roles from the real catalog so the grouping logic is deterministic
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> catalog = CrewRolesDepartmentCatalog.GetRolesByDepartment();
        KeyValuePair<string, IReadOnlyCollection<CrewRoleType>> firstDept = catalog.First();
        string dept1 = firstDept.Key;
        CrewRoleType roleFromDept1 = firstDept.Value.First();

        KeyValuePair<string, IReadOnlyCollection<CrewRoleType>> secondDept = catalog.Skip(1).First();
        string dept2 = secondDept.Key;
        CrewRoleType roleFromDept2 = secondDept.Value.First();

        IReadOnlyCollection<CrewSkill> skills = new List<CrewSkill>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id,
                RoleType = roleFromDept1,
                CrewMember = crew
            },
            new()
            {
                Id = Guid.NewGuid(), 
                CrewMemberId = crew.Id,
                RoleType = roleFromDept2, 
                CrewMember = crew
            }
        };

        Production production = new Production
        {
            Id = Guid.NewGuid(),
            Title = "Production One",
            StatusType = ProductionStatusType.Production
        };

        Scene scene = new Scene
        {
            Id = Guid.NewGuid(),
            SceneName = "Scene 1",
            Production = production,
            ProductionId = production.Id,
            Location = "loc",
            SceneType = SceneType.Interior
        };

        ProductionCrew productionCrew = new ProductionCrew
        {
            ProductionId = production.Id,
            Production = production,
            CrewMemberId = crew.Id,
            CrewMember = crew,
            RoleType = roleFromDept1
        };

        SceneCrew sceneCrew = new SceneCrew
        {
            SceneId = scene.Id,
            Scene = scene,
            CrewMemberId = crew.Id,
            CrewMember = crew,
            RoleType = roleFromDept2
        };

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(crew);

        profileRepositoryMock
            .Setup(pr => pr.GetCrewSkillsAsync(crew.Id))
            .ReturnsAsync(skills);

        profileRepositoryMock
            .Setup(pr => pr.GetCrewProductionsAsync(crew.Id))
            .ReturnsAsync(new List<ProductionCrew> { productionCrew });

        profileRepositoryMock
            .Setup(pr => pr.GetCrewScenesAsync(crew.Id))
            .ReturnsAsync(new List<SceneCrew> { sceneCrew });

        // Act
        CrewProfileDto result = await crewProfileService.GetCrewProfileDataAsync(username);

        // Assert base mapping
        Assert.That(result.FirstName, Is.EqualTo(crew.FirstName));
        Assert.That(result.LastName, Is.EqualTo(crew.LastName));
        Assert.That(result.ProfileImagePath, Is.EqualTo(crew.ProfileImagePath));
        Assert.That(result.Nickname, Is.EqualTo(EmptyNickname));
        Assert.That(result.UserName, Is.EqualTo(crew.User.UserName));
        Assert.That(result.Email, Is.EqualTo(crew.User.Email));
        Assert.That(result.PhoneNumber, Is.EqualTo(crew.User.PhoneNumber));
        Assert.That(result.IsActive, Is.EqualTo(crew.IsActive));
        Assert.That(result.Biography, Is.EqualTo(crew.Biography));

        // Assert department grouping
        Assert.That(result.DepartmentSkills.ContainsKey(dept1), Is.True);
        Assert.That(result.DepartmentSkills[dept1], Does.Contain(roleFromDept1));

        Assert.That(result.DepartmentSkills.ContainsKey(dept2), Is.True);
        Assert.That(result.DepartmentSkills[dept2], Does.Contain(roleFromDept2));

        // productions mapping
        Assert.That(result.Productions.Count, Is.EqualTo(1));
        CrewMemberProductionDto prodDto = result.Productions.Single();
        Assert.That(prodDto.ProductionId, Is.EqualTo(production.Id.ToString()));
        Assert.That(prodDto.ProductionTitle, Is.EqualTo(production.Title));
        Assert.That(prodDto.ProjectStatus, Is.EqualTo(production.StatusType.ToString()));
        Assert.That(prodDto.RoleType, Is.EqualTo(roleFromDept1));

        // scenes mapping
        Assert.That(result.Scenes.Count, Is.EqualTo(1));
        CrewMemberSceneDto sceneDto = result.Scenes.Single();
        Assert.That(sceneDto.SceneId, Is.EqualTo(scene.Id.ToString()));
        Assert.That(sceneDto.SceneName, Is.EqualTo(scene.SceneName));
        Assert.That(sceneDto.ProductionTitle, Is.EqualTo(production.Title));
        Assert.That(sceneDto.RoleType, Is.EqualTo(roleFromDept2));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewSkillsAsync(crew.Id), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewProductionsAsync(crew.Id), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewScenesAsync(crew.Id), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public void GetEditCrewProfileAsync_WhenCrewNotFound_ThrowsArgumentNullExceptionWithMessage()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.GetEditCrewProfileAsync(username));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CrewNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetEditCrewProfileAsync_WhenCrewExists_MapsEditDtoCorrectly()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(crew);

        // Act
        EditCrewProfileDto result = await crewProfileService.GetEditCrewProfileAsync(username);

        // Assert
        Assert.That(result.FirstName, Is.EqualTo(crew.FirstName));
        Assert.That(result.LastName, Is.EqualTo(crew.LastName));
        Assert.That(result.Nickname, Is.EqualTo(crew.Nickname));
        Assert.That(result.PhoneNumber, Is.EqualTo(crew.User.PhoneNumber));
        Assert.That(result.Biography, Is.EqualTo(crew.Biography));
        Assert.That(result.ProfileImage, Is.Null);

        Assert.That(result.Email, Is.EqualTo(crew.User.Email));
        Assert.That(result.CurrentProfileImagePath, Is.EqualTo(crew.ProfileImagePath));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public void UpdateCrewProfileAsync_WhenCrewNotFound_RollsBackAndThrowsException()
    {
        // Arrange
        const string username = "missing.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        EditCrewProfileDto crewDto = new EditCrewProfileDto
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
            .Setup(pr => pr.GetCrewByUsernameAsync(username))
            .ReturnsAsync((Crew?)null);

        profileRepositoryMock
            .Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object))
            .Returns(Task.CompletedTask);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(
            () => crewProfileService.UpdateCrewProfileAsync(username, crewDto));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CrewNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.BeginTransactionAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateCrewProfileAsync_WhenNoNewImage_UpdatesFields_SavesAndCommits()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            isActive: true,
            biography: "old bio");

        EditCrewProfileDto crewDto = new EditCrewProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = null,
            CurrentProfileImagePath = crew.ProfileImagePath
        };

        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);
        profileRepositoryMock.Setup(pr => pr.CommitTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        await crewProfileService.UpdateCrewProfileAsync(username, crewDto);

        // Assert
        Assert.That(crew.FirstName, Is.EqualTo("NewFirst"));
        Assert.That(crew.LastName, Is.EqualTo("NewLast"));
        Assert.That(crew.Nickname, Is.EqualTo("NewNick"));
        Assert.That(crew.Biography, Is.EqualTo("new bio"));
        Assert.That(crew.User.PhoneNumber, Is.EqualTo("+359999"));
        Assert.That(crew.ProfileImagePath, Is.EqualTo("/img/profile/old.webp"));

        profileRepositoryMock.Verify(pr => pr.BeginTransactionAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateCrewProfileAsync_WhenNewImageProvided_ReplacesImage_SavesAndCommits()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            isActive: true,
            biography: "old bio");

        IFormFile newImage = CreateFormFile("pic.png", [1, 2, 3]);

        EditCrewProfileDto crewDto = new EditCrewProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = newImage,
            CurrentProfileImagePath = crew.ProfileImagePath
        };

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        const string newWebPath = "/img/profile/new.webp";

        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsync(username)).ReturnsAsync(crew);

        imageStrategyResolverMock.Setup(isr => isr.Resolve(ProfileFolderName)).Returns(strategy);

        imageServiceMock
            .Setup(img => img.ReplaceAsync(crewDto.CurrentProfileImagePath, crewDto.ProfileImage, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newWebPath);

        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);
        profileRepositoryMock.Setup(pr => pr.CommitTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        await crewProfileService.UpdateCrewProfileAsync(username, crewDto);

        // Assert
        Assert.That(crew.ProfileImagePath, Is.EqualTo(newWebPath));

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.ReplaceAsync(crewDto.CurrentProfileImagePath, crewDto.ProfileImage, strategy, It.IsAny<CancellationToken>()), Times.Once);

        profileRepositoryMock.Verify(pr => pr.BeginTransactionAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateCrewProfileAsync_WhenImageReplaceThrowsNotSupported_RollsBackAndRethrowsNotSupported()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            isActive: true,
            biography: "old bio");

        IFormFile newImage = CreateFormFile("pic.png", [1, 2, 3]);

        EditCrewProfileDto crewDto = new EditCrewProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = newImage,
            CurrentProfileImagePath = crew.ProfileImagePath
        };

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        NotSupportedException nse = new NotSupportedException("bad image");

        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsync(username)).ReturnsAsync(crew);

        imageStrategyResolverMock.Setup(isr => isr.Resolve(ProfileFolderName)).Returns(strategy);

        imageServiceMock
            .Setup(img => img.ReplaceAsync(crewDto.CurrentProfileImagePath, crewDto.ProfileImage, strategy, It.IsAny<CancellationToken>()))
            .ThrowsAsync(nse);

        profileRepositoryMock.Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        NotSupportedException? ex = Assert.ThrowsAsync<NotSupportedException>(
            () => crewProfileService.UpdateCrewProfileAsync(username, crewDto));

        // Assert
        Assert.That(ex.Message, Does.Contain("bad image"));
        Assert.That(ex.InnerException, Is.Not.Null);

        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.ReplaceAsync(crewDto.CurrentProfileImagePath, crewDto.ProfileImage, strategy, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void UpdateCrewProfileAsync_WhenSaveAllChangesThrowsException_RollsBackAndThrowsException()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Old",
            lastName: "Old",
            nickname: "OldNick",
            profileImagePath: "/img/profile/old.webp",
            isActive: true,
            biography: "old bio");

        EditCrewProfileDto crewDto = new EditCrewProfileDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Nickname = "NewNick",
            Biography = "new bio",
            PhoneNumber = "+359999",
            ProfileImage = null,
            CurrentProfileImagePath = crew.ProfileImagePath
        };

        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsync(username)).ReturnsAsync(crew);

        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ThrowsAsync(new InvalidOperationException("db fail"));
        profileRepositoryMock.Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(
            () => crewProfileService.UpdateCrewProfileAsync(username, crewDto));

        // Assert
        Assert.That(ex.Message, Does.Contain("db fail"));

        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public void GetEditSkillsAsync_WhenCrewNotFound_ThrowsArgumentNullException()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.GetEditSkillsAsync(username));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetEditSkillsAsync_WhenCrewExists_ReturnsCurrentSkillsAndCatalog()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        IReadOnlyCollection<CrewSkill> crewSkills = new List<CrewSkill>
        {
            new()
            {
                Id = Guid.NewGuid(), 
                CrewMemberId = crew.Id,
                RoleType = CrewRoleType.Director, 
                CrewMember = crew
            },
            new()
            {
                Id = Guid.NewGuid(), 
                CrewMemberId = crew.Id,
                RoleType = CrewRoleType.Gaffer,
                CrewMember = crew
            }
        };

        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.GetCrewSkillsAsync(crew.Id)).ReturnsAsync(crewSkills);

        // Act
        EditSkillsDto result = await crewProfileService.GetEditSkillsAsync(username);

        // Assert
        Assert.That(result.CurrentSkills, Has.Count.EqualTo(2));
        Assert.That(result.CurrentSkills, Does.Contain(CrewRoleType.Director));
        Assert.That(result.CurrentSkills, Does.Contain(CrewRoleType.Gaffer));
        Assert.That(result.AllDepartments, Is.Not.Empty);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewSkillsAsync(crew.Id), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateSkillsAsync_WhenCrewNotFound_ThrowsArgumentNullException()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        UpdateSkillsDto skillsDto = new UpdateSkillsDto { SelectedSkills = "1,2" };

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.UpdateSkillsAsync(username, skillsDto));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public void UpdateSkillsAsync_WhenNoSkillsSelected_ThrowsArgumentException()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.GetCrewSkillsForUpdateAsync(crew.Id)).ReturnsAsync([]);

        UpdateSkillsDto skillsDto = new UpdateSkillsDto { SelectedSkills = "   " };

        // Act
        ArgumentException? ex = Assert.ThrowsAsync<ArgumentException>(
            () => crewProfileService.UpdateSkillsAsync(username, skillsDto));

        // Assert
        Assert.That(ex.Message, Does.Contain(NoSkillsSelected));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCrewSkillsForUpdateAsync(crew.Id), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateSkillsAsync_WhenOnlyRemoveNeeded_RemovesSkills_SavesAndCommits()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        List<CrewSkill> currentSkills =
        [
            new()
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id, 
                RoleType = CrewRoleType.Director,
                CrewMember = crew
            },
            new()
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id,
                RoleType = CrewRoleType.Gaffer,
                CrewMember = crew
            }
        ];

        // new: Director only => remove Gaffer
        UpdateSkillsDto skillsDto = new UpdateSkillsDto
        {
            SelectedSkills = ((int)CrewRoleType.Director).ToString()
        };

        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.GetCrewSkillsForUpdateAsync(crew.Id)).ReturnsAsync(currentSkills);
        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

        IEnumerable<CrewSkill>? removed = null;
        profileRepositoryMock
            .Setup(pr => pr.RemoveCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()))
            .Callback<IEnumerable<CrewSkill>>(items => removed = items)
            .Returns(Task.CompletedTask);

        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);
        profileRepositoryMock.Setup(pr => pr.CommitTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        await crewProfileService.UpdateSkillsAsync(username, skillsDto);

        // Assert
        IEnumerable<CrewSkill> crewSkills = removed!.ToArray();
        Assert.That(crewSkills, Is.Not.Null);
        Assert.That(crewSkills.Select(r => r.RoleType), Does.Contain(CrewRoleType.Gaffer));
        Assert.That(crewSkills.Select(r => r.RoleType), Does.Not.Contain(CrewRoleType.Director));

        profileRepositoryMock.Verify(pr => pr.RemoveCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Once);
        profileRepositoryMock.Verify(pr => pr.AddCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Never);

        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
    }

    [Test]
    public async Task UpdateSkillsAsync_WhenOnlyAddNeeded_AddsSkills_SavesAndCommits()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        // current: Director only
        List<CrewSkill> currentSkills =
        [
            new()
            {
                Id = Guid.NewGuid(), 
                CrewMemberId = crew.Id, 
                RoleType = CrewRoleType.Director,
                CrewMember = crew
            }
        ];

        // new: Director + Gaffer => add Gaffer
        UpdateSkillsDto skillsDto = new UpdateSkillsDto
        {
            SelectedSkills = $"{(int)CrewRoleType.Director},{(int)CrewRoleType.Gaffer}"
        };

        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.GetCrewSkillsForUpdateAsync(crew.Id)).ReturnsAsync(currentSkills);
        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

        IEnumerable<CrewSkill>? added = null;
        profileRepositoryMock
            .Setup(pr => pr.AddCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()))
            .Callback<IEnumerable<CrewSkill>>(items => added = items)
            .Returns(Task.CompletedTask);

        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);
        profileRepositoryMock.Setup(pr => pr.CommitTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        await crewProfileService.UpdateSkillsAsync(username, skillsDto);

        // Assert
        IEnumerable<CrewSkill>? crewSkills = added!.ToArray();
        Assert.That(crewSkills, Is.Not.Null);
        Assert.That(crewSkills.Select(a => a.RoleType), Does.Contain(CrewRoleType.Gaffer));
        Assert.That(crewSkills.All(a => a.Id != Guid.Empty), Is.True);

        profileRepositoryMock.Verify(pr => pr.AddCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RemoveCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Never);

        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
    }

    [Test]
    public async Task UpdateSkillsAsync_WhenAddAndRemoveNeeded_PerformsBoth_SavesAndCommits()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        // current: Director, Gaffer
        List<CrewSkill> currentSkills =
        [
            new()
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id,
                RoleType = CrewRoleType.Director,
                CrewMember = crew
            },

            new()
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id,
                RoleType = CrewRoleType.Gaffer,
                CrewMember = crew
            }
        ];

        // new: Director + Editor => remove Gaffer, add Editor
        UpdateSkillsDto skillsDto = new UpdateSkillsDto
        {
            SelectedSkills = $"{(int)CrewRoleType.Director},{(int)CrewRoleType.Editor}"
        };

        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.GetCrewSkillsForUpdateAsync(crew.Id)).ReturnsAsync(currentSkills);

        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
        profileRepositoryMock.Setup(pr => pr.RemoveCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>())).Returns(Task.CompletedTask);
        profileRepositoryMock.Setup(pr => pr.AddCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>())).Returns(Task.CompletedTask);

        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);
        profileRepositoryMock.Setup(pr => pr.CommitTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        await crewProfileService.UpdateSkillsAsync(username, skillsDto);

        // Assert
        profileRepositoryMock.Verify(pr => pr.RemoveCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Once);
        profileRepositoryMock.Verify(pr => pr.AddCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Once);

        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
    }

    [Test]
    public void UpdateSkillsAsync_WhenExceptionOccurs_RollsBackAndThrowsException()
    {
        // Arrange
        const string username = "crew.user";
        Mock<IDbContextTransaction> transactionMock = CreateTransactionMock();

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: "Nick",
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        List<CrewSkill> currentSkills =
        [
            new()
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id,
                RoleType = CrewRoleType.Director,
                CrewMember = crew
            }
        ];

        UpdateSkillsDto skillsDto = new UpdateSkillsDto
        {
            SelectedSkills = ((int)CrewRoleType.Director).ToString()
        };

        profileRepositoryMock.Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username)).ReturnsAsync(crew);
        profileRepositoryMock.Setup(pr => pr.GetCrewSkillsForUpdateAsync(crew.Id)).ReturnsAsync(currentSkills);

        profileRepositoryMock.Setup(pr => pr.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);

        // no remove/add calls (both empty), but fail on save
        profileRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ThrowsAsync(new InvalidOperationException("db fail"));
        profileRepositoryMock.Setup(pr => pr.RollbackTransactionAsync(transactionMock.Object)).Returns(Task.CompletedTask);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(
            () => crewProfileService.UpdateSkillsAsync(username, skillsDto));

        // Assert
        Assert.That(ex.Message, Does.Contain("db fail"));

        profileRepositoryMock.Verify(pr => pr.RollbackTransactionAsync(transactionMock.Object), Times.Once);
        profileRepositoryMock.Verify(pr => pr.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);

        // also ensure add/remove not called when sets are same
        profileRepositoryMock.Verify(pr => pr.AddCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Never);
        profileRepositoryMock.Verify(pr => pr.RemoveCrewSkillsAsync(It.IsAny<IEnumerable<CrewSkill>>()), Times.Never);
    }

    [Test]
    public void GetDeleteCrewProfileAsync_WhenCrewNotFound_ThrowsArgumentNullException()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.GetDeleteCrewProfileAsync(username));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CrewNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDeleteCrewProfileAsync_WhenCrewExists_ReturnsMappedDto()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: null,
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        crew.CrewMemberProductions = new List<ProductionCrew> { new(), new() };
        crew.CrewMemberScenes = new List<SceneCrew> { new() };
        crew.Skills = new List<CrewSkill> { new(), new(), new() };

        profileRepositoryMock
            .Setup(pr => pr.GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(crew);

        // Act
        DeleteProfileDto result = await crewProfileService.GetDeleteCrewProfileAsync(username);

        // Assert
        Assert.That(result.FirstName, Is.EqualTo(crew.FirstName));
        Assert.That(result.LastName, Is.EqualTo(crew.LastName));
        Assert.That(result.ProfileImagePath, Is.EqualTo(crew.ProfileImagePath));
        Assert.That(result.UserName, Is.EqualTo(crew.User.UserName));
        Assert.That(result.Email, Is.EqualTo(crew.User.Email));
        Assert.That(result.PhoneNumber, Is.EqualTo(crew.User.PhoneNumber));
        
        Assert.That(result.ProductionsCount, Is.EqualTo(2));
        Assert.That(result.ScenesCount, Is.EqualTo(1));
        Assert.That(result.SkillsCount, Is.EqualTo(3));

        profileRepositoryMock.Verify(pr => pr.GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public void DeleteCrewProfileAsync_WhenCrewNotFound_ThrowsArgumentNullException()
    {
        // Arrange
        const string username = "missing.user";
        DeleteProfileDto dto = new DeleteProfileDto { Password = "password" };

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsync(username))
            .ReturnsAsync((Crew?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.DeleteCrewProfileAsync(username, dto));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CrewNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteCrewProfileAsync_WhenPasswordInvalid_ReturnsFalseAndLogsError()
    {
        // Arrange
        const string username = "crew.user";
        DeleteProfileDto dto = new DeleteProfileDto { Password = "wrong_password" };

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: null,
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsync(username))
            .ReturnsAsync(crew);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(crew.User, dto.Password))
            .ReturnsAsync(false);

        // Act
        bool result = await crewProfileService.DeleteCrewProfileAsync(username, dto);

        // Assert
        Assert.That(result, Is.False);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsync(username), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(crew.User, dto.Password), Times.Once);
        
        // Уверяваме се, че няма извиквания за изтриване
        profileRepositoryMock.Verify(pr => pr.DeleteCrewProfileAsync(It.IsAny<Guid>()), Times.Never);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Never);
    }

    [Test]
    public async Task DeleteCrewProfileAsync_WhenValid_DeletesProfileAndReturnsTrue()
    {
        // Arrange
        const string username = "crew.user";
        DeleteProfileDto dto = new DeleteProfileDto { Password = "correct_password" };

        Crew crew = CreateCrew(
            username: username,
            email: "crew@wrap.local",
            phone: "+359888000000",
            firstName: "Crew",
            lastName: "User",
            nickname: null,
            profileImagePath: "/img/profile/crew.webp",
            isActive: true,
            biography: "bio");

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsync(username))
            .ReturnsAsync(crew);

        userManagerMock
            .Setup(um => um.CheckPasswordAsync(crew.User, dto.Password))
            .ReturnsAsync(true);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ProfileFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.DeleteAsync(crew.ProfileImagePath, strategy, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        profileRepositoryMock
            .Setup(pr => pr.DeleteCrewProfileAsync(crew.Id))
            .ReturnsAsync(true);

        profileRepositoryMock
            .Setup(pr => pr.SaveAllChangesAsync())
            .ReturnsAsync(1);

        signInManagerMock
            .Setup(sm => sm.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        bool result = await crewProfileService.DeleteCrewProfileAsync(username, dto);

        // Assert
        Assert.That(result, Is.True);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsync(username), Times.Once);
        userManagerMock.Verify(um => um.CheckPasswordAsync(crew.User, dto.Password), Times.Once);
        imageStrategyResolverMock.Verify(isr => isr.Resolve(ProfileFolderName), Times.Once);
        imageServiceMock.Verify(img => img.DeleteAsync(crew.ProfileImagePath, strategy, It.IsAny<CancellationToken>()), Times.Once);
        profileRepositoryMock.Verify(pr => pr.DeleteCrewProfileAsync(crew.Id), Times.Once);
        profileRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
    }
    
    [Test]
    public void DownloadCrewProfileDataAsync_WhenDataIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.DownloadCrewDataAsync(username))
            .ReturnsAsync((CrewPersonalDataDto[]?)null);

        // Act
        ArgumentNullException? ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => crewProfileService.DownloadCrewProfileDataAsync(username));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(CrewNotFoundMessage, username)));

        profileRepositoryMock.Verify(pr => pr.DownloadCrewDataAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DownloadCrewProfileDataAsync_WhenDataExists_ReturnsJsonString()
    {
        // Arrange
        const string username = "crew.user";
        
        CrewPersonalDataDto[] data =
        [
            new()
        ];

        profileRepositoryMock
            .Setup(pr => pr.DownloadCrewDataAsync(username))
            .ReturnsAsync(data);

        // Act
        string result = await crewProfileService.DownloadCrewProfileDataAsync(username);

        // Assert
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        
        Assert.That(result.TrimStart(), Does.StartWith("["));
        Assert.That(result.TrimEnd(), Does.EndWith("]"));

        profileRepositoryMock.Verify(pr => pr.DownloadCrewDataAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
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
        httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContext);

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
    
    private static Crew CreateCrew(string username, string email, string phone, string firstName, string lastName,
        string? nickname, string profileImagePath, bool isActive, string? biography)
    {
        Crew newCrew = new Crew
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Nickname = nickname,
            ProfileImagePath = profileImagePath,
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
        
        return newCrew;
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
        IFormFile file =  new FormFile(stream, 0, content.Length, "file", fileName);
        
        return file;
    }
}
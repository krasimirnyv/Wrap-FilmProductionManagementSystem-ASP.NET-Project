namespace Wrap.Services.Tests;

using Microsoft.Extensions.Logging;

using Moq;
using NUnit.Framework;

using Core;
using Core.Interfaces;
using Models.NavBar;
using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using GCommon.Enums;

using static GCommon.OutputMessages;
using static GCommon.OutputMessages.NavBar;

[TestFixture]
public class NavBarServiceTests
{
    private Mock<INavBarRepository> navBarRepositoryMock = null!;
    private Mock<ILogger<NavBarService>> loggerMock = null!;

    private INavBarService navBarService = null!;

    [SetUp]
    public void SetUp()
    {
        navBarRepositoryMock = new Mock<INavBarRepository>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<NavBarService>>(MockBehavior.Loose);

        navBarService = new NavBarService(navBarRepositoryMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task GetNavBarUserAsync_WhenCrewUserExists_ReturnsCrewDtoAndDoesNotQueryCast()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        Crew crew = CreateCrew(
            userId: userId,
            userName: "crew.user",
            profileImagePath: "/img/profile/crew.webp");

        navBarRepositoryMock
            .Setup(nbr => nbr.GetCrewUserAsync(userId))
            .ReturnsAsync(crew);

        // Act
        NavBarUserDto? result = await navBarService.GetNavBarUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserName, Is.EqualTo("crew.user"));
        Assert.That(result.ProfileImagePath, Is.EqualTo("/img/profile/crew.webp"));
        Assert.That(result.Role, Is.EqualTo(CrewString));

        navBarRepositoryMock.Verify(nbr => nbr.GetCrewUserAsync(userId), Times.Once);
        navBarRepositoryMock.Verify(nbr => nbr.GetCastUserAsync(It.IsAny<Guid>()), Times.Never);
        navBarRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetNavBarUserAsync_WhenCrewIsNullAndCastUserExists_ReturnsCastDto()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        Cast cast = CreateCast(
            userId: userId,
            userName: "cast.user",
            profileImagePath: "/img/profile/cast.webp");

        navBarRepositoryMock
            .Setup(nbr => nbr.GetCrewUserAsync(userId))
            .ReturnsAsync((Crew?)null);

        navBarRepositoryMock
            .Setup(nbr => nbr.GetCastUserAsync(userId))
            .ReturnsAsync(cast);

        // Act
        NavBarUserDto? result = await navBarService.GetNavBarUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserName, Is.EqualTo("cast.user"));
        Assert.That(result.ProfileImagePath, Is.EqualTo("/img/profile/cast.webp"));
        Assert.That(result.Role, Is.EqualTo(CastString));

        navBarRepositoryMock.Verify(nbr => nbr.GetCrewUserAsync(userId), Times.Once);
        navBarRepositoryMock.Verify(nbr => nbr.GetCastUserAsync(userId), Times.Once);
        navBarRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public void GetNavBarUserAsync_WhenCrewAndCastAreNull_ThrowsArgumentNullExceptionWithUserNotFoundMessage()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        navBarRepositoryMock
            .Setup(nbr => nbr.GetCrewUserAsync(userId))
            .ReturnsAsync((Crew?)null);

        navBarRepositoryMock
            .Setup(nbr => nbr.GetCastUserAsync(userId))
            .ReturnsAsync((Cast?)null);

        // Act
        ArgumentNullException ex = Assert.ThrowsAsync<ArgumentNullException>(
            () => navBarService.GetNavBarUserAsync(userId))!;

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(UserNotFoundMessage, userId)));

        navBarRepositoryMock.Verify(nbr => nbr.GetCrewUserAsync(userId), Times.Once);
        navBarRepositoryMock.Verify(nbr => nbr.GetCastUserAsync(userId), Times.Once);
        navBarRepositoryMock.VerifyNoOtherCalls();
    }

    private static Crew CreateCrew(Guid userId, string? userName, string? profileImagePath)
    {
        Crew newCrew = new Crew
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = "Crew",
            LastName = "User",
            IsActive = true,
            IsDeleted = false,
            ProfileImagePath = profileImagePath,
            User = new ApplicationUser
            {
                Id = userId,
                UserName = userName
            }
        };
        
        return newCrew;
    }

    private static Cast CreateCast(Guid userId, string? userName, string? profileImagePath)
    {
        Cast newCast = new Cast
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = "Cast",
            LastName = "User",
            BirthDate = new DateTime(2000, 1, 1),
            Gender = GenderType.Male,
            IsActive = true,
            IsDeleted = false,
            ProfileImagePath = profileImagePath,
            User = new ApplicationUser
            {
                Id = userId,
                UserName = userName
            }
        };
        
        return newCast;
    }
}
namespace Wrap.Services.Tests;

using Moq;
using NUnit.Framework;

using Data.Models;
using Data.Repository.Interfaces;
using Core;
using Models.Profile;

[TestFixture]
public class ProfileServiceTests
{
    private Mock<IProfileRepository> profileRepositoryMock = null!;
    private ProfileService profileService = null!;

    [SetUp]
    public void SetUp()
    {
        profileRepositoryMock = new Mock<IProfileRepository>(MockBehavior.Strict);
        profileService = new ProfileService(profileRepositoryMock.Object);
    }

    [Test]
    public async Task IsUserCrewAsync_WhenCrewExists_ReturnsTrue()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = new Crew { Id = Guid.NewGuid() };

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(crew);

        // Act
        bool result = await profileService.IsUserCrewAsync(username);

        // Assert
        Assert.That(result, Is.True);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task IsUserCrewAsync_WhenCrewDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        // Act
        bool result = await profileService.IsUserCrewAsync(username);

        // Assert
        Assert.That(result, Is.False);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task IsUserCastAsync_WhenCastExists_ReturnsTrue()
    {
        // Arrange
        const string username = "cast.user";

        Cast cast = new Cast { Id = Guid.NewGuid() };

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(cast);

        // Act
        bool result = await profileService.IsUserCastAsync(username);

        // Assert
        Assert.That(result, Is.True);

        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task IsUserCastAsync_WhenCastDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Cast?)null);

        // Act
        bool result = await profileService.IsUserCastAsync(username);

        // Assert
        Assert.That(result, Is.False);

        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetRoleInfoAsync_WhenCrewExists_ReturnsCrewTrueCastFalse_AndDoesNotQueryCast()
    {
        // Arrange
        const string username = "crew.user";

        Crew crew = new Crew { Id = Guid.NewGuid() };

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(crew);

        // Act
        ProfileRoleDto result = await profileService.GetRoleInfoAsync(username);

        // Assert
        Assert.That(result.IsCrew, Is.True);
        Assert.That(result.IsCast, Is.False);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsNoTrackingAsync(It.IsAny<string>()), Times.Never);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetRoleInfoAsync_WhenCrewDoesNotExistButCastExists_ReturnsCrewFalseCastTrue()
    {
        // Arrange
        const string username = "cast.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        Cast cast = new Cast { Id = Guid.NewGuid() };

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync(cast);

        // Act
        ProfileRoleDto result = await profileService.GetRoleInfoAsync(username);

        // Assert
        Assert.That(result.IsCrew, Is.False);
        Assert.That(result.IsCast, Is.True);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetRoleInfoAsync_WhenNeitherCrewNorCastExists_ReturnsCrewFalseCastFalse()
    {
        // Arrange
        const string username = "missing.user";

        profileRepositoryMock
            .Setup(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Crew?)null);

        profileRepositoryMock
            .Setup(pr => pr.GetCastByUsernameAsNoTrackingAsync(username))
            .ReturnsAsync((Cast?)null);

        // Act
        ProfileRoleDto result = await profileService.GetRoleInfoAsync(username);

        // Assert
        Assert.That(result.IsCrew, Is.False);
        Assert.That(result.IsCast, Is.False);

        profileRepositoryMock.Verify(pr => pr.GetCrewByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.Verify(pr => pr.GetCastByUsernameAsNoTrackingAsync(username), Times.Once);
        profileRepositoryMock.VerifyNoOtherCalls();
    }
}
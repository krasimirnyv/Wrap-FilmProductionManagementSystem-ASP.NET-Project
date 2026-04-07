namespace Wrap.Infrastructure.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

using Data.Seeding;
using Data.Seeding.Interfaces;
using Data.Models.Infrastructure;

using static GCommon.ApplicationConstants;
using static GCommon.OutputMessages.ApplicationRoles;

[TestFixture]
public class ApplicationRoleSeederTests
{
    private Mock<RoleManager<ApplicationRole>> roleManagerMock = null!;
    private IApplicationRoleSeeder applicationRoleSeeder = null!;

    [SetUp]
    public void SetUp()
    {
        roleManagerMock = CreateRoleManagerMock();

        applicationRoleSeeder = new ApplicationRoleSeeder(roleManagerMock.Object);
    }

    [Test]
    public async Task SeedRolesAsync_WhenRolesAlreadyExist_DoesNotCreateRoles()
    {
        // Arrange
        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker))
            .ReturnsAsync(true);

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Actor))
            .ReturnsAsync(true);

        // Act
        await applicationRoleSeeder.SeedRolesAsync();

        // Assert
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker), Times.Once);
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Actor), Times.Once);

        roleManagerMock.Verify(rm => rm.CreateAsync(It.IsAny<ApplicationRole>()), Times.Never);
    }

    [Test]
    public async Task SeedRolesAsync_WhenFilmmakerRoleDoesNotExist_CreatesFilmmakerRole()
    {
        // Arrange
        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker))
            .ReturnsAsync(false);

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Actor))
            .ReturnsAsync(true);

        roleManagerMock
            .Setup(rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await applicationRoleSeeder.SeedRolesAsync();

        // Assert
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker), Times.Once);
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Actor), Times.Once);

        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)),
            Times.Once);

        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)),
            Times.Never);
    }

    [Test]
    public async Task SeedRolesAsync_WhenActorRoleDoesNotExist_CreatesActorRole()
    {
        // Arrange
        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker))
            .ReturnsAsync(true);

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Actor))
            .ReturnsAsync(false);

        roleManagerMock
            .Setup(rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await applicationRoleSeeder.SeedRolesAsync();

        // Assert
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker), Times.Once);
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Actor), Times.Once);

        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)),
            Times.Once);

        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)),
            Times.Never);
    }

    [Test]
    public async Task SeedRolesAsync_WhenBothRolesDoNotExist_CreatesBothRoles()
    {
        // Arrange
        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker))
            .ReturnsAsync(false);

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Actor))
            .ReturnsAsync(false);

        roleManagerMock
            .Setup(rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)))
            .ReturnsAsync(IdentityResult.Success);

        roleManagerMock
            .Setup(rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await applicationRoleSeeder.SeedRolesAsync();

        // Assert
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker), Times.Once);
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Actor), Times.Once);

        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)),
            Times.Once);

        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)),
            Times.Once);
    }

    [Test]
    public void SeedRolesAsync_WhenFilmmakerCreationFails_ThrowsExceptionWithCorrectMessage()
    {
        // Arrange
        IdentityResult failedResult = IdentityResult.Failed(
            new IdentityError { Description = "Role creation failed" });

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker))
            .ReturnsAsync(false);

        roleManagerMock
            .Setup(rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)))
            .ReturnsAsync(failedResult);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(() => applicationRoleSeeder.SeedRolesAsync());

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain(string.Format(
            RoleSeedingExceptionMessage,
            IdentityRoles.Filmmaker,
            "Role creation failed")));

        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker), Times.Once);
        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Filmmaker)),
            Times.Once);

        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Actor), Times.Never);
    }

    [Test]
    public void SeedRolesAsync_WhenActorCreationFails_ThrowsExceptionWithCorrectMessage()
    {
        // Arrange
        IdentityResult failedResult = IdentityResult.Failed(
            new IdentityError { Description = "Actor creation failed" });

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker))
            .ReturnsAsync(true);

        roleManagerMock
            .Setup(rm => rm.RoleExistsAsync(IdentityRoles.Actor))
            .ReturnsAsync(false);

        roleManagerMock
            .Setup(rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)))
            .ReturnsAsync(failedResult);

        // Act
        Exception? ex = Assert.ThrowsAsync<Exception>(() => applicationRoleSeeder.SeedRolesAsync());

        // Assert
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain(string.Format(
            RoleSeedingExceptionMessage,
            IdentityRoles.Actor,
            "Actor creation failed")));

        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Filmmaker), Times.Once);
        roleManagerMock.Verify(rm => rm.RoleExistsAsync(IdentityRoles.Actor), Times.Once);
        roleManagerMock.Verify(
            rm => rm.CreateAsync(It.Is<ApplicationRole>(r => r.Name == IdentityRoles.Actor)),
            Times.Once);
    }

    private static Mock<RoleManager<ApplicationRole>> CreateRoleManagerMock()
    {
        Mock<IRoleStore<ApplicationRole>> roleStoreMock = new Mock<IRoleStore<ApplicationRole>>();

        Mock<RoleManager<ApplicationRole>> roleManagerMock = new Mock<RoleManager<ApplicationRole>>
        (
            roleStoreMock.Object,
            Array.Empty<IRoleValidator<ApplicationRole>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<ILogger<RoleManager<ApplicationRole>>>()
        );

        return roleManagerMock;
    }
}
namespace Wrap.Services.Tests;

using Moq;
using NUnit.Framework;

using Core;
using Core.Interfaces;
using Core.Utilities.Providers.Interfaces;
using Models.General;
using Data.Models;
using Data.Repository.Interfaces;
using GCommon.Enums;

[TestFixture]
public class HomeServiceTests
{
    private Mock<IHomeRepository> homeRepositoryMock = null!;
    private Mock<IDateTimeProvider> dateTimeProviderMock = null!;
    private IHomeService homeService = null!;

    [SetUp]
    public void SetUp()
    {
        homeRepositoryMock = new Mock<IHomeRepository>(MockBehavior.Strict);
        dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        homeService = new HomeService(homeRepositoryMock.Object, dateTimeProviderMock.Object);
    }

    [Test]
    public async Task GetDashboardDataAsync_WhenRepositoryReturnsEmptyProductions_ReturnsDashboardWithZeroUpcomingAndEmptyProductions()
    {
        // Arrange
        DateTime now = new DateTime(2026, 3, 31, 12, 0, 0);

        dateTimeProviderMock.SetupGet(dtp => dtp.Now).Returns(now);

        homeRepositoryMock.Setup(hr => hr.GetCrewCountAsync()).ReturnsAsync(5);
        homeRepositoryMock.Setup(hr => hr.GetCastCountAsync()).ReturnsAsync(7);
        homeRepositoryMock.Setup(hr => hr.GetProductionsAsync()).ReturnsAsync(Array.Empty<Production>());

        // Act
        DashboardDataDto result = await homeService.GetDashboardDataAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CrewMembersCount, Is.EqualTo(5));
        Assert.That(result.CastMembersCount, Is.EqualTo(7));
        Assert.That(result.UpcomingScenesTotal, Is.Zero);
        Assert.That(result.Productions, Is.Not.Null);
        Assert.That(result.Productions.Count, Is.Zero);

        dateTimeProviderMock.VerifyGet(dtp => dtp.Now, Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCrewCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCastCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetProductionsAsync(), Times.Once);

        homeRepositoryMock.VerifyNoOtherCalls();
        dateTimeProviderMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDashboardDataAsync_WhenProductionsContainUpcomingAndPastScenes_MapsAndCountsUpcomingScenesCorrectly()
    {
        // Arrange
        DateTime now = new DateTime(2026, 3, 31, 12, 0, 0);

        dateTimeProviderMock.SetupGet(dtp => dtp.Now).Returns(now);

        Production production1 = CreateProduction(
            title: "production1",
            description: "description",
            scenes:
            [
                CreateSceneWithShootingDays(
                    CreateShootingDay(now.AddDays(1)), // upcoming
                    CreateShootingDay(now.AddDays(-1)) // past
                ),
                CreateSceneWithShootingDays(
                    CreateShootingDay(now.AddHours(1)) // upcoming
                )
            ]);

        Production production2 = CreateProduction(
            title: "production2",
            description: null,
            scenes:
            [
                CreateSceneWithShootingDays(
                    CreateShootingDay(now), // NOT upcoming because strictly " > "
                    CreateShootingDay(now.AddMinutes(-5))
                )
            ]);

        homeRepositoryMock.Setup(hr => hr.GetCrewCountAsync()).ReturnsAsync(10);
        homeRepositoryMock.Setup(hr => hr.GetCastCountAsync()).ReturnsAsync(20);
        homeRepositoryMock.Setup(hr => hr.GetProductionsAsync()).ReturnsAsync([production1, production2]);

        // Act
        DashboardDataDto result = await homeService.GetDashboardDataAsync();

        // Assert
        Assert.That(result.CrewMembersCount, Is.EqualTo(10));
        Assert.That(result.CastMembersCount, Is.EqualTo(20));

        Assert.That(result.Productions, Has.Count.EqualTo(2));

        ProductionDashboardDto dto1 = result.Productions.Single(pdd => pdd.Title == "production1");
        Assert.That(dto1.Description, Is.EqualTo("description"));
        Assert.That(dto1.UpcomingScenesCount, Is.EqualTo(2));

        ProductionDashboardDto dto2 = result.Productions.Single(pdd => pdd.Title == "production2");
        Assert.That(dto2.Description, Is.Null);
        Assert.That(dto2.UpcomingScenesCount, Is.Zero);

        Assert.That(result.UpcomingScenesTotal, Is.EqualTo(2));

        dateTimeProviderMock.VerifyGet(dtp => dtp.Now, Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCrewCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCastCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetProductionsAsync(), Times.Once);

        homeRepositoryMock.VerifyNoOtherCalls();
        dateTimeProviderMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDashboardDataAsync_WhenMultipleProductionsHaveUpcomingScenes_SumsUpcomingScenesTotalCorrectly()
    {
        // Arrange
        DateTime now = new DateTime(2026, 3, 31, 12, 0, 0);

        dateTimeProviderMock.SetupGet(dtp => dtp.Now).Returns(now);

        Production production1 = CreateProduction(
            title: "production1",
            description: "description1",
            scenes:
            [
                CreateSceneWithShootingDays(CreateShootingDay(now.AddDays(2))) // 1 upcoming
            ]);

        Production production2 = CreateProduction(
            title: "production2",
            description: "description2",
            scenes:
            [
                CreateSceneWithShootingDays(CreateShootingDay(now.AddMinutes(1))), // 1 upcoming
                CreateSceneWithShootingDays( // 2 upcoming
                    CreateShootingDay(now.AddMinutes(2)),
                    CreateShootingDay(now.AddMinutes(3))
                )
            ]);

        homeRepositoryMock.Setup(hr => hr.GetCrewCountAsync()).ReturnsAsync(1);
        homeRepositoryMock.Setup(hr => hr.GetCastCountAsync()).ReturnsAsync(2);
        homeRepositoryMock.Setup(hr => hr.GetProductionsAsync()).ReturnsAsync([production1, production2]);

        // Act
        DashboardDataDto result = await homeService.GetDashboardDataAsync();

        // Assert
        Assert.That(result.Productions.Single(pdd => pdd.Title == "production1").UpcomingScenesCount, Is.EqualTo(1));
        Assert.That(result.Productions.Single(pdd => pdd.Title == "production2").UpcomingScenesCount, Is.EqualTo(3));
        Assert.That(result.UpcomingScenesTotal, Is.EqualTo(4));

        dateTimeProviderMock.VerifyGet(dtp => dtp.Now, Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCrewCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCastCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetProductionsAsync(), Times.Once);

        homeRepositoryMock.VerifyNoOtherCalls();
        dateTimeProviderMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDashboardDataAsync_WhenRepositoryReturnsCounts_ReturnsThoseCountsInDashboard()
    {
        // Arrange
        DateTime now = new DateTime(2026, 3, 31, 12, 0, 0);

        dateTimeProviderMock.SetupGet(dtp => dtp.Now).Returns(now);

        Production production = CreateProduction(
            title: "production",
            description: "desc",
            scenes: [CreateSceneWithShootingDays(CreateShootingDay(now.AddDays(1)))]);

        homeRepositoryMock.Setup(hr => hr.GetCrewCountAsync()).ReturnsAsync(123);
        homeRepositoryMock.Setup(hr => hr.GetCastCountAsync()).ReturnsAsync(456);
        homeRepositoryMock.Setup(hr => hr.GetProductionsAsync()).ReturnsAsync([production]);

        // Act
        DashboardDataDto result = await homeService.GetDashboardDataAsync();

        // Assert
        Assert.That(result.CrewMembersCount, Is.EqualTo(123));
        Assert.That(result.CastMembersCount, Is.EqualTo(456));

        dateTimeProviderMock.VerifyGet(dtp => dtp.Now, Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCrewCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetCastCountAsync(), Times.Once);
        homeRepositoryMock.Verify(hr => hr.GetProductionsAsync(), Times.Once);

        homeRepositoryMock.VerifyNoOtherCalls();
        dateTimeProviderMock.VerifyNoOtherCalls();
    }

    private static Production CreateProduction(string title, string? description, IEnumerable<Scene> scenes)
    {
        Production production = new Production
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            StatusType = ProductionStatusType.Concept,
            StatusStartDate = DateTime.UtcNow,
            Scenes = scenes.ToList()
        };

        return production;
    }

    private static Scene CreateSceneWithShootingDays(params ShootingDay[] shootingDays)
    {
        Scene scene = new Scene
        {
            Id = Guid.NewGuid(),
            SceneNumber = 1,
            SceneType = SceneType.Interior,
            SceneName = "SceneName",
            Location = "Location",
            ProductionId = Guid.NewGuid(),
            Production = new Production
            {
                Id = Guid.NewGuid(),
                Title = "dummy-production",
                StatusType = ProductionStatusType.Concept,
                StatusStartDate = DateTime.UtcNow
            }
        };

        int order = 1;
        foreach (ShootingDay shootingDay in shootingDays)
        {
            scene.ShootingDayScenes
                .Add(new ShootingDayScene
                {
                    Id = Guid.NewGuid(),
                    Order = order++,
                    ShootingDayId = shootingDay.Id,
                    ShootingDay = shootingDay,
                    SceneId = scene.Id,
                    Scene = scene
                });
        }

        return scene;
    }

    private static ShootingDay CreateShootingDay(DateTime date)
    {
        ShootingDay newShootingDay = new ShootingDay
        {
            Id = Guid.NewGuid(),
            Date = date,
            Notes = null,
            ProductionId = Guid.NewGuid(),
            Production = new Production
            {
                Id = Guid.NewGuid(),
                Title = "dummy-production",
                StatusType = ProductionStatusType.Concept,
                StatusStartDate = DateTime.UtcNow
            }
        };

        return newShootingDay;
    }
}
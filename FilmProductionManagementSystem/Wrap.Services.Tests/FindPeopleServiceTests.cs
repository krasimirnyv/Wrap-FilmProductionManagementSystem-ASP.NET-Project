namespace Wrap.Services.Tests;

using Moq;
using NUnit.Framework;

using Core;
using Core.Interfaces;
using Data.Repository.Interfaces;
using GCommon.Enums;
using Models.FindPeople;

using static GCommon.ApplicationConstants;

[TestFixture]
public class FindPeopleServiceTests
{
    private Mock<IFindPeopleRepository> findPeopleRepositoryMock = null!;
    private Mock<IProductionRepository> productionRepositoryMock = null!;

    private IFindPeopleService findPeopleService = null!;

    [SetUp]
    public void SetUp()
    {
        findPeopleRepositoryMock = new Mock<IFindPeopleRepository>(MockBehavior.Strict);
        productionRepositoryMock = new Mock<IProductionRepository>(MockBehavior.Strict);

        findPeopleService = new FindPeopleService(
            findPeopleRepositoryMock.Object,
            productionRepositoryMock.Object);
    }

    [Test]
    public async Task GetFilmmakersAsync_WhenCalledWithDefaults_UsesPageOneAndReturnsDto()
    {
        // Arrange
        IReadOnlyCollection<FilmmakerListDto> filmmakers =
        [
            new()
            {
                CrewId = Guid.NewGuid(),
                ProfileImagePath = "/img/profile/crew1.webp",
                FullName = "Ivan Petrov",
                Nickname = "Ivo",
                Role = CrewRoleType.Director.ToString(),
                IsAlreadyInProduction = false
            },
            new()
            {
                CrewId = Guid.NewGuid(),
                ProfileImagePath = "/img/profile/crew2.webp",
                FullName = "Maria Ivanova",
                Nickname = null,
                Role = CrewRoleType.Producer.ToString(),
                IsAlreadyInProduction = true
            }
        ];

        findPeopleRepositoryMock
            .Setup(repo => repo.GetFilmmakersPagedAsync(0, DefaultPeoplePerPage, null, null, null))
            .ReturnsAsync((filmmakers, 2));

        // Act
        FindFilmmakersDto result = await findPeopleService.GetFilmmakersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.FilmmakerListDtos, Has.Count.EqualTo(2));
        Assert.That(result.FilmmakerListDtos.First().FullName, Is.EqualTo("Ivan Petrov"));

        findPeopleRepositoryMock.Verify(
            repo => repo.GetFilmmakersPagedAsync(0, DefaultPeoplePerPage, null, null, null),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetFilmmakersAsync_WhenPageNumberIsLessThanOne_UsesSafePageOne()
    {
        // Arrange
        IReadOnlyCollection<FilmmakerListDto> filmmakers = Array.Empty<FilmmakerListDto>();

        findPeopleRepositoryMock
            .Setup(repo => repo.GetFilmmakersPagedAsync(0, 8, "john", CrewRoleType.Director, null))
            .ReturnsAsync((filmmakers, 0));

        // Act
        FindFilmmakersDto result = await findPeopleService.GetFilmmakersAsync(
            pageNumber: 0,
            search: "john",
            roleType: CrewRoleType.Director,
            productionId: null,
            peoplePerPage: 8);

        // Assert
        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.FilmmakerListDtos, Is.Empty);

        findPeopleRepositoryMock.Verify(
            repo => repo.GetFilmmakersPagedAsync(0, 8, "john", CrewRoleType.Director, null),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetFilmmakersAsync_WhenCalledWithFiltersAndLaterPage_PassesCorrectSkipAndArguments()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();
        int pageNumber = 3;
        int peoplePerPage = 10;
        int expectedSkip = 20;

        IReadOnlyCollection<FilmmakerListDto> filmmakers =
        [
            new()
            {
                CrewId = Guid.NewGuid(),
                ProfileImagePath = "/img/profile/geo.webp",
                FullName = "Georgi Dimitrov",
                Nickname = "Gogo",
                Role = CrewRoleType.DirectorOfPhotography.ToString(),
                IsAlreadyInProduction = false
            }
        ];

        findPeopleRepositoryMock
            .Setup(repo => repo.GetFilmmakersPagedAsync(
                expectedSkip,
                peoplePerPage,
                "geo",
                CrewRoleType.DirectorOfPhotography,
                productionId))
            .ReturnsAsync((filmmakers, 11));

        // Act
        FindFilmmakersDto result = await findPeopleService.GetFilmmakersAsync(
            pageNumber,
            "geo",
            CrewRoleType.DirectorOfPhotography,
            productionId,
            peoplePerPage);

        // Assert
        Assert.That(result.TotalCount, Is.EqualTo(11));
        Assert.That(result.FilmmakerListDtos, Has.Count.EqualTo(1));
        Assert.That(result.FilmmakerListDtos.Single().FullName, Is.EqualTo("Georgi Dimitrov"));

        findPeopleRepositoryMock.Verify(
            repo => repo.GetFilmmakersPagedAsync(
                expectedSkip,
                peoplePerPage,
                "geo",
                CrewRoleType.DirectorOfPhotography,
                productionId),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetActorsAsync_WhenCalledWithDefaults_UsesPageOneAndReturnsDto()
    {
        // Arrange
        IReadOnlyCollection<ActorListDto> actors =
        [
            new()
            {
                CastId = Guid.NewGuid(),
                ProfileImagePath = "/img/profile/actor1.webp",
                FullName = "Petar Petrov",
                Nickname = "Pepi",
                Age = 25,
                Gender = "Male",
                IsAlreadyInProduction = false
            }
        ];

        findPeopleRepositoryMock
            .Setup(repo => repo.GetActorsPagedAsync(0, DefaultPeoplePerPage, null, null, null, null))
            .ReturnsAsync((actors, 1));

        // Act
        FindActorsDto result = await findPeopleService.GetActorsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.ActorListDtos, Has.Count.EqualTo(1));
        Assert.That(result.ActorListDtos.Single().FullName, Is.EqualTo("Petar Petrov"));

        findPeopleRepositoryMock.Verify(
            repo => repo.GetActorsPagedAsync(0, DefaultPeoplePerPage, null, null, null, null),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetActorsAsync_WhenPageNumberIsLessThanOne_UsesSafePageOne()
    {
        // Arrange
        IReadOnlyCollection<ActorListDto> actors = Array.Empty<ActorListDto>();

        findPeopleRepositoryMock
            .Setup(repo => repo.GetActorsPagedAsync(0, 12, "anna", 22, "Female", null))
            .ReturnsAsync((actors, 0));

        // Act
        FindActorsDto result = await findPeopleService.GetActorsAsync(
            pageNumber: -5,
            search: "anna",
            age: 22,
            gender: "Female",
            productionId: null,
            peoplePerPage: 12);

        // Assert
        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.ActorListDtos, Is.Empty);

        findPeopleRepositoryMock.Verify(
            repo => repo.GetActorsPagedAsync(0, 12, "anna", 22, "Female", null),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetActorsAsync_WhenCalledWithFiltersAndLaterPage_PassesCorrectSkipAndArguments()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();
        int pageNumber = 4;
        int peoplePerPage = 5;
        int expectedSkip = 15;

        IReadOnlyCollection<ActorListDto> actors =
        [
            new()
            {
                CastId = Guid.NewGuid(),
                ProfileImagePath = "/img/profile/elena.webp",
                FullName = "Elena Koleva",
                Nickname = null,
                Age = 30,
                Gender = "Female",
                IsAlreadyInProduction = true
            }
        ];

        findPeopleRepositoryMock
            .Setup(repo => repo.GetActorsPagedAsync(
                expectedSkip,
                peoplePerPage,
                "el",
                30,
                "Female",
                productionId))
            .ReturnsAsync((actors, 9));

        // Act
        FindActorsDto result = await findPeopleService.GetActorsAsync(
            pageNumber,
            "el",
            30,
            "Female",
            productionId,
            peoplePerPage);

        // Assert
        Assert.That(result.TotalCount, Is.EqualTo(9));
        Assert.That(result.ActorListDtos, Has.Count.EqualTo(1));
        Assert.That(result.ActorListDtos.Single().FullName, Is.EqualTo("Elena Koleva"));

        findPeopleRepositoryMock.Verify(
            repo => repo.GetActorsPagedAsync(
                expectedSkip,
                peoplePerPage,
                "el",
                30,
                "Female",
                productionId),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CanManageProductionAsync_WhenUserIdIsNull_ReturnsFalse()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        // Act
        bool result = await findPeopleService.CanManageProductionAsync(productionId, null!);

        // Assert
        Assert.That(result, Is.False);

        productionRepositoryMock.VerifyNoOtherCalls();
        findPeopleRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CanManageProductionAsync_WhenUserIdIsInvalidGuid_ReturnsFalse()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        // Act
        bool result = await findPeopleService.CanManageProductionAsync(productionId, "not-a-guid");

        // Assert
        Assert.That(result, Is.False);

        productionRepositoryMock.VerifyNoOtherCalls();
        findPeopleRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CanManageProductionAsync_WhenRepositoryReturnsFalse_ReturnsFalse()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        productionRepositoryMock
            .Setup(repo => repo.IsUserProductionLeaderAsync(productionId, userId))
            .ReturnsAsync(false);

        // Act
        bool result = await findPeopleService.CanManageProductionAsync(productionId, userId.ToString());

        // Assert
        Assert.That(result, Is.False);

        productionRepositoryMock.Verify(
            repo => repo.IsUserProductionLeaderAsync(productionId, userId),
            Times.Once);

        productionRepositoryMock.VerifyNoOtherCalls();
        findPeopleRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CanManageProductionAsync_WhenRepositoryReturnsTrue_ReturnsTrue()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        productionRepositoryMock
            .Setup(repo => repo.IsUserProductionLeaderAsync(productionId, userId))
            .ReturnsAsync(true);

        // Act
        bool result = await findPeopleService.CanManageProductionAsync(productionId, userId.ToString());

        // Assert
        Assert.That(result, Is.True);

        productionRepositoryMock.Verify(
            repo => repo.IsUserProductionLeaderAsync(productionId, userId),
            Times.Once);

        productionRepositoryMock.VerifyNoOtherCalls();
        findPeopleRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddCrewAsync_WhenCalled_AddsCrewToProductionAndSavesChanges()
    {
        // Arrange
        AddFilmmakerDto dto = new AddFilmmakerDto
        {
            ProductionId = Guid.NewGuid(),
            CrewId = Guid.NewGuid(),
            RoleType = CrewRoleType.Director
        };

        findPeopleRepositoryMock
            .Setup(repo => repo.AddCrewToProductionAsync(dto.ProductionId, dto.CrewId, dto.RoleType))
            .Returns(Task.CompletedTask);

        findPeopleRepositoryMock
            .Setup(repo => repo.SaveAllChangesAsync())
            .ReturnsAsync(1);

        // Act
        await findPeopleService.AddCrewAsync(dto);

        // Assert
        findPeopleRepositoryMock.Verify(
            repo => repo.AddCrewToProductionAsync(dto.ProductionId, dto.CrewId, dto.RoleType),
            Times.Once);

        findPeopleRepositoryMock.Verify(
            repo => repo.SaveAllChangesAsync(),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task RemoveCrewAsync_WhenCalled_RemovesCrewFromProductionAndSavesChanges()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();
        Guid crewId = Guid.NewGuid();

        findPeopleRepositoryMock
            .Setup(repo => repo.RemoveCrewFromProductionAsync(productionId, crewId))
            .Returns(Task.CompletedTask);

        findPeopleRepositoryMock
            .Setup(repo => repo.SaveAllChangesAsync())
            .ReturnsAsync(1);

        // Act
        await findPeopleService.RemoveCrewAsync(productionId, crewId);

        // Assert
        findPeopleRepositoryMock.Verify(
            repo => repo.RemoveCrewFromProductionAsync(productionId, crewId),
            Times.Once);

        findPeopleRepositoryMock.Verify(
            repo => repo.SaveAllChangesAsync(),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddCastAsync_WhenCalled_AddsCastToProductionAndSavesChanges()
    {
        // Arrange
        AddActorDto dto = new AddActorDto
        {
            ProductionId = Guid.NewGuid(),
            CastId = Guid.NewGuid(),
            RoleName = "Detective"
        };

        findPeopleRepositoryMock
            .Setup(repo => repo.AddCastToProductionAsync(dto.ProductionId, dto.CastId, dto.RoleName))
            .Returns(Task.CompletedTask);

        findPeopleRepositoryMock
            .Setup(repo => repo.SaveAllChangesAsync())
            .ReturnsAsync(1);

        // Act
        await findPeopleService.AddCastAsync(dto);

        // Assert
        findPeopleRepositoryMock.Verify(
            repo => repo.AddCastToProductionAsync(dto.ProductionId, dto.CastId, dto.RoleName),
            Times.Once);

        findPeopleRepositoryMock.Verify(
            repo => repo.SaveAllChangesAsync(),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task RemoveCastAsync_WhenCalled_RemovesCastFromProductionAndSavesChanges()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();
        Guid castId = Guid.NewGuid();

        findPeopleRepositoryMock
            .Setup(repo => repo.RemoveCastFromProductionAsync(productionId, castId))
            .Returns(Task.CompletedTask);

        findPeopleRepositoryMock
            .Setup(repo => repo.SaveAllChangesAsync())
            .ReturnsAsync(1);

        // Act
        await findPeopleService.RemoveCastAsync(productionId, castId);

        // Assert
        findPeopleRepositoryMock.Verify(
            repo => repo.RemoveCastFromProductionAsync(productionId, castId),
            Times.Once);

        findPeopleRepositoryMock.Verify(
            repo => repo.SaveAllChangesAsync(),
            Times.Once);

        findPeopleRepositoryMock.VerifyNoOtherCalls();
        productionRepositoryMock.VerifyNoOtherCalls();
    }
}
namespace Wrap.Services.Tests;

using System.Collections.ObjectModel;

using Microsoft.AspNetCore.Http;

using Moq;
using NUnit.Framework;

using Data.Models;
using Data.Models.MappingEntities;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using GCommon.Enums;
using GCommon.UI;
using Core;
using Core.Utilities.ImageLogic.Interfaces;
using Models.Production;
using Models.Production.NestedDtos;

using static GCommon.ApplicationConstants;
using static GCommon.OutputMessages.Production;
using static GCommon.DataFormat;

[TestFixture]
public class ProductionServiceTests
{
    private Mock<IProductionRepository> productionRepositoryMock = null!;
    private Mock<IImageService> imageServiceMock = null!;
    private Mock<IVariantImageStrategyResolver> imageStrategyResolverMock = null!;

    private ProductionService productionService = null!;

    [SetUp]
    public void SetUp()
    {
        productionRepositoryMock = new Mock<IProductionRepository>(MockBehavior.Strict);
        imageServiceMock = new Mock<IImageService>(MockBehavior.Strict);
        imageStrategyResolverMock = new Mock<IVariantImageStrategyResolver>(MockBehavior.Strict);

        productionService = new ProductionService
        (
            productionRepositoryMock.Object,
            imageServiceMock.Object,
            imageStrategyResolverMock.Object
        );
    }

    [Test]
    public async Task GetAllProductionsAsync_WhenCalled_UsesPagingAndMapsDtosAndSetsStatusClass()
    {
        // Arrange
        int pageNumber = 2;
        int productionsPerPage = 5;

        int expectedSkip = (pageNumber - 1) * productionsPerPage;

        ProductionStatusType statusType = GetStatusFromCatalogOrFallback();

        ReadOnlyCollection<Production> productions = new List<Production>
        {
            CreateProduction(
                id: Guid.NewGuid(),
                title: "p1",
                thumbnail: "/img/productions/p1.webp",
                statusType: statusType),
            CreateProduction(
                id: Guid.NewGuid(),
                title: "p2",
                thumbnail: "/img/productions/p2.webp",
                statusType: statusType)
        }.AsReadOnly();

        productionRepositoryMock
            .Setup(pr => pr.GetAllAsync(expectedSkip, productionsPerPage))
            .ReturnsAsync(productions);

        // Act
        IReadOnlyCollection<ProductionDto> result =
            await productionService.GetAllProductionsAsync(pageNumber, productionsPerPage);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));

        ProductionDto dto1 = result.First(p => p.Title == "p1");
        Assert.That(dto1.ThumbnailPath, Is.EqualTo("/img/productions/p1.webp"));
        Assert.That(dto1.StatusType, Is.EqualTo(statusType));
        Assert.That(dto1.StatusAbstractClass, Is.Not.Empty);

        productionRepositoryMock.Verify(pr => pr.GetAllAsync(expectedSkip, productionsPerPage), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetProductionsCountAsync_WhenRepositoryReturnsCount_ReturnsSameCount()
    {
        // Arrange
        productionRepositoryMock
            .Setup(pr => pr.CountAsync())
            .ReturnsAsync(42);

        // Act
        int result = await productionService.GetProductionsCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(42));

        productionRepositoryMock.Verify(pr => pr.CountAsync(), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetProductionDetailsAsync_WhenIdIsNullOrWhitespace_ReturnsNull()
    {
        // Act
        DetailsProductionDto? result1 = await productionService.GetProductionDetailsAsync(null);
        DetailsProductionDto? result2 = await productionService.GetProductionDetailsAsync("   ");

        // Assert
        Assert.That(result1, Is.Null);
        Assert.That(result2, Is.Null);

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetProductionDetailsAsync_WhenIdIsInvalidGuid_ReturnsNull()
    {
        // Act
        DetailsProductionDto? result = await productionService.GetProductionDetailsAsync("not-a-guid");

        // Assert
        Assert.That(result, Is.Null);

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetProductionDetailsAsync_WhenRepositoryReturnsAllNullTuple_ReturnsNull()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        productionRepositoryMock
            .Setup(pr => pr.GetDetailsAsync(productionId))
            .ReturnsAsync((production: null,
                productionCrews: null,
                productionCasts: null,
                productionScenes: null,
                productionAssets: null,
                productionShootingDays: null));

        // Act
        DetailsProductionDto? result = await productionService.GetProductionDetailsAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.Null);

        productionRepositoryMock.Verify(pr => pr.GetDetailsAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetProductionDetailsAsync_WhenDataExists_MapsAllNestedCollectionsAndStatusClass()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        ProductionStatusType statusType = GetStatusFromCatalogOrFallback();

        Production production = new Production
        {
            Id = productionId,
            Title = "My Production",
            Description = "desc",
            Budget = 1000m,
            Thumbnail = "/img/productions/thumb.webp",
            StatusType = statusType,
            StatusStartDate = new DateTime(2026, 1, 1),
            StatusEndDate = new DateTime(2026, 2, 1),
            Script = new Script { Id = Guid.NewGuid() }
        };

        Crew crew = new Crew
        {
            Id = Guid.NewGuid(),
            FirstName = "CrewFirst",
            LastName = "CrewLast",
            ProfileImagePath = "/img/profile/crew.webp",
            User = new ApplicationUser
            {
                UserName = "crew.user", 
                Email = "crew@wrap.local",
                PhoneNumber = "+359"
            }
        };

        Cast cast = new Cast
        {
            Id = Guid.NewGuid(),
            FirstName = "CastFirst",
            LastName = "CastLast",
            ProfileImagePath = "/img/profile/cast.webp",
            BirthDate = new DateTime(2000, 1, 1),
            Gender = GenderType.Male,
            User = new ApplicationUser
            {
                UserName = "cast.user", 
                Email = "cast@wrap.local",
                PhoneNumber = "+359"
            }
        };

        ProductionCrew productionCrew = new ProductionCrew
        {
            ProductionId = productionId,
            Production = production,
            CrewMemberId = crew.Id,
            CrewMember = crew,
            RoleType = CrewRoleType.Director
        };

        ProductionCast productionCast = new ProductionCast
        {
            ProductionId = productionId,
            Production = production,
            CastMemberId = cast.Id,
            CastMember = cast,
            Role = "Detective"
        };

        Scene scene = new Scene
        {
            Id = Guid.NewGuid(),
            SceneNumber = 1,
            SceneType = SceneType.Interior,
            SceneName = "SceneName",
            Location = "Location",
            ProductionId = productionId,
            Production = production
        };

        ProductionAsset asset = new ProductionAsset
        {
            Id = Guid.NewGuid(),
            ProductionId = productionId,
            Production = production,
            AssetType = ProductionAssetType.Storyboard,
            Title = "Storyboard"
        };

        ShootingDay shootingDay = new ShootingDay
        {
            Id = Guid.NewGuid(),
            ProductionId = productionId,
            Production = production,
            Date = new DateTime(2026, 3, 31)
        };

        productionRepositoryMock
            .Setup(pr => pr.GetDetailsAsync(productionId))
            .ReturnsAsync((
                production: production,
                productionCrews: new List<ProductionCrew> { productionCrew }.AsReadOnly(),
                productionCasts: new List<ProductionCast> { productionCast }.AsReadOnly(),
                productionScenes: new List<Scene> { scene }.AsReadOnly(),
                productionAssets: new List<ProductionAsset> { asset }.AsReadOnly(),
                productionShootingDays: new List<ShootingDay> { shootingDay }.AsReadOnly()
            ));

        // Act
        DetailsProductionDto? result = await productionService.GetProductionDetailsAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(productionId));
        Assert.That(result.Title, Is.EqualTo("My Production"));
        Assert.That(result.Thumbnail, Is.EqualTo("/img/productions/thumb.webp"));
        Assert.That(result.Description, Is.EqualTo("desc"));
        Assert.That(result.Budget, Is.EqualTo(1000m));
        Assert.That(result.StatusType, Is.EqualTo(statusType));
        Assert.That(result.StatusAbstractClass, Is.Not.Empty);
        Assert.That(result.ScriptId, Is.EqualTo(production.Script.Id));

        Assert.That(result.ProductionCrewMembers.Count, Is.EqualTo(1));
        
        ProductionCrewMemberDto crewDto = result.ProductionCrewMembers.Single();
        Assert.That(crewDto.FirstName, Is.EqualTo("CrewFirst"));
        Assert.That(crewDto.LastName, Is.EqualTo("CrewLast"));
        Assert.That(crewDto.ProfileImagePath, Is.EqualTo("/img/profile/crew.webp"));
        Assert.That(crewDto.Role, Is.EqualTo(CrewRoleType.Director));

        Assert.That(result.ProductionCastMembers.Count, Is.EqualTo(1));
        
        ProductionCastMemberDto castDto = result.ProductionCastMembers.Single();
        Assert.That(castDto.FirstName, Is.EqualTo("CastFirst"));
        Assert.That(castDto.LastName, Is.EqualTo("CastLast"));
        Assert.That(castDto.ProfileImagePath, Is.EqualTo("/img/profile/cast.webp"));
        Assert.That(castDto.Role, Is.EqualTo("Detective"));
        Assert.That(castDto.Gender, Is.EqualTo(GenderType.Male));

        Assert.That(result.ProductionScenes.Count, Is.EqualTo(1));
        
        ProductionSceneDto sceneDto = result.ProductionScenes.Single();
        Assert.That(sceneDto.SceneNumber, Is.EqualTo(1));
        Assert.That(sceneDto.SceneType, Is.EqualTo(SceneType.Interior));
        Assert.That(sceneDto.SceneName, Is.EqualTo("SceneName"));
        Assert.That(sceneDto.Location, Is.EqualTo("Location"));

        Assert.That(result.ProductionAssets.Count, Is.EqualTo(1));
        
        ProductionAssetDto assetDto = result.ProductionAssets.Single();
        Assert.That(assetDto.Title, Is.EqualTo("Storyboard"));
        Assert.That(assetDto.AssetType, Is.EqualTo(ProductionAssetType.Storyboard));

        Assert.That(result.ProductionShootingDays.Count, Is.EqualTo(1));
        
        ProductionShootingDayDto sdDto = result.ProductionShootingDays.Single();
        Assert.That(sdDto.Date, Is.EqualTo(new DateTime(2026, 3, 31)));

        productionRepositoryMock.Verify(pr => pr.GetDetailsAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public async Task CreateProductionAsync_WhenValidDto_SavesImage_AddsProduction_SavesChanges_ReturnsId()
    {
        // Arrange
        IFormFile thumbnailFile = CreateFormFile("thumb.png", [1, 2, 3]);

        CreateProductionDto dto = new CreateProductionDto
        {
            ThumbnailImage = thumbnailFile,
            Title = "New Production",
            Description = "desc",
            Budget = 500m,
            StatusType = ProductionStatusType.Production,
            StatusStartDate = new DateTime(2026, 3, 1),
            StatusEndDate = null
        };

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        const string savedThumbnailPath = "/img/productions/saved.webp";

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ThumbnailFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.SaveImageAsync(dto.ThumbnailImage, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedThumbnailPath);

        Production? captured = null;
        productionRepositoryMock
            .Setup(pr => pr.AddAsync(It.IsAny<Production>()))
            .Callback<Production>(p => captured = p)
            .Returns(Task.CompletedTask);

        productionRepositoryMock
            .Setup(pr => pr.SaveAllChangesAsync())
            .ReturnsAsync(1);

        // Act
        string resultId = await productionService.CreateProductionAsync(dto);

        // Assert
        Assert.That(Guid.TryParse(resultId, out _), Is.True);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(captured.Title, Is.EqualTo(dto.Title));
        Assert.That(captured.Description, Is.EqualTo(dto.Description));
        Assert.That(captured.Budget, Is.EqualTo(dto.Budget));
        Assert.That(captured.StatusType, Is.EqualTo(dto.StatusType));
        Assert.That(captured.StatusStartDate, Is.EqualTo(dto.StatusStartDate));
        Assert.That(captured.StatusEndDate, Is.EqualTo(dto.StatusEndDate));
        Assert.That(captured.Thumbnail, Is.EqualTo(savedThumbnailPath));

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ThumbnailFolderName), Times.Once);
        imageServiceMock.Verify(img => img.SaveImageAsync(dto.ThumbnailImage, strategy, It.IsAny<CancellationToken>()), Times.Once);
        productionRepositoryMock.Verify(pr => pr.AddAsync(It.IsAny<Production>()), Times.Once);
        productionRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetEditProductionAsync_WhenIdInvalid_ReturnsNull()
    {
        // Act
        EditProductionDto? result1 = await productionService.GetEditProductionAsync(null);
        EditProductionDto? result2 = await productionService.GetEditProductionAsync("not-guid");

        // Assert
        Assert.That(result1, Is.Null);
        Assert.That(result2, Is.Null);

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetEditProductionAsync_WhenProductionNotFound_ReturnsNull()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsNoTrackingAsync(productionId))
            .ReturnsAsync((Production?)null);

        // Act
        EditProductionDto? result = await productionService.GetEditProductionAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.Null);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsNoTrackingAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetEditProductionAsync_WhenProductionExists_MapsDtoCorrectly()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        Production production = CreateProduction(
            id: productionId,
            title: "Title",
            thumbnail: "/img/productions/old.webp",
            statusType: ProductionStatusType.Preproduction,
            description: "desc",
            budget: 123m,
            statusStart: new DateTime(2026, 1, 1),
            statusEnd: new DateTime(2026, 2, 1));

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsNoTrackingAsync(productionId))
            .ReturnsAsync(production);

        // Act
        EditProductionDto? result = await productionService.GetEditProductionAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProductionId, Is.EqualTo(productionId));
        Assert.That(result.Title, Is.EqualTo("Title"));
        Assert.That(result.Description, Is.EqualTo("desc"));
        Assert.That(result.Budget, Is.EqualTo(123m));
        Assert.That(result.StatusType, Is.EqualTo(ProductionStatusType.Preproduction));
        Assert.That(result.StatusStartDate, Is.EqualTo(new DateTime(2026, 1, 1)));
        Assert.That(result.StatusEndDate, Is.EqualTo(new DateTime(2026, 2, 1)));
        Assert.That(result.CurrentThumbnailPath, Is.EqualTo("/img/productions/old.webp"));
        Assert.That(result.ThumbnailImage, Is.Null);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsNoTrackingAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateProductionAsync_WhenProductionNotFound_ReturnsFalse()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        EditProductionDto dto = new EditProductionDto
        {
            ProductionId = productionId,
            Title = "New",
            Budget = 1,
            StatusType = ProductionStatusType.Production,
            StatusStartDate = new DateTime(2026, 1, 1),
            CurrentThumbnailPath = "/img/productions/old.webp",
            ThumbnailImage = null
        };

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsync(productionId))
            .ReturnsAsync((Production?)null);

        // Act
        bool result = await productionService.UpdateProductionAsync(dto);

        // Assert
        Assert.That(result, Is.False);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateProductionAsync_WhenNoNewThumbnail_UpdatesFields_SavesAndReturnsTrue()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        Production production = CreateProduction(
            id: productionId,
            title: "Old",
            thumbnail: "/img/productions/old.webp",
            statusType: ProductionStatusType.Concept,
            description: "old desc",
            budget: 10m,
            statusStart: new DateTime(2026, 1, 1),
            statusEnd: null);

        EditProductionDto dto = new EditProductionDto
        {
            ProductionId = productionId,
            Title = "New Title",
            Description = "new desc",
            Budget = 99m,
            StatusType = ProductionStatusType.Production,
            StatusStartDate = new DateTime(2026, 3, 1),
            StatusEndDate = new DateTime(2026, 4, 1),
            CurrentThumbnailPath = "/img/productions/old.webp",
            ThumbnailImage = null
        };

        productionRepositoryMock.Setup(pr => pr.GetByIdAsync(productionId)).ReturnsAsync(production);
        productionRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);

        // Act
        bool result = await productionService.UpdateProductionAsync(dto);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(production.Title, Is.EqualTo("New Title"));
        Assert.That(production.Description, Is.EqualTo("new desc"));
        Assert.That(production.Budget, Is.EqualTo(99m));
        Assert.That(production.StatusType, Is.EqualTo(ProductionStatusType.Production));
        Assert.That(production.StatusStartDate, Is.EqualTo(new DateTime(2026, 3, 1)));
        Assert.That(production.StatusEndDate, Is.EqualTo(new DateTime(2026, 4, 1)));
        Assert.That(production.Thumbnail, Is.EqualTo("/img/productions/old.webp"));

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateProductionAsync_WhenNewThumbnailProvided_ReplacesThumbnail_SavesAndReturnsTrue()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        Production production = CreateProduction(
            id: productionId,
            title: "Old",
            thumbnail: "/img/productions/old.webp",
            statusType: ProductionStatusType.Concept);

        IFormFile newThumb = CreateFormFile("new.png", [1, 2, 3]);

        EditProductionDto dto = new EditProductionDto
        {
            ProductionId = productionId,
            Title = "New",
            Description = null,
            Budget = 1m,
            StatusType = ProductionStatusType.Production,
            StatusStartDate = new DateTime(2026, 3, 1),
            StatusEndDate = null,
            CurrentThumbnailPath = production.Thumbnail,
            ThumbnailImage = newThumb
        };

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();
        const string replacedPath = "/img/productions/replaced.webp";

        productionRepositoryMock.Setup(pr => pr.GetByIdAsync(productionId)).ReturnsAsync(production);

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ThumbnailFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.ReplaceAsync(dto.CurrentThumbnailPath, dto.ThumbnailImage, strategy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(replacedPath);

        productionRepositoryMock.Setup(pr => pr.SaveAllChangesAsync()).ReturnsAsync(1);

        // Act
        bool result = await productionService.UpdateProductionAsync(dto);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(production.Thumbnail, Is.EqualTo(replacedPath));

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ThumbnailFolderName), Times.Once);
        imageServiceMock.Verify(img => img.ReplaceAsync(dto.CurrentThumbnailPath, dto.ThumbnailImage, strategy, It.IsAny<CancellationToken>()), Times.Once);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDeleteProductionAsync_WhenIdInvalid_ReturnsNull()
    {
        // Act
        DeleteProductionDto? result1 = await productionService.GetDeleteProductionAsync(null);
        DeleteProductionDto? result2 = await productionService.GetDeleteProductionAsync("not-guid");

        // Assert
        Assert.That(result1, Is.Null);
        Assert.That(result2, Is.Null);

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDeleteProductionAsync_WhenProductionNotFound_ReturnsNull()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsync(productionId))
            .ReturnsAsync((Production?)null);

        // Act
        DeleteProductionDto? result = await productionService.GetDeleteProductionAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.Null);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetDeleteProductionAsync_WhenProductionExists_MapsCountsCorrectly()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        Production production = CreateProduction(
            id: productionId,
            title: "Title",
            thumbnail: "/img/productions/t.webp",
            statusType: ProductionStatusType.Production,
            description: "desc",
            budget: 100m);

        production.ProductionCrewMembers = new List<ProductionCrew>
        {
            new() 
                { ProductionId = productionId,
                    CrewMemberId = Guid.NewGuid(),
                    RoleType = CrewRoleType.Director,
                    Production = production, 
                    CrewMember = new Crew()
                }
        };

        production.ProductionCastMembers = new List<ProductionCast>
        {
            new()
            {
                ProductionId = productionId, 
                CastMemberId = Guid.NewGuid(), 
                Role = "Role", 
                Production = production, 
                CastMember = new Cast()
            }
        };

        production.Scenes = new List<Scene>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SceneName = "S1", 
                Location = "L", 
                SceneType = SceneType.Interior,
                Production = production, 
                ProductionId = productionId
            }
        };

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsync(productionId))
            .ReturnsAsync(production);

        // Act
        DeleteProductionDto? result = await productionService.GetDeleteProductionAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(productionId));
        Assert.That(result.Title, Is.EqualTo("Title"));
        Assert.That(result.Thumbnail, Is.EqualTo("/img/productions/t.webp"));
        Assert.That(result.Description, Is.EqualTo("desc"));
        Assert.That(result.StatusType, Is.EqualTo(ProductionStatusType.Production));
        Assert.That(result.Budget, Is.EqualTo(100m));
        Assert.That(result.CrewMembersCount, Is.EqualTo(1));
        Assert.That(result.CastMembersCount, Is.EqualTo(1));
        Assert.That(result.ScenesCount, Is.EqualTo(1));

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public void DeleteProductionAsync_WhenIdInvalid_ThrowsArgumentExceptionWithMessage()
    {
        // Arrange
        const string invalidId = "not-guid";

        // Act
        ArgumentException? ex = Assert.ThrowsAsync<ArgumentException>(
            () => productionService.DeleteProductionAsync(invalidId));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(IdIsNullOrEmptyMessage, invalidId)));

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteProductionAsync_WhenProductionNotFound_ReturnsFalse()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsync(productionId))
            .ReturnsAsync((Production?)null);

        // Act
        bool result = await productionService.DeleteProductionAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.False);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.VerifyNoOtherCalls();

        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteProductionAsync_WhenProductionExists_DeletesImageAndEntity_SavesAndReturnsTrue()
    {
        // Arrange
        Guid productionId = Guid.NewGuid();

        Production production = CreateProduction(
            id: productionId,
            title: "Title",
            thumbnail: "/img/productions/t.webp",
            statusType: ProductionStatusType.Production);

        productionRepositoryMock
            .Setup(pr => pr.GetByIdAsync(productionId))
            .ReturnsAsync(production);

        IVariantImageStrategy strategy = Mock.Of<IVariantImageStrategy>();

        imageStrategyResolverMock
            .Setup(isr => isr.Resolve(ThumbnailFolderName))
            .Returns(strategy);

        imageServiceMock
            .Setup(img => img.DeleteAsync(production.Thumbnail, strategy, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        productionRepositoryMock
            .Setup(pr => pr.DeleteAsync(production))
            .Returns(Task.CompletedTask);

        productionRepositoryMock
            .Setup(pr => pr.SaveAllChangesAsync())
            .ReturnsAsync(1);

        // Act
        bool result = await productionService.DeleteProductionAsync(productionId.ToString());

        // Assert
        Assert.That(result, Is.True);

        imageStrategyResolverMock.Verify(isr => isr.Resolve(ThumbnailFolderName), Times.Once);
        imageServiceMock.Verify(img => img.DeleteAsync(production.Thumbnail, strategy, It.IsAny<CancellationToken>()), Times.Once);

        productionRepositoryMock.Verify(pr => pr.GetByIdAsync(productionId), Times.Once);
        productionRepositoryMock.Verify(pr => pr.DeleteAsync(production), Times.Once);
        productionRepositoryMock.Verify(pr => pr.SaveAllChangesAsync(), Times.Once);

        productionRepositoryMock.VerifyNoOtherCalls();
        imageServiceMock.VerifyNoOtherCalls();
        imageStrategyResolverMock.VerifyNoOtherCalls();
    }

    private static Production CreateProduction(Guid id, string title, string? thumbnail, ProductionStatusType statusType,
        string? description = null, decimal budget = 0m, DateTime? statusStart = null, DateTime? statusEnd = null)
    {
        Production newProduction =  new Production
        {
            Id = id,
            Title = title,
            Description = description,
            Budget = budget,
            Thumbnail = thumbnail,
            StatusType = statusType,
            StatusStartDate = statusStart ?? new DateTime(2026, 1, 1),
            StatusEndDate = statusEnd
        };
        
        return newProduction;
    }

    private static IFormFile CreateFormFile(string fileName, byte[] content)
    {
        MemoryStream stream = new MemoryStream(content);
        IFormFile file = new FormFile(stream, 0, content.Length, "file", fileName);
        
        return file;
    }

    private static ProductionStatusType GetStatusFromCatalogOrFallback()
    {
        // Идеята e да вземем status от каталога, за да сме сигурни, че StatusAbstractClass няма да е празен
        IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> catalog = ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction();

        // Опитваме да вземем някой от очакваните keys (ако ги има)
        if (catalog.TryGetValue(PreProductionKey, out IReadOnlyCollection<ProductionStatusType>? preProdStatuses) && preProdStatuses.Count > 0)
            return preProdStatuses.First();

        if (catalog.TryGetValue(ProductionKey, out IReadOnlyCollection<ProductionStatusType>? prodStatuses) && prodStatuses.Count > 0)
            return prodStatuses.First();

        if (catalog.TryGetValue(PostProductionKey, out IReadOnlyCollection<ProductionStatusType>? postStatuses) && postStatuses.Count > 0)
            return postStatuses.First();

        if (catalog.TryGetValue(DistributionKey, out IReadOnlyCollection<ProductionStatusType>? distStatuses) && distStatuses.Count > 0)
            return distStatuses.First();

        // Fallback (ако някой промени каталога) – тестът пак е стабилен.
        return ProductionStatusType.Production;
    }
}
namespace Wrap.Services.Core;

using Interfaces;
using Utilities.ImageLogic.Interfaces;
using Models.Production;
using Models.Production.NestedDtos;
using Data.Models;
using Data.Models.MappingEntities;
using Data.Repository.Interfaces;
using GCommon.Enums;
using GCommon.UI;

using static GCommon.ApplicationConstants;
using static GCommon.OutputMessages.Production;
using static GCommon.DataFormat;

public class ProductionService(IProductionRepository productionRepository,
                               IImageService imageService,
                               IVariantImageStrategyResolver imageStrategyResolver) : IProductionService
{
    private static readonly IReadOnlyDictionary<ProductionStatusType, string> StatusAbstractMap =
        BuildStatusCssMap();

    private static string GetStatusAbstractClass(ProductionStatusType statusType)
        => StatusAbstractMap.GetValueOrDefault(statusType, DefaultStatus);
    
    public async Task<IReadOnlyCollection<ProductionDto>> GetAllProductionsAsync
        (int pageNumber = 1, string? status = null, bool? isActive = null, int productionsPerPage = DefaultProductionsPerPage)
    {
        int skipCount = (pageNumber - 1) * productionsPerPage;
        
        IReadOnlyCollection<ProductionStatusType>? possibleStatuses = ProductionStatusTypes(status);
        IReadOnlyCollection<Production> production = await productionRepository.GetAllProductionsAsync(
            skipCount: skipCount, 
            takeCount: productionsPerPage,
            statuses: possibleStatuses,
            isActive: isActive);

        IReadOnlyCollection<ProductionDto> dto = production
            .Select(p => new ProductionDto
            {
                Id = p.Id,
                Title = p.Title,
                ThumbnailPath = p.Thumbnail!,
                StatusType = p.StatusType
            })
            .ToArray()
            .AsReadOnly();
            
        foreach (ProductionDto productionDto in dto)
            productionDto.StatusAbstractClass = GetStatusAbstractClass(productionDto.StatusType);

        return dto;
    }

    public async Task<int> GetProductionsCountAsync(string? status = null, bool? isActive = null)
    {
        IReadOnlyCollection<ProductionStatusType>? possibleStatuses = ProductionStatusTypes(status);
        
        int productionsCount = await productionRepository.ProductionCountAsync(possibleStatuses, isActive);
        
        return productionsCount;
    }

    public async Task<DetailsProductionDto?> GetProductionDetailsAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        (Production?                              production, 
            IReadOnlyCollection<ProductionCrew>?  productionCrews, 
            IReadOnlyCollection<ProductionCast>?  productionCasts, 
            IReadOnlyCollection<Scene>?           productionScenes, 
            IReadOnlyCollection<ProductionAsset>? productionAssets, 
            IReadOnlyCollection<ShootingDay>?     productionShootingDays) data = await productionRepository.GetDetailsAsync(productionId.Value);
        
        if (data is (production:             null,
                     productionCrews:        null,
                     productionCasts:        null, 
                     productionScenes:       null,
                     productionAssets:       null, 
                     productionShootingDays: null))
            return null;

        DetailsProductionDto baseDto = new DetailsProductionDto
        {
            Id = data.production!.Id,
            Thumbnail = data.production.Thumbnail!,
            Title = data.production.Title,
            Description = data.production.Description,
            Budget = data.production.Budget,
            StatusType = data.production.StatusType,
            StatusStartDate = data.production.StatusStartDate,
            StatusEndDate = data.production.StatusEndDate,
            ScriptId = data.production.Script?.Id
        };

        IReadOnlyCollection<ProductionCrewMemberDto> crewDtos = data
            .productionCrews!
            .Select(pc => new ProductionCrewMemberDto
            {
                Id = pc.CrewMember.Id.ToString(),
                ProfileImagePath = pc.CrewMember.ProfileImagePath!,
                FirstName = pc.CrewMember.FirstName,
                LastName = pc.CrewMember.LastName,
                Role = pc.RoleType
            })
            .ToArray()
            .AsReadOnly();

        IReadOnlyCollection<ProductionCastMemberDto> castDtos = data
            .productionCasts!
            .Select(pc => new ProductionCastMemberDto
            {
                Id = pc.CastMember.Id.ToString(),
                ProfileImagePath = pc.CastMember.ProfileImagePath!,
                FirstName = pc.CastMember.FirstName,
                LastName = pc.CastMember.LastName,
                Role = pc.Role,
                Age = pc.CastMember.Age,
                Gender = pc.CastMember.Gender
            })
            .ToArray()
            .AsReadOnly();

        IReadOnlyCollection<ProductionSceneDto> sceneDtos = data
            .productionScenes!
            .Select(s => new ProductionSceneDto
            {
                SceneNumber = s.SceneNumber,
                SceneType = s.SceneType,
                SceneName = s.SceneName,
                Location = s.Location
            })
            .ToArray()
            .AsReadOnly();
        
        IReadOnlyCollection<ProductionAssetDto> assetDtos = data
            .productionAssets!
            .Select(pa => new ProductionAssetDto
            {
                Title = pa.Title,
                AssetType = pa.AssetType
            })
            .ToArray()
            .AsReadOnly();

        IReadOnlyCollection<ProductionShootingDayDto> shootingDayDtos = data
            .productionShootingDays!
            .Select(sd => new ProductionShootingDayDto
            {
                Date = sd.Date
            })
            .ToArray()
            .AsReadOnly();

        baseDto.ProductionCrewMembers = crewDtos;
        baseDto.ProductionCastMembers = castDtos;
        baseDto.ProductionScenes = sceneDtos;
        baseDto.ProductionAssets = assetDtos;
        baseDto.ProductionShootingDays = shootingDayDtos;
        
        baseDto.StatusAbstractClass = GetStatusAbstractClass(baseDto.StatusType);

        return baseDto;
    }
    
    public async Task<string?> CreateProductionAsync(CreateProductionDto dto)
    {
        IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ThumbnailFolderName);
        string thumbnail = await imageService.SaveImageAsync(dto.ThumbnailImage, strategy);

        Crew? creator = await GetCrewIdByUserIdAsync(dto.CreatorId);
        if (creator is null)
            return null;
        
        Production production = new Production
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Budget = dto.Budget,
            StatusType = dto.StatusType,
            StatusStartDate = dto.StatusStartDate,
            StatusEndDate = dto.StatusEndDate,
            Thumbnail = thumbnail,
            CreatedByUserId = dto.CreatorId
        };

        await productionRepository.AddAsync(production);
        await productionRepository.AddDirectorToProductionAsync(production.Id, creator, CrewRoleType.Director);
        await productionRepository.SaveAllChangesAsync();

        return production.Id.ToString();
    }

    public async Task<EditProductionDto?> GetEditProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        Production? production = await productionRepository.GetProductionByIdAsNoTrackingAsync(productionId.Value);
        if (production is null)
            return null;

        EditProductionDto dto = new EditProductionDto
        {
            ProductionId = production.Id,
            Title = production.Title,
            Description = production.Description,
            Budget = production.Budget,
            StatusType = production.StatusType,
            StatusStartDate = production.StatusStartDate,
            StatusEndDate = production.StatusEndDate,
            CurrentThumbnailPath = production.Thumbnail,
            ThumbnailImage = null
        };

        return dto;
    }

    public async Task<bool> UpdateProductionAsync(EditProductionDto dto)
    {
        Production? production = await productionRepository.GetProductionByIdAsync(dto.ProductionId);
        if (production is null)
            return false;

        production.Title = dto.Title;
        production.Description = dto.Description;
        production.Budget = dto.Budget;
        production.StatusType = dto.StatusType;
        production.StatusStartDate = dto.StatusStartDate;
        production.StatusEndDate = dto.StatusEndDate;

        if (dto.ThumbnailImage is not null && dto.ThumbnailImage.Length > 0)
        {
            IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ThumbnailFolderName);
            production.Thumbnail = await imageService.ReplaceAsync(dto.CurrentThumbnailPath, dto.ThumbnailImage, strategy);
        }

        await productionRepository.SaveAllChangesAsync();
        
        return true;
    }

    public async Task<DeleteProductionDto?> GetDeleteProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        Production? production = await productionRepository.GetProductionWithDataByIdAsync(productionId.Value);
        if (production is null)
            return null;

        DeleteProductionDto dto = new DeleteProductionDto
        {
            Id = production.Id,
            Title = production.Title!,
            Thumbnail = production.Thumbnail!,
            Description = production.Description,
            StatusType = production.StatusType,
            Budget = production.Budget,
            CrewMembersCount = production.ProductionCrewMembers.Count,
            CastMembersCount = production.ProductionCastMembers.Count,
            ScenesCount = production.Scenes.Count,
            AssetsCount = production.ProductionAssets.Count
        };
        
        return dto;
    }

    public async Task<bool> DeleteProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            throw new ArgumentException(string.Format(IdIsNullOrEmptyMessage, id));

        Production? production = await productionRepository.GetProductionByIdAsync(productionId.Value);
        if (production is null)
            return false;
        
        IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ThumbnailFolderName);
        
        await imageService.DeleteAsync(production.Thumbnail, strategy);
        await productionRepository.DeleteAsync(production);
        await productionRepository.SaveAllChangesAsync();
        
        return true;
    }
    
    public async Task<Guid?> GetUserIdIfIsCrewAsync(string? userId)
    {
        Guid? applicationUserId = ValidateGuid(userId);
        if (applicationUserId is null)
            return null;
        
        Crew? crew = await productionRepository.GetCrewByUserIdAsync(applicationUserId.Value);
        return crew is null ? null : applicationUserId;
    }

    public async Task<bool> IsUserAllowedToManageProductionAsync(string? productionId, string userId)
    {
        Guid? productionGuidId = ValidateGuid(productionId);
        if (productionGuidId is null)
            return false;
        
        Guid? userGuidId = ValidateGuid(userId);
        if (userGuidId is null)
            return false;

        bool isLeader = await productionRepository.IsUserProductionLeaderAsync(productionGuidId.Value, userGuidId.Value);
        return isLeader;
    }

    private async Task<Crew?> GetCrewIdByUserIdAsync(Guid userId)
    { 
        Crew? crew = await productionRepository.GetCrewByUserIdAsync(userId);
        return crew;
    }
    
    
    private static Guid? ValidateGuid(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        bool isValidId = Guid.TryParse(id, out Guid productionId);
        if (!isValidId)
            return null;
        
        return productionId;
    }
    
    private static IReadOnlyCollection<ProductionStatusType>? ProductionStatusTypes(string? status)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> statusMap 
                = ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction();

            if (statusMap.TryGetValue(status, out IReadOnlyCollection<ProductionStatusType>? statuses))
                return statuses;
        }

        return null;
    }
    
    private static IReadOnlyDictionary<ProductionStatusType, string> BuildStatusCssMap()
    {
        Dictionary<string, string> abstractNames = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [PreProductionKey] = PreProductionStatus,
            [ProductionKey] = ProductionStatus,
            [PostProductionKey] = PostProductionStatus,
            [DistributionKey] = DistributionStatus
        };

        Dictionary<ProductionStatusType, string> map = new Dictionary<ProductionStatusType, string>();

        foreach (KeyValuePair<string, IReadOnlyCollection<ProductionStatusType>> kvp in
                 ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction())
        {
            string abstractName = kvp.Key;
            IReadOnlyCollection<ProductionStatusType> statuses = kvp.Value;
            
            string cssClass = abstractNames.GetValueOrDefault(abstractName, DefaultStatus);
            
            foreach (ProductionStatusType status in statuses)
                map[status] = cssClass;
        }

        return map;
    }
}
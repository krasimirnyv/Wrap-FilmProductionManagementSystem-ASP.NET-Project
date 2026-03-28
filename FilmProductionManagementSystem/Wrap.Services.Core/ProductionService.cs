namespace Wrap.Services.Core;

using Microsoft.AspNetCore.Hosting;

using Interfaces;
using Models.Production;
using Data.Models;
using Wrap.Data.Repository.Interfaces;
using GCommon.Enums;
using GCommon.UI;

using static Utilities.HelperSaveThumbnail;
using static GCommon.ApplicationConstants;
using static GCommon.OutputMessages.Production;

public class ProductionService(IProductionRepository repository,
                               IWebHostEnvironment environment) : IProductionService
{
    
    
    private static readonly IReadOnlyDictionary<ProductionStatusType, string> StatusAbstractMap =
        BuildStatusCssMap();

    private static string GetStatusAbstractClass(ProductionStatusType statusType)
        => StatusAbstractMap.GetValueOrDefault(statusType, DefaultStatus);
    
    public async Task<ICollection<ProductionDto>> GetAllProductionsAsync(int pageNumber = 1, int productionsPerPage = DefaultProductionsPerPage)
    {
        int skipCount = (pageNumber - 1) * productionsPerPage;
        
        ICollection<ProductionDto> data = await repository.GetAllAsync(skipCount, productionsPerPage);

        foreach (ProductionDto productionDto in data)
            productionDto.StatusAbstractClass = GetStatusAbstractClass(productionDto.StatusType);

        return data;
    }

    public async Task<int> GetProductionsCountAsync()
    {
        int productionsCount = await repository.CountAsync();
        
        return productionsCount;
    }

    public async Task<DetailsProductionDto?> GetProductionDetailsAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        DetailsProductionDto? dto = await repository.GetDetailsAsync(productionId.Value);
        if (dto is null)
            return null;
        
        dto.StatusAbstractClass = GetStatusAbstractClass(dto.StatusType);
        
        return dto;
    }
    
    public async Task<string> CreateProductionAsync(CreateProductionDto dto)
    {
        Production production = new Production
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Budget = dto.Budget,
            StatusType = dto.StatusType,
            StatusStartDate = dto.StatusStartDate,
            StatusEndDate = dto.StatusEndDate,
            Thumbnail = await SaveThumbnailAsync(environment, dto.ThumbnailImage)
        };

        await repository.AddAsync(production);
        await repository.SaveAllChangesAsync();

        return production.Id.ToString();
    }

    public async Task<EditProductionDto?> GetEditProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        Production? production = await repository.GetByIdAsNoTrackingAsync(productionId.Value);
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
        Production? production = await repository.GetByIdAsync(dto.ProductionId);
        if (production is null)
            return false;

        production.Title = dto.Title;
        production.Description = dto.Description;
        production.Budget = dto.Budget;
        production.StatusType = dto.StatusType;
        production.StatusStartDate = dto.StatusStartDate;
        production.StatusEndDate = dto.StatusEndDate;

        if (dto.ThumbnailImage is not null)
            production.Thumbnail = await SaveThumbnailAsync(environment, dto.ThumbnailImage);

        await repository.SaveAllChangesAsync();
        
        return true;
    }

    public async Task<DeleteProductionDto?> GetDeleteProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        DeleteProductionDto? dto = await repository.GetDeleteAsync(productionId.Value);
        
        return dto;
    }

    public async Task<bool> DeleteProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            throw new ArgumentException(string.Format(IdIsNullOrEmptyMessage, id));

        Production? production = await repository.GetByIdAsync(productionId.Value);
        if (production is null)
            return false;
        
        await repository.DeleteAsync(production);
        await repository.SaveAllChangesAsync();
        
        return true;
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
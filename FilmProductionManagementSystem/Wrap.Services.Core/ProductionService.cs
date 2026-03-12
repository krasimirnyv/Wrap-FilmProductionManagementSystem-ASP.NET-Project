namespace Wrap.Services.Core;

using System.Globalization;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using Interface;
using Data;
using Data.Models;
using ViewModels.Production;
using ViewModels.Production.HelperViewModels;
using GCommon.Enums;

using static Utilities.HelperSaveThumbnail;
using static ViewModels.General.Helper.ProductionStatusAbstraction;
using static GCommon.DataFormat;
using static GCommon.ApplicationConstants;
using static GCommon.OutputMessages.Production;

public class ProductionService(FilmProductionDbContext context,
                               IWebHostEnvironment environment) : IProductionService
{
    
    
    private static readonly IReadOnlyDictionary<ProductionStatusType, string> StatusAbstractMap =
        BuildStatusCssMap();

    private static string GetStatusAbstractClass(ProductionStatusType statusType)
        => StatusAbstractMap.GetValueOrDefault(statusType, DefaultStatus);
    
    public async Task<IEnumerable<AllProductionsViewModel>> GetAllProductionsAsync()
    {
        var productionData = await context
            .Productions
            .AsNoTracking()
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Thumbnail,
                p.StatusType
            })
            .ToArrayAsync();

        IEnumerable<AllProductionsViewModel> productions = productionData
            .Select(p => new AllProductionsViewModel
            {
                Id = p.Id.ToString(),
                Title = p.Title,
                Thumbnail = p.Thumbnail,
                StatusType = p.StatusType.ToString(),
                StatusAbstractClass = GetStatusAbstractClass(p.StatusType)
            })
            .ToArray();

        return productions;
    }

    public async Task<DetailsProductionViewModel?> GetProductionDetailsAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        // Get production data
        var productionData = await context
            .Productions
            .AsNoTracking()
            .Where(p => p.Id == productionId)
            .Select(p => new
            {
                p.Id,
                p.Thumbnail,
                p.Title,
                p.Description,
                p.Budget,
                p.StatusType,
                p.StatusStartDate,
                p.StatusEndDate,
                ScriptId = (Guid?)p.Script!.Id
            })
            .SingleOrDefaultAsync();

        if (productionData is null)
            return null; 
        
        // Get crew members data
        IReadOnlyCollection<ProductionCrewViewModel> crewMembers = await context.
            ProductionsCrewMembers
            .AsNoTracking()
            .Where(pc => pc.ProductionId == productionId)
            .Select(pc => new ProductionCrewViewModel
            {
                ProfileImagePath = pc.CrewMember.ProfileImagePath,
                FirstName = pc.CrewMember.FirstName,
                LastName = pc.CrewMember.LastName
            })
            .ToArrayAsync();

        // Get cast members data
        IReadOnlyCollection<ProductionCastViewModel> castMembers = await context
            .ProductionsCastMembers
            .AsNoTracking()
            .Where(pm => pm.ProductionId == productionId)
            .Select(pm => new ProductionCastViewModel
            {
                ProfileImagePath = pm.CastMember.ProfileImagePath,
                FirstName = pm.CastMember.FirstName,
                LastName = pm.CastMember.LastName,
                Role = pm.CastMember.Role,
                Age = pm.CastMember.Age.ToString(),
                Gender = pm.CastMember.Gender.ToString()
            })
            .ToArrayAsync();

        // Get scenes data
        IReadOnlyCollection<ProductionSceneViewModel> scenes = await context
            .Scenes
            .AsNoTracking()
            .Where(s => s.ProductionId == productionId)
            .Select(s => new ProductionSceneViewModel
            {
                SceneNumber = s.SceneNumber.ToString(),
                SceneType = s.SceneType.ToString(),
                SceneName = s.SceneName,
                Location = s.Location
            })
            .ToArrayAsync();

        // Get production's assets data
        IReadOnlyCollection<ProductionAssetViewModel> assets = await context
            .ProductionsAssets
            .AsNoTracking()
            .Where(a => a.ProductionId == productionId)
            .Select(a => new ProductionAssetViewModel
            {
                AssetType = a.AssetType.ToString(),
                Title = a.Title
            })
            .ToArrayAsync();

        // Get shooting days data
        IReadOnlyCollection<ProductionShootingDayViewModel> shootingDays = await context
            .ShootingDays
            .AsNoTracking()
            .Where(sd => sd.ProductionId == productionId)
            .Select(sd => new ProductionShootingDayViewModel
            {
                Date = sd.Date.ToString(DateFormat, CultureInfo.CurrentCulture)
            })
            .ToArrayAsync();

        // Assemble the details production view model
        DetailsProductionViewModel? production = new DetailsProductionViewModel
        {
            Id = productionData.Id.ToString(),
            Thumbnail = productionData.Thumbnail!,
            Title = productionData.Title,
            Description = productionData.Description,
            Budget = productionData.Budget.ToString(CurrencyFormat, CultureInfo.CurrentCulture),
            StatusType = productionData.StatusType.ToString(),
            StatusAbstractClass = GetStatusAbstractClass(productionData.StatusType),
            StatusStartDate = productionData.StatusStartDate.ToString(DateFormat, CultureInfo.CurrentCulture),
            StatusEndDate = productionData.StatusEndDate?.ToString(DateFormat, CultureInfo.CurrentCulture) ?? NotAvailableFormat,
            ScriptId = productionData.ScriptId?.ToString(),

            ProductionCrewMembers = crewMembers,
            ProductionCastMembers = castMembers,
            Scenes = scenes,
            ProductionAssets = assets,
            ShootingDays = shootingDays
        };
        
        return production;
    }
    
    public async Task<string> CreateProductionInputModelAsync(CreateProductionInputModel inputModel)
    {
        Production production = new Production
        {
            Id = Guid.NewGuid(),
            Title = inputModel.Title,
            Description = inputModel.Description,
            Budget = inputModel.Budget,
            StatusType = inputModel.StatusType,
            StatusStartDate = inputModel.StatusStartDate,
            StatusEndDate = inputModel.StatusEndDate,
            Thumbnail = await SaveThumbnailAsync(environment, inputModel.Thumbnail)
        };

        await context.Productions.AddAsync(production);
        await context.SaveChangesAsync();

        return production.Id.ToString();
    }

    public async Task<EditProductionInputModel?> GetEditProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        Production? production = await context
            .Productions
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == productionId);

        if (production is null)
            return null;

        EditProductionInputModel model = new EditProductionInputModel
        {
            Title = production.Title,
            Description = production.Description,
            Budget = production.Budget,
            StatusType = production.StatusType,
            StatusStartDate = production.StatusStartDate,
            StatusEndDate = production.StatusEndDate,
            CurrentThumbnail = production.Thumbnail
        };

        return model;
    }

    public async Task UpdateProductionAsync(string id, EditProductionInputModel inputModel)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            throw new ArgumentException(string.Format(IdIsNullOrEmptyMessage, id));

        Production? production = await context
            .Productions
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        if (production is null)
            throw new InvalidOperationException(string.Format(NotFoundMessage, id));

        production.Title = inputModel.Title;
        production.Description = inputModel.Description;
        production.Budget = inputModel.Budget;
        production.StatusType = inputModel.StatusType;
        production.StatusStartDate = inputModel.StatusStartDate;
        production.StatusEndDate = inputModel.StatusEndDate;

        if (inputModel.Thumbnail is not null)
            production.Thumbnail = await SaveThumbnailAsync(environment, inputModel.Thumbnail);
        
        await context.SaveChangesAsync();
    }

    public async Task<DeleteProductionViewModel?> GetDeleteProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            return null;

        var productionData = await context
            .Productions
            .AsNoTracking()
            .Where(p => p.Id == productionId)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Thumbnail,
                p.Description,
                p.StatusType,
                p.Budget,
                CrewCount = p.ProductionCrewMembers.Count,
                CastCount = p.ProductionCastMembers.Count,
                ScenesCount = p.Scenes.Count
            })
            .SingleOrDefaultAsync();
        
        if (productionData is null)
            return null;

        DeleteProductionViewModel model = new DeleteProductionViewModel
        {
            Id = productionData.Id.ToString(),
            Title = productionData.Title,
            Thumbnail = productionData.Thumbnail,
            Description = productionData.Description,
            StatusType = productionData.StatusType.ToString(),
            Budget = productionData.Budget.ToString(CurrencyFormat, CultureInfo.CurrentCulture),
            CrewMembersCount = productionData.CrewCount,
            CastMembersCount = productionData.CastCount,
            ScenesCount = productionData.ScenesCount
        };
        
        return model;
    }

    public async Task DeleteProductionAsync(string? id)
    {
        Guid? productionId = ValidateGuid(id);
        if (productionId is null)
            throw new ArgumentException(string.Format(IdIsNullOrEmptyMessage, id));

        Production? production = await context
            .Productions
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        if (production is null)
            throw new InvalidOperationException(string.Format(NotFoundMessage, id));
        
        context.Productions.Remove(production);
        await context.SaveChangesAsync();
    }
    
    private static Guid? ValidateGuid(string? id)
    {
        if (string.IsNullOrEmpty(id))
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

        foreach ((string abstractName, IReadOnlyCollection<ProductionStatusType> statuses) in GetStatusTypeByAbstraction())
        {
            string cssClass = abstractNames.GetValueOrDefault(abstractName, DefaultStatus);
            foreach (ProductionStatusType status in statuses)
                map[status] = cssClass;
        }

        return map;
    }
}
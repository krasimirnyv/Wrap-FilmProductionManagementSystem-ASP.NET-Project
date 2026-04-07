namespace FilmProductionManagementSystem.Web.Controllers;

using System.Globalization;

using Microsoft.AspNetCore.Mvc;

using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.Production;
using Wrap.Web.ViewModels.Production;
using Wrap.Web.ViewModels.Production.NestedViewModels;

using static Wrap.GCommon.OutputMessages;
using static Wrap.GCommon.OutputMessages.Production;
using static Wrap.GCommon.OutputMessages.Profile;
using static Wrap.GCommon.ApplicationConstants;
using static Wrap.GCommon.DataFormat;

public class ProductionController(IProductionService productionService,
                                  ILogger<ProductionController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AllProductionsIndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData[ErrorTempDateKey] = PaginationFailedMessage;
            return RedirectToAction("Dashboard", "Home");
        }
        
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            IReadOnlyCollection<ProductionDto> productionDtos = await productionService.GetAllProductionsAsync(
                pageNumber: model.PageNumber,
                status: model.SelectedStatus,
                isActive: model.IsActive);
            
            AllProductionsIndexViewModel viewModel = await MapToAllProductionsIndexViewModelFromDto(model, productionDtos);
            
            Guid? userIsCrew = await productionService.GetUserIdIfIsCrewAsync(userId); 
            viewModel.IsUserCrew = userIsCrew is not null;
            
            return View(viewModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(LoadingManyProductionsErrorMessageWithException, e.Message));
            TempData[ErrorTempDateKey] = LoadingManyProductionsErrorMessage;
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string? productionId)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            DetailsProductionDto? dto = await productionService.GetProductionDetailsAsync(productionId);
            if (dto is null)
            {
                logger.LogWarning(string.Format(IdIsNullOrEmptyMessage, productionId));
                return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));
            }
            
            DetailsProductionViewModel viewModel = MapToDetailsProductionViewModelFromDto(dto);
            
            bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
            viewModel.CanManage = canManage;
            
            return View(viewModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, DetailsMessage, e.Message));
            return View(nameof(NotFound), string.Format(IdIsNullOrEmptyMessage, productionId));
        }
    }
    
    private async Task<AllProductionsIndexViewModel> MapToAllProductionsIndexViewModelFromDto(AllProductionsIndexViewModel model, IReadOnlyCollection<ProductionDto> productionDtos)
    {
        int productionsTotalCount = await productionService.GetProductionsCountAsync(
            status: model.SelectedStatus,
            isActive: model.IsActive);
        
        IReadOnlyCollection<ProductionViewModel> productions = MapToProductionViewModelsFromDtos(productionDtos);
        
        AllProductionsIndexViewModel viewModel = new AllProductionsIndexViewModel
        {
            PageNumber = model.PageNumber,
            TotalPages = (int)Math.Ceiling(productionsTotalCount / (double)DefaultProductionsPerPage),
            ShowingPages = model.ShowingPages,
            Productions = productions,
            TotalCount = productionsTotalCount,
            SelectedStatus = model.SelectedStatus,
            IsActive = model.IsActive
        };

        if (viewModel.PageNumber > viewModel.TotalPages && viewModel.TotalPages != 0)
            viewModel.PageNumber = viewModel.TotalPages;
        else if (viewModel.PageNumber < 1)
            viewModel.PageNumber = 1;
        
        return viewModel;
    }
    
    private static IReadOnlyCollection<ProductionViewModel> MapToProductionViewModelsFromDtos(IReadOnlyCollection<ProductionDto> productionDtos)
    {
        IReadOnlyCollection<ProductionViewModel> viewModels = productionDtos
            .Select(dto => new ProductionViewModel
            {
                Id = dto.Id.ToString(),
                Title = dto.Title,
                ThumbnailPath = dto.ThumbnailPath,
                StatusType = dto.StatusType.ToString()
            })
            .ToArray()
            .AsReadOnly();
        
        return viewModels;
    }
    
    private static DetailsProductionViewModel MapToDetailsProductionViewModelFromDto(DetailsProductionDto dto)
    {
        DetailsProductionViewModel viewModel = new DetailsProductionViewModel
        {
            Id = dto.Id.ToString(),
            Thumbnail = dto.Thumbnail,
            Title = dto.Title,
            Description = dto.Description,
            Budget = dto.Budget.ToString(CurrencyFormat, CultureInfo.CurrentCulture),
            StatusType = dto.StatusType.ToString(),
            StatusAbstractClass = dto.StatusAbstractClass,
            StatusStartDate = dto.StatusStartDate.ToString(DateFormat, CultureInfo.CurrentCulture),
            StatusEndDate = dto.StatusEndDate?.ToString(DateFormat, CultureInfo.CurrentCulture) ?? NotAvailableFormat,
            ScriptId = dto.ScriptId.ToString() ?? string.Empty,

            ProductionCrewMembers = dto.ProductionCrewMembers
                .Select(pCrew => new ProductionCrewMemberViewModel
                {
                    Id = pCrew.Id,
                    ProfileImagePath = pCrew.ProfileImagePath,
                    FirstName = pCrew.FirstName,
                    LastName = pCrew.LastName,
                    Role = pCrew.Role.ToString()
                })
                .ToArray()
                .AsReadOnly(),

            ProductionCastMembers = dto.ProductionCastMembers
                .Select(pCast => new ProductionCastMemberViewModel
                {
                    Id = pCast.Id,
                    ProfileImagePath = pCast.ProfileImagePath,
                    FirstName = pCast.FirstName,
                    LastName = pCast.LastName,
                    Role = pCast.Role,
                    Age = pCast.Age.ToString(),
                    Gender = pCast.Gender.ToString()
                })
                .ToArray()
                .AsReadOnly(),

            ProductionScenes = dto.ProductionScenes
                .Select(ps => new ProductionSceneViewModel
                {
                    SceneNumber = ps.SceneNumber.ToString(),
                    SceneType = ps.SceneType.ToString(),
                    SceneName = ps.SceneName,
                    Location = ps.Location
                })
                .ToArray()
                .AsReadOnly(),

            ProductionAssets = dto.ProductionAssets
                .Select(pa => new ProductionAssetViewModel
                {
                    AssetType = pa.AssetType.ToString(),
                    Title = pa.Title
                })
                .ToArray()
                .AsReadOnly(),

            ProductionShootingDays = dto.ProductionShootingDays
                .Select(psd => new ProductionShootingDayViewModel
                {
                    Date = psd.Date.ToString(DateFormat, CultureInfo.CurrentCulture)
                })
                .ToArray()
                .AsReadOnly()
        };
        
        return viewModel;
    }
}
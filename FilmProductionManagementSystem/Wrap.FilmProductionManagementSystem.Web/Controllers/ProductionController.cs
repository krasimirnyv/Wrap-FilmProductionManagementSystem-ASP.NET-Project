namespace FilmProductionManagementSystem.Web.Controllers;

using System.Globalization;

using Microsoft.AspNetCore.Mvc;

using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.Production;
using Wrap.Web.ViewModels.Production;
using Wrap.Web.ViewModels.Production.NestedViewModels;

using static Wrap.GCommon.OutputMessages;
using static Wrap.GCommon.OutputMessages.Production;
using static Wrap.GCommon.ApplicationConstants;
using static Wrap.GCommon.DataFormat;

public class ProductionController(IProductionService productionService,
                                  ILogger<ProductionController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(AllProductionsIndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData[ErrorTempDateKey] = PaginationFailedMessage;
            return RedirectToAction("Dashboard", "Home");
        }
        
        try
        {
            IReadOnlyCollection<ProductionDto> productionDtos = await productionService.GetAllProductionsAsync(model.PageNumber);
            AllProductionsIndexViewModel viewModel = await MapToAllProductionsIndexViewModelFromDto(model, productionDtos);
            
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
        try
        {
            DetailsProductionDto? dto = await productionService.GetProductionDetailsAsync(productionId);
            if (dto is null)
            {
                logger.LogWarning(string.Format(IdIsNullOrEmptyMessage, productionId));
                return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));
            }
            
            DetailsProductionViewModel viewModel = MapToDetailsProductionViewModelFromDto(dto);
            
            return View(viewModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, DetailsMessage, e.Message));
            return View(nameof(NotFound), string.Format(IdIsNullOrEmptyMessage, productionId));
        }
    }

    [HttpGet]
    public IActionResult Create()
        => View(new CreateProductionInputModel());

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductionInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View(inputModel);

        try
        {
            CreateProductionDto dto = MapToCreateProductionDtoFromInputModel(inputModel);
            string productionId = await productionService.CreateProductionAsync(dto);
            
            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, CreatedMessage);
            return RedirectToAction(nameof(Details), new { productionId });
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, CreatingMessage, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(CrudFailureMessage, CreatingMessage, e.Message));
            return View(inputModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? productionId)
    {
        try
        {
            EditProductionDto? dto = await productionService.GetEditProductionAsync(productionId);
            if (dto is null)
                return View(nameof(NotFound), string.Format(IdIsNullOrEmptyMessage, productionId));

            EditProductionInputModel inputModel = MapToEditProductionInputModelFromDto(dto);
            
            return View(inputModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(LoadingProductionErrorMessageWithException, productionId, e.Message));
            return View(nameof(BadRequest), string.Format(LoadingProductionErrorMessage, productionId));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProductionInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View(inputModel);
        
        try
        {
            EditProductionDto dto = MapToEditProductionDtoFromInputModel(inputModel);
            
            bool isUpdated = await productionService.UpdateProductionAsync(dto);
            if (!isUpdated)
                return View(nameof(NotFound), string.Format(NotFoundMessage, dto.ProductionId));
            
            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, UpdatedMessage);
            return RedirectToAction(nameof(Details), new { dto.ProductionId });
        }
        catch (ArgumentException ae)
        {
            logger.LogError(ae, string.Format(CrudFailureMessage, UpdatingMessage, ae.Message));
            TempData[ErrorTempDateKey] = string.Format(CrudFailureMessage, UpdatingMessage, ae.Message);
            return View(nameof(NotFound), string.Format(NotFoundMessage, inputModel.ProductionId));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, UpdatingMessage, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(CrudFailureMessage, UpdatingMessage, e.Message));
            return View(inputModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string? productionId)
    {
        try
        {
            DeleteProductionDto? dto = await productionService.GetDeleteProductionAsync(productionId);
            if (dto is null)
                return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));

            DeleteProductionViewModel viewModel = MapToDeleteProductionViewModelFromDto(dto);
            
            return View(viewModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, DeletingMessage, e.Message));
            return View(nameof(BadRequest), string.Format(LoadingProductionErrorMessage, productionId));
        }
    }

    [HttpPost] 
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(string? productionId)
    {
        try
        {
            bool isDeleted = await productionService.DeleteProductionAsync(productionId);
            if (!isDeleted)
                return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));

            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, DeletedMessage);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ae)
        {
            logger.LogError(ae, string.Format(CrudFailureMessage, DeletingMessage, ae.Message));
            TempData[ErrorTempDateKey] = string.Format(CrudFailureMessage, DeletingMessage, ae.Message);
            return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, DeletingMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(CrudFailureMessage, DeletingMessage, e.Message);
            return RedirectToAction(nameof(Details), new { productionId });
        }
    }
    
    // TODO: Future functionality - not implemented yet but will be added in the future.
    public IActionResult AllActiveProjects()
    {
        throw new NotImplementedException();
    }
    
    public IActionResult AddMember()
    {
        throw new NotImplementedException();
    }
    
    public IActionResult Budget()
    {
        throw new NotImplementedException();
    }
    
    private async Task<AllProductionsIndexViewModel> MapToAllProductionsIndexViewModelFromDto(AllProductionsIndexViewModel model, IReadOnlyCollection<ProductionDto> productionDtos)
    {
        int productionsTotalCount = await productionService.GetProductionsCountAsync();
        IReadOnlyCollection<ProductionViewModel> productions = MapToProductionViewModelsFromDtos(productionDtos);
        
        AllProductionsIndexViewModel viewModel = new AllProductionsIndexViewModel
        {
            PageNumber = model.PageNumber,
            TotalPages = (int)Math.Ceiling(productionsTotalCount / (double)DefaultProductionsPerPage),
            ShowingPages = model.ShowingPages,
            Productions = productions,
            TotalCount = productionsTotalCount
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
    
    private static CreateProductionDto MapToCreateProductionDtoFromInputModel(CreateProductionInputModel inputModel)
    {
        CreateProductionDto dto = new CreateProductionDto
        {
            ThumbnailImage = inputModel.ThumbnailImage,
            Title = inputModel.Title,
            Description = inputModel.Description,
            Budget = inputModel.Budget,
            StatusType = inputModel.StatusType,
            StatusStartDate = inputModel.StatusStartDate,
            StatusEndDate = inputModel.StatusEndDate
        };
        
        return dto;
    }
    
    private static EditProductionInputModel MapToEditProductionInputModelFromDto(EditProductionDto dto)
    {
        EditProductionInputModel inputModel = new EditProductionInputModel
        {
            ProductionId = dto.ProductionId.ToString(),
            ThumbnailImage = dto.ThumbnailImage,
            Title = dto.Title,
            Description = dto.Description,
            Budget = dto.Budget,
            StatusType = dto.StatusType,
            StatusStartDate = dto.StatusStartDate,
            StatusEndDate = dto.StatusEndDate,
            CurrentThumbnailPath = dto.CurrentThumbnailPath
        };
        
        return inputModel;
    }
    
    private static EditProductionDto MapToEditProductionDtoFromInputModel(EditProductionInputModel inputModel)
    {
        EditProductionDto dto = new EditProductionDto
        {
            ProductionId = Guid.Parse(inputModel.ProductionId),
            ThumbnailImage = inputModel.ThumbnailImage,
            Title = inputModel.Title,
            Description = inputModel.Description,
            Budget = inputModel.Budget,
            StatusType = inputModel.StatusType,
            StatusStartDate = inputModel.StatusStartDate,
            StatusEndDate = inputModel.StatusEndDate,
            CurrentThumbnailPath = inputModel.CurrentThumbnailPath
        };
        
        return dto;
    }
    
    private static DeleteProductionViewModel MapToDeleteProductionViewModelFromDto(DeleteProductionDto dto)
    {
        DeleteProductionViewModel viewModel = new DeleteProductionViewModel
        {
            Id = dto.Id.ToString(),
            Title = dto.Title,
            Thumbnail = dto.Thumbnail,
            Description = dto.Description,
            StatusType = dto.StatusType.ToString(),
            Budget = dto.Budget.ToString(CurrencyFormat, CultureInfo.CurrentCulture),
            CrewMembersCount = dto.CrewMembersCount,
            CastMembersCount = dto.CastMembersCount,
            ScenesCount = dto.ScenesCount
        };
        
        return viewModel;
    }
}
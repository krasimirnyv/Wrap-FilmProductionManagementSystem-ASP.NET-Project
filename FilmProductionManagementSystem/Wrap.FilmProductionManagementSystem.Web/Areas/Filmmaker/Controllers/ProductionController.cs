namespace FilmProductionManagementSystem.Web.Areas.Filmmaker.Controllers;

using System.Globalization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using FilmProductionManagementSystem.Web.Controllers;
using Wrap.Web.ViewModels.Production;
using Wrap.Web.ViewModels.FindPeople;
using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.Production;
using Wrap.Services.Models.FindPeople;
using Wrap.GCommon.Enums;

using static Wrap.GCommon.ApplicationConstants;
using static Wrap.GCommon.OutputMessages;
using static Wrap.GCommon.OutputMessages.Profile;
using static Wrap.GCommon.OutputMessages.Production;
using static Wrap.GCommon.DataFormat;

[Area("Filmmaker")]
[Authorize(Roles = IdentityRoles.Filmmaker)]
public class ProductionController(IProductionService productionService,
                                  IFindPeopleService findPeopleService,
                                  ILogger<ProductionController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        Guid? creatorId = await productionService.GetUserIdIfIsCrewAsync(userId);
        if (creatorId is null)
        {
            logger.LogWarning(CastTryingToCreateProductionMessage);
            return View(nameof(BadRequest), CastTryingToCreateProductionMessage);
        }

        return View(new CreateProductionInputModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductionInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View(inputModel);

        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        Guid? creatorId = await productionService.GetUserIdIfIsCrewAsync(userId);
        if (creatorId is null)
        {
            logger.LogWarning(CastTryingToCreateProductionMessage);
            return View(nameof(BadRequest), CastTryingToCreateProductionMessage);
        }

        try
        {
            CreateProductionDto dto = MapToCreateProductionDtoFromInputModel(inputModel, creatorId.Value);
            string? productionId = await productionService.CreateProductionAsync(dto);
            if (productionId is null)
            {
                logger.LogError(string.Format(CrudFailureMessage, CreatingMessage, UserNotIdentifiedMessage));
                return View(nameof(BadRequest), string.Format(CrudFailureMessage, CreatingMessage, UserNotIdentifiedMessage));
            }

            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, CreatedMessage);
            return RedirectToAction("Details", "Production", new { area = string.Empty, productionId});
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
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Index", "Production", new { area = string.Empty });
        }

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

        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(inputModel.ProductionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Index", "Production", new { area = string.Empty });
        }

        try
        {
            EditProductionDto dto = MapToEditProductionDtoFromInputModel(inputModel);
            bool isUpdated = await productionService.UpdateProductionAsync(dto);
            if (!isUpdated)
                return View(nameof(NotFound), string.Format(NotFoundMessage, dto.ProductionId));

            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, UpdatedMessage);
            return RedirectToAction("Details", "Production", new { area = string.Empty, productionId = dto.ProductionId });
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
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Index", "Production", new { area = string.Empty });
        }

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
    public async Task<IActionResult> DeleteConfirmed(string? productionId)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Index", "Production", new { area = string.Empty });
        }

        try
        {
            bool isDeleted = await productionService.DeleteProductionAsync(productionId);
            if (!isDeleted)
                return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));

            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, DeletedMessage);
            return RedirectToAction("Index", "Production", new { area = string.Empty });
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
            return RedirectToAction("Details", "Production", new { area = string.Empty, productionId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddCrewMember(string? productionId)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Details", "Production", new { area = string.Empty, productionId });
        }
        
        return RedirectToAction("FindFilmmakers", "Home", new { area = string.Empty, ProductionId = productionId });
    }
    
    [HttpPost]
    public async Task<IActionResult> AddFilmmakerToProduction(AddFilmmakerViewModel viewModel)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(viewModel.ProductionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Details", "Production", new { area = string.Empty, viewModel.ProductionId });
        }

        AddFilmmakerDto? dto = MapFromAddFilmmakerViewModelToDto(viewModel);
        if (dto is null)
            return View(nameof(BadRequest), SomethingWentWrong);

        await findPeopleService.AddCrewAsync(dto);
        TempData[SuccessTempDataKey] = AddedCrew;
        return RedirectToAction("FindFilmmakers", "Home", new { area = string.Empty, viewModel.ProductionId });
    }

    [HttpPost]
    public async Task<IActionResult> AddActorToProduction(AddActorViewModel viewModel)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(viewModel.ProductionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Details", "Production", new { area = string.Empty, viewModel.ProductionId });
        }

        AddActorDto? dto = MapFromAddActorViewModelToDto(viewModel);
        if (dto is null)
            return View(nameof(BadRequest), SomethingWentWrong);

        await findPeopleService.AddCastAsync(dto);
        TempData[SuccessTempDataKey] = AddedCast;
        return RedirectToAction("FindActors", "Home", new { area = string.Empty, viewModel.ProductionId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFilmmakerFromProduction(string productionId, string crewId)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Details", "Production", new { area = string.Empty, productionId });
        }

        bool isProductionIdGuid = Guid.TryParse(productionId, out Guid productionGuid);
        bool isCrewIdGuid = Guid.TryParse(crewId, out Guid crewGuid);
        if (!isProductionIdGuid || !isCrewIdGuid)
            return View(nameof(BadRequest), SomethingWentWrong);

        await findPeopleService.RemoveCrewAsync(productionGuid, crewGuid);
        TempData[SuccessTempDataKey] = RemovedCrew;
        return RedirectToAction("FindFilmmakers", "Home", new { area = string.Empty, ProductionId = productionId });
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveActorFromProduction(string productionId, string castId)
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        bool canManage = await productionService.IsUserAllowedToManageProductionAsync(productionId, userId);
        if (!canManage)
        {
            TempData[WarningTempDataKey] = ManageDenied;
            return RedirectToAction("Details", "Production", new { area = string.Empty, productionId });
        }

        bool isProductionIdGuid = Guid.TryParse(productionId, out Guid productionGuid);
        bool isCastIdGuid = Guid.TryParse(castId, out Guid castGuid);
        if (!isProductionIdGuid || !isCastIdGuid)
            return View(nameof(BadRequest), SomethingWentWrong);

        await findPeopleService.RemoveCastAsync(productionGuid, castGuid);
        TempData[SuccessTempDataKey] = RemovedCast;
        return RedirectToAction("FindActors", "Home", new { area = string.Empty, ProductionId = productionId });
    }
    
    private static CreateProductionDto MapToCreateProductionDtoFromInputModel(CreateProductionInputModel inputModel, Guid userId)
    {
        CreateProductionDto dto = new CreateProductionDto
        {
            ThumbnailImage = inputModel.ThumbnailImage,
            Title = inputModel.Title,
            Description = inputModel.Description,
            Budget = inputModel.Budget,
            StatusType = inputModel.StatusType,
            StatusStartDate = inputModel.StatusStartDate,
            StatusEndDate = inputModel.StatusEndDate,
            CreatorId = userId
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
            ScenesCount = dto.ScenesCount,
            AssetsCount = dto.AssetsCount
        };

        return viewModel;
    }
    
    private static AddFilmmakerDto? MapFromAddFilmmakerViewModelToDto(AddFilmmakerViewModel viewModel)
    {
        bool isProductionIdOk = Guid.TryParse(viewModel.ProductionId, out Guid productionId);
        bool isCrewIdOk = Guid.TryParse(viewModel.CrewId, out Guid crewId);
        bool isRoleOk = Enum.TryParse(viewModel.RoleType, out CrewRoleType roleType);

        if (!isProductionIdOk || !isCrewIdOk || !isRoleOk) 
            return null;
        
        AddFilmmakerDto dto = new AddFilmmakerDto
        {
            ProductionId = productionId,
            CrewId = crewId,
            RoleType = roleType
        };
            
        return dto;
    }
    
    private static AddActorDto? MapFromAddActorViewModelToDto(AddActorViewModel viewModel)
    {
        bool isProductionIdOk = Guid.TryParse(viewModel.ProductionId, out Guid productionId);
        bool isCastIdOk = Guid.TryParse(viewModel.CastId, out Guid castId);

        if (!isProductionIdOk || !isCastIdOk || string.IsNullOrWhiteSpace(viewModel.RoleName)) 
            return null;
        
        AddActorDto dto = new AddActorDto
        {
            ProductionId = productionId,
            CastId = castId,
            RoleName = viewModel.RoleName
        };
        
        return dto;
    }
}
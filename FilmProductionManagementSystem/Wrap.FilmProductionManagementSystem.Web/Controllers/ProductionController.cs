namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

using Wrap.Services.Core.Interfaces;
using Wrap.ViewModels.Production;

using static Wrap.GCommon.OutputMessages;
using static Wrap.GCommon.OutputMessages.Production;
using static Wrap.GCommon.ApplicationConstants;

public class ProductionController(IProductionService productionService,
                                  ILogger<ProductionController> logger) : BaseController
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            IEnumerable<AllProductionsViewModel> model = await productionService.GetAllProductionsAsync();
            
            return View(model);
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
            DetailsProductionViewModel? model = await productionService.GetProductionDetailsAsync(productionId);
            if (model is null)
            {
                logger.LogWarning(string.Format(IdIsNullOrEmptyMessage, productionId));
                return View(nameof(NotFound), string.Format(NotFoundMessage, productionId));
            }
            
            return View(model);
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
    public async Task<IActionResult> Create(CreateProductionInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            string productionId = await productionService.CreateProductionInputModelAsync(model);
            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, CreatedMessage);
            return RedirectToAction(nameof(Details), new { productionId });
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, CreatingMessage, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(CrudFailureMessage, CreatingMessage, e.Message));
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? productionId)
    {
        try
        {
            EditProductionInputModel? model = await productionService.GetEditProductionAsync(productionId);
            
            if (model is null)
                return View(nameof(NotFound), string.Format(IdIsNullOrEmptyMessage, productionId));
            
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(LoadingProductionErrorMessageWithException, productionId, e.Message));
            return View(nameof(BadRequest), string.Format(LoadingProductionErrorMessage, productionId));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string productionId, EditProductionInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await productionService.UpdateProductionAsync(productionId, model);
            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, UpdatedMessage);
            return RedirectToAction(nameof(Details), new { productionId });
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, UpdatingMessage, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(CrudFailureMessage, UpdatingMessage, e.Message));
            return View(model);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(string? productionId)
    {
        try
        {
            DeleteProductionViewModel? model = await productionService.GetDeleteProductionAsync(productionId);
            if (model is null)
                return View(nameof(NotFound), string.Format(IdIsNullOrEmptyMessage, productionId));
            
            return View(model);
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
            await productionService.DeleteProductionAsync(productionId);
            TempData[SuccessTempDataKey] = string.Format(CrudSuccessMessage, DeletedMessage);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(CrudFailureMessage, DeletingMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(CrudFailureMessage, DeletingMessage, e.Message);
            return RedirectToAction(nameof(Details), new { productionId });
        }
    }
    
    public IActionResult AllActiveProjects()
    {
        throw new NotImplementedException();
    }
    
    // TODO: Future functionality - not implemented yet but will be added in the future.
    public IActionResult AddMember()
    {
        throw new NotImplementedException();
    }
    
    public IActionResult Budget()
    {
        throw new NotImplementedException();
    }
}
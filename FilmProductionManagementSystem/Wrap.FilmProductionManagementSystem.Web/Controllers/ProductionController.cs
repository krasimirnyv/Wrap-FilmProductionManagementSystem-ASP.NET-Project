namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Wrap.Services.Core.Interface;

using Wrap.ViewModels.Production;

[Authorize]
public class ProductionController(IProductionService productionService,
                                  ILogger<ProductionController> logger) : Controller
{
    private const string CreateSuccessMessage = "Production created successfully!";
    private const string UpdateSuccessMessage = "Production updated successfully!";
    private const string DeleteSuccessMessage = "Production deleted successfully!";
    
    private const string NotFoundMessage = "Production's ID is not found!";
    private const string IdIsNullOrEmptyMessage = "Production's ID is null or empty.";


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
            logger.LogError(e, "Error loading productions. {ExMessage}", e.Message);
            TempData["Error"] = "Error loading productions.";
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string? productionId)
    {
        try
        {
            if (string.IsNullOrEmpty(productionId))
            {
                logger.LogWarning(IdIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }
            
            DetailsProductionViewModel? model = await productionService.GetProductionDetailsAsync(productionId);
            if (model is null)
                return View("NotFound", NotFoundMessage);
            
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to visualize the details of the project with ID {productionId}. {ExMessage}", productionId, e.Message);
            return View("NotFound", NotFoundMessage);
        }
    }

    [HttpGet]
    public IActionResult Create()
        => View(new CreateProductionInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductionInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            string productionId = await productionService.CreateProductionInputModelAsync(model);
            TempData["SuccessMessage"] = CreateSuccessMessage;
            return RedirectToAction("Details", new { productionId });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to create a production with the provided data. {ExMessage}", e.Message);
            ModelState.AddModelError(string.Empty, $"Error: {e.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? productionId)
    {
        try
        {
            if (string.IsNullOrEmpty(productionId))
            {
                logger.LogWarning(IdIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }
            
            EditProductionInputModel? model = await productionService.GetEditProductionAsync(productionId);
            
            if (model is null)
                return View("NotFound", NotFoundMessage);
            
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error loading production for edit. {ExMessage}", e.Message);
            return View("BadRequest");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string productionId, EditProductionInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await productionService.UpdateProductionAsync(productionId, model);
            TempData["SuccessMessage"] = UpdateSuccessMessage;
            return RedirectToAction("Details", new { productionId });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to create a production with the provided data, {ExMessage}", e.Message);
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(string? productionId)
    {
        try
        {
            if (string.IsNullOrEmpty(productionId))
            {
                logger.LogWarning(IdIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }
            
            DeleteProductionViewModel? model = await productionService.GetDeleteProductionAsync(productionId);
            if (model == null)
                return View("NotFound", NotFoundMessage);
            
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error loading production for delete. {ExMessage}", e.Message);
            return View("BadRequest");
        }
    }
    
    [HttpPost] 
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string productionId)
    {
        try
        {
            await productionService.DeleteProductionAsync(productionId);
            TempData["SuccessMessage"] = DeleteSuccessMessage;
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting production. {ExMessage}", e.Message);
            TempData["Error"] = $"Error deleting production: {e.Message}";
            return RedirectToAction("Details", new { productionId });
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
namespace FilmProductionManagementSystem.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Wrap.ViewModels;
using Wrap.ViewModels.General;
using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.General;
using Wrap.GCommon.Enums;
using Wrap.GCommon.UI;

using static Wrap.GCommon.OutputMessages.Home;

public class HomeController(IHomeService homeService) : BaseController
{ 
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        DashboardDataDto dto = await homeService.GetDashboardDataAsync();

        IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> statusMap = 
            ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction();
        
        DashboardViewModel viewModel = MapToDashboardViewModelFromDto(dto, statusMap);
            
        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/Home/Error/{statusCode:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => View(nameof(BadRequest)),
            StatusCodes.Status404NotFound => View(nameof(NotFound)),
            StatusCodes.Status500InternalServerError => View("InternalServerError"),
            StatusCodes.Status405MethodNotAllowed => RedirectToAction(nameof(Index)),
            _ => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier })
        }; 
    }
    
    private DashboardViewModel MapToDashboardViewModelFromDto(DashboardDataDto dto, IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> statusMap)
    {
        DashboardViewModel viewModel = new DashboardViewModel
        {
            CrewMembersCount = dto.CrewMembersCount,
            CastMembersCount = dto.CastMembersCount,
            UpcomingScenesTotal = dto.UpcomingScenesTotal,
            Productions = dto.Productions
                .Select(p => new ProductionInfoViewModel
                {
                    Title = p.Title,
                    Description = p.Description,
                    StatusType = p.StatusType.ToString(),
                    AbstractStatus = ResolveAbstractionStatus(statusMap, p.StatusType)
                })
                .ToArray()
        };
        
        return viewModel;
    }
    
    private string ResolveAbstractionStatus(IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> statusMap, ProductionStatusType statusType)
    {
        string? key = statusMap
            .FirstOrDefault(kvp => kvp.Value.Contains(statusType))
            .Key;
        
        return string.IsNullOrWhiteSpace(key) ? UnknownStatus : key;
    }
}
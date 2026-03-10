namespace FilmProductionManagementSystem.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Wrap.ViewModels;
using Wrap.ViewModels.General;
using Wrap.Services.Core.Interface;

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
        DashboardViewModel general = await homeService.GetGeneralInformation();
        return View(general);
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
}
namespace FilmProductionManagementSystem.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Wrap.ViewModels;
using Wrap.ViewModels.General;

using Wrap.Services.Core.Interface;

[Authorize]
public class HomeController(IHomeService homeService) : Controller
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
        GeneralPageViewModel general = await homeService.GetGeneralInformation();
        return View(general);
    }

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpGet]
    [AllowAnonymous]
    [Route("Error/{statusCode:int}")]
    public IActionResult StatusCodeError(int statusCode)
    {
        return statusCode switch
        {
            404 => View("NotFound"),
            400 => View("BadRequest"),
            _ => View("Error")
        };
    }
}
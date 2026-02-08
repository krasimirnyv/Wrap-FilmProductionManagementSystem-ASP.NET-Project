namespace FilmProductionManagementSystem.Web.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Wrap.ViewModels;

public class HomeController : Controller
{
    //ILogger<HomeController> logger
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult GeneralPage()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpGet]
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
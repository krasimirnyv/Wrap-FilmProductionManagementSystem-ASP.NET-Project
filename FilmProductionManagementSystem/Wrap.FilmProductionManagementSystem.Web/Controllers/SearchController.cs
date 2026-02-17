using Microsoft.AspNetCore.Mvc;

namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
public class SearchController : Controller
{
    public IActionResult Index()
    {
        throw new NotImplementedException();
    }

    public IActionResult FindFilmmakers()
    {
        throw new NotImplementedException();
    }

    public IActionResult FindActors()
    {
        throw new NotImplementedException();
    }
}
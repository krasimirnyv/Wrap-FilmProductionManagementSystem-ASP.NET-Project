using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmProductionManagementSystem.Web.Controllers;

[Authorize]
public class AssetsController : Controller
{
    public IActionResult Index()
    {
        throw new NotImplementedException();
    }
}
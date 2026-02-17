using Microsoft.AspNetCore.Mvc;

namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
public class SceneController : Controller
{
    public IActionResult Details()
    {
        throw new NotImplementedException();
    }
}
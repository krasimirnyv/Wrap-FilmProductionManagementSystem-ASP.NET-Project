using Microsoft.AspNetCore.Mvc;

namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
public class ScheduleController : Controller
{
    public IActionResult Index()
    {
        throw new NotImplementedException();
    }

    public IActionResult UpcomingShoots()
    {
        throw new NotImplementedException();
    }
}
namespace FilmProductionManagementSystem.Web.Controllers;

using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class BaseController : Controller
{
    protected string? GetUserId()
        => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    
    protected bool IsAuthenticated()
        => User.Identity?.IsAuthenticated ?? false;
}
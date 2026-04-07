namespace FilmProductionManagementSystem.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Wrap.Web.ViewModels;
using Wrap.Web.ViewModels.General;
using Wrap.Web.ViewModels.FindPeople;
using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.General;
using Wrap.Services.Models.FindPeople;
using Wrap.GCommon.Enums;
using Wrap.GCommon.UI;

using static Wrap.GCommon.ApplicationConstants;
using static Wrap.GCommon.OutputMessages.Home;
using static Wrap.GCommon.OutputMessages.Profile;

public class HomeController(IHomeService homeService, 
                            IFindPeopleService findPeopleService, 
                            ILogger<HomeController> logger) : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index() => View();
    
    [HttpGet]
    [Route("/Home/Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        string? userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            DashboardDataDto dto = await homeService.GetDashboardDataAsync(userId);

            IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> statusMap = 
                ProductionStatusAbstractionCatalog.GetStatusTypeByAbstraction();
        
            DashboardViewModel viewModel = MapToDashboardViewModelFromDto(dto, statusMap);
        
            return View(viewModel);
        }
        catch (ArgumentNullException ane)
        {
            logger.LogError(UsernameIsNullOrEmptyMessage + ane.Message);
            TempData[ErrorTempDateKey] = UsernameIsNullOrEmptyMessage + ane.Message;
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> FindFilmmakers([FromQuery] FindFilmmakersViewModel viewModel)
    {
        Guid? productionGuidId = Guid.TryParse(viewModel.ProductionId, out Guid guidId) ? guidId : null;
        if (productionGuidId.HasValue)
        {
            string? userId = GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
                viewModel.CanManageProduction = await findPeopleService.CanManageProductionAsync(guidId, userId);
        }
        
        CrewRoleType? roleType = null;
        if (viewModel.RoleType.HasValue && viewModel.RoleType.Value > 0)
        {
            int raw = viewModel.RoleType.Value;
            if (Enum.IsDefined(typeof(CrewRoleType), raw))
                roleType = (CrewRoleType)raw;
        }
        
        FindFilmmakersDto dto = await findPeopleService.GetFilmmakersAsync
        (
            pageNumber: viewModel.PageNumber,
            search: viewModel.Search,
            roleType: roleType,
            productionId: productionGuidId
        );

        viewModel.TotalCount = dto.TotalCount;
        viewModel.TotalPages = (int)Math.Ceiling(dto.TotalCount / (double)DefaultPeoplePerPage);

        if (viewModel.PageNumber > viewModel.TotalPages && viewModel.TotalPages != 0)
            viewModel.PageNumber = viewModel.TotalPages;
        else if (viewModel.PageNumber < 1)
            viewModel.PageNumber = 1;
        
        MapDataToFindFilmmakersViewModel(viewModel, dto);
        return View(viewModel);
    }
    
    [HttpGet]
    public async Task<IActionResult> FindActors([FromQuery]  FindActorsViewModel viewModel)
    {
        Guid? productionId = Guid.TryParse(viewModel.ProductionId, out Guid guidId) ? guidId : null;
        if (productionId.HasValue)
        {
            string? userId = GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
                viewModel.CanManageProduction = await findPeopleService.CanManageProductionAsync(guidId, userId);
        }
        
        FindActorsDto dto = await findPeopleService.GetActorsAsync
        (
            pageNumber: viewModel.PageNumber,
            search: viewModel.Search,
            age: viewModel.Age,
            gender: viewModel.Gender,
            productionId: productionId
        );
        
        viewModel.TotalCount = dto.TotalCount;
        viewModel.TotalPages = (int)Math.Ceiling(dto.TotalCount / (double)DefaultPeoplePerPage);
        
        if (viewModel.PageNumber > viewModel.TotalPages && viewModel.TotalPages != 0)
            viewModel.PageNumber = viewModel.TotalPages;
        else if (viewModel.PageNumber < 1)
            viewModel.PageNumber = 1;
        
        MapDataToFindActorsViewModel(viewModel, dto);
        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Home/Error")]
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
                .ToArray(),
            IsUserCrew = dto.IsUserCrew,
            HasOwnProductions = dto.HasOwnProductions
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
    
    private static void MapDataToFindFilmmakersViewModel(FindFilmmakersViewModel viewModel, FindFilmmakersDto dto)
    {
        viewModel.FilmmakerList = dto
            .FilmmakerListDtos
            .Select(f => new FilmmakerListViewModel
            {
                CrewId = f.CrewId.ToString(),
                ProfileImagePath = f.ProfileImagePath,
                FullName = f.FullName,
                Nickname = f.Nickname,
                TopRole = f.Role,
                IsAlreadyInProduction = f.IsAlreadyInProduction
            })
            .ToArray();
    }
    
    private static void MapDataToFindActorsViewModel(FindActorsViewModel viewModel, FindActorsDto dto)
    {
        viewModel.ActorList = dto
            .ActorListDtos
            .Select(a => new ActorListViewModel
            {
                CastId = a.CastId.ToString(),
                ProfileImagePath = a.ProfileImagePath,
                FullName = a.FullName,
                Nickname = a.Nickname,
                Age = a.Age.ToString(),
                Gender = a.Gender,
                IsAlreadyInProduction = a.IsAlreadyInProduction
            })
            .ToArray();
    }
}
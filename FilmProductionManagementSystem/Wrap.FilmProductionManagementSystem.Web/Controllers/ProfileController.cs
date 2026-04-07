namespace FilmProductionManagementSystem.Web.Controllers;

using System.Text;

using Microsoft.AspNetCore.Mvc;

using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.Profile;
using Wrap.Web.ViewModels.Profile;
using Wrap.Web.ViewModels.Profile.NestedViewModels;

using static Wrap.GCommon.OutputMessages;
using static Wrap.GCommon.OutputMessages.Profile;
using static Wrap.GCommon.ApplicationConstants;

public class ProfileController(IProfileService profileService, 
                               ICrewProfileService crewProfileService, 
                               ICastProfileService castProfileService,
                               ILogger<ProfileController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            ProfileRoleDto roleDto = await profileService.GetRoleInfoAsync(username);
            if (roleDto.IsCrew)
                return RedirectToAction(nameof(FilmmakerProfile), new { username });

            if (roleDto.IsCast)
                return RedirectToAction(nameof(ActorProfile), new { username });

            logger.LogWarning(string.Format(UserNotFoundMessage, username));
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorFindingUserMessage, e.Message));
            return View(nameof(BadRequest), UserNotIdentifiedMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> FilmmakerProfile(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            CrewProfileDto dto = await crewProfileService.GetCrewProfileDataAsync(username);
            CrewProfileViewModel viewModel = MapToCrewProfileViewModelFromDto(dto);

            return View(viewModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(DataExceptionMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingProfileMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ActorProfile(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            CastProfileDto dto = await castProfileService.GetCastProfileDataAsync(username);
            CastProfileViewModel viewModel = MapToCastProfileViewModelFromDto(dto);
            
            return View(viewModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CastNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(DataExceptionMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingProfileMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            ProfileRoleDto roleDto = await profileService.GetRoleInfoAsync(username);
            if (roleDto.IsCrew)
                return RedirectToAction(nameof(EditFilmmaker), new { username });

            if (roleDto.IsCast)
                return RedirectToAction(nameof(EditActor), new { username });
            
            logger.LogWarning(string.Format(UserNotFoundMessage, username));
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorFindingUserMessage, e.Message));
            return View(nameof(BadRequest), UserNotIdentifiedMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditFilmmaker(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(EditFilmmaker)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditCrewProfileDto dto = await crewProfileService.GetEditCrewProfileAsync(username);
            EditCrewProfileInputModel inputModel = MapToEditCrewProfileInputModelFromDto(dto);
            
            return View("EditCrewProfile", inputModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingEditorMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingEditorMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditFilmmaker(EditCrewProfileInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View("EditCrewProfile", inputModel);

        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(EditFilmmaker)} {HttpPostAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditCrewProfileDto dto = MapToEditCrewProfileDtoFromInputModel(inputModel);
            
            await crewProfileService.UpdateCrewProfileAsync(username, dto);

            TempData[SuccessTempDataKey] = UpdateProfileSuccessMessage;
            return RedirectToAction(nameof(FilmmakerProfile));
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (NotSupportedException nse)
        {
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + nse.Message);
            ModelState.AddModelError(string.Empty, string.Format(ErrorUpdatingProfile, nse.Message));
            return View("EditCrewProfile", inputModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorUpdatingProfile, username) + e.Message);
            TempData[ErrorTempDateKey] = string.Format(ErrorUpdatingProfile, e.Message);
            return View("EditCrewProfile", inputModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditActor(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(EditActor)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditCastProfileDto dto = await castProfileService.GetEditCastProfileAsync(username);
            EditCastProfileInputModel inputModel = MapToEditCastProfileInputModelFromDto(dto);
            
            return View("EditCastProfile", inputModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CastNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingEditorMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingEditorMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditActor(EditCastProfileInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View("EditCastProfile", inputModel);

        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(EditActor)} {HttpPostAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditCastProfileDto dto = MapToEditCastProfileDtoFromInputModel(inputModel);
                
            await castProfileService.UpdateCastProfileAsync(username, dto);

            TempData[SuccessTempDataKey] = UpdateProfileSuccessMessage;
            return RedirectToAction(nameof(ActorProfile));
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CastNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (NotSupportedException nse)
        {
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + nse.Message);
            ModelState.AddModelError(string.Empty, string.Format(ErrorUpdatingProfile, nse.Message));
            return View("EditCastProfile", inputModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorUpdatingProfile, username) + e.Message);
            TempData[ErrorTempDateKey] = string.Format(ErrorUpdatingProfile, e.Message);
            return View("EditCastProfile", inputModel);
        }
    }
    

    [HttpGet]
    public async Task<IActionResult> EditSkills(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(EditSkills)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditSkillsDto dto = await crewProfileService.GetEditSkillsAsync(username);
            EditSkillsInputModel inputModel = MapEditSkillsInputModelFromDto(dto);
            
            return View(inputModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingSkillsMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingSkillsMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditSkills(EditSkillsInputModel model)
    {
        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(EditSkills)} {HttpPostAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            if (!ModelState.IsValid)
            {
                EditSkillsDto dtoCurrentSkills = await crewProfileService.GetEditSkillsAsync(username);
                EditSkillsInputModel inputModel = MapEditSkillsInputModelFromDto(dtoCurrentSkills);
            
                return View(inputModel);
            }
            
            UpdateSkillsDto dto = MapToUpdateSkillsDtoFromEditSkillsInputModel(model);
            
            await crewProfileService.UpdateSkillsAsync(username, dto);

            TempData[SuccessTempDataKey] = UpdateSkillsSuccessMessage;
            return RedirectToAction(nameof(FilmmakerProfile));
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorUpdatingProfile, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorUpdatingProfile, e.Message));
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteProfileOverview()
    {
        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            ProfileRoleDto roleDto = await profileService.GetRoleInfoAsync(username);
            if (roleDto.IsCrew)
                return RedirectToAction(nameof(DeleteFilmmakerOverview), new { username });

            if (roleDto.IsCast)
                return RedirectToAction(nameof(DeleteActorOverview), new { username });

            logger.LogWarning(string.Format(UserNotFoundMessage, username));
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorFindingUserMessage, e.Message));
            return View(nameof(BadRequest), UserNotIdentifiedMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteFilmmakerOverview(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(DeleteFilmmakerOverview)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            DeleteProfileDto dto = await crewProfileService.GetDeleteCrewProfileAsync(username);
            DeleteProfileInputModel inputModel = MapToDeleteProfileInputModelFromDto(dto);
            
            return View("DeleteCrewProfile", inputModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingDeleteMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingDeleteMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFilmmaker(DeleteProfileInputModel inputModel)
    {
        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(DeleteFilmmaker)} {HttpPostAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        if (!inputModel.IsConfirmed)
        {
            DeleteProfileInputModel filled = await FillDeleteFilmmakerModelAsync(username, inputModel);
            TempData[WarningTempDataKey] = CheckBoxForDeleteErrorMessage;
            return View("DeleteCrewProfile", filled);
        }
        
        try
        {
            DeleteProfileDto dto = MapToDeleteProfileDtoFromInputModel(inputModel);
            
            bool isSucceeded = await crewProfileService.DeleteCrewProfileAsync(username, dto);
            if (!isSucceeded)
            {
                ModelState.AddModelError(nameof(DeleteProfileInputModel.Password), FailedPassword);
                DeleteProfileInputModel filled = await FillDeleteFilmmakerModelAsync(username, inputModel);

                return View("DeleteCrewProfile", filled);
            }
            
            TempData[SuccessTempDataKey] = DeleteProfileSuccessMessage;
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorDeletingProfile, username) + e.Message);
            TempData[ErrorTempDateKey] = string.Format(ErrorDeletingProfile, e.Message);
            
            DeleteProfileInputModel filled = await FillDeleteFilmmakerModelAsync(username, inputModel);
            return View("DeleteCrewProfile", filled);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteActorOverview(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(DeleteActorOverview)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            DeleteProfileDto dto = await castProfileService.GetDeleteCastProfileAsync(username);
            DeleteProfileInputModel inputModel = MapToDeleteProfileInputModelFromDto(dto);
            
            return View("DeleteCastProfile", inputModel);
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CastNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingDeleteMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingDeleteMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteActor(DeleteProfileInputModel inputModel)
    {
        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(DeleteActor)} {HttpPostAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        if (!inputModel.IsConfirmed)
        {
            DeleteProfileInputModel filled = await FillDeleteActorModelAsync(username, inputModel);
            TempData[WarningTempDataKey] = CheckBoxForDeleteErrorMessage;
            return View("DeleteCastProfile", filled);
        }
        
        try
        {
            DeleteProfileDto dto = MapToDeleteProfileDtoFromInputModel(inputModel);
            
            bool isSucceeded = await castProfileService.DeleteCastProfileAsync(username, dto);
            if (!isSucceeded)
            {
                ModelState.AddModelError(nameof(DeleteProfileInputModel.Password), FailedPassword);
                DeleteProfileInputModel filled = await FillDeleteActorModelAsync(username, inputModel);
                
                return View("DeleteCastProfile", filled);
            }
            
            TempData[SuccessTempDataKey] = DeleteProfileSuccessMessage;
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CastNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorDeletingProfile, username) + e.Message);
            TempData[ErrorTempDateKey] = string.Format(ErrorDeletingProfile, e.Message);
            
            DeleteProfileInputModel filled = await FillDeleteActorModelAsync(username, inputModel);
            return View("DeleteCastProfile", filled);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadPersonalData()
    {
        string? username = GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            ProfileRoleDto roleDto = await profileService.GetRoleInfoAsync(username);
            if (roleDto.IsCrew)
                return RedirectToAction(nameof(DownloadFilmmakerPersonalData), new { username });

            if (roleDto.IsCast)
                return RedirectToAction(nameof(DownloadActorPersonalData), new { username });

            logger.LogWarning(string.Format(UserNotFoundMessage, username));
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorFindingUserMessage, e.Message));
            return View(nameof(BadRequest), UserNotIdentifiedMessage);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadFilmmakerPersonalData(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(DownloadFilmmakerPersonalData)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            string personalData = await crewProfileService.DownloadCrewProfileDataAsync(username);
            byte[] bytes = Encoding.UTF8.GetBytes(personalData);
            
            string fileName = string.Format(ExportJsonFileName, username);
            FileContentResult contentResult = File(bytes, ContentType, fileName);
            
            return contentResult;
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CrewNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorDownloadingProfileData, username) + e.Message);
            TempData[ErrorTempDateKey] = string.Format(ErrorDownloadingProfileData, e.Message);
            return RedirectToAction("FilmmakerProfile");
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadActorPersonalData(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning($"{nameof(DownloadActorPersonalData)} {HttpGetAction} {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        try
        {
            string personalData = await castProfileService.DownloadCastProfileDataAsync(username);
            byte[] bytes = Encoding.UTF8.GetBytes(personalData);
            
            string fileName = string.Format(ExportJsonFileName, username);
            FileContentResult contentResult = File(bytes, ContentType, fileName);
            
            return contentResult;
        }
        catch (ArgumentNullException nullEx)
        {
            logger.LogError(nullEx.Message);
            TempData[ErrorTempDateKey] = string.Format(CastNotFoundMessage, username);
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorDownloadingProfileData, username) + e.Message);
            TempData[ErrorTempDateKey] = string.Format(ErrorDownloadingProfileData, e.Message);
            return RedirectToAction("ActorProfile");
        }
    }
    
    private static CrewProfileViewModel MapToCrewProfileViewModelFromDto(CrewProfileDto dto)
    {
        CrewProfileViewModel viewModel = new CrewProfileViewModel
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ProfileImagePath = dto.ProfileImagePath,
            Nickname = dto.Nickname,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = dto.IsActive,
            Biography = dto.Biography,
            DepartmentSkills = dto.DepartmentSkills,
            CrewMemberProductions = dto.Productions
                .Select(p => new CrewMemberProduction
                {
                    ProductionId = p.ProductionId,
                    ProductionTitle = p.ProductionTitle,
                    RoleType = p.RoleType.ToString(),
                    ProjectStatus = p.ProjectStatus
                })
                .ToList(),
            CrewMemberScenes = dto.Scenes.Select(s => new CrewMemberScene
                {
                    SceneId = s.SceneId,
                    SceneName = s.SceneName,
                    ProductionTitle = s.ProductionTitle,
                    RoleType = s.RoleType.ToString()
                })
                .ToList()
        };
        
        return viewModel;
    }
    
    private static CastProfileViewModel MapToCastProfileViewModelFromDto(CastProfileDto dto)
    {
        CastProfileViewModel viewModel = new CastProfileViewModel
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ProfileImagePath = dto.ProfileImagePath,
            Nickname = dto.Nickname,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Age = dto.Age,
            Gender = dto.Gender,
            IsActive = dto.IsActive,
            Biography = dto.Biography,
            CastMemberProductions = dto.Productions
                .Select(p => new CastMemberProduction
                {
                    ProductionId = p.ProductionId,
                    ProductionTitle = p.ProductionTitle,
                    CharacterName = p.CharacterName,
                    ProjectStatus = p.ProjectStatus
                })
                .ToList(),
            CastMemberScenes = dto.Scenes.Select(s => new CastMemberScene
                {
                    SceneId = s.SceneId,
                    SceneName = s.SceneName,
                    ProductionTitle = s.ProductionTitle,
                    CharacterName = s.CharacterName
                })
                .ToList()
        };
        
        return viewModel;
    }
    
    private static EditCrewProfileInputModel MapToEditCrewProfileInputModelFromDto(EditCrewProfileDto dto)
    {
        EditCrewProfileInputModel inputModel = new EditCrewProfileInputModel
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Nickname = dto.Nickname,
            PhoneNumber = dto.PhoneNumber,
            Biography = dto.Biography,
            ProfileImage = dto.ProfileImage,
            Email = dto.Email,
            CurrentProfileImagePath = dto.CurrentProfileImagePath
        };
        
        return inputModel;
    }
    
    private static EditCrewProfileDto MapToEditCrewProfileDtoFromInputModel(EditCrewProfileInputModel inputModel)
    {
        EditCrewProfileDto dto = new EditCrewProfileDto
        {
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
            Nickname = inputModel.Nickname,
            PhoneNumber = inputModel.PhoneNumber,
            Biography = inputModel.Biography,
            ProfileImage = inputModel.ProfileImage,
            Email = inputModel.Email,
            CurrentProfileImagePath = inputModel.CurrentProfileImagePath
        };
        
        return dto;
    }
    
    private static EditCastProfileInputModel MapToEditCastProfileInputModelFromDto(EditCastProfileDto dto)
    {
        EditCastProfileInputModel viewModel = new EditCastProfileInputModel
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Nickname = dto.Nickname,
            PhoneNumber = dto.PhoneNumber,
            Biography = dto.Biography,
            ProfileImage = dto.ProfileImage,
            Email = dto.Email,
            CurrentProfileImagePath = dto.CurrentProfileImagePath,
            Age = dto.Age,
            Gender = dto.Gender
        };
        
        return viewModel;
    }
    
    private static EditCastProfileDto MapToEditCastProfileDtoFromInputModel(EditCastProfileInputModel inputModel)
    {
        EditCastProfileDto dto = new EditCastProfileDto
        {
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
            Nickname = inputModel.Nickname,
            PhoneNumber = inputModel.PhoneNumber,
            Biography = inputModel.Biography,
            ProfileImage = inputModel.ProfileImage,
            Email = inputModel.Email,
            CurrentProfileImagePath = inputModel.CurrentProfileImagePath,
            Age = inputModel.Age,
            Gender = inputModel.Gender
        };
        
        return dto;
    }
    
    private static EditSkillsInputModel MapEditSkillsInputModelFromDto(EditSkillsDto dto)
    {
        EditSkillsInputModel inputModel = new EditSkillsInputModel
        {
            CurrentSkills = dto.CurrentSkills,
            AllDepartments = dto.AllDepartments
        };
        
        return inputModel;
    }
    
    private static UpdateSkillsDto MapToUpdateSkillsDtoFromEditSkillsInputModel(EditSkillsInputModel model)
    {
        UpdateSkillsDto dto = new UpdateSkillsDto
        {
            SelectedSkills = model.SelectedSkills
        };
        
        return dto;
    }
    
    private DeleteProfileInputModel MapToDeleteProfileInputModelFromDto(DeleteProfileDto dto)
    {
        DeleteProfileInputModel viewModel = new DeleteProfileInputModel
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ProfileImagePath = dto.ProfileImagePath,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            ProductionsCount = dto.ProductionsCount,
            ScenesCount = dto.ScenesCount,
            SkillsCount = dto.SkillsCount
        };
        
        return viewModel;
    }
    
    private DeleteProfileDto MapToDeleteProfileDtoFromInputModel(DeleteProfileInputModel inputModel)
    {
        DeleteProfileDto dto = new DeleteProfileDto
        {
           Password = inputModel.Password,
           IsConfirmed = inputModel.IsConfirmed,
        };
        
        return dto;
    }
    
    private async Task<DeleteProfileInputModel> FillDeleteFilmmakerModelAsync(string username, DeleteProfileInputModel posted)
    {
        DeleteProfileDto dto = await crewProfileService.GetDeleteCrewProfileAsync(username);
        DeleteProfileInputModel filled = MapToDeleteProfileInputModelFromDto(dto);

        filled.Password = posted.Password;
        filled.IsConfirmed = posted.IsConfirmed;

        return filled;
    }
    
    private async Task<DeleteProfileInputModel> FillDeleteActorModelAsync(string username, DeleteProfileInputModel posted)
    {
        DeleteProfileDto dto = await castProfileService.GetDeleteCastProfileAsync(username);
        DeleteProfileInputModel filled = MapToDeleteProfileInputModelFromDto(dto);

        filled.Password = posted.Password;
        filled.IsConfirmed = posted.IsConfirmed;

        return filled;
    }
}
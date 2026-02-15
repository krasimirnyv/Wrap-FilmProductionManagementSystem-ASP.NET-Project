namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

using Data;
using Data.Models;
using Data.Models.Infrastructure;

using ViewModels.LoginAndRegistration;
using ViewModels.LoginAndRegistration.Helpers;

using Interface;

using GCommon.Enums;

public class WrapService(UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager,
                        FilmProductionDbContext context, 
                        IWebHostEnvironment environment) : IWrapService
{
    public async Task<string> GetProfileImagePathAsync(Guid userId, string userType)
    {
        string? imagePath = null;

        if (userType == "Crew")
        {
            Crew? crew = await context.CrewMembers
                .SingleOrDefaultAsync(c => Equals(c.UserId, userId) && !c.IsDeleted);

            imagePath = crew?.ProfileImagePath;
        }
        else if (userType == "Cast")
        {
            Cast? cast = await context.CastMembers
                .SingleOrDefaultAsync(c => Equals(c.UserId, userId) && !c.IsDeleted);
            
            imagePath = cast?.ProfileImagePath;
        }
        
        string defaultPath = Path.Combine("img", "profile", "default-profile.png");
        return imagePath ?? defaultPath;
    }
}
namespace Wrap.Services.Core;

using Interfaces;
using Utilities.Providers.Interfaces;
using Models.General;
using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;

public class HomeService(IHomeRepository homeRepository, IDateTimeProvider dateTimeProvider) : IHomeService
{
    public async Task<DashboardDataDto> GetDashboardDataAsync(string userId)
    {
        DateTime now = dateTimeProvider.Now;
        
        int crewCount = await homeRepository.GetCrewCountAsync();
        int castCount = await homeRepository.GetCastCountAsync();

        IReadOnlyCollection<Production> productions = await homeRepository.GetProductionsAsync();

        IReadOnlyCollection<ProductionDashboardDto> productionsDto = productions
            .Select(p => new ProductionDashboardDto
            {
                Title = p.Title,
                Description = p.Description,
                StatusType = p.StatusType,
                UpcomingScenesCount = p.Scenes
                    .SelectMany(s => s.ShootingDayScenes)
                    .Count(sds => sds.ShootingDay.Date > now)
            })
            .ToArray();

        int upcomingScenesTotal = productionsDto.Sum(p => p.UpcomingScenesCount);

        Guid? applicationUserId = ValidateGuid(userId);
        if (applicationUserId is null)
            throw new ArgumentNullException(nameof(applicationUserId));
        
        bool isUserCrew = await IfUserIsCrewGetTrueAsync(applicationUserId.Value);
        bool hasOwnProductions = await homeRepository.IsUserOwnsProductionsAsync(applicationUserId.Value);
        
        DashboardDataDto dashboardDataDto = new DashboardDataDto
        {
            CrewMembersCount = crewCount,
            CastMembersCount = castCount,
            UpcomingScenesTotal = upcomingScenesTotal,
            Productions = productionsDto,
            IsUserCrew = isUserCrew,
            HasOwnProductions = hasOwnProductions
        };
        
        return dashboardDataDto;
    }

    private async Task<bool> IfUserIsCrewGetTrueAsync(Guid userId)
    {
        ApplicationUser? user = await homeRepository.GetApplicationUserDataAsync(userId);
        if (user is null)
            return false;
        
        Crew? crew = await homeRepository.GetCrewByUserIdAsync(user.Id);
        if (crew is null)
            return false;
        
        return true;
    }
    
    private static Guid? ValidateGuid(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        bool isValidId = Guid.TryParse(id, out Guid productionId);
        if (!isValidId)
            return null;
        
        return productionId;
    }
}
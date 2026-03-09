namespace Wrap.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Models;
using Models.MappingEntities;
using Models.Infrastructure;

public class FilmProductionDbContext : IdentityDbContext<ApplicationUser>
{
    public FilmProductionDbContext(DbContextOptions<FilmProductionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Crew> CrewMembers { get; set; } = null!;
    public virtual DbSet<CrewSkill> CrewSkills { get; set; } = null!;
    public virtual DbSet<Cast> CastMembers { get; set; } = null!;
    
    public virtual DbSet<Production> Productions { get; set; } = null!;
    public virtual DbSet<Scene> Scenes { get; set; } = null!;
    public virtual DbSet<ProductionAsset> ProductionsAssets { get; set; } = null!;

    public virtual DbSet<Script> Scripts { get; set; } = null!;
    public virtual DbSet<ScriptBlock> ScriptBlocks { get; set; } = null!;

    public virtual DbSet<ShootingDay> ShootingDays { get; set; } = null!;
    public virtual DbSet<ShootingDayScene> ShootingDaysScenes { get; set; } = null!;

    public virtual DbSet<ProductionCrew> ProductionsCrewMembers { get; set; } = null!;
    public virtual DbSet<ProductionCast> ProductionsCastMembers { get; set; } = null!;

    public virtual DbSet<SceneCrew> ScenesCrewMembers { get; set; } = null!;
    public virtual DbSet<SceneCast> ScenesCastMembers { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(FilmProductionDbContext).Assembly);
    }
}
namespace FilmProductionManagementSystem.Web.Data;

using Models;
using Models.Enums;
using Models.MappingEntities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FilmProductionDbContext : IdentityDbContext
{
    public FilmProductionDbContext(DbContextOptions<FilmProductionDbContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Production> Productions { get; set; } = null!;
    
    public virtual DbSet<Scene> Scenes { get; set; } = null!;
    
    public virtual DbSet<ProductionAsset> ProductionsAssets { get; set; } = null!;
    
    public virtual DbSet<Crew> CrewMembers { get; set; } = null!;
    
    public virtual DbSet<Cast> CastMembers { get; set; } = null!;
    
    public virtual DbSet<Script> Scripts { get; set; } = null!;
    
    public virtual DbSet<ShootingDay> ShootingDays { get; set; } = null!;
    
    public virtual DbSet<ShootingDayScene> ShootingDaysScenes { get; set; } = null!;
    
    public virtual DbSet<ProductionCrew> ProductionsCrewMembers { get; set; } = null!;
    
    public virtual DbSet<ProductionCast> ProductionsCastMembers { get; set; } = null!;
    
    public virtual DbSet<SceneCrew> ScenesCrewMembers { get; set; } = null!;

    public virtual DbSet<SceneCast> ScenesCastMembers { get; set; } = null!;
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductionCrew>(entity =>
        {
            entity
                .HasKey(pCrew => new { pCrew.ProductionId, pCrew.CrewMemberId });
        });

        builder.Entity<ProductionCast>(entity =>
        {
            entity
                .HasKey(pCast => new { pCast.ProductionId, pCast.CastMemberId });
        });

        builder.Entity<SceneCrew>(entity =>
        {
            entity
                .HasKey(sCrew => new { sCrew.SceneId, sCrew.CrewMemberId });
        });

        builder.Entity<SceneCast>(entity =>
        {
            entity
                .HasKey(sCast => new { sCast.SceneId, sCast.CastMemberId });
        });
        
        builder
            .Entity<Production>()
            .HasOne(p => p.Script)
            .WithOne(s => s.Production)
            .HasForeignKey<Script>(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<Scene>()
            .HasOne(s => s.ShootingDayScene)
            .WithOne(sds => sds.Scene)
            .HasForeignKey<ShootingDayScene>(sds => sds.SceneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
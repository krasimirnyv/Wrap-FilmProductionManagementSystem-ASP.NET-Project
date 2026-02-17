namespace Wrap.Data;

using Models;
using Models.MappingEntities;
using Models.Infrastructure;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class FilmProductionDbContext : IdentityDbContext<ApplicationUser>
{
    public FilmProductionDbContext(DbContextOptions<FilmProductionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Production> Productions { get; set; } = null!;
    
    public virtual DbSet<Scene> Scenes { get; set; } = null!;
    
    public virtual DbSet<ProductionAsset> ProductionsAssets { get; set; } = null!;
    
    public virtual DbSet<Crew> CrewMembers { get; set; } = null!;

    public virtual DbSet<CrewSkill> CrewSkills { get; set; } = null!;
    
    public virtual DbSet<Cast> CastMembers { get; set; } = null!;
    
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

        builder
            .Entity<Crew>()
            .HasOne(c => c.User)
            .WithMany(au => au.CrewMembers)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<CrewSkill>()
            .HasOne(cs => cs.CrewMember)
            .WithMany(c => c.Skills)
            .HasForeignKey(cs => cs.CrewMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Entity<Cast>()
            .HasOne(c => c.User)
            .WithMany(au => au.CastMembers)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Entity<Production>()
            .HasOne(p => p.Script)
            .WithOne(s => s.Production)
            .HasForeignKey<Script>(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<Scene>()
            .HasOne(s => s.Production)
            .WithMany(p => p.Scenes)
            .HasForeignKey(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<ShootingDay>()
            .HasOne(sd => sd.Production)
            .WithMany(p => p.ShootingDays)
            .HasForeignKey(sd => sd.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Entity<Script>()
            .HasMany(s => s.ScriptBlocks)
            .WithOne(sb => sb.Script)
            .HasForeignKey(sb => sb.ScriptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<ScriptBlock>()
            .HasIndex(sb => new { sb.ScriptId, sb.OrderIndex });

        builder.Entity<ProductionCrew>(entity =>
        {
            entity
                .HasKey(pc => new { pc.ProductionId, pc.CrewMemberId });

            entity
                .HasOne(pc => pc.Production)
                .WithMany(p => p.ProductionCrewMembers)
                .HasForeignKey(pc => pc.ProductionId);
            
            entity
                .HasOne(pc => pc.CrewMember)
                .WithMany(c => c.CrewMemberProductions)
                .HasForeignKey(pc => pc.CrewMemberId);
        });
        
        builder.Entity<ProductionCast>(entity =>
        {
            entity
                .HasKey(pCast => new { pCast.ProductionId, pCast.CastMemberId });

            entity
                .HasOne(pc => pc.Production)
                .WithMany(p => p.ProductionCastMembers)
                .HasForeignKey(pc => pc.ProductionId);
            
            entity
                .HasOne(pc => pc.CastMember)
                .WithMany(c => c.CastMemberProductions)
                .HasForeignKey(pc => pc.CastMemberId);
        });
        
        builder.Entity<SceneCrew>(entity =>
        {
            entity
                .HasKey(sCrew => new { sCrew.SceneId, sCrew.CrewMemberId });

            entity
                .HasOne(sc => sc.Scene)
                .WithMany(s => s.SceneCrewMembers)
                .HasForeignKey(sc => sc.SceneId);
            
            entity
                .HasOne(sc => sc.CrewMember)
                .WithMany(c => c.CrewMemberScenes)
                .HasForeignKey(sc => sc.CrewMemberId);
        });
        
        builder.Entity<SceneCast>(entity =>
        {
            entity
                .HasKey(sCast => new { sCast.SceneId, sCast.CastMemberId });

            entity
                .HasOne(sc => sc.Scene)
                .WithMany(s => s.SceneCastMembers)
                .HasForeignKey(sc => sc.SceneId);
            
            entity
                .HasOne(sc => sc.CastMember)
                .WithMany(c => c.CastMemberScenes)
                .HasForeignKey(sc => sc.CastMemberId);
        });
        
        builder.Entity<ShootingDayScene>(entity =>
        {
            entity
                .HasIndex(sds => new { sds.ShootingDayId, sds.SceneId });

            entity
                .HasOne(sds => sds.Scene)
                .WithMany(s => s.ShootingDayScenes)
                .HasForeignKey(sds => sds.SceneId)
                .OnDelete(DeleteBehavior.NoAction);

            entity
                .HasOne(sds => sds.ShootingDay)
                .WithMany(sd => sd.ShootingDayScenes)
                .HasForeignKey(sds => sds.ShootingDayId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.ApplyConfigurationsFromAssembly(typeof(FilmProductionDbContext).Assembly);
    }
}
namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static Common.EntityIdentificationConstants;

public class SceneConfiguration : IEntityTypeConfiguration<Scene>
{
    public void Configure(EntityTypeBuilder<Scene> entity)
    {
        entity
            .HasOne(s => s.Production)
            .WithMany(p => p.Scenes)
            .HasForeignKey(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasIndex(s => new { s.ProductionId, s.SceneNumber });

        entity.HasData(ScenesSeed);
    }
    
    private static readonly Scene[] ScenesSeed =
    [
        // Production 01 - "Test Film"
        new()
        {
            Id = SceneId_TestFilm_01,
            ProductionId = ProductionIdTestFilm,
            SceneNumber = 1,
            SceneType = SceneType.Interior,
            SceneName = "Coffee Shop Conversation",
            Location = "Downtown Coffee Shop",
            Description = "A tense meeting sets the stakes for the story."
        },
        new()
        {
            Id = SceneId_TestFilm_02,
            ProductionId = ProductionIdTestFilm,
            SceneNumber = 2,
            SceneType = SceneType.Exterior,
            SceneName = "Street Chase",
            Location = "Main Boulevard",
            Description = "Fast-paced pursuit through the city streets."
        },
        new()
        {
            Id = SceneId_TestFilm_03,
            ProductionId = ProductionIdTestFilm,
            SceneNumber = 3,
            SceneType = SceneType.InteriorExterior,
            SceneName = "Apartment Reveal",
            Location = "Character A Apartment",
            Description = "Key evidence is discovered."
        },

        // Production 02 - "Midnight Dreams"
        new()
        {
            Id = SceneId_MidnightDreams_01,
            ProductionId = ProductionIdMidnightDreams,
            SceneNumber = 1,
            SceneType = SceneType.Interior,
            SceneName = "Interrogation Room",
            Location = "Police Station",
            Description = "A suspect breaks under pressure."
        },
        new()
        {
            Id = SceneId_MidnightDreams_02,
            ProductionId = ProductionIdMidnightDreams,
            SceneNumber = 2,
            SceneType = SceneType.Exterior,
            SceneName = "Rooftop Argument",
            Location = "Office Building Rooftop",
            Description = "Conflict escalates with a dramatic skyline backdrop."
        },

        // Production 03 - "Paper Planes"
        new()
        {
            Id = SceneId_PaperPlanes_01,
            ProductionId = ProductionIdPaperPlanes,
            SceneNumber = 1,
            SceneType = SceneType.Exterior,
            SceneName = "Night Alley Encounter",
            Location = "Old Town Alley",
            Description = "A mysterious figure appears in the shadows."
        },
        new()
        {
            Id = SceneId_PaperPlanes_02,
            ProductionId = ProductionIdPaperPlanes,
            SceneNumber = 2,
            SceneType = SceneType.Interior,
            SceneName = "Workshop Planning",
            Location = "Garage Workshop",
            Description = "The team plans the next move."
        },

        // Production 04 - "The Last Take"
        new()
        {
            Id = SceneId_TheLastTake_01,
            ProductionId = ProductionIdTheLastTake,
            SceneNumber = 1,
            SceneType = SceneType.Interior,
            SceneName = "Hospital Corridor",
            Location = "City Hospital",
            Description = "Emotional turning point for the lead character."
        },
        new()
        {
            Id = SceneId_TheLastTake_02,
            ProductionId = ProductionIdTheLastTake,
            SceneNumber = 2,
            SceneType = SceneType.Exterior,
            SceneName = "Park Reunion",
            Location = "Central Park",
            Description = "Two characters reconcile after a fallout."
        },
        new()
        {
            Id = SceneId_TheLastTake_03,
            ProductionId = ProductionIdTheLastTake,
            SceneNumber = 3,
            SceneType = SceneType.Exterior,
            SceneName = "Finale: Sunrise Cliff",
            Location = "Cliffside Overlook",
            Description = "Closing moment with a hopeful sunrise."
        }
    ];
}
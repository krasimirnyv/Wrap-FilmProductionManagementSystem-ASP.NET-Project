namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static Common.EntityIdentificationConstants;

public class CrewSkillConfiguration : IEntityTypeConfiguration<CrewSkill>
{
    public void Configure(EntityTypeBuilder<CrewSkill> entity)
    {
        entity
            .HasOne(cs => cs.CrewMember)
            .WithMany(c => c.Skills)
            .HasForeignKey(cs => cs.CrewMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(CrewSkillsSeed);
    }
    
    private static readonly CrewSkill[] CrewSkillsSeed =
    [
        new() { Id = CrewSkillId_001, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },
        new() { Id = CrewSkillId_002, CrewMemberId = CrewId1, RoleType = CrewRoleType.FirstAssistantDirector },
        new() { Id = CrewSkillId_003, CrewMemberId = CrewId1, RoleType = CrewRoleType.DirectorOfPhotography },
        new() { Id = CrewSkillId_004, CrewMemberId = CrewId1, RoleType = CrewRoleType.CameraOperator },

        new() { Id = CrewSkillId_005, CrewMemberId = CrewId2, RoleType = CrewRoleType.ProductionDesigner },
        new() { Id = CrewSkillId_006, CrewMemberId = CrewId2, RoleType = CrewRoleType.ArtDirector },
        new() { Id = CrewSkillId_007, CrewMemberId = CrewId2, RoleType = CrewRoleType.CostumeDesigner },
        new() { Id = CrewSkillId_008, CrewMemberId = CrewId2, RoleType = CrewRoleType.MakeupArtist },

        new() { Id = CrewSkillId_009, CrewMemberId = CrewId3, RoleType = CrewRoleType.ProductionSoundMixer },
        new() { Id = CrewSkillId_010, CrewMemberId = CrewId3, RoleType = CrewRoleType.BoomOperator },
        new() { Id = CrewSkillId_011, CrewMemberId = CrewId3, RoleType = CrewRoleType.Editor },
        new() { Id = CrewSkillId_012, CrewMemberId = CrewId3, RoleType = CrewRoleType.Colorist }
    ];
}
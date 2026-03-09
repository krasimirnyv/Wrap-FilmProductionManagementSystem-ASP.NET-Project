namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class CrewSkillConfiguration : IEntityTypeConfiguration<CrewSkill>
{
    public void Configure(EntityTypeBuilder<CrewSkill> entity)
    {
        entity
            .HasOne(cs => cs.CrewMember)
            .WithMany(c => c.Skills)
            .HasForeignKey(cs => cs.CrewMemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
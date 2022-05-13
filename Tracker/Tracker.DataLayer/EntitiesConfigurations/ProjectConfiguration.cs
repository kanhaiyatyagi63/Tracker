using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Core.Data;
using Tracker.Core.Models.Constants;
using Tracker.DataLayer.Entities;

namespace Tracker.DataLayer.EntitiesConfigurations
{
    public class ProjectConfiguration : BaseEntityTypeConfiguration<Project, int>
    {
        public override void Config(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(StringLengthConstants.Name);
            builder.Property(x => x.Description).HasMaxLength(StringLengthConstants.Description);
            builder.Property(x => x.TechnologyStack).HasMaxLength(StringLengthConstants.Default);
            builder.Property(x => x.Description).HasMaxLength(StringLengthConstants.Default);
        }
    }
}

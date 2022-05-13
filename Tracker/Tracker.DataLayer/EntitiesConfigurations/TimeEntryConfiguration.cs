using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Core.Data;
using Tracker.Core.Models.Constants;
using Tracker.DataLayer.Entities;

namespace Tracker.DataLayer.EntitiesConfigurations
{
    public class TimeEntryConfiguration : BaseEntityTypeConfiguration<TimeEntry, int>
    {
        public override void Config(EntityTypeBuilder<TimeEntry> builder)
        {
            builder.ToTable("TimeEntries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Hours)
                   .IsRequired()
                   .HasMaxLength(StringLengthConstants.ShortCode);
            builder.Property(x => x.Comments)
                   .IsRequired(true)
                   .HasMaxLength(StringLengthConstants.Description);

            builder.Property(x => x.LogTime).IsRequired();

            builder.HasOne(x => x.Project)
                   .WithMany(x => x.TimeEntries)
                   .HasForeignKey(x => x.ProjectId);
        }
    }
}

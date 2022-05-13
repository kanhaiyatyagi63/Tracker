using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Core.Models.Constants;

namespace Tracker.Core.Data
{
    public abstract class BaseEntityTypeConfiguration<T, TKey> : IEntityTypeConfiguration<T>
        where T : Entity<TKey>
    {
        public abstract void Config(EntityTypeBuilder<T> builder);

        public void Configure(EntityTypeBuilder<T> builder)
        {
            if (builder == null)
            {
                return;
            }

            builder.HasKey(x => x.Id);

            Config(builder);

            builder.Property(x => x.CreatedDate).IsRequired()
                                                .HasColumnType("DATETIME");
            builder.Property(x => x.CreatedBy).HasMaxLength(StringLengthConstants.ApplicationUserId);
            builder.Property(x => x.UpdatedDate).HasColumnType("DATETIME");
            builder.Property(x => x.UpdatedBy).HasMaxLength(StringLengthConstants.ApplicationUserId);
        }
    }
}

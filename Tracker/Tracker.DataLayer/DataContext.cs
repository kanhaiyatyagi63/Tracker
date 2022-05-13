using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tracker.Core.Models.Constants;
using Tracker.Core.Services.Abstractions;
using Tracker.DataLayer.Entities;
using Tracker.DataLayer.Enumerations;


namespace Tracker.DataLayer
{
    public class DataContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IUserContextService _userContextService;
        public DataContext(DbContextOptions options, IUserContextService userContextService) : base(options)
        {
            _userContextService = userContextService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Set Rules for ApplicationUser properties
            var userEntity = modelBuilder.Entity<ApplicationUser>();

            userEntity.Property(x => x.CreatedDate).HasColumnType("DATETIME");
            userEntity.Property(x => x.UpdatedDate).HasColumnType("DATETIME");
            userEntity.Property(x => x.ApplicationRoleType).IsRequired(true).HasDefaultValue(ApplicationRoleType.SuperAdmin);
            userEntity.Property(x => x.PhoneCode).IsRequired().HasMaxLength(StringLengthConstants.PhoneCode);

            //Set Rules for ApplicationRole properties
            var applicationRoleEntity = modelBuilder.Entity<ApplicationRole>();

            applicationRoleEntity.Property(x => x.CreatedDate).HasColumnType("DATETIME");
            applicationRoleEntity.Property(x => x.UpdatedDate).HasColumnType("DATETIME");
            applicationRoleEntity.Property(x => x.ApplicationRoleType).IsRequired(true).HasDefaultValue(ApplicationRoleType.SuperAdmin);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaveChanges()
        {
            base.ChangeTracker.DetectChanges();
            var now = DateTime.Now;
            string userId = _userContextService?.GetUserId() ?? null;

            foreach (EntityEntry entry in base.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                if (entry.State == EntityState.Added && entry.Metadata.FindProperty("CreatedDate") != null)
                {
                    entry.Property("CreatedDate").CurrentValue = now;
                    entry.Property("UpdatedDate").CurrentValue = now;
                    entry.Property("CreatedBy").CurrentValue = userId;
                    entry.Property("UpdatedBy").CurrentValue = userId;
                }

                if (entry.State == EntityState.Modified && entry.Metadata.FindProperty("UpdatedDate") != null)
                {
                    entry.Property("UpdatedDate").CurrentValue = now;
                    entry.Property("UpdatedBy").CurrentValue = userId;
                }
            }
        }
    }
}

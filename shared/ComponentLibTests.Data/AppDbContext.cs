using ComponentLibTests.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComponentLibTests.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<EntityInternRiskLevel> EntityInternRiskLevels => Set<EntityInternRiskLevel>();
    public DbSet<EntityMatchStatus> EntityMatchStatuses => Set<EntityMatchStatus>();
    public DbSet<ProfileRiskLevel> ProfileRiskLevels => Set<ProfileRiskLevel>();
    public DbSet<EntitySource> EntitySources => Set<EntitySource>();
    public DbSet<ApplicationUser> ApplicationUsers => Users;
    public DbSet<ProfileExtern> ProfilesExtern => Set<ProfileExtern>();
    public DbSet<EntityInternEntity> EntityInternEntities => Set<EntityInternEntity>();
    public DbSet<EntityMatchResult> EntityMatchResults => Set<EntityMatchResult>();
    public DbSet<MatchResultStateChange> MatchResultStateChanges => Set<MatchResultStateChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EntityInternRiskLevel>(e =>
        {
            e.Property(p => p.RiskLevel).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<EntityMatchStatus>(e =>
        {
            e.Property(p => p.Status).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<ProfileRiskLevel>(e =>
        {
            e.Property(p => p.RiskLevel).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<EntitySource>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ApplicationUser>(e =>
        {
            e.ToTable("ApplicationUsers");
            e.Property(p => p.DisplayName).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<ProfileExtern>(e =>
        {
            e.Property(p => p.Entity).HasMaxLength(200).IsRequired();
            e.Property(p => p.FirstName).HasMaxLength(100);
            e.Property(p => p.LastName).HasMaxLength(100);
            e.Property(p => p.Nationality).HasMaxLength(100);
            e.Property(p => p.Gender).HasMaxLength(20);
            e.Property(p => p.Remarks).HasMaxLength(2000);

            e.HasOne(p => p.ProfileRiskLevel).WithMany(p => p.ProfilesExtern)
                .HasForeignKey(p => p.ProfileRiskLevelId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.EntitySource).WithMany(p => p.ProfilesExtern)
                .HasForeignKey(p => p.EntitySourceId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.EntitySourceId);
            e.HasIndex(p => p.LastName);
        });

        modelBuilder.Entity<EntityInternEntity>(e =>
        {
            e.Property(p => p.Entity).HasMaxLength(200).IsRequired();
            e.Property(p => p.Label).HasMaxLength(200);

            e.HasOne(p => p.EntityInternRiskLevel).WithMany(p => p.EntityInternEntities)
                .HasForeignKey(p => p.EntityInternRiskLevelId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.EntityInternRiskLevelId);
            e.HasIndex(p => p.Active);
            e.HasIndex(p => p.Entity);
        });

        modelBuilder.Entity<EntityMatchResult>(e =>
        {
            e.Property(p => p.Comment).HasMaxLength(2000);

            e.HasOne(p => p.EntityInternEntity).WithMany(p => p.EntityMatchResults)
                .HasForeignKey(p => p.EntityInternEntitiesId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.ProfileExtern).WithMany(p => p.EntityMatchResults)
                .HasForeignKey(p => p.ProfilesExternId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.EntityMatchStatus).WithMany(p => p.EntityMatchResults)
                .HasForeignKey(p => p.EntityMatchStatusId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.EntityInternEntitiesId);
            e.HasIndex(p => p.ProfilesExternId);
            e.HasIndex(p => p.EntityMatchStatusId);
            e.HasIndex(p => p.Created);
            e.HasIndex(p => p.MatchingValue);
        });

        modelBuilder.Entity<MatchResultStateChange>(e =>
        {
            e.Property(p => p.Comment).HasMaxLength(2000);

            e.HasOne(p => p.EntityMatchResult).WithMany(p => p.MatchResultStateChanges)
                .HasForeignKey(p => p.MatchResultId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.UpdateUser).WithMany(p => p.MatchResultStateChanges)
                .HasForeignKey(p => p.UpdateUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.NewMatchStatus).WithMany(p => p.MatchResultStateChanges)
                .HasForeignKey(p => p.NewMatchStatusId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.MatchResultId);
            e.HasIndex(p => p.UpdateDate);
        });
    }
}

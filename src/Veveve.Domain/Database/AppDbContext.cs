using Veveve.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Domain.Database;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext()
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
    public DbSet<ClientEntity> Clients => Set<ClientEntity>();
    public DbSet<UserClaimEntity> UserClaims => Set<UserClaimEntity>();
    public DbSet<EmailLogEntity> EmailLogs => Set<EmailLogEntity>();
    public DbSet<JobQueueEntity> JobQueue => Set<JobQueueEntity>();
    public DbSet<JobErrorEntity> JobErrors => Set<JobErrorEntity>();

    public override int SaveChanges() => SaveChangesAsync().Result;
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
                ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.UtcNow;
            ((BaseEntity)entityEntry.Entity).LastModifiedDate = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>().HasIndex(b => b.Email).IsUnique();
        modelBuilder.Entity<AccountEntity>().HasIndex(b => b.GoogleAdsAccountId).IsUnique();
        modelBuilder.Entity<ClientEntity>().HasIndex(b => b.Name).IsUnique();
        modelBuilder.Entity<EmailLogEntity>().HasIndex(b => b.Reference).IsUnique();
        modelBuilder.Entity<JobQueueEntity>()
            .ToTable("JobQueue") // to avoid adding an 's' to the end of the table name
            .HasIndex(b => b.FeatureName);
    }
}
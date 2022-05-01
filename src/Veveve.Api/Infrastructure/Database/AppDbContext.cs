using Veveve.Api.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Veveve.Api.Infrastructure.Database;

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
        modelBuilder.Entity<AccountEntity>().HasIndex(b => b.ClientId).IsUnique();
        modelBuilder.Entity<EmailLogEntity>().HasIndex(b => b.Reference).IsUnique();
    }
}
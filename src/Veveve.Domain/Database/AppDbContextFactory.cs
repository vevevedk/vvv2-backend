using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Veveve.Domain.Database;

// this is used by the EF CLI tools. Not the actual program itself.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=unused;Database=unused;Username=unused;Password=unused");

        return new AppDbContext(optionsBuilder.Options);
    }
}
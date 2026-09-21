using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ComponentLibTests.Data;

/// <summary>Design-time factory so `dotnet ef migrations` works without a hosting project.</summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("COMPONENTLIBTESTS_CONNECTION")
            ?? "Server=localhost,1433;Database=ComponentLibBenchmark;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}

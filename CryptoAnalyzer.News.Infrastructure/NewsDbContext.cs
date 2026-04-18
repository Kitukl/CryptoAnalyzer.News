using Microsoft.EntityFrameworkCore;

namespace CryptoAnalyzer.News.Infrastructure;

public class NewsDbContext(DbContextOptions<NewsDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.News> News { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NewsDbContext).Assembly);
    }
}
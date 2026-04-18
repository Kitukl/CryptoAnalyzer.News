using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoAnalyzer.News.Infrastructure.News;

public class NewsConfiguration : IEntityTypeConfiguration<Domain.Entities.News>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.News> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Text);
    }
}
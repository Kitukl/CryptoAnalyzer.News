using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoAnalyzer.News.Core;
using CryptoAnalyzer.News.Core.Interfaces;
using Moq;
using Xunit;
using NewsEntity = CryptoAnalyzer.News.Domain.Entities.News;

namespace CryptoAnalyzer.NewsService.Tests;

public class GetNewsQueryHandlerTests
{
    [Fact]
    public async Task Handle_FiltersOutOlderThanSevenDays()
    {
        var repository = new Mock<INewsRepository>();
        repository.Setup(x => x.GetNewsAsync())
            .ReturnsAsync(new List<NewsEntity>
            {
                new() { Text = "old", Grade = 1, isGenerated = false, Date = DateTime.UtcNow.AddDays(-8) },
                new() { Text = "recent", Grade = 2, isGenerated = true, Date = DateTime.UtcNow.AddDays(-2) }
            });

        var handler = new GetNewsQueryHandler(repository.Object);

        var result = await handler.Handle(new GetNewsQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("recent", result.First().Text);
    }
}

using CryptoAnalyzer.Core.Events;
using CryptoAnalyzer.News.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoAnalyzer.News.Core;

public class AnalyzeNews : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPublishEndpoint _eventBus;

    public AnalyzeNews(IServiceProvider serviceProvider, IPublishEndpoint eventBus)
    {
        _serviceProvider = serviceProvider;
        _eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<INewsRepository>();
                    var latestNews = (await repository.GetLatestNewsAsync()).ToList();
                    var newsInDb = await repository.GetNewsAsync();

                    var news = latestNews.Where(x => newsInDb.All(n => n.Text != x.Text)).ToList();
                    
                    var rawNews = string.Join("/n", news.Select(n => n.Text));

                    if (!string.IsNullOrWhiteSpace(rawNews))
                    {
                        var analyzedNews = await repository.NormalizeNews(rawNews);
                        news.Add(analyzedNews);

                        await _eventBus.Publish(new NewsEvent
                        {
                            NewsText = analyzedNews.Text,
                            Sentiment = analyzedNews.Grade
                        });
                        
                        await repository.CreateNewsAsync(news);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in background service: {ex.Message}");
            }
            await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
        }
    }
}
using CryptoAnalyzer.News.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoAnalyzer.News.Core;

public class AnalyzeNews : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AnalyzeNews(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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

                    var rawNews = await repository.GetLatestNewsAsync();

                    if (!string.IsNullOrWhiteSpace(rawNews))
                    {
                        var analyzedNews = await repository.NormalizeNews(rawNews);
                        await repository.CreateNewsAsync(analyzedNews);
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
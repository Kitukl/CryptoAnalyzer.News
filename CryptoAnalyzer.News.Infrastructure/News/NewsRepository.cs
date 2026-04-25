using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;
using CryptoAnalyzer.News.Core.Interfaces;
using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CryptoAnalyzer.News.Infrastructure.News;

public class NewsRepository : INewsRepository
{
    private readonly NewsDbContext _context;
    private readonly Client _client;

    public NewsRepository(NewsDbContext context, IOptions<GeminiOptions> options)
    {
        _context = context;
        _client = new Client(apiKey: options.Value.ApiKey);
    }
    
    public async Task<IEnumerable<Domain.Entities.News>> GetNewsAsync()
    {
        return await _context.News
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Domain.Entities.News>> CreateNewsAsync(IEnumerable<Domain.Entities.News> news)
    {
        await _context.AddRangeAsync(news);
        await _context.SaveChangesAsync();

        return news;
    }

    public async Task<IEnumerable<Domain.Entities.News>> GetLatestNewsAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        var stream = await httpClient.GetStreamAsync("https://cointelegraph.com/rss");
        using var reader = XmlReader.Create(stream);
        var feed = SyndicationFeed.Load(reader);

        var newsItems = feed.Items
            .Take(20)
            .Select(item => new Domain.Entities.News
            {
                Id = Guid.NewGuid(),
                Text = item.Title.Text,
                isGenerated = false,
                Date = DateTime.UtcNow
            })
            .ToList();
        
        return newsItems;
    }

    public async Task<Domain.Entities.News> NormalizeNews(string rowNews)
    {
        var model = "models/gemini-2.5-flash";
        
        var prompt = $@"
            Проаналізуй ці новини криптовалют:
            {rowNews}

            Дай відповідь СУВОРО у форматі JSON:
            {{
              ""text"": ""короткий підсумок українською (2 речення)"",
              ""grade"": 0.5,
              ""createdAt"": дата створення джсонки
            }}
            Оцінка score має бути від -2.0 (повна паніка) до 2.0 (ракета/ейфорія).";

        var response = await _client.Models.GenerateContentAsync(model, prompt);
        var rawText = response.Text;
        
        var cleanJson = rawText
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };

        var normalizedNews = JsonSerializer.Deserialize<Domain.Entities.News>(cleanJson, options);

        if (normalizedNews != null)
        {
            normalizedNews.isGenerated = true;
            normalizedNews.Date = DateTime.UtcNow;
        }

        return normalizedNews;
    }
}
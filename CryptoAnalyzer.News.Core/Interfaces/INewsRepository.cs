namespace CryptoAnalyzer.News.Core.Interfaces;

public interface INewsRepository
{
    public Task<IEnumerable<Domain.Entities.News>> GetNewsAsync();
    public Task<Domain.Entities.News> CreateNewsAsync(Domain.Entities.News news);
    public  Task<string> GetLatestNewsAsync();
    public Task<Domain.Entities.News> NormalizeNews(string rowNews);
}
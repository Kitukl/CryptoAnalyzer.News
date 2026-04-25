namespace CryptoAnalyzer.News.Core.Interfaces;

public interface INewsRepository
{
    public Task<IEnumerable<Domain.Entities.News>> GetNewsAsync();
    public Task<IEnumerable<Domain.Entities.News>> CreateNewsAsync(IEnumerable<Domain.Entities.News> news);
    public  Task<IEnumerable<Domain.Entities.News>> GetLatestNewsAsync();
    public Task<Domain.Entities.News> NormalizeNews(string rowNews);
}
using CryptoAnalyzer.News.Core.Interfaces;
using CryptoAnalyzer.NewsService.Common;
using MediatR;

namespace CryptoAnalyzer.News.Core;

public record GetNewsQuery : IRequest<IEnumerable<NewsResponse>>;

public class GetNewsQueryHandler : IRequestHandler<GetNewsQuery, IEnumerable<NewsResponse>>
{
    private readonly INewsRepository _repository;

    public GetNewsQueryHandler(INewsRepository repository)
    {
        _repository = repository;
    }
    public async Task<IEnumerable<NewsResponse>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        var news = await _repository.GetNewsAsync();
        IEnumerable<NewsResponse> response = news
            .Where(c => c.Date.AddDays(7) >= DateTime.UtcNow)
            .Select(c => new NewsResponse
            {
                Text = c.Text,
                Grade = c.Grade,
                Date = c.Date
            });

        return response;
    }
}
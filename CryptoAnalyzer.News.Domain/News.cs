namespace CryptoAnalyzer.News.Domain.Entities;

public class News
{
    public Guid Id { get; set; }
    public double? Grade { get; set; }
    public string Text { get; set; }
    public bool isGenerated { get; set; }
    public DateTime Date { get; set; }
}
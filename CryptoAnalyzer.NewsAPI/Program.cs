using CryptoAnalyzer.News.Core;
using CryptoAnalyzer.News.Core.Interfaces;
using CryptoAnalyzer.News.Infrastructure;
using CryptoAnalyzer.News.Infrastructure.News;
using CryptoAnalyzer.NewsService.Extensions;
using dotenv.net;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("EventBus");
        cfg.Host(new Uri(connectionString));
    });
});

builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddHostedService<AnalyzeNews>();

builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("GeminiOptions"));

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NewsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(GetNewsQueryHandler).Assembly));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddJwtAuthentication(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JWTOptions>>());

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
using System.Diagnostics;
using Meilisearch;
using Meilisearch.QueryParameters;
using TooniverseAPI.Database;

namespace TooniverseAPI.Services;

public class MeiliSearchSyncService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly MeilisearchClient _meiliSearchClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MeiliSearchSyncService(IServiceScopeFactory serviceScopeFactory, MeilisearchClient meiliSearchClient)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _meiliSearchClient = meiliSearchClient;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _dbContext = scope.ServiceProvider.GetRequiredService<TooniverseContext>();
        var index = await _meiliSearchClient.GetIndexAsync("anime");
        await index.DeleteAllDocumentsAsync();
        await Task.Delay(20_000);
        var ids = await index.GetDocumentsAsync<Tuple<int>>(new DocumentsQuery()
        {
            Fields = new List<string>() { "id" },
            Limit = 50_000
        });
        var idsInIndex = ids.Results.Select(id => id.Item1).ToHashSet();
        var dataToSync = _dbContext.Anime
            .Where(d => !idsInIndex.Contains(d.Id))
            .Select(m => new MediaDto
            {
                Id = m.Id,
                Title = m.Title,
                Overview = m.Overview,
                Year = m.Year,
                Poster = m.Poster,
                Banner = m.Banner,
                Genres = m.Genres,
                Tags = m.Tags,
                Status = m.Status,
                Favorites = m.Favorites,
                Format = m.Format,
                AverageScore = m.AverageScore,
                Popularity = m.Popularity,
                Mappings = m.Mappings,
                Color = m.Color,
                Season = m.Season
            })
            .ToList();

        if (!dataToSync.Any()) return;

        await index.AddDocumentsAsync(dataToSync);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

public class MediaDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Poster { get; set; }
    public string? Banner { get; set; }
    public int? Year { get; set; }
    public string? Overview { get; set; }
    public string[]? Genres { get; set; }
    public string[]? Tags { get; set; }
    public string? Status { get; set; }
    public int? Favorites { get; set; }
    public string? Format { get; set; }
    public int? AverageScore { get; set; }
    public int? Popularity { get; set; }
    public string? Color { get; set; }
    public string? Season { get; set; }
    public ICollection<Mapping> Mappings { get; set; }
}
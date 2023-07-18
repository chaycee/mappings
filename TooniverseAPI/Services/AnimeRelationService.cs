using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Meilisearch;
using Meilisearch.QueryParameters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Crawling.Anime;
using TooniverseAPI.Mappings.Providers.Id;

namespace TooniverseAPI.Services;

public class AnimeRelationService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AnimeRelationService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TooniverseContext>();
            var alreadyMapped = dbContext.Anime.Where(x => x.RecommendedTo.Count == 0 && x.RelatedTo.Count == 0)
                .Select(x => x.Id).ToArray();
            DoWork(alreadyMapped);
        }
        catch (Exception e)
        {
        }

        return Task.CompletedTask;
    }

    private async void DoWork(int[] ids)
    {
        var actionBlock = new ActionBlock<int[]>(
            async array =>
            {
                try
                {
                    Console.WriteLine("Changes Started");
                    using (var newContext = new TooniverseContext())
                    {
                        var animes = newContext.Anime.Where(a => array.Contains(a.Id))
                            .Include(x => x.Recommendations)
                            .Include(x => x.Relations).AsSplitQuery().AsNoTracking().AsParallel().Select(x =>
                                new TempMedia
                                {
                                    Id = x.Id,
                                    Relations = x.Relations,
                                    Recommendations = x.Recommendations
                                }).ToList();

                        foreach (var anime in animes)
                        {
                            var relationAnimes = await GetRelations(anime, newContext);
                            var recommendationAnimes = await GetRecommendations(anime, newContext);
                            var media = await newContext.Anime.Where(x => x.Id == anime.Id).AsTracking()
                                .FirstOrDefaultAsync();
                            media.RelatedTo = relationAnimes.DistinctBy(x => x.Id).ToList();
                            media.RecommendedTo = recommendationAnimes.DistinctBy(x => x.Id).ToList();
                            // media.RecommendationsMedia = recommendationAnimes;
                        }

                        Console.WriteLine("Changes Saved");
                        await newContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

        for (var i = 0; i < ids.Length; i += 100)
        {
            var chunk = ids.Skip(i).Take(100).ToArray();
            actionBlock.Post(chunk);
        }

        actionBlock.Complete();
        await actionBlock.Completion;
        Console.WriteLine("Mapping Finished");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Mapping Stopped");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private async Task<List<Media>> GetRelations(TempMedia anime, TooniverseContext context)
    {
        var relationFrom = anime.Relations
            .Select(r => r.From)
            .ToList();
        var relationFromId = anime.Relations
            .Select(r => r.FromId)
            .ToList();

        return context.Anime
            .Where(a => a.Mappings.Any(mapping =>
                relationFrom.Contains(mapping.Source) && relationFromId.Contains(mapping.SourceId)))
            .AsSplitQuery()
            .ToList();
    }

    private async Task<List<Media>> GetRecommendations(TempMedia anime, TooniverseContext context)
    {
        var recommendationFrom = anime.Recommendations
            .Select(r => r.From)
            .ToList();
        var recommendationFromId = anime.Recommendations
            .Select(r => r.FromId)
            .ToList();

        return context.Anime
            .Where(a => a.Mappings.Any(mapping =>
                recommendationFrom.Contains(mapping.Source) && recommendationFromId.Contains(mapping.SourceId)))
            .AsSplitQuery()
            .ToList();
    }

    internal class TempMedia
    {
        public int Id { get; set; }
        public ICollection<Relation>? Relations { get; set; }
        public ICollection<Recommendation>? Recommendations { get; set; }
    }
}
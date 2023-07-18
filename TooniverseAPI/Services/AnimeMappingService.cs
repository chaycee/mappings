using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using HotChocolate.Language;
using Meilisearch;
using Meilisearch.QueryParameters;
using Newtonsoft.Json;
using ShellProgressBar;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Crawling.Anime;
using TooniverseAPI.Mappings.Providers.Id;
using TooniverseAPI.Mappings.Providers.Meta.Shared.Models;

namespace TooniverseAPI.Services;

public class AnimeMappingService : IHostedService, IDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AnimeCrawler _crawler = new();

    public AnimeMappingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TooniverseContext>();
            var alreadyMapped = dbContext.Anime.Select(x => int.Parse(x.MappedFrom.Id)).ToArray();
            //var ids = MalSyncBackup.GetIds();
            var ids = GetAnimeIDs().Result.ToArray();
            Console.WriteLine(ids.Length);
            var difference = ids.Except(alreadyMapped).ToArray();
            Console.WriteLine(difference.Length);
            Array.Sort(difference);
            DoWork(difference);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Task.CompletedTask;
    }

    private async void DoWork(int[] ids)
    {
        var totalTasks = (int)Math.Ceiling(ids.Length / 18.0);

        using (var pbar = new ProgressBar(totalTasks, "Processing... ", ConsoleColor.Red))
        {
            var actionBlock = new ActionBlock<int[]>(
                async array =>
                {
                    try
                    {
                        List<Task<Media[]>> tasks = new();

                        for (var i = 0; i < array.Length; i += 6)
                        {
                            var chunk = array.Skip(i).Take(6).ToArray();
                            tasks.Push(_crawler.MapChunkFromProvider(chunk));
                        }

                        var medias = await Task.WhenAll(tasks);
                        using (var newContext = new TooniverseContext())
                        {
                            newContext.Anime.AddRange(medias.SelectMany(x => x));
                            await newContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        // Increment the ProgressBar after each "chunk" of work is done
                        pbar.Tick($"Processed {JsonConvert.SerializeObject(array)}");
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount * 3
                });

            for (var i = 0; i < ids.Length; i += 18)
            {
                var chunk = ids.Skip(i).Take(18).ToArray();
                actionBlock.Post(chunk);
            }

            actionBlock.Complete();
            await actionBlock.Completion;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mapping Finished");
        }
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Mapping Stopped");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Console.WriteLine("Mapping Disposed");
    }

    public async Task<int[]> GetAnimeIDs()
    {
        using (var client = new HttpClient())
        {
            var idList =
                await client.GetStringAsync("https://raw.githubusercontent.com/5H4D0WILA/IDFetch/main/ids.txt");
            var idStrings = idList.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var ids = Array.ConvertAll(idStrings, int.Parse);
            return ids;
        }
    }
}
using HotChocolate.Language;
using Juro.Models.Anime;
using Juro.Providers.Anime;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Providers.Id;
using TooniverseAPI.Mappings.Providers.Meta.Shared;
using TooniverseAPI.Models;

namespace TooniverseAPI.Mappings.Providers.Media.Anime;

public class AllAnimeProviders
{

    //TODO: Ensure ALL mappings from MALSYNC are here
    public async Task<List<ProviderResult>> SearchAsync(string query, int id, int? year = null, string? format = null)
    {
        async Task<ProviderResult> SearchProviderAsync(IAnimeProvider provider)
        {
            try
            {
                ValueTask<List<IAnimeInfo>> searchTask;
                if (provider is AnimePahe animePaheProvider)
                {
                    searchTask =animePaheProvider.SearchAsync(query, true);
                }
                else
                {
                    searchTask = provider.SearchAsync(query);
                }
                var searchRes = await searchTask;
                var result = searchRes.Select(x => new SearchResult
                {
                    Id = x.Id,
                    Title = x.Title,
                    Year = x.Released,
                    Poster = x.Image
                }).ToList();

                return new ProviderResult
                {
                    Name = Source.MapToSource(provider.Name),
                    Results = result
                };
            }
            catch (Exception)
            {
                // If an exception occurs, return null
                return null;
            }
        }

        var mapped = MalSyncBackup.GetMappings(id.ToString());

        var alreadyMapped = mapped?.Mappings?.Select(x => x.Source).ToList();
        var allProviderNames = Clients.AllAnimeProviders.Select(x => Source.MapToSource(x.Name)).ToList();

        var animeTasks = Clients.AllAnimeProviders
            .Select(x =>
            {
                if (alreadyMapped?.Contains(Source.MapToSource(x.Name)) == true)
                    return Task.FromResult(new ProviderResult
                    {
                        Name = Source.MapToSource(x.Name),
                        Mapping = mapped?.Mappings?.FirstOrDefault(m =>
                            Source.MapToSource(m.Source) == Source.MapToSource(x.Name)),
                        Artwork = mapped?.Artworks?.FirstOrDefault(a =>
                            Source.MapToSource(a.Source) == Source.MapToSource(x.Name)),
                        HasMapping = true
                    });

                // If no mappings, then call SearchProviderAsync
                return SearchProviderAsync(x);
            });


        var providerContainsMappings = alreadyMapped?.Where(map => !allProviderNames.Contains(map)).ToList();

        if (providerContainsMappings is not null)
            foreach (var map in providerContainsMappings)
                if (map is not null)
                    animeTasks = animeTasks.Append(Task.FromResult(new ProviderResult
                    {
                        Name = map,
                        Mapping = mapped?.Mappings?.FirstOrDefault(m =>
                            Source.MapToSource(m.Source) == Source.MapToSource(map)),
                        Artwork = mapped?.Artworks?.FirstOrDefault(a =>
                            Source.MapToSource(a.Source) == Source.MapToSource(map)),
                        HasMapping = true
                    }));

        var metaProviderTasks = Clients.AllMetaProviders.Select(x => x.SearchAsync(query, year, format));
        foreach (var t in metaProviderTasks) animeTasks = animeTasks.Append(t);

        var results = await Task.WhenAll(animeTasks);
        return results
            .Where(result => result != null)
            .ToList();
    }
}
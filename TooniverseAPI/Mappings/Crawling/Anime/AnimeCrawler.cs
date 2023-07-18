using Newtonsoft.Json;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;
using TooniverseAPI.Mappings.Providers.Media.Anime;
using TooniverseAPI.Mappings.Utils;
using TooniverseAPI.Mappings.Utils.Extensions;
using TooniverseAPI.Mappings.Utils.StringSimilarity;
using Media = TooniverseAPI.Database.Media;

namespace TooniverseAPI.Mappings.Crawling.Anime;

public partial class AnimeCrawler
{
    private readonly AnilistInfo _anilistInfo = new();
    private readonly AllAnimeProviders _allProviders = new();
    private readonly double _similarityThreshold = 0.6;

    public async Task<Media[]> MapChunkFromProvider(IEnumerable<int> providerIds)
    {
        var providerDataResult = await _anilistInfo.GetInfoBatchAsync(providerIds);
        var mapTasks = providerDataResult?.data?.Values.Select(anime =>
                anime.media?.ElementAtOrDefault(0) != null ? MapFromProvider(anime.media[0]) : null)?
            .ToList().Where(x => x != null);
        if (mapTasks == null)
            return Array.Empty<Media>();

        return await Task.WhenAll(mapTasks);
    }

    private async Task<Media> MapFromProvider(Medium anime)
    {
        var mappedResult = MapAnimeToMediaFromAnilist(anime);

        var mappedProviders = await MapToBaseProviders(anime);

        mappedResult.MappedFrom = new MappedFrom()
        {
            Id = anime.id.ToString()!,
            Source = Source.AniList
        };
        mappedResult.Mappings!.Add(new Mapping()
        {
            SourceId = anime.id.ToString()!,
            Source = Source.AniList
        });
        mappedResult.Mappings = Combine.Enumerable(mappedResult.Mappings, mappedProviders.Mappings).ToArray();
        mappedResult.Artworks = Combine.Enumerable(mappedResult.Artworks, mappedProviders.Artworks).ToArray();
        mappedResult.Tags = Combine.Enumerable(mappedResult.Tags, mappedProviders.Tags).ToArray().RemoveDuplicates();
        mappedResult.Genres =
            Combine.Enumerable(mappedResult.Genres, mappedProviders.Genres).ToArray().RemoveDuplicates();
        mappedResult.Titles =
            Combine.Enumerable(mappedResult.Titles, mappedProviders.Titles).ToArray()!.RemoveDuplicates();
        mappedResult.Characters = Combine.Enumerable(mappedResult.Characters, mappedProviders.Characters).ToArray();
        mappedResult.Relations = Combine.Enumerable(mappedResult.Relations, mappedProviders.Relations).ToArray();
        mappedResult.YoutubeTrailers = Combine.Enumerable(mappedResult.YoutubeTrailers, mappedProviders.YoutubeTrailers)
            .ToArray().RemoveDuplicates();
        ;
        var subMediaTvdb = await GetSubMediaTVDB(mappedResult.Mappings);

        var hqPoster =
            subMediaTvdb?.Artworks?.OrderByDescending(x => x.Rating).FirstOrDefault(x => x.Type == "Poster") ??
            mappedProviders?.Artworks?.FirstOrDefault(x => x.Source == Source.Tmdb && x.Type == "Poster");
        if (hqPoster != null)
            mappedResult.Poster = hqPoster.Image;

        mappedResult.Artworks = Combine.Enumerable(mappedResult.Artworks, subMediaTvdb?.Artworks).ToArray();
        mappedResult.Mappings = Combine.Enumerable(mappedResult.Mappings, subMediaTvdb?.Mappings).ToArray();
        mappedResult.YoutubeTrailers =
            Combine.Enumerable(mappedResult.YoutubeTrailers, subMediaTvdb?.YoutubeTrailers).ToArray();
        return mappedResult;
    }
}
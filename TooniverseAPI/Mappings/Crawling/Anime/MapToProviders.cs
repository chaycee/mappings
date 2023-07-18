using System.Diagnostics;
using Newtonsoft.Json;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;
using TooniverseAPI.Mappings.Utils.Extensions;
using TooniverseAPI.Mappings.Utils.StringSimilarity;

namespace TooniverseAPI.Mappings.Crawling.Anime;

public partial class AnimeCrawler
{
    private async Task<SubMedia> MapToBaseProviders(Medium anime)
    {
        var title = CleanTitle(anime.title);
        var titles = GetTitles(anime);
        var year = anime.startDate.year ?? anime.seasonYear;
        var searchResults = await SearchProviders(title, anime);
        var artworksAndMappings = ProcessSearchResults(searchResults, titles, year);

        return ConstructSubMedia(artworksAndMappings);
    }

    private string CleanTitle(Title title)
    {
        return StringSimilarity.Clean((title?.english ?? title?.romaji ?? title?.native)!);
    }

    private string?[] GetTitles(Medium? anime)
    {
        var baseTitles = new[]
        {
            anime?.title?.english,
            anime?.title?.romaji,
            anime?.title?.native
        }.Where(t => !string.IsNullOrWhiteSpace(t)); // Use Where to filter out null titles

        var synonyms =
            anime?.synonyms ??
            Enumerable.Empty<string>(); // Use ?? to default to an empty collection if `anime.synonyms` is null
        return baseTitles.Concat(synonyms).Distinct().ToArray();
    }

    private async Task<List<ProviderResult>> SearchProviders(string title, Medium anime)
    {
        return await _allProviders.SearchAsync(
            title,
            anime.id,
            anime.startDate.year ?? anime.seasonYear,
            anime.format);
    }

    private (List<Artwork> artworks, List<Mapping> mappings) ProcessSearchResults(
        List<ProviderResult> searchResults,
        string[] titles,
        int? year)
    {
        var artworks = new List<Artwork>();
        var mappings = new List<Mapping>();

        foreach (var providerResult in searchResults)
            if (providerResult.HasMapping)
                AddExtraMappingsAndArtwork(providerResult, mappings, artworks);
            else
                AddBestMatchMappingsAndArtwork(providerResult, titles, year, mappings, artworks);

        return (artworks, mappings);
    }

    private void AddExtraMappingsAndArtwork(
        ProviderResult providerResult,
        List<Mapping> mappings,
        List<Artwork> artworks)
    {
        mappings.Add(providerResult.Mapping!);
        artworks.Add(providerResult.Artwork!);
    }

    private void AddBestMatchMappingsAndArtwork(
        ProviderResult providerResult,
        string[] titles,
        int? year,
        List<Mapping> mappings,
        List<Artwork> artworks)
    {
        if (providerResult.Results == null)
            return;

        var titlesToCheck = providerResult.Results?
            .Select(result => new[] { result?.Title ?? "" }.Concat(result?.Titles ?? Array.Empty<string>()).ToArray())
            .ToArray()!;

        var bestMatch = StringSimilarity.FindBestMatch2DArray(
            titles.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray()!,
            titlesToCheck);
        if (bestMatch.BestMatchIndex < providerResult.Results!.Count)
        {
            var best = providerResult.Results[bestMatch.BestMatchIndex];

            if (bestMatch.BestMatch.Value > _similarityThreshold)
            {
                if (!string.IsNullOrEmpty(best.Poster))
                    artworks.Add(new Artwork()
                    {
                        Source = Source.MapToSource(providerResult.Name),
                        Image = best.Poster,
                        Type = "Poster"
                    });

                mappings.Add(new Mapping()
                {
                    Source = Source.MapToSource(providerResult.Name),
                    SourceId = best.Id,
                    Type = best.Type
                });
            }
        }
    }

    private SubMedia ConstructSubMedia((List<Artwork> artworks, List<Mapping> mappings) artworksAndMappings)
    {
        return new SubMedia()
        {
            Mappings = artworksAndMappings.mappings,
            Artworks = artworksAndMappings.artworks
        };
    }
}
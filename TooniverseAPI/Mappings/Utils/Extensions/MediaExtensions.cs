using Microsoft.EntityFrameworkCore;
using TooniverseAPI.Database;

namespace TooniverseAPI.Mappings.Utils.Extensions;

public static class MediaExtensions
{
    public static ParallelQuery<AnimeType> GenerateReturnType(this IQueryable<Media> media)
    {
        return media
            .Include(x => x.Characters)
            .ThenInclude(x => x.VoiceActors).Select(x =>
                new AnimeType
                {
                    Id = x.Id,
                    Title = x.Title,
                    Titles = x.Titles,
                    Poster = x.Poster,
                    Banner = x.Banner,
                    Overview = x.Overview,
                    Genres = x.Genres,
                    Tags = x.Tags,
                    SeasonYear = x.SeasonYear,
                    Status = x.Status,
                    Format = x.Format,
                    Favorites = x.Favorites,
                    Duration = x.Duration,
                    AverageScore = x.AverageScore,
                    MeanScore = x.MeanScore,
                    Popularity = x.Popularity,
                    Artwork = x.Artworks.Take(15).Select(f => new ArtworkType()
                    {
                        Source = f.Source,
                        Image = f.Image,
                        Type = f.Type
                    }),
                    Characters = x.Characters,
                    Related = x.RelatedTo.Take(15).Select(f => new SlimAnimeType()
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Titles = f.Titles,
                        Overview = f.Overview,
                        Genres = f.Genres,
                        Poster = f.Poster,
                        Banner = f.Banner,
                        SeasonYear  = f.SeasonYear,
                        StartDate  = f.StartDate,
                        Favorites = x.Favorites,
                        Duration = x.Duration,
                        AverageScore = x.AverageScore,
                        Popularity = x.Popularity,
                        Color = f.Color
                    }),
                    Recommended = x.RecommendedTo.Take(15).Select(f => new SlimAnimeType()
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Titles = f.Titles,
                        Overview = f.Overview,
                        Genres = f.Genres,
                        Poster = f.Poster,
                        Banner = f.Banner,
                        SeasonYear  = f.SeasonYear,
                        StartDate  = f.StartDate,
                        Favorites = x.Favorites,
                        Duration = x.Duration,
                        AverageScore = x.AverageScore,
                        Popularity = x.Popularity,
                        Color = f.Color,
                    }),
                    Mappings = x.Mappings,
                    Color = x.Color,
                    StartDate = x.StartDate

                }).AsSplitQuery()
            .AsNoTracking()
            .AsParallel();

    }
     public static ParallelQuery<SlimAnimeType> GenerateSlimReturnType(this IQueryable<Media> media)
    {
        return media
          .Select(x =>
                new SlimAnimeType
                {
                    Id = x.Id,
                    Title = x.Title,
                    Titles = x.Titles,
                    Poster = x.Poster,
                    Banner = x.Banner,
                    Overview = x.Overview,
                    Genres = x.Genres,
                    SeasonYear = x.SeasonYear,
                    Status = x.Status,
                    Format = x.Format,
                    Favorites = x.Favorites,
                    Duration = x.Duration,
                    AverageScore = x.AverageScore,
                    MeanScore = x.MeanScore,
                    Popularity = x.Popularity,
                    Color = x.Color,
                    StartDate = x.StartDate
                }).AsSplitQuery()
            .AsNoTracking()
            .AsParallel();

    }
}
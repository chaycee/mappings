using Microsoft.EntityFrameworkCore;
using TooniverseAPI.Database;

namespace TooniverseAPI.Mappings.Utils.Extensions;

public static class MediaExtensions
{
    public static AnimeType? GenerateReturnType(this IQueryable<Media> media)
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
                    Year = x.Year,
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
                    Related = x.RelatedTo.Take(15).Select(f => new RelatedType()
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Titles = f.Titles,
                        Overview = f.Overview,
                        Genres = f.Genres,
                        Tags = f.Tags,
                        Poster = f.Poster,
                        Banner = f.Banner,
                        Year = f.Year,
                        Favorites = x.Favorites,
                        Duration = x.Duration,
                        AverageScore = x.AverageScore,
                        Popularity = x.Popularity,
                        Color = f.Color
                    }),
                    Recommended = x.RecommendedTo.Take(15).Select(f => new RelatedType()
                    {
                        Id = f.Id,
                        Title = f.Title,
                        Titles = f.Titles,
                        Overview = f.Overview,
                        Genres = f.Genres,
                        Tags = f.Tags,
                        Poster = f.Poster,
                        Banner = f.Banner,
                        Year = f.Year,
                        Favorites = x.Favorites,
                        Duration = x.Duration,
                        AverageScore = x.AverageScore,
                        Popularity = x.Popularity,
                        Color = f.Color
                    }),
                    Mappings = x.Mappings,
                    Color = x.Color
                }).AsSplitQuery()
            .AsNoTracking()
            .AsParallel()
            .FirstOrDefault();
    }
}
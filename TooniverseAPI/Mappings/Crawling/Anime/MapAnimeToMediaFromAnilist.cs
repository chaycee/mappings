using HotChocolate.Language;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist.Characters;

namespace TooniverseAPI.Mappings.Crawling.Anime;

using Database;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;
using Media = Database.Media;
using Type = TooniverseAPI.Database.Type;

public partial class AnimeCrawler
{
    private Media MapAnimeToMediaFromAnilist(Medium anime)
    {
        return new Media
        {
            Id = anime.id!,
            Title = GetAnimeTitle(anime),
            Titles = GetAnimeTitles(anime),
            Overview = anime.description,
            Status = anime.status,
            Tags = GetAnimeTags(anime),
            Poster = GetAnimePoster(anime),
            Banner = anime.bannerImage,
            Year = GetAnimeYear(anime),
            Genres = GetAnimeGenres(anime),
            Color = GetAnimeColor(anime),
            InsertedAt = DateTime.UtcNow,
            Season = anime.season,
            Type = Type.Anime,
            Relations = GetMediaRelations(anime),
            Characters = GetCharacters(anime),
            Recommendations = GetRecommendations(anime),
            Mappings = GetAnimeMapping(anime),
            Artworks = GetAnimeArtwork(anime),
            Favorites = anime.favourites,
            Format = anime.format,
            Duration = anime.duration,
            MeanScore = anime.meanScore,
            AverageScore = anime.averageScore,
            Popularity = anime.popularity,
            YoutubeTrailers = GetYoutubeTrailer(anime)
        };
    }

    private string[] GetYoutubeTrailer(Medium anime)
    {
        var trailers = new List<string>();
        if (anime.trailer is not null)
            if (anime.trailer.site == "youtube")
                trailers.Push($"https://www.youtube.com/watch?v={anime.trailer.id}");


        return trailers.ToArray();
    }

    private ICollection<Mapping> GetAnimeMapping(Medium anime)
    {
        var mappings = new List<Mapping>();
        if (anime.idMal is not null)
            mappings.Add(new Mapping()
            {
                Type = "anime",
                Source = Source.MyAnimeList,
                SourceId = anime.idMal.ToString()
            });

        return mappings;
    }

    private ICollection<Artwork> GetAnimeArtwork(Medium anime)
    {
        var artworks = new List<Artwork>();
        if (!string.IsNullOrEmpty(anime.coverImage?.extraLarge))
            artworks.Add(new Artwork()
            {
                Image = anime.coverImage?.extraLarge,
                Type = "Poster",
                Source = Source.AniList
            });
        if (!string.IsNullOrEmpty(anime?.bannerImage))
            artworks.Add(new Artwork()
            {
                Image = anime?.bannerImage,
                Type = "Banner",
                Source = Source.AniList
            });

        return artworks;
    }

    private string GetAnimeTitle(Medium anime)
    {
        return (anime.title?.english ?? anime.title?.romaji ?? anime.title?.native)!;
    }

    private string[] GetAnimeTitles(Medium anime)
    {
        return (new string[] { anime.title?.english, anime.title?.romaji, anime.title?.native }.Where(x =>
            !string.IsNullOrWhiteSpace(x)) ?? Array.Empty<string>()).ToArray();
    }

    private int? GetAnimeYear(Medium anime)
    {
        return anime.startDate.year ?? anime.seasonYear;
    }

    private string[] GetAnimeGenres(Medium anime)
    {
        return anime.genres.ToArray();
    }

    private string[] GetAnimeTags(Medium anime)
    {
        return anime.tags.Select(x => x.name).ToArray();
    }

    private string GetAnimePoster(Medium anime)
    {
        return anime.coverImage?.extraLarge ?? anime.coverImage?.large;
    }

    private string? GetAnimeColor(Medium anime)
    {
        return anime.coverImage?.color;
    }

    private Character[] GetCharacters(Medium anime)
    {
        var mainCharacters = GetMainCharacters(anime);
        var popularCharacters = GetPopularCharacters(anime);

        return mainCharacters.Concat(popularCharacters).ToArray();
    }

    private Relation[] GetMediaRelations(Medium anime)
    {
        return anime.relations?.edges?.Select(x => new Relation
        {
            FromId = x.node?.id?.ToString(),
            From = Source.AniList,
            FromType = x.relationType
        })?.ToArray() ?? Array.Empty<Relation>();
    }

    private Character[] GetMainCharacters(Medium anime)
    {
        return anime.characters?.edges?.Where(x => x.role == "MAIN")
            .Select(x => BuildCharacter(x, true))
            .ToArray() ?? Array.Empty<Character>();
    }

    private Character[] GetPopularCharacters(Medium anime)
    {
        return anime.characters?.edges?.Where(x => x.role != "MAIN")
            .OrderByDescending(x => x.node?.favourites)?.Take(5)
            .Select(x => BuildCharacter(x, true))
            .ToArray() ?? Array.Empty<Character>();
    }

    private Recommendation[] GetRecommendations(Medium anime)
    {
        return anime?.recommendations?.nodes?.Where(x => x?.mediaRecommendation?.id is not null).Select(x =>
        {
            return new Recommendation()
            {
                From = Source.AniList,
                FromId = x?.mediaRecommendation?.id.ToString(),
                FromType = "Recommendation"
            };
        }).ToArray() ?? Array.Empty<Recommendation>();
    }

// Extra method to reduce code duplication
    private static Character BuildCharacter(CharaEdge characterEdge, bool sortByFavourites = false)
    {
        var character = new Character
        {
            FullName = characterEdge.node?.name?.full,
            Image = characterEdge.node?.image?.large,
            Role = characterEdge.role,
            Age = characterEdge.node?.age,
            Favourites = characterEdge.node?.favourites
        };

        var voiceActorList = characterEdge.voiceActors?.Select(y => new VoiceActor()
        {
            FullName = y.name?.full,
            Image = y.image?.large,
            Age = y.age?.ToString(),
            Favourites = y.favourites,
            Gender = y.gender
        });

        character.VoiceActors = sortByFavourites
            ? voiceActorList?.OrderByDescending(p => p.Favourites)?.Take(2)?.ToArray() ?? Array.Empty<VoiceActor>()
            : voiceActorList?.ToArray() ?? Array.Empty<VoiceActor>();

        return character;
    }
}
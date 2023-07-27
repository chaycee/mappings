using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meilisearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace TooniverseAPI.Database;

public class TooniverseContext : DbContext
{
    private const string LocalString = "Host=localhost;Port=5432;Database=tooniverse;User Id=postgres;Password=1997;";
    private const string LocalStringTooni = "Host=localhost;Port=5432;Database=tooni;User Id=postgres;Password=1997;";

    private const string TooniVPSString =
        "Host=109.122.221.131;Port=9000;Database=clk5o63f000099so7bv8e4jyd;User Id=clk5o63ez00079so79h23e5z7;Password=IA3QEtetxMzP7tk3xUBGsSut;";

    private const string TooniVPSOracleString =
        "Host=150.136.90.137;Port=6969;Database=toonify;User Id=postgres;Password=989353c2269cd07eb89b;";

    public DbSet<Media> Anime { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(TooniVPSOracleString);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Media>().OwnsOne(media => media.MappedFrom)
            .HasIndex(mappedFrom => new { mappedFrom.Id, mappedFrom.Source })
            .IsUnique()
            .HasFilter(null);


        modelBuilder.Entity<Media>()
            .HasMany(workItem => workItem.RelatedFrom)
            .WithMany(workItem => workItem.RelatedTo)
            .UsingEntity<MediaRelation>(
                right => right
                    .HasOne(joinEntity => joinEntity.FromItem)
                    .WithMany(),
                left => left
                    .HasOne(joinEntity => joinEntity.ToItem)
                    .WithMany());

        modelBuilder.Entity<Media>()
            .HasMany(workItem => workItem.RecommendedFrom)
            .WithMany(workItem => workItem.RecommendedTo)
            .UsingEntity<MediaRecommendation>(
                right => right
                    .HasOne(joinEntity => joinEntity.FromItem)
                    .WithMany(),
                left => left
                    .HasOne(joinEntity => joinEntity.ToItem)
                    .WithMany());

        modelBuilder.Entity<MediaRelation>()
            .HasIndex(u => new { u.FromItemId, u.ToItemId })
            .IsUnique();
        modelBuilder.Entity<MediaRecommendation>()
            .HasIndex(u => new { u.FromItemId, u.ToItemId })
            .IsUnique();
    }
}

public class Media
{
    public int Id { get; set; }
    public MappedFrom MappedFrom { get; set; }
    public string Title { get; set; }
    public Type Type { get; set; }
    public string? Poster { get; set; }
    public string? Banner { get; set; }
    public int? SeasonYear { get; set; }
    public StartDate? StartDate { get; set; }
    public ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
    public string[] Titles { get; set; } = Array.Empty<string>();
    public string? Overview { get; set; }
    public string[]? Genres { get; set; }
    public string[]? Tags { get; set; }
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public string? Status { get; set; }
    public string? Season { get; set; }
    public string? Format { get; set; }
    public int? Favorites { get; set; }
    public int? Trending { get; set; }
    public int? Duration { get; set; }
    public int? AverageScore { get; set; }
    public int? MeanScore { get; set; }
    public int? Popularity { get; set; }

    public ICollection<Mapping> Mappings { get; set; } = new List<Mapping>();
    public ICollection<Relation> Relations { get; set; } = new List<Relation>();

    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();


    public IList<Media> RelatedFrom { get; set; } = new List<Media>();
    public IList<Media> RelatedTo { get; set; } = new List<Media>();

    public IList<Media> RecommendedFrom { get; set; } = new List<Media>();
    public IList<Media> RecommendedTo { get; set; } = new List<Media>();

    public DateTime? InsertedAt { get; set; } = DateTime.Now;
    public string[] YoutubeTrailers { get; set; } = Array.Empty<string>();
    public string? Color { get; set; }
}

public class StartDate
{
    public int Id { get; set; }
    public int? Day { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}

public class MediaRelation
{
    public int Id { get; set; }
    public int FromItemId { get; set; }
    public int ToItemId { get; set; }
    public virtual Media FromItem { get; set; }
    public virtual Media ToItem { get; set; }
}

public class MediaRecommendation
{
    public int Id { get; set; }
    public int FromItemId { get; set; }
    public int ToItemId { get; set; }
    public virtual Media FromItem { get; set; }
    public virtual Media ToItem { get; set; }
}

public class MappedFrom
{
    public string Id { get; set; }
    public string Source { get; set; }
}

public class Character
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Image { get; set; }
    public string? Role { get; set; }
    public string? Gender { get; set; }
    public string? Age { get; set; }
    public int? Favourites { get; set; }
    public ICollection<VoiceActor>? VoiceActors { get; set; }
}

public class VoiceActor
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Image { get; set; }
    public string? Gender { get; set; }
    public string? Age { get; set; }
    public int? Favourites { get; set; }
}

public class Relation
{
    public int Id { get; set; }
    public string? From { get; set; }
    public string? FromId { get; set; }
    public string? FromType { get; set; }
}

public class Recommendation
{
    public int Id { get; set; }
    public string? From { get; set; }
    public string? FromId { get; set; }
    public string? FromType { get; set; }
}

public enum Type
{
    Tv,
    Movie,
    Anime,
    Manga
}

public class Mapping
{
    public int Id { get; set; }
    public string? Source { get; set; }
    public string? Type { get; set; }
    public string? SourceId { get; set; }
    
    public double? Similarity { get; set; }
}

public class Artwork
{
    public int Id { get; set; }
    public string? Image { get; set; }
    public string? Type { get; set; }
    public string? Source { get; set; }
    public int? Rating { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public string? Language { get; set; }
}

public static class Source
{
    public const string AniList = "AniList";
    public const string MyAnimeList = "MyAnimeList";
    public const string Tmdb = "Tmdb";
    public const string Tvdb = "Tvdb";
    public const string Zoro = "Zoro";
    public const string Gogoanime = "Gogoanime";
    public const string AnimePahe = "AnimePahe";
    public const string NineAnime = "9Anime";
    public const string Marin = "Marin";
    public const string Imdb = "Imdb";
    public const string Eidr = "Eidr";
    public const string Wikipedia = "Wikipedia";

    private static readonly Dictionary<string, string> NameMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "9anime", NineAnime },
        { "nineanime", NineAnime },
        { "gogo", Gogoanime },
        { "gogoanime", Gogoanime },
        { "zoro", Zoro },
        { "marin", Marin },
        { "tmdb", Tmdb },
        { "themoviedb.com", Tmdb },
        { "tvdb", Tvdb },
        { "mal", MyAnimeList },
        { "myanimelist", MyAnimeList },
        { "animepahe", AnimePahe },
        { "imdb", Imdb },
        { "eidr", Eidr },
        { "wikipedia", Wikipedia }
    };

    public static string MapToSource(string? input)
    {
        if (NameMappings.TryGetValue(input.ToLower().Trim(), out var source)) return source;

        return input; // Or throw an exception if desired
    }
}

public class SubMedia
{
    public ICollection<Artwork>? Artworks { get; set; }
    public string?[]? Titles { get; set; }
    public string[]? Genres { get; set; }
    public string[]? Tags { get; set; }
    public ICollection<Character>? Characters { get; set; }
    public ICollection<Mapping>? Mappings { get; set; }
    public ICollection<Relation>? Relations { get; set; }
    public string[]? YoutubeTrailers { get; set; }
}
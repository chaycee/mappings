namespace TooniverseAPI.Database;

public class AnimeType
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string?[]? Titles { get; set; }
    public string? Overview { get; set; }
    public string[]? Genres { get; set; }
    public string[]? Tags { get; set; }
    public string? Poster { get; set; }
    public string? Banner { get; set; }
    public int? Year { get; set; }
    public IEnumerable<ArtworkType> Artwork { get; set; }
    public ICollection<Character>? Characters { get; set; }
    public IEnumerable<RelatedType> Related { get; set; }
    public IEnumerable<RelatedType> Recommended { get; set; }
    public ICollection<Mapping>? Mappings { get; set; }
    public string? Color { get; set; }
    public string? Status { get; set; }
    public string? Format { get; set; }
    public int? Favorites { get; set; }
    public int? Duration { get; set; }
    public int? AverageScore { get; set; }
    public int? MeanScore { get; set; }
    public int? Popularity { get; set; }
}
public class  SlimAnimeType
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string?[]? Titles { get; set; }
    public string? Overview { get; set; }
    public string[]? Genres { get; set; }
    public string? Poster { get; set; }
    public string? Banner { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public string? Status { get; set; }
    public string? Format { get; set; }
    public int? Favorites { get; set; }
    public int? Duration { get; set; }
    public int? AverageScore { get; set; }
    public int? MeanScore { get; set; }
    public int? Popularity { get; set; }
}

public class ArtworkType
{
    public string? Source { get; set; }
    public string? Image { get; set; }
    public string? Type { get; set; }
}

public class RelatedType
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string?[]? Titles { get; set; }
    public string? Overview { get; set; }
    public string[]? Genres { get; set; }
    public string[]? Tags { get; set; }
    public string? Poster { get; set; }
    public string? Banner { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public int? Favorites { get; set; }
    public int? Duration { get; set; }
    public int? AverageScore { get; set; }
    public int? Popularity { get; set; }
}
using TooniverseAPI.Database;

namespace TooniverseAPI.Models;

public class SearchResult
{
    public string Id { get; set; }
    public string Title { get; set; }

    public string? Overview { get; set; }

    public string[] Titles { get; set; } = Array.Empty<string>();
    public string? Year { get; set; }

    public string? Poster { get; set; }
    public string? Backdrop { get; set; }

    public string? Type { get; set; }
}
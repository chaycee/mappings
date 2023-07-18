using TooniverseAPI.Database;
using TooniverseAPI.Models;
using Artwork = TooniverseAPI.Database.Artwork;

public class ProviderResult
{
    public string Name { get; set; }
    public List<SearchResult>? Results { get; set; }
    public bool HasMapping { get; set; } = false;
    public Mapping? Mapping { get; set; }
    public Artwork? Artwork { get; set; }
}
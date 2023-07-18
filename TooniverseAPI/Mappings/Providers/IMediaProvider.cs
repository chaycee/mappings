using TooniverseAPI.Models;

namespace TooniverseAPI.Mappings.Providers;

public interface IMediaProvider
{
    /// <summary>
    /// Search for media.
    /// </summary>
    ///
    public string Name { get; }

    ValueTask<List<SearchResult>> SearchAsync(string query);
}
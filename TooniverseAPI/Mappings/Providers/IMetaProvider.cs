using TooniverseAPI.Database;

namespace TooniverseAPI.Mappings;

public interface IMetaProvider
{
    public string Name { get; }
    public Task<ProviderResult> SearchAsync(string query, int? year = null, string? format = null);
    public Task<SubMedia> GetInfoAsync(string id, string type);
}
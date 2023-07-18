using HotChocolate.Language;
using Newtonsoft.Json;
using TooniverseAPI.Database;
using Path = System.IO.Path;

namespace TooniverseAPI.Mappings.Providers.Id;

public static class MalSyncBackup
{
    private static string _path = @"C:\Users\chayce\anilist\MAL-Sync-Backup\data\anilist\anime";

    public static SubMedia? GetMappings(string id)
    {
        var fullPath = Path.Combine(_path, id + ".json");

        if (!File.Exists(fullPath))
            return null;

        var json = File.ReadAllText(fullPath);
        var response = JsonConvert.DeserializeObject<Response>(json);

        var mappings = response?.Pages.SelectMany(page =>
            page.Value.Select(map => new Mapping()
            {
                Source = Source.MapToSource(map.Value.page),
                SourceId = map.Value.identifier,
                Type = map.Value.type
            })).ToList();

        var artworks = response?.Pages.SelectMany(page =>
            page.Value.Select(map => new Artwork()
            {
                Source = Source.MapToSource(map.Value.page),
                Image = map.Value.image,
                Type = map.Value.type
            })).ToList();

        return new SubMedia()
        {
            Mappings = mappings,
            Artworks = artworks
        };
    }

    public static int[] GetIds()
    {
        var fullPath = Path.Combine(_path, "_index.json");
        if (!File.Exists(fullPath))
            return Array.Empty<int>();
        var json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<int[]>(json)!;
    }
}

internal class AnimePage
{
    public int id { get; set; }
    public string identifier { get; set; }
    public string url { get; set; }
    public string image { get; set; }
    public string type { get; set; }
    public string page { get; set; }
    public int? malId { get; set; }
    public int? aniId { get; set; }
}

internal class Response
{
    public int? malId { get; set; }
    public Dictionary<string, Dictionary<string, AnimePage>> Pages { get; set; }
}
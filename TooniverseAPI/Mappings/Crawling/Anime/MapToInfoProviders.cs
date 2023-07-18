using System.Diagnostics;
using Newtonsoft.Json;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;
using TooniverseAPI.Mappings.Utils.Extensions;
using TooniverseAPI.Mappings.Utils.StringSimilarity;

namespace TooniverseAPI.Mappings.Crawling.Anime;

public partial class AnimeCrawler
{
    public async Task<SubMedia> GetSubMediaTVDB(ICollection<Mapping> mappings)
    {
        var data = mappings.FirstOrDefault(x => Source.MapToSource(x.Source) == Source.Tvdb);
        if (data == null)
            return new SubMedia();

        var result = await Clients._tvdb.GetInfoAsync(data.SourceId, data.Type!);

        return result;
    }
}
using System.Text;
using Newtonsoft.Json;
using TooniverseAPI.Mappings.Utils.Generators;
using TooniverseAPI.Mappings.Utils.Http;

namespace TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;

public class AnilistInfo : Request
{
    private const string AnilistUrl = "https://graphql.anilist.co";
    private const string Origin = "anilist.co";

    public async Task<Root?> GetInfoBatchAsync(IEnumerable<int> ids)
    {
        var request = CreateRequestMessage(GenerateBatchAnilist.Generate(ids));
        var response = await SendWithProxyAsync(request);
        Console.WriteLine(response);
        return await ResponseToData(response);
    }

    private HttpRequestMessage CreateRequestMessage(string content)
    {
        return new HttpRequestMessage(HttpMethod.Post, AnilistUrl)
        {
            Headers = { { "Origin", Origin } },
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
    }

    private async Task<Root?> ResponseToData(HttpResponseMessage? responseMessage)
    {
        var json = await responseMessage?.Content.ReadAsStringAsync()!;
        try
        {
            return JsonConvert.DeserializeObject<Root>(json);
        }
        catch
        {
            return null;
        }
    }
}
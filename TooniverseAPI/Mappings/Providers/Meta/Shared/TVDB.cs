using System.Runtime.InteropServices.JavaScript;
using System.Text;
using HotChocolate.Language;
using Newtonsoft.Json;
using TMDbLib.Client;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Providers.Meta.Shared.Models;
using TooniverseAPI.Mappings.Utils.Http;
using TooniverseAPI.Models;
using Artwork = TooniverseAPI.Database.Artwork;

namespace TooniverseAPI.Mappings.Providers.Meta.Shared;

public class TVDB : Request, IMetaProvider
{
    public string Name { get; } = Source.Tvdb;
    private string _baseUrl = "https://api4.thetvdb.com/v4";

    private readonly string[] _apiKeys =
    {
        "f5744a13-9203-4d02-b951-fbd7352c1657",
        "8f406bec-6ddb-45e7-8f4b-e1861e10f1bb",
        "5476e702-85aa-45fd-a8da-e74df3840baf",
        "51020266-18f7-4382-81fc-75a4014fa59f"
    };

    private List<string> _tokens = new();


    public async Task<ProviderResult> SearchAsync(string query, int? year = null, string? format = null)
    {
        try
        {
            var token = await GetTokenAsync();


            string formattedType;
            if (format == "TV" || format == "TV_SHORT" || format == "SPECIAL")
                formattedType = "series";
            else if (format == "MOVIE")
                formattedType = "movie";
            else
                formattedType = null;
            var isSeason = query.ToLower().Contains("season");
            if (isSeason) query = query.ToLower().Replace("season", "").Trim();
            var url =
                $"/search?query={Uri.EscapeDataString(query)}" +
                $"{(year != null && !isSeason ? $"&year={year}" : "")}" +
                $"{(formattedType != null ? $"&type={formattedType}" : "")}";

            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + url)
            {
                Headers =
                {
                    { "Authorization", $"Bearer {token}" }
                }
            };
            var response = await SendAsync(request);
            if (!response?.IsSuccessStatusCode ?? true)
                return new ProviderResult()
                {
                    Results = new List<SearchResult>(),
                    Name = "TVDB"
                };
            var json = await response?.Content.ReadAsStringAsync()!;
            var data = JsonConvert.DeserializeObject<TVDBRoot>(json);
            var searchResults = data?.data?.Select(x =>
            {
                var titles = new List<string> { x.name };
                if (x?.aliases != null)
                    titles.AddRange(x.aliases);

                return new SearchResult()
                {
                    Title = x.name,
                    Titles = titles.ToArray(),
                    Id = x.tvdb_id.ToString(),
                    Type = x.type,
                    Year = x.year,
                    Poster = x.image_url,
                    Overview = x.overviews?.eng ?? x.overview
                };
            }).ToList();
            return new ProviderResult()
            {
                Results = searchResults,
                Name = "TVDB"
            };
        }
        catch
        {
            return new ProviderResult()
            {
                Results = new List<SearchResult>(),
                Name = "TVDB"
            };
        }
    }

    private Dictionary<string, int[]> artworkIds = new()
    {
        { "banner", new int[] { 1, 16, 6 } },
        { "poster", new int[] { 2, 7, 14, 27 } },
        { "backgrounds", new int[] { 3, 8, 15 } },
        { "icon", new int[] { 5, 10, 18, 19, 26 } },
        { "clearArt", new int[] { 22, 24 } },
        { "clearLogo", new int[] { 23, 25 } },
        { "fanart", new int[] { 11, 12 } },
        { "actorPhoto", new int[] { 13 } },
        { "cinemagraphs", new int[] { 20, 21 } }
    };

    public async Task<SubMedia> GetInfoAsync(string? id, string type)
    {
        var subMedia = new SubMedia();
        if (string.IsNullOrEmpty(id))
            return subMedia;
        try
        {
            var token = await GetTokenAsync();
            var url =
                $"/{type}/{id}/extended";
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + url)
            {
                Headers =
                {
                    { "Authorization", $"Bearer {token}" }
                }
            };
            var response = await SendAsync(request);
            var json = await response?.Content.ReadAsStringAsync()!;
            var data = JsonConvert.DeserializeObject<TVDBInfo>(json);
            var artworks = data?.data?.artworks?.Select(x => new Artwork()
            {
                Image = x.image,
                Type = GetArtworkType(x.type),
                Source = Source.Tvdb,
                Rating = x.score,
                Height = x.height,
                Width = x.width,
                Language = x.language
            });
            subMedia.Artworks = artworks?.ToList();
            var trailers = Array.Empty<string>();
            if (data?.data?.trailers != null)
                trailers = data?.data?.trailers.Where(x => x.language == "eng" && x.url.Contains("youtube"))
                    .Select(x => x.url).ToArray();
            subMedia.YoutubeTrailers = trailers;
            if (data?.data?.remoteIds != null)
                subMedia.Mappings = data?.data?.remoteIds?.Select(x => new Mapping()
                {
                    SourceId = x.id,
                    Source = Source.MapToSource(x.sourceName),
                    Type = "remoteIds"
                }).ToList();
        }
        catch (Exception e)
        {
        }

        return subMedia;
    }

    private async Task<string> GetTokenAsync()
    {
        var url = "/login";
        if (_tokens.Count == 0)
            foreach (var apiKey in _apiKeys)
            {
                var body = new
                {
                    apikey = apiKey
                };
                var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                };
                var data = await SendAsync(request);
                var content = await data.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<dynamic>(content)!;
                string? token = obj.data.token;
                if (token != null)
                    _tokens.Push(token);
            }

        return _tokens[new Random().Next(_tokens.Count)];
    }

    private string GetArtworkType(int? typeId)
    {
        // Convert artwork type based on the provided mapping
        Dictionary<int, string> typeMapping = new()
        {
            { 1, "Banner" },
            { 16, "Banner" },
            { 6, "Banner" },
            { 2, "Poster" },
            { 7, "Poster" },
            { 14, "Poster" },
            { 27, "Poster" },
            { 3, "Background" },
            { 4, "Icon" },
            { 8, "Background" },
            { 15, "Background" },
            { 5, "Icon" },
            { 10, "Icon" },
            { 18, "Icon" },
            { 19, "Icon" },
            { 26, "Icon" },
            { 22, "ClearArt" },
            { 24, "ClearArt" },
            { 23, "ClearLogo" },
            { 25, "ClearLogo" },
            { 11, "Fanart" },
            { 12, "Fanart" },
            { 13, "ActorPhoto" },
            { 20, "Cinemagraphs" },
            { 21, "Cinemagraphs" }
        };

        if (typeId != null && typeMapping.ContainsKey(typeId.Value))
            return typeMapping[typeId.Value];

        return "Unknown";
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Datum
{
    public string objectID { get; set; }
    public string country { get; set; }
    public string id { get; set; }
    public string image_url { get; set; }
    public string name { get; set; }
    public string first_air_time { get; set; }
    public string overview { get; set; }
    public string primary_language { get; set; }
    public string primary_type { get; set; }
    public string status { get; set; }
    public string type { get; set; }
    public string tvdb_id { get; set; }
    public string year { get; set; }
    public string slug { get; set; }
    public Overviews overviews { get; set; }
    public Dictionary<string, string> translations { get; set; }
    public string network { get; set; }
    public List<RemoteId> remote_ids { get; set; }
    public string thumbnail { get; set; }
    public List<string> aliases { get; set; }
    public string director { get; set; }
    public string extended_title { get; set; }
    public List<string> genres { get; set; }
    public List<string> studios { get; set; }
}

public class Links
{
    public object prev { get; set; }
    public string self { get; set; }
    public string next { get; set; }
    public int? total_items { get; set; }
    public int? page_size { get; set; }
}

public class Overviews
{
    public string ara { get; set; }
    public string deu { get; set; }
    public string eng { get; set; }
    public string fra { get; set; }
    public string hun { get; set; }
    public string ita { get; set; }
    public string jpn { get; set; }
    public string kor { get; set; }
    public string pol { get; set; }
    public string por { get; set; }
    public string pt { get; set; }
    public string rus { get; set; }
    public string spa { get; set; }
    public string tur { get; set; }
    public string ces { get; set; }
    public string heb { get; set; }
    public string zho { get; set; }
    public string zhtw { get; set; }
}

public class RemoteId
{
    public string id { get; set; }
    public int? type { get; set; }
    public string sourceName { get; set; }
}

public class TVDBRoot
{
    public string status { get; set; }
    public List<Datum> data { get; set; }
    public Links links { get; set; }
}

public class Translations
{
    public string ara { get; set; }
    public string deu { get; set; }
    public string eng { get; set; }
    public string fra { get; set; }
    public string heb { get; set; }
    public string hun { get; set; }
    public string ita { get; set; }
    public string jpn { get; set; }
    public string kor { get; set; }
    public string pol { get; set; }
    public string por { get; set; }
    public string pt { get; set; }
    public string rus { get; set; }
    public string spa { get; set; }
    public string tha { get; set; }
    public string tur { get; set; }
    public string zho { get; set; }
    public string zhtw { get; set; }
    public string ces { get; set; }
    public string nld { get; set; }
    public string nor { get; set; }
    public string swe { get; set; }
    public string dan { get; set; }
    public string tgl { get; set; }
}
using Newtonsoft.Json;
using TMDbLib.Client;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Utils.Http;
using TooniverseAPI.Models;

namespace TooniverseAPI.Mappings.Providers.Meta.Shared;

public class TMDB : Request, IMetaProvider
{
    public string Name { get; } = Source.Tmdb;

    private string _baseUrl = "https://api.themoviedb.org/3";

    private readonly string[] _apiKeys =
    {
        "7f4a0bd0bd3315bb832e17feda70b5cd",
        "83cf4ee97bb728eeaf9d4a54e64356a1",
        "5201b54eb0968700e693a30576d7d4dc",
        "9beb1634cec80c0b62602a3d1ee9bdf9",
        "19f84e11932abbc79e6d83f82d6d1045",
        "04c35731a5ee918f014970082a0088b1",
        "cfe422613b250f702980a3bbf9e90716",
        "4e44d9029b1270a757cddc766a1bcb63",
        "3fd2be6f0c70a2a598f084ddfb75487c",
        "eb32a449fa8baebded9cd3b02bc0fef4",
        "0ccbee0a69447c2b1bd0090bf76b0358",
        "d56e51fb77b081a9cb5192eaaa7823ad",
        "ae700a17fe68acb1deb66c34b41c174f",
        "3006db21fe36d2d320d95f8e9e7e950a"
    };


    public async Task<ProviderResult> SearchAsync(string query, int? year = null, string? format = null)
    {
        var url =
            $"/search/multi?api_key={_apiKeys[new Random().Next(_apiKeys.Length)]}&language=en-US&include_adult=true&query={Uri.EscapeDataString(query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + url)
        {
            Headers = { { "Origin", "api.themoviedb.org" } }
        };
        var response = await SendAsync(request);
        var json = await response?.Content.ReadAsStringAsync()!;
        var data = JsonConvert.DeserializeObject<Root>(json);
        List<SearchResult> searchResults = new();
        if (data?.results != null)
        {
            foreach (var x in data.results)
                try
                {
                    var obj = new SearchResult
                    {
                        Title = (x?.title ?? x?.name) ?? "NO MATCH NO TITLE NO NOTHING DIE BITCH",
                        Type = x.media_type,
                        Year = x.release_date?.Split('-')[0] ?? x.first_air_date?.Split('-')[0],
                        Titles = new[] { x.title, x.name, x.original_title, x.original_name }.Where(s => s != null)
                            .ToArray(),
                        Poster = "https://image.tmdb.org/t/p/original" + x.poster_path,
                        Backdrop = "https://image.tmdb.org/t/p/original" + x.poster_path,
                        Overview = x.overview
                    };
                    searchResults.Add(obj);
                }
                catch (Exception e)
                {
                }

            searchResults = data.results.Select(x =>
            {
                return new SearchResult
                {
                    Title = (x?.title ?? x?.name)!,
                    Id = x.id.ToString(),
                    Type = x.media_type,
                    Year = x.release_date?.Split('-')[0] ?? x.first_air_date?.Split('-')[0],
                    Titles = new[] { x.title, x.name, x.original_title, x.original_name }.Where(s => s != null)
                        .ToArray(),
                    Poster = "https://image.tmdb.org/t/p/original" + x.poster_path,
                    Backdrop = "https://image.tmdb.org/t/p/original" + x.poster_path,
                    Overview = x.overview
                };
            }).ToList();
        }

        return new ProviderResult()
        {
            Results = searchResults,
            Name = "TMDB"
        };
    }

    public Task<SubMedia> GetInfoAsync(string id, string type)
    {
        throw new NotImplementedException();
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Result
{
    public bool adult { get; set; }
    public string backdrop_path { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string original_language { get; set; }
    public string original_name { get; set; }
    public string overview { get; set; }
    public string poster_path { get; set; }
    public string media_type { get; set; }
    public List<int> genre_ids { get; set; }
    public double popularity { get; set; }
    public string first_air_date { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
    public List<string> origin_country { get; set; }
    public string title { get; set; }
    public string original_title { get; set; }
    public string release_date { get; set; }
    public bool? video { get; set; }
}

public class Root
{
    public int page { get; set; }
    public List<Result> results { get; set; }
    public int total_pages { get; set; }
    public int total_results { get; set; }
}
using TooniverseAPI.Models;

namespace TooniverseAPI.Mappings.Providers.TV;

public class FlixHq : IMediaProvider
{
    public string Name => "FlixHq";

    public async ValueTask<List<SearchResult>> SearchAsync(string query)
    {
        var results = new List<SearchResult>();

        var searchRes = await Clients.MovieClient.FlixHQ.SearchAsync(query);
        searchRes.ForEach(x =>
        {
            results.Add(new SearchResult()
            {
                Id = x.Id,
                Title = x.Title
            });
        });
        return results;
    }
}
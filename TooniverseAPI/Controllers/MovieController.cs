using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using TooniverseAPI.Database;

namespace TooniverseAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly TMDbClient _client = new("5201b54eb0968700e693a30576d7d4dc");
    private readonly TooniverseContext db;

    public MovieController(TooniverseContext db)
    {
        this.db = db;
    }

    [HttpGet("/serie/search")]
    public async Task<SearchContainer<SearchTv>> GetSerieSearchV4(string q)
    {
        return await _client.SearchTvShowAsync(q, includeAdult: true);
    }

    [HttpGet("/movie/search")]
    public async Task<SearchContainer<SearchMovie>> GetMovieSearchV4(string q)
    {
        return await _client.SearchMovieAsync(q, includeAdult: true);
    }

    [HttpGet("/serie/{id}")]
    public async Task<TvShow> GetSerieV4(int id)
    {
        return await _client.GetTvShowAsync(id);
    }

    [HttpGet("/movie/{id}")]
    public async Task<Movie> GetMovieV4(int id)
    {
        return await _client.GetMovieAsync(id);
    }


    [HttpGet("/media/{source}/{value}")]
    public Media? GetMediaViaMappings(string source, string value)
    {
        return db.Anime
            .Where(m => m.Mappings.Any(mapping => mapping.Source == source && mapping.SourceId == value))
            .Include(m => m.Mappings)
            .Include(m => m.Artworks)
            .FirstOrDefault();
    }

    [HttpGet("/media/{id}")]
    public Media? GetMediaViaIdV2(int id)
    {
        var anime = db.Anime.FirstOrDefault(x => x.Id == id);

        if (anime != null)
        {
            db.Entry(anime).Collection(x => x.Mappings).Load();
            db.Entry(anime).Collection(x => x.Artworks).Load();
            db.Entry(anime).Collection(x => x.Recommendations).Load();
            db.Entry(anime).Collection(x => x.Relations).Load();
            db.Entry(anime).Collection(x => x.Characters).Load();
        }

        return anime;
    }
}
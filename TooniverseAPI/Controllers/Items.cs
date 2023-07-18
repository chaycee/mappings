using System.Diagnostics;
using Juro.Models;
using Meilisearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings;
using TooniverseAPI.Models;
using TooniverseAPI.Services;
using Artwork = TooniverseAPI.Database.Artwork;
using Type = TooniverseAPI.Database.Type;

namespace TooniverseAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class Items : ControllerBase
{
    private readonly HttpClient _client = new();

    private readonly TooniverseContext _context;

    private readonly MeilisearchClient _meiliSearchClient;

    public Items(TooniverseContext context, MeilisearchClient meiliSearchClient)
    {
        _context = context;
        _meiliSearchClient = meiliSearchClient;
    }

    //create get route to get all items
    [HttpGet]
    public ActionResult<IEnumerable<Media>> GetItems()
    {
        var items = _context.Anime.ToList();
        return Ok(items);
    }
    //
    // [HttpGet("import")]
    // public async Task<ActionResult> ImportFromKato(string jsonUrl)
    // {
    //     var json = await _client.GetStringAsync(jsonUrl);
    //     var db = JsonConvert.DeserializeObject<Root>(json);
    //     var medias = new List<Media>();
    //     foreach (var anime in db.anime)
    //     {
    //         var title = (anime.title?.english ?? anime.title?.romaji ?? anime.title?.native)!;
    //         var titles = new string[] { anime.title?.english, anime.title?.romaji, anime.title?.native };
    //         var artworks = new List<Artwork>();
    //         var mappings = new List<Mapping>();
    //         anime.artwork.ForEach(artwork =>
    //         {
    //             artworks.Add(new Artwork()
    //             {
    //                 Source = artwork.providerId,
    //                 Image = artwork.img,
    //                 Type = artwork.type
    //             });
    //         });
    //
    //         anime.mappings.ForEach(mapping =>
    //         {
    //             mappings.Add(new Mapping()
    //             {
    //                 Source = mapping.providerId,
    //                 SourceId = mapping.id
    //             });
    //         });
    //         mappings.Add(new Mapping()
    //         {
    //             Source = "anilist",
    //             SourceId = anime.id
    //         });
    //
    //
    //         medias.Add(new Media()
    //         {
    //             Title = title,
    //             Type = Type.Anime,
    //             Overview = anime.description,
    //             Artworks = artworks,
    //             Status = anime.status,
    //             Tags = anime.tags.ToArray(),
    //             Poster = anime.coverImage,
    //             Banner = anime.bannerImage,
    //             Year = anime.year,
    //             Mappings = mappings,
    //             Genres = anime.genres.ToArray(),
    //             YoutubeTrailers = new string[] { anime.trailer },
    //             Titles = titles.Where(title => title != null).ToArray(),
    //             InsertedAt = DateTime.UtcNow
    //         });
    //         //create utc date time
    //     }
    //
    //     await _context.Anime.AddRangeAsync(medias);
    //     await _context.SaveChangesAsync();
    //     return Ok("Success");
    // }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string q)
    {
        var meiliItems = await _meiliSearchClient.Index("anime").SearchAsync<MediaDto>(q);
        return Ok(meiliItems);
    }

    [HttpGet("episodes")]
    public async Task<IActionResult> Episodes(string id)
    {
        return Ok(await Clients._zoro.GetEpisodesAsync(id));
    }

    [HttpGet("videoServers")]
    public async Task<IActionResult> VideoServers(string id)
    {
        return Ok(await Clients._zoro.GetVideoServersAsync(id));
    }

    [HttpGet("videos")]
    public async Task<IActionResult> Videos()
    {
        var server = new VideoServer()
        {
            Name = "(DUB) Vidstreaming",
            Embed = new FileUrl("https://rapid-cloud.co/embed-6/4iahRvbJ0cCH?k=1")
        };
        return Ok(await Clients._zoro.GetVideosAsync(server));
    }

    [HttpGet("max")]
    public async Task<IActionResult> MaxLength()
    {
        var mediaList = await _context.Anime
            .Include(x => x.Artworks)
            .Where(x => x.Artworks.Count > 15)
            .ToListAsync();

        foreach (var media in mediaList)
            if (media.Artworks != null && media.Artworks.Count > 15)
                media.Artworks = media.Artworks.Take(15).ToList();


        _context.Anime.UpdateRange(mediaList);
        var status = await _context.SaveChangesAsync();

        return Ok(status);
    }
}

// public class Root
// {
//     public List<Anime> anime { get; set; }
// }
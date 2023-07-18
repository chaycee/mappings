using System.Diagnostics;
using Juro.Models;
using Meilisearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings;
using TooniverseAPI.Mappings.Crawling.Anime;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;
using TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist.Characters;
using TooniverseAPI.Mappings.Utils;
using TooniverseAPI.Mappings.Utils.Extensions;
using TooniverseAPI.Models;
using TooniverseAPI.Services;
using Artwork = TooniverseAPI.Database.Artwork;
using Mapping = TooniverseAPI.Database.Mapping;
using Type = TooniverseAPI.Database.Type;

namespace TooniverseAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class Mappings : ControllerBase
{
    private readonly TooniverseContext _context;
    private readonly AnimeCrawler _crawler = new();

    public Mappings(TooniverseContext context)
    {
        _context = context;
    }

    [HttpGet("info/{id}")]
    public ActionResult Info(int id)
    {
        var anime = _context.Anime.Where(x => x.Id == id).GenerateReturnType();
        anime?.RemoveStringArrayDuplicates();
        return Ok(anime);
    }

    private Random _rand = new();

    [HttpGet("info/random")]
    public ActionResult RandomInfo()
    {
        var id = _rand.Next(1, _context.Anime.Count());
        var anime = _context.Anime.Where(x => x.Id == id).GenerateReturnType();
        anime?.RemoveStringArrayDuplicates();
        return Ok(anime);
    }

    [OutputCache(Duration = 3600)]
    [HttpGet("from/{source}/{value}")]
    public AnimeType? GetMediaViaMappings(string source, string value)
    {
        return _context.Anime
            .Where(m => m.Mappings.Any(mapping => mapping.Source == source && mapping.SourceId == value))
            .GenerateReturnType();
    }
}
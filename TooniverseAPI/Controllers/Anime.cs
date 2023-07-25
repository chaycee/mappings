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
public class Anime : ControllerBase
{
    private readonly TooniverseContext _context;
    private const int MaxPerPage = 50;

    public Anime(TooniverseContext context)
    {
        _context = context;
    }
    
    [OutputCache(Duration = 3600)]
    [HttpGet("info/{id}")]
    public ActionResult Info(int id)
    {
        var anime = _context.Anime.Where(x => x.Id == id).GenerateReturnType().FirstOrDefault();;
        anime?.RemoveStringArrayDuplicates();
        return Ok(anime);
    }

    private Random _rand = new();

    [OutputCache(Duration = 3600)]
    [HttpGet("random")]
    public ActionResult RandomInfo()
    {
        var id = _rand.Next(1, _context.Anime.Count());
        var anime = _context.Anime
            .Where(x => x.Id == id)
            .GenerateReturnType()
            .FirstOrDefault();
        anime?.RemoveStringArrayDuplicates();
        return Ok(anime);
    }
    
    [HttpGet("top")]
    public ActionResult TopInfo(int perPage = 25,int page =1)
    {
        if (perPage > MaxPerPage) 
        {
            perPage = MaxPerPage;
        }
        
        var anime = _context.Anime
            .Where(x=>x.AverageScore != null)
            .OrderByDescending(x=>x.AverageScore)
            .Skip(perPage * (page - 1))
            .Take(perPage*page)
            .GenerateSlimReturnType();
        
        return Ok(anime);
    }
    [HttpGet("trending")]
    public ActionResult TrendingInfo(int perPage = 25,int page =1)
    {
        if (perPage > MaxPerPage) 
        {
            perPage = MaxPerPage;
        }
        var anime = _context.Anime
            .Where(x=>x.Popularity != null&&x.Year!=null)
            .OrderByDescending(x=>x.Year).ThenByDescending(x=>x.Popularity)
            .Skip(perPage * (page - 1))
            .Take(perPage)
            .GenerateSlimReturnType();
        
        return Ok(anime);
    }
    [HttpGet("popular")]
    public ActionResult PopularInfo(int perPage = 25,int page =1)
    {
        if (perPage > MaxPerPage) 
        {
            perPage = MaxPerPage;
        }
        var anime = _context.Anime
            .Where(x=>x.Popularity != null)
            .OrderByDescending(x=>x.Popularity)
            .Skip(perPage * (page - 1))
            .Take(perPage)
            .GenerateSlimReturnType();
        
        return Ok(anime);
    }

    [OutputCache(Duration = 3600)]
    [HttpGet("info/from/{source}/{value}")]
    public AnimeType? GetMediaViaMappings(string source, string value)
    {
        return _context.Anime
            .Where(m => m.Mappings.Any(mapping => mapping.Source == source && mapping.SourceId == value))
            .GenerateReturnType().FirstOrDefault();;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings.Utils.Extensions;

namespace TooniverseAPI.Controllers;
//TODO: add update history for things like rating eg. popularity, score, etc.
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
    [OutputCache(Duration = 86_400)]
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
    [OutputCache(Duration = 86_400)]
    [HttpGet("trending")]
    public ActionResult TrendingInfo(int perPage = 25,int page =1)
    {
        if (perPage > MaxPerPage) 
        {
            perPage = MaxPerPage;
        }
        var anime = _context.Anime
                    .Where(x=>x.Trending != null&&x.Status != "NOT_YET_RELEASED"&&x.Status != "FINISHED")
                    .OrderByDescending(x=>x.Trending)
                    .Skip(perPage * (page - 1))
                    .Take(perPage)
                    .GenerateSlimReturnType();
    
        return Ok(anime);
    }
    [OutputCache(Duration = 86_400)]
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
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TooniverseAPI.Database;

namespace TooniverseAPI.Data;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Media> Media([Service] TooniverseContext context)
    {
        return context.Anime;
    }


    [UseFirstOrDefault]
    public IExecutable<Media> MediaById(
        [Service] TooniverseContext context,
        int id)
    {
        return context.Anime.Where(x => x.Id == id).Include(x => x.Artworks).Include(x => x.Mappings).AsExecutable();
    }


    [UseFirstOrDefault]
    public IExecutable<Media> MediaByMapping(
        [Service] TooniverseContext context,
        string source, string value)
    {
        return context.Anime
            .Where(m => m.Mappings.Any(mapping => mapping.Source == source && mapping.SourceId == value))
            .Include(m => m.Mappings)
            .Include(m => m.Artworks)
            .AsExecutable();
    }
}
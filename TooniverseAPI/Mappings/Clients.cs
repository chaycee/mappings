using Juro.Clients;
using Juro.Providers.Anime;
using TooniverseAPI.Mappings.Providers.Meta.Shared;
using AnimePahe = Juro.Providers.Anime.AnimePahe;
using NineAnime = Juro.Providers.Anime.NineAnime;
using Zoro = Juro.Providers.Anime.Zoro;

namespace TooniverseAPI.Mappings;

public static class Clients
{
    public static readonly Gogoanime _gogoanime = new();
    public static readonly Zoro _zoro = new();
    public static readonly NineAnime _nineAnime = new();
    public static AnimePahe _animePahe = new();
    public static TMDB _tmdb = new();
    public static TVDB _tvdb = new();

    public static readonly IEnumerable<IAnimeProvider> AllAnimeProviders = new List<IAnimeProvider>()
    {
        _nineAnime,
        _gogoanime,
        _zoro,
        _animePahe
    };

    public static readonly IEnumerable<IMetaProvider> AllMetaProviders = new List<IMetaProvider>()
    {
        _tmdb,
        _tvdb
    };

    public static MovieClient MovieClient = new();
}
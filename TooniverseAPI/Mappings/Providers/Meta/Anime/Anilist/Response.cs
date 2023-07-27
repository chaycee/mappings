namespace TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Anime
{
    public List<Medium>? media { get; set; }
}

public class CoverImage
{
    public string extraLarge { get; set; }
    public string large { get; set; }
    public string color { get; set; }
}

public class Data
{
    public Dictionary<string, Anime> Animes { get; set; }
}

public class Edge
{
    public int? id { get; set; }
    public string relationType { get; set; }
    public Node node { get; set; }
}

public class EndDate
{
    public object year { get; set; }
    public object month { get; set; }
    public object day { get; set; }
}

public class Medium
{
    public int id { get; set; }
    public int? idMal { get; set; }
    public Title title { get; set; }
    public CoverImage coverImage { get; set; }
    public string bannerImage { get; set; }
    public StartDate startDate { get; set; }
    public EndDate endDate { get; set; }
    public string description { get; set; }
    public string season { get; set; }
    public int? seasonYear { get; set; }
    public string type { get; set; }
    public string format { get; set; }
    public string status { get; set; }
    public object episodes { get; set; }
    public int? duration { get; set; }
    public object chapters { get; set; }
    public object volumes { get; set; }
    public List<string> genres { get; set; }
    public List<string> synonyms { get; set; }
    public string source { get; set; }
    public bool? isAdult { get; set; }
    public int? meanScore { get; set; }
    public int? averageScore { get; set; }
    public int? popularity { get; set; }
    public int? favourites { get; set; }
    public int? trending { get; set; }
    public string countryOfOrigin { get; set; }
    public bool? isLicensed { get; set; }
    public Relations relations { get; set; }
    public Characters.Characters characters { get; set; }
    public List<StreamingEpisode> streamingEpisodes { get; set; }
    public Recommendations recommendations { get; set; }
    public Trailer? trailer { get; set; }
    public List<Tag> tags { get; set; }
}

public class Trailer
{
    public string id { get; set; }
    public string site { get; set; }
    public string thumbnail { get; set; }
}

public class Recommendations
{
    public List<NodeRecommendation> nodes { get; set; }
}

public class NodeRecommendation
{
    public MediaRecommendation mediaRecommendation { get; set; }
}

public class MediaRecommendation
{
    public int id { get; set; }
}

public class Node
{
    public int? id { get; set; }
    public Title title { get; set; }
    public string format { get; set; }
    public string type { get; set; }
    public string status { get; set; }
    public string bannerImage { get; set; }
    public CoverImage coverImage { get; set; }
}

public class Relations
{
    public List<Edge> edges { get; set; }
}

public class Root
{
    public Dictionary<string, Anime> data { get; set; }
}

public class StartDate
{
    public int? year { get; set; }
    public int? month { get; set; }
    public int? day { get; set; }
}

public class StreamingEpisode
{
    public string title { get; set; }
    public string thumbnail { get; set; }
    public string url { get; set; }
}

public class Tag
{
    public int? id { get; set; }
    public string name { get; set; }
}

public class Title
{
    public string? romaji { get; set; }
    public string? english { get; set; }
    public string? native { get; set; }
}
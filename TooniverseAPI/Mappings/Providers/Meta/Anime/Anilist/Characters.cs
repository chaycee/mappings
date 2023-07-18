namespace TooniverseAPI.Mappings.Providers.Meta.Anime.Anilist.Characters;

public class Characters
{
    public List<CharaEdge> edges { get; set; }
}

public class CharaEdge
{
    public string? role { get; set; }
    public Node node { get; set; }
    public List<VoiceActor>? voiceActors { get; set; }
}

public class Image
{
    public string? large { get; set; }
}

public class Name
{
    public string? first { get; set; }
    public string? last { get; set; }
    public string? middle { get; set; }
    public string? full { get; set; }
}

public class Node
{
    public int? favourites { get; set; }
    public Name name { get; set; }
    public string? age { get; set; }
    public string? gender { get; set; }
    public Image image { get; set; }
}

public class VoiceActor
{
    public Name name { get; set; }
    public int? favourites { get; set; }
    public string? gender { get; set; }
    public int? age { get; set; }
    public Image image { get; set; }
}
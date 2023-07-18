namespace TooniverseAPI.Mappings.Providers.Meta.Shared.Models;

public class AirsDays
{
    public bool? sunday { get; set; }
    public bool? monday { get; set; }
    public bool? tuesday { get; set; }
    public bool? wednesday { get; set; }
    public bool? thursday { get; set; }
    public bool? friday { get; set; }
    public bool? saturday { get; set; }
}

public class Alias
{
    public string language { get; set; }
    public string name { get; set; }
}

public class Artwork
{
    public int? id { get; set; }
    public string image { get; set; }
    public string thumbnail { get; set; }
    public string language { get; set; }
    public int? type { get; set; }
    public int? score { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public bool? includesText { get; set; }
    public int? thumbnailWidth { get; set; }
    public int? thumbnailHeight { get; set; }
    public int? updatedAt { get; set; }
    public Status status { get; set; }
    public object tagOptions { get; set; }
    public int? seasonId { get; set; }
}

public class Character
{
    public int? id { get; set; }
    public string name { get; set; }
    public int? peopleId { get; set; }
    public int? seriesId { get; set; }
    public object series { get; set; }
    public object movie { get; set; }
    public object movieId { get; set; }
    public object episodeId { get; set; }
    public int? type { get; set; }
    public string image { get; set; }
    public int? sort { get; set; }
    public bool? isFeatured { get; set; }
    public string url { get; set; }
    public object nameTranslations { get; set; }
    public object overviewTranslations { get; set; }
    public object aliases { get; set; }
    public string peopleType { get; set; }
    public string personName { get; set; }
    public object tagOptions { get; set; }
    public string personImgURL { get; set; }
}

public class Companies
{
    public object studio { get; set; }
    public object network { get; set; }
    public object production { get; set; }
    public object distributor { get; set; }
    public object special_effects { get; set; }
    public int? id { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
    public List<string> nameTranslations { get; set; }
    public List<string> overviewTranslations { get; set; }
    public List<Alias> aliases { get; set; }
    public string country { get; set; }
    public int? primaryCompanyType { get; set; }
    public object activeDate { get; set; }
    public object inactiveDate { get; set; }
    public CompanyType companyType { get; set; }
    public ParentCompany parentCompany { get; set; }
    public object tagOptions { get; set; }
}

public class CompanyType
{
    public int? companyTypeId { get; set; }
    public string companyTypeName { get; set; }
}

public class ContentRating
{
    public int? id { get; set; }
    public string name { get; set; }
    public string country { get; set; }
    public string description { get; set; }
    public string contentType { get; set; }
    public int? order { get; set; }
    public object fullname { get; set; }
}

public class Data
{
    public int? id { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
    public string image { get; set; }
    public List<string> nameTranslations { get; set; }
    public List<string> overviewTranslations { get; set; }
    public List<Alias> aliases { get; set; }
    public string firstAired { get; set; }
    public string lastAired { get; set; }
    public string nextAired { get; set; }
    public int? score { get; set; }
    public Status status { get; set; }
    public string originalCountry { get; set; }
    public string originalLanguage { get; set; }
    public int? defaultSeasonType { get; set; }
    public bool? isOrderRandomized { get; set; }
    public string lastUpdated { get; set; }
    public int? averageRuntime { get; set; }
    public object episodes { get; set; }
    public string overview { get; set; }
    public string year { get; set; }
    public List<Artwork> artworks { get; set; }
    public OriginalNetwork originalNetwork { get; set; }
    public LatestNetwork latestNetwork { get; set; }
    public List<Genre> genres { get; set; }
    public List<Trailer>? trailers { get; set; }
    public List<List> lists { get; set; }
    public List<RemoteId> remoteIds { get; set; }
    public List<Character> characters { get; set; }
    public AirsDays airsDays { get; set; }
    public string airsTime { get; set; }
    public List<Season> seasons { get; set; }
    public List<Tag> tags { get; set; }
    public List<ContentRating> contentRatings { get; set; }
    public List<SeasonType> seasonTypes { get; set; }
}

public class Trailer
{
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string language { get; set; }
    public int runtime { get; set; }
}

public class Genre
{
    public int? id { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
}

public class LatestNetwork
{
    public int? id { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
    public List<string> nameTranslations { get; set; }
    public List<object> overviewTranslations { get; set; }
    public List<object> aliases { get; set; }
    public string country { get; set; }
    public int? primaryCompanyType { get; set; }
    public object activeDate { get; set; }
    public object inactiveDate { get; set; }
    public CompanyType companyType { get; set; }
    public ParentCompany parentCompany { get; set; }
    public object tagOptions { get; set; }
}

public class List
{
    public int? id { get; set; }
    public string name { get; set; }
    public string overview { get; set; }
    public string url { get; set; }
    public bool? isOfficial { get; set; }
    public List<string> nameTranslations { get; set; }
    public List<string> overviewTranslations { get; set; }
    public List<Alias> aliases { get; set; }
    public int? score { get; set; }
    public string image { get; set; }
    public bool? imageIsFallback { get; set; }
    public object remoteIds { get; set; }
    public object tags { get; set; }
}

public class OriginalNetwork
{
    public int? id { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
    public List<string> nameTranslations { get; set; }
    public List<object> overviewTranslations { get; set; }
    public List<object> aliases { get; set; }
    public string country { get; set; }
    public int? primaryCompanyType { get; set; }
    public object activeDate { get; set; }
    public object inactiveDate { get; set; }
    public CompanyType companyType { get; set; }
    public ParentCompany parentCompany { get; set; }
    public object tagOptions { get; set; }
}

public class ParentCompany
{
    public object id { get; set; }
    public object name { get; set; }
    public Relation relation { get; set; }
}

public class Relation
{
    public object id { get; set; }
    public object typeName { get; set; }
}

public class RemoteId
{
    public string id { get; set; }
    public int? type { get; set; }
    public string sourceName { get; set; }
}

public class TVDBInfo
{
    public string status { get; set; }
    public Data data { get; set; }
}

public class Season
{
    public int? id { get; set; }
    public int? seriesId { get; set; }
    public Type type { get; set; }
    public int? number { get; set; }
    public List<string> nameTranslations { get; set; }
    public List<string> overviewTranslations { get; set; }
    public string image { get; set; }
    public int? imageType { get; set; }
    public Companies companies { get; set; }
    public string lastUpdated { get; set; }
    public string name { get; set; }
}

public class SeasonType
{
    public int? id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public object alternateName { get; set; }
}

public class Status
{
    public int? id { get; set; }
    public string name { get; set; }
    public string recordType { get; set; }
    public bool? keepUpdated { get; set; }
}

public class Tag
{
    public int? id { get; set; }
    public int? tag { get; set; }
    public string tagName { get; set; }
    public string name { get; set; }
    public string helpText { get; set; }
}

public class Type
{
    public int? id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public object alternateName { get; set; }
}
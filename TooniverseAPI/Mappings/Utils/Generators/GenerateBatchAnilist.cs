using Newtonsoft.Json;

namespace TooniverseAPI.Mappings.Utils.Generators;

public static class GenerateBatchAnilist
{
    public static string Generate(IEnumerable<int> ids)
    {
        return BodyGenerator(string.Join("\n", ids.Select(QueryGenerator)));
    }

    private class Query
    {
        public string query { get; set; }
    }

    private static string BodyGenerator(string query)
    {
        var body =
            $$"""
              query{
                  {{query}}
              }
              """;
        var q = new Query
        {
            query = body
        };
        return JsonConvert.SerializeObject(q);
    }

    private static string QueryGenerator(int id)
    {
        return $$"""
                         anime{{id}}:Page(page: 0, perPage: 10){
                             media(id:{{id}}){
                                id
                                 idMal
                                 title {
                                     romaji
                                     english
                                     native
                                 }
                                 coverImage {
                                     extraLarge
                                     color
                                 }
                                 bannerImage
                                 startDate {
                                     year
                                     month
                                     day
                                 }
                                 description
                                 season
                                 seasonYear
                                 type
                                 format
                                 status(version: 2)
                                 episodes
                                 duration
                                 genres
                                 synonyms
                                 isAdult
                                 averageScore
                                 meanScore
                                 favourites
                                 popularity
                                 countryOfOrigin
                                 characters(sort:[ROLE,RELEVANCE,ID], perPage:25 ) {
                                   edges{
                                       role
                                       node{
                                            name{
                                               full
                                           }
                                          favourites
                                         gender
                                         
                                           age
                                           image{
                                               large
                                           }
                 
                                       }
                                       voiceActors{
                 
                                          name{
                                               full
                                           }
                                          favourites
                                         gender
                                           age
                                           image{
                                               large
                                           }
                 
                                       }
                                   }
                                }
                                recommendations (sort:[RATING_DESC,ID]){
                                    nodes{
                                        mediaRecommendation{
                                            id
                                            
                                        }
                                    }
                                }
                                 relations {
                                     edges {
                                         id
                                         relationType(version: 2)
                                         node {
                                             id
                                             title {
                                                 userPreferred
                                             }
                                             format
                                             type
                                             status(version: 2)
                                             bannerImage
                                             coverImage {
                                                 large
                                             }
                                         }
                                     }
                                 }
                                 trailer {
                                     id
                                     site
                                 }
                                 tags {
                                     id
                                     name
                                 }
                             }
                     }
                 """;
    }
}
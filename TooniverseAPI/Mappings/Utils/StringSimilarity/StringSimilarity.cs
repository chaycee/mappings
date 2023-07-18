using System.Text.RegularExpressions;
using F23.StringSimilarity;
using Newtonsoft.Json;

namespace TooniverseAPI.Mappings.Utils.StringSimilarity;

public static partial class StringSimilarity
{
    private static readonly NormalizedLevenshtein NormalizedLevenshtein = new();

    public static StringResult FindBestMatch2DArray(IEnumerable<string> mainStrings, string[][] targetStrings)
    {
        var overallBestMatch = new StringResult
        {
            Ratings = new List<Rating>(),
            BestMatch = new Rating() { Target = "", Value = 0 },
            BestMatchIndex = 0
        };

        foreach (var mainString in mainStrings)
        foreach (var targetArray in targetStrings.Select((value, index) => new { value, index }))
        {
            var ratings = new List<Rating>();

            foreach (var targetString in targetArray.value)
            {
                var currentRating = NormalizedLevenshtein.Similarity(Clean(mainString.ToLower().Trim()),
                    Clean(targetString).ToLower().Trim());

                ratings.Add(new Rating { Main = mainString, Target = targetString, Value = currentRating });
            }

            var bestMatchIndex = ratings
                .Select((value, index) => new { Value = value.Value, Index = index })
                .Aggregate((best, x) => x.Value > best.Value ? x : best)
                .Index;

            if (ratings[bestMatchIndex].Value > overallBestMatch.BestMatch.Value)
            {
                var bestMatch = ratings[bestMatchIndex];
                overallBestMatch = new StringResult
                {
                    Ratings = ratings,
                    BestMatch = bestMatch,
                    BestMatchIndex = targetArray.index
                };
            }
        }

        return overallBestMatch;
    }
}

public class StringResult
{
    public List<Rating> Ratings { get; set; }
    public Rating BestMatch { get; set; }
    public int BestMatchIndex { get; set; }
}

public class Rating
{
    public string Main { get; set; }
    public string Target { get; set; }
    public double Value { get; set; }
}
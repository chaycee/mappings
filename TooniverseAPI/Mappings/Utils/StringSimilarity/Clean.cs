using System.Text.RegularExpressions;

namespace TooniverseAPI.Mappings.Utils.StringSimilarity;

public static partial class StringSimilarity
{
    public static string Clean(string title)
    {
        return TransformSpecificVariations(
            RemoveSpecialChars(
                title.ReplaceRegex(@"[^A-Za-z0-9!@#$%^&*() ]", " ")
                    .ReplaceRegex(@"(th|rd|nd|st) (Season|season)", "")
                    .ReplaceRegex(@"\([^\(]*\)$", "")
                    .Replace("season", "")
                    .Replace("  ", " ")
                    .Replace("\"", "")
                    .TrimEnd()
            )
        );
    }

    public static string RemoveSpecialChars(string title)
    {
        return title.ReplaceRegex(@"[^A-Za-z0-9!@#$%^&*()\-= ]", " ")
            .ReplaceRegex(@"[^A-Za-z0-9\-= ]", "")
            .Replace("  ", " ");
    }

    public static string TransformSpecificVariations(string title)
    {
        return title.Replace("yuu", "yu").Replace(" ou", " oh");
    }


    public static string ReplaceRegex(this string input, string pattern, string replacement)
    {
        return Regex.Replace(input, pattern, replacement);
    }
}
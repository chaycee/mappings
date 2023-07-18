using System.Text.RegularExpressions;

namespace TooniverseAPI.Mappings.Utils.Extensions;

internal static class StringExtensions
{
    private const string Pattern = @"\b\d{4}\b";
    private static readonly Regex Regex = new(Pattern);

    public static int? ExtractYear(this string text)
    {
        var match = Regex.Match(text);
        if (match.Success) return int.Parse(match.Value);
        return null;
    }

    //string array remove duplicates and empty or null strings
    public static string[] RemoveDuplicates(this string[] array)
    {
        var list = new List<string>();
        foreach (var item in array)
            if (!string.IsNullOrEmpty(item) && !list.Contains(item))
                list.Add(item);
        return list.ToArray();
    }
}
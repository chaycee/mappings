namespace TooniverseAPI.Mappings.Utils;

public static class Combine
{
    public static IEnumerable<T> Enumerable<T>(IEnumerable<T>? first, IEnumerable<T>? second)
    {
        return (first ?? Array.Empty<T>()).Concat(second ?? Array.Empty<T>())!;
    }
}
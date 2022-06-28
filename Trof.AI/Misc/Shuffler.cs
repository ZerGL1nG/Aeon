namespace Trof.AI.Misc;

public static class Shuffler
{
    public static List<T> Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Shared.Next(i - 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    public static List<T> CopyShuffle<T>(this IEnumerable<T> list) => Shuffle<T>(new List<T>(list));

    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> e, int amount)
    {
        List<T> list = new(e);
        for (var i = 0; i < amount; i++) {
            int j = Random.Shared.Next(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list.Take(amount);
    }
}
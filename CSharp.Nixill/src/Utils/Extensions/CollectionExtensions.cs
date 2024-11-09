namespace Nixill.Utils.Extensions;

public static class CollectionExtensions
{
  public static T Pop<T>(this IList<T> list)
  {
    T value = list[0];
    list.RemoveAt(0);
    return value;
  }

  public static IEnumerable<T> Repeat<T>(this IEnumerable<T> seq, int count)
  {
    foreach (int i in Enumerable.Range(0, count))
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  public static IEnumerable<T> RepeatInfinite<T>(this IEnumerable<T> seq)
  {
    while (true)
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  public static IDictionary<K, IEnumerable<V>> ToDictionary<K, V>(this IEnumerable<IGrouping<K, V>> items)
    where K : notnull
    => items.Select(g => new KeyValuePair<K, IEnumerable<V>>(g.Key, g)).ToDictionary();
}
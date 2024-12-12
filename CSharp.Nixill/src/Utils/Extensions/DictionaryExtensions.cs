namespace Nixill.Utils.Extensions;

public static class DictionaryExtensions
{
  public static V GetOrSet<K, V>(this IDictionary<K, V> dictionary, K key, V value)
    => dictionary.GetOrSet(key, () => value);

  public static V GetOrSet<K, V>(this IDictionary<K, V> dictionary, K key, Func<V> value)
  {
    V returnValue;

    if (!dictionary.TryGetValue(key, out returnValue!))
    {
      returnValue = value();
      dictionary[key] = returnValue;
    }

    return returnValue;
  }
}
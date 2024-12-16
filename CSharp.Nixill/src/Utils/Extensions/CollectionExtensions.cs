namespace Nixill.Utils.Extensions;

/// <summary>
///   Extension methods that act on collections of specific types.
/// </summary>
public static class CollectionExtensions
{
  /// <summary>
  ///   Returns a value associated with a given key from the dictionary,
  ///   or saves a new association and returns it if the given key isn't
  ///   already found.
  /// </summary>
  /// <typeparam name="K">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="V">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key to retrieve.</param>
  /// <param name="value">
  ///   The value to save if the key is not found.
  /// </param>
  /// <returns>
  ///   Either the value already associated with the key, or <c>value</c>.
  /// </returns>
  public static V GetOrSet<K, V>(this IDictionary<K, V> dictionary, K key, V value)
    => dictionary.GetOrSet(key, () => value);

  /// <summary>
  ///   Returns a value associated with a given key from the dictionary,
  ///   or saves a new association and returns it if the given key isn't
  ///   already found.
  /// </summary>
  /// <remarks>
  ///   This differs from the
  ///   <see cref="GetOrSet{K, V}(IDictionary{K, V}, K, V)"/> overload in
  ///   that the <see cref="Func{I, O}"/> is only evaluated if the key is
  ///   not found, which can be used as a form of short circuiting (unlike
  ///   the other overload which is always evaluated).
  /// </remarks>
  /// <typeparam name="K">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="V">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key to retrieve.</param>
  /// <param name="value">
  ///   The function to run if the key is not found.
  /// </param>
  /// <returns>
  ///   Either the value already associated with the key, or <c>value()</c>.
  /// </returns>
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

  /// <summary>
  ///   Removes the first item from an <see cref="IList{T}"/> and returns it.
  /// </summary>
  /// <typeparam name="T">The type of items in the list.</typeparam>
  /// <param name="list">The list.</param>
  /// <returns>The first item.</returns>
  public static T Pop<T>(this IList<T> list)
  {
    T value = list[0];
    list.RemoveAt(0);
    return value;
  }

  /// <summary>
  ///   Converts a sequence of <c>IGrouping&lt;K, V&gt;</c>s to a single
  ///   dictionary of type <c>IDictionary&lt;K, IEnumerable&lt;V&gt;&gt;</c>.
  /// </summary>
  /// <typeparam name="K">The type of keys in the groupings.</typeparam>
  /// <typeparam name="V">The type of values in the groupings.</typeparam>
  /// <param name="items">The groupings.</param>
  /// <returns>The dictionary.</returns>
  public static IDictionary<K, IEnumerable<V>> ToDictionary<K, V>(this IEnumerable<IGrouping<K, V>> items)
    where K : notnull
    => items.Select(g => new KeyValuePair<K, IEnumerable<V>>(g.Key, g)).ToDictionary();
}
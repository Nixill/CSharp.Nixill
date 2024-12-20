using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Collections;

internal class RecursiveDictionaryPrefixView<K, V> : IRecursiveDictionary<K, V> where K : notnull
{
  readonly RecursiveDictionary<K, V> ViewedDictionary;
  readonly RecursiveDictionary<K, V> RootDictionary;
  readonly K[] PathToViewedDictionary;

  public RecursiveDictionaryPrefixView(RecursiveDictionary<K, V> viewing, RecursiveDictionary<K, V> root, IEnumerable<K> path)
  {
    ViewedDictionary = viewing;
    RootDictionary = root;
    PathToViewedDictionary = path.ToArray();
  }

  public V this[IEnumerable<K> key] { get => ((IDictionary<IEnumerable<K>, V>)ViewedDictionary)[key]; set => ((IDictionary<IEnumerable<K>, V>)ViewedDictionary)[key] = value; }

  public bool HasEmptyKeyValue => ((IRecursiveDictionary<K, V>)ViewedDictionary).HasEmptyKeyValue;

  public ICollection<K> Prefixes => ((IRecursiveDictionary<K, V>)ViewedDictionary).Prefixes;

  public ICollection<IEnumerable<K>> Keys => ((IDictionary<IEnumerable<K>, V>)ViewedDictionary).Keys;

  public ICollection<V> Values => ((IDictionary<IEnumerable<K>, V>)ViewedDictionary).Values;

  public int Count => ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).Count;

  public bool IsReadOnly => ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).IsReadOnly;

  public void Add(IEnumerable<K> key, V value)
  {
    ((IDictionary<IEnumerable<K>, V>)ViewedDictionary).Add(key, value);
  }

  public void Add(KeyValuePair<IEnumerable<K>, V> item)
  {
    ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).Add(item);
  }

  public void Clear()
  {
    ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).Clear();
  }

  public bool Contains(KeyValuePair<IEnumerable<K>, V> item)
  {
    return ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).Contains(item);
  }

  public bool ContainsKey(IEnumerable<K> key)
  {
    return ((IDictionary<IEnumerable<K>, V>)ViewedDictionary).ContainsKey(key);
  }

  public bool ContainsPrefix(IEnumerable<K> prefix)
  {
    return ((IRecursiveDictionary<K, V>)ViewedDictionary).ContainsPrefix(prefix);
  }

  public void CopyTo(KeyValuePair<IEnumerable<K>, V>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).CopyTo(array, arrayIndex);
  }

  public IEnumerator<KeyValuePair<IEnumerable<K>, V>> GetEnumerator()
  {
    return ((IEnumerable<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).GetEnumerator();
  }

  public IRecursiveDictionary<K, V> GetPrefix(IEnumerable<K> prefix)
  {
    return ((IRecursiveDictionary<K, V>)ViewedDictionary).GetPrefix(prefix);
  }

  public bool Remove(IEnumerable<K> key)
  {
    return ((IDictionary<IEnumerable<K>, V>)ViewedDictionary).Remove(key);
  }

  public bool Remove(KeyValuePair<IEnumerable<K>, V> item)
  {
    return ((ICollection<KeyValuePair<IEnumerable<K>, V>>)ViewedDictionary).Remove(item);
  }

  public int RemovePrefix(IEnumerable<K> prefix)
  {
    return ((IRecursiveDictionary<K, V>)ViewedDictionary).RemovePrefix(prefix);
  }

  public bool TryGetPrefix(IEnumerable<K> prefix, [MaybeNullWhen(false)] out IRecursiveDictionary<K, V> result)
  {
    return ((IRecursiveDictionary<K, V>)ViewedDictionary).TryGetPrefix(prefix, out result);
  }

  public bool TryGetValue(IEnumerable<K> key, [MaybeNullWhen(false)] out V value)
  {
    return ((IDictionary<IEnumerable<K>, V>)ViewedDictionary).TryGetValue(key, out value);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)ViewedDictionary).GetEnumerator();
  }
}
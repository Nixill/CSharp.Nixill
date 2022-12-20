using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Collections
{
  internal class ReadOnlyNavigableDictionaryWrapper<K, V> : IReadOnlyNavigableDictionary<K, V>
  {
    internal INavigableDictionary<K, V> Backing;

    public V this[K key] { get => Backing[key]; set => Backing[key] = value; }

    public IEnumerable<K> Keys => Backing.Keys;

    public IEnumerable<V> Values => Backing.Values;

    public int Count => Backing.Count;

    public KeyValuePair<K, V> CeilingEntry(K from)
    {
      return Backing.CeilingEntry(from);
    }

    public K CeilingKey(K from)
    {
      return Backing.CeilingKey(from);
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
      return Backing.Contains(item);
    }

    public bool ContainsCeiling(K from)
    {
      return Backing.ContainsCeiling(from);
    }

    public bool ContainsFloor(K from)
    {
      return Backing.ContainsFloor(from);
    }

    public bool ContainsHigher(K from)
    {
      return Backing.ContainsHigher(from);
    }

    public bool ContainsKey(K key)
    {
      return Backing.ContainsKey(key);
    }

    public bool ContainsLower(K from)
    {
      return Backing.ContainsLower(from);
    }

    public KeyValuePair<K, V> FloorEntry(K from)
    {
      return Backing.FloorEntry(from);
    }

    public K FloorKey(K from)
    {
      return Backing.FloorKey(from);
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
      return Backing.GetEnumerator();
    }

    public KeyValuePair<K, V> HigherEntry(K from)
    {
      return Backing.HigherEntry(from);
    }

    public K HigherKey(K from)
    {
      return Backing.HigherKey(from);
    }

    public KeyValuePair<K, V> HighestEntry()
    {
      return Backing.HighestEntry();
    }

    public K HighestKey()
    {
      return Backing.HighestKey();
    }

    public KeyValuePair<K, V> LowerEntry(K from)
    {
      return Backing.LowerEntry(from);
    }

    public K LowerKey(K from)
    {
      return Backing.LowerKey(from);
    }

    public KeyValuePair<K, V> LowestEntry()
    {
      return Backing.LowestEntry();
    }

    public K LowestKey()
    {
      return Backing.LowestKey();
    }

    public bool Remove(K key)
    {
      return Backing.Remove(key);
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
      return Backing.Remove(item);
    }

    public bool TryGetCeilingEntry(K from, out KeyValuePair<K, V> value)
    {
      return Backing.TryGetCeilingEntry(from, out value);
    }

    public bool TryGetCeilingKey(K from, out K value)
    {
      return Backing.TryGetCeilingKey(from, out value);
    }

    public bool TryGetFloorEntry(K from, out KeyValuePair<K, V> value)
    {
      return Backing.TryGetFloorEntry(from, out value);
    }

    public bool TryGetFloorKey(K from, out K value)
    {
      return Backing.TryGetFloorKey(from, out value);
    }

    public bool TryGetHigherEntry(K from, out KeyValuePair<K, V> value)
    {
      return Backing.TryGetHigherEntry(from, out value);
    }

    public bool TryGetHigherKey(K from, out K value)
    {
      return Backing.TryGetHigherKey(from, out value);
    }

    public bool TryGetLowerEntry(K from, out KeyValuePair<K, V> value)
    {
      return Backing.TryGetLowerEntry(from, out value);
    }

    public bool TryGetLowerKey(K from, out K value)
    {
      return Backing.TryGetLowerKey(from, out value);
    }

    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
    {
      return Backing.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)Backing).GetEnumerator();
    }
  }

  internal class ReadOnlyNavigableSetWrapper<T> : IReadOnlyNavigableSet<T>
  {
    internal INavigableSet<T> Backing;

    public int Count => Backing.Count;

    public T Ceiling(T from)
    {
      return Backing.Ceiling(from);
    }

    public bool Contains(T item)
    {
      return Backing.Contains(item);
    }

    public bool ContainsCeiling(T from)
    {
      return Backing.ContainsCeiling(from);
    }

    public bool ContainsFloor(T from)
    {
      return Backing.ContainsFloor(from);
    }

    public bool ContainsHigher(T from)
    {
      return Backing.ContainsHigher(from);
    }

    public bool ContainsLower(T from)
    {
      return Backing.ContainsLower(from);
    }

    public T Floor(T from)
    {
      return Backing.Floor(from);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return Backing.GetEnumerator();
    }

    public T Higher(T from)
    {
      return Backing.Higher(from);
    }

    public T HighestValue()
    {
      return Backing.HighestValue();
    }

    public T Lower(T from)
    {
      return Backing.Lower(from);
    }

    public T LowestValue()
    {
      return Backing.LowestValue();
    }

    public bool TryGetCeiling(T from, out T value)
    {
      return Backing.TryGetCeiling(from, out value);
    }

    public bool TryGetFloor(T from, out T value)
    {
      return Backing.TryGetFloor(from, out value);
    }

    public bool TryGetHigher(T from, out T value)
    {
      return Backing.TryGetHigher(from, out value);
    }

    public bool TryGetLower(T from, out T value)
    {
      return Backing.TryGetLower(from, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)Backing).GetEnumerator();
    }
  }

  public static class NavigableExtensions
  {
    public static IReadOnlyNavigableDictionary<K, V> AsReadOnly<K, V>(this INavigableDictionary<K, V> dict) => new ReadOnlyNavigableDictionaryWrapper<K, V>() { Backing = dict };
    public static IReadOnlyNavigableSet<T> AsReadOnly<T>(this INavigableSet<T> set) => new ReadOnlyNavigableSetWrapper<T>() { Backing = set };
  }
}